using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFAutoFoundation.Enums;
//using TNFAutoFoundation.Excel;
using TNFAutoFoundation.Models;
using TNFAutoFoundation.ReadingExcel;
using TNFAutoFoundation.TNFModel;

namespace TNFAutoFoundation.Command
{
    [Transaction(TransactionMode.Manual)]
    public class TnfAutoFoundationCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var app = uiApp.Application;
            var doc = uiDoc.Document;
            try
            {
                var fileDiag = new FileOpenDialog("Excel File (*.xlsx)|*.xlsx");
                if (fileDiag.Show() == ItemSelectionDialogResult.Confirmed)
                {
                    var filePathModel = fileDiag.GetSelectedModelPath();
                    var filePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(filePathModel);
                    var excelRead = new TNFExcelReading(filePath);
                    List<Element> newInstances = null;
                    using (var transaction = new Transaction(doc, "Create new Family Symbol"))
                    {
                        transaction.Start();
                        var tnfFoundationDict = new Dictionary<string, FamilySymbol>();
                        CreateFoundationType(excelRead.TNFFoundationTypes, doc, tnfFoundationDict);
                        CreateRoofTypes(excelRead.TNFImproves, doc);
                        CreateBeamType(excelRead.TNFBeams, doc);
                        newInstances = CreateFoundationInstance(excelRead.TNFFoundationIntances, doc, tnfFoundationDict);
                        var excelWriting = new TNFExcelWriting(filePath, excelRead.TNFFoundationIntances);
                        transaction.Commit();
                        TaskDialog.Show("Complete", "Complete");
                    }
                    using(var transaction = new Transaction(doc,"FindDuplicationFoudantion"))
                    {
                        transaction.Start();
                        if (newInstances != null)
                        {
                            foreach(var instance in newInstances)
                            {
                                var intersect = FindIntersectFoundation(instance, doc);
                                if (intersect != null)
                                    foreach(var ins in intersect)
                                    {
                                        doc.Delete(ins.Id);
                                    }

                            }
                        }
                        transaction.Commit();
                    }
                    return Result.Succeeded;
                }

                return Result.Cancelled;


            }
            catch (Exception e)
            {
                message = e.Message;

                return Result.Failed;
            }

        }
        private IEnumerable<Element> FindIntersectFoundation(Element element,Document doc)
        {
            var boundingBox = element.get_BoundingBox(doc.ActiveView);
            var outLine = new Outline(boundingBox.Min, boundingBox.Max);
            var bbFilter = new BoundingBoxIntersectsFilter(outLine);
            FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            ICollection<ElementId> idsExclude = new List<ElementId>();
            idsExclude.Add(element.Id);
            var intersections = collector.Excluding(idsExclude).WherePasses(bbFilter).ToElements();
            var foundationIntersections = from intersectElement in intersections
                                         where intersectElement is FamilyInstance instance && CheckInstanceIsFoundation(instance, doc)
                                         select intersectElement;
            return foundationIntersections;

        }



        /// <summary>
        /// Tạo Type móng
        /// </summary>
        /// <param name="tnfList"></param>
        /// <param name="doc"></param>
        private void CreateFoundationType(List<TNFBase> tnfList, Document doc, Dictionary<string, FamilySymbol> tnfFoundationDict)
        {
            // Check if this is Takeuchi Bim File
            var collector = new FilteredElementCollector(doc);
            var symbols = collector.OfClass(typeof(ElementType)).WhereElementIsElementType().ToElements();

            ElementType normalSymbol = null;
            ElementType dTypeSymbol = null;

            foreach (var element in symbols)
            {
                if (normalSymbol != null && dTypeSymbol != null)
                {
                    break;
                }

                if (normalSymbol == null && element is ElementType familySymbol && familySymbol.FamilyName == "基礎_通常")
                {
                    normalSymbol = familySymbol;
                }

                if (dTypeSymbol == null && element is ElementType familySymbol2 && familySymbol2.FamilyName == "基礎_D")
                {
                    dTypeSymbol = familySymbol2;
                }

            }



            foreach (var tnfPackage in tnfList)
            {
                var tnfSymbol = GetExistingElement(symbols, tnfPackage.FullName);

                var tnfFoundation = tnfPackage as TNFFoundation;
                // if it is not existed

                if (tnfSymbol == null)
                {
                    if (tnfFoundation.FoundationType == TNFString.TNF_DTYPE)
                    {

                        tnfSymbol = dTypeSymbol.Duplicate(tnfPackage.FullName) as FamilySymbol;
                    }
                    else
                    {
                        tnfSymbol = normalSymbol.Duplicate(tnfPackage.FullName) as FamilySymbol;
                    }
                }

                if (tnfSymbol != null)
                {
                    SetTNFParameters(tnfSymbol.Parameters, tnfPackage.Parameters);
                    tnfFoundationDict.Add(tnfPackage.Name, tnfSymbol as FamilySymbol);
                }
                //if (tnfSymbol != null)
                //{
                //    var tnfSetParametes = TnfSetParametersFactory.CreateSetTnf(tnfPackage, tnfSymbol);
                //    tnfSetParametes.SetParameters();

                //}
                //else
                //{
                //    message = "Can not Create new Symmbol";
                //    return Result.Failed;
                //}

            }
        }
        /// <summary>
        /// Set Param cho TNF Package
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="tnfParams"></param>
        /// <param name="elementParamsDict"></param>
        private void SetTNFParameters(ParameterSet parameters, List<TNFParameter> tnfParams, Dictionary<string, ElementId> elementParamsDict = null)
        {
            var revitParams = parameters.Cast<Parameter>().ToList();
            foreach (var tnfParam in tnfParams)
            {
                var revitParam = revitParams.FirstOrDefault(x => x.Definition.Name == tnfParam.JName || x.Definition.Name == tnfParam.EName);
                if (revitParam != null)
                {
                    if (revitParam.IsReadOnly)
                        continue;
                    switch (revitParam.Definition.ParameterType)
                    {
                        case ParameterType.Length:
                            {
                                var value = Convert.ToDouble(tnfParam.Value).MmToInch();
                                revitParam.Set(value);
                            }

                            break;
                        case ParameterType.Text:
                            revitParam.Set(tnfParam.Value);
                            break;
                        case ParameterType.Integer:
                        case ParameterType.Number:
                            {
                                var value = Convert.ToInt32((string)tnfParam.Value);
                                revitParam.Set(value);
                            }
                            break;
                        case ParameterType.YesNo:
                            {
                                var value = tnfParam.Value == "Yes" ? 1 : 0;
                                revitParam.Set(value);
                            }
                            break;
                        case ParameterType.Invalid:
                            if (revitParam.StorageType is StorageType.ElementId)
                            {
                                if (elementParamsDict != null)
                                {
                                    elementParamsDict.TryGetValue(tnfParam.EName, out var elementId);
                                    if(elementId!=null)
                                    revitParam.Set(elementId);
                                }

                            }
                            break;

                    }
                }
            }
        }
        /// <summary>
        /// Lấy ra đối tượng đã tồn tại
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private ElementType GetExistingElement(IList<Element> elements, string name)
        {
            // Checkif tnfFoundation is existed in Database
            foreach (var element in elements)
            {
                if (element is ElementType tnfElement && tnfElement.Name == name)
                {
                    return tnfElement;

                }
            }

            return null;

        }
        private void CreateBeamType(List<TNFBase> tnfBeams, Document doc)
        {
            // Check if this is Takeuchi Bim File
            var collector = new FilteredElementCollector(doc);
            var symbols = collector.OfClass(typeof(ElementType)).WhereElementIsElementType().ToElements();

            var defaultBeam = symbols.FirstOrDefault(x => ((ElementType)x).FamilyName == TNFString.TNF_BEAM_FAMILY_NAME) as ElementType;



            foreach (var tnfPackage in tnfBeams)
            {
                var tnfSymbol = GetExistingElement(symbols, tnfPackage.FullName);
                ElementType elementType = null;
                var tnfFoundation = tnfPackage as TNFBeamType;
                // if it is not existed
                if (tnfSymbol != null)
                {
                    elementType = tnfSymbol;
                }
                else
                {
                    elementType = defaultBeam.Duplicate(tnfPackage.FullName);
                }
                if (elementType != null)
                {
                    SetTNFParameters(elementType.Parameters, tnfPackage.Parameters);
                }

            }
        }
        /// <summary>
        /// Tạo mới các cải tạo
        /// Ứng với mỗi cải tạo sẽ tạo 1 Curtain Panel
        /// </summary>
        /// <param name="tnfList"></param>
        /// <param name="doc"></param>
        private void CreateRoofTypes(Dictionary<TNFBase, TNFBase> tnfList, Document doc)
        {
            foreach (var tnf in tnfList)
            {
                var elementIdParamsDict = new Dictionary<string, ElementId>();
                CreateCurtainPanel(tnf.Value, doc, elementIdParamsDict);
                CreateRoofType(tnf.Key, doc, elementIdParamsDict);
            }
        }

        /// <summary>
        /// Tạo mới CurtainPanel
        /// </summary>
        /// <param name="tnf"></param>
        /// <param name="doc"></param>
        /// <param name="elementIdParamsDict"></param>
        private void CreateCurtainPanel(TNFBase tnf, Document doc, Dictionary<string, ElementId> elementIdParamsDict)
        {
            var collector = new FilteredElementCollector(doc);
            var panelSymbols = collector.OfClass(typeof(PanelType)).WhereElementIsElementType().ToElements();
            var defaultCurtainPanel = panelSymbols.FirstOrDefault(x => ((PanelType)x).FamilyName == TNFString.TNF_CURTAINPANEL_FAMILY_NAME && x.Name.Contains(TNFString.TNF_CURTAINPANEL_FAMILY_STRING)) as ElementType;

            // Nếu đã có sẵn thì ko cần tạo mới
            var tnfCurtainPanel = panelSymbols.FirstOrDefault(x => x.Name == tnf.FullName);
            // Chưa có sẵn thì phải tạo mới.
            if (tnfCurtainPanel == null)
            {
                var newTNFCurtainPanel = defaultCurtainPanel.Duplicate(tnf.FullName);
                SetTNFParameters(newTNFCurtainPanel.Parameters, tnf.Parameters);
                if (!elementIdParamsDict.ContainsKey(TNFString.TNF_CURTAINPANEL_PARAMS_NAME))
                    elementIdParamsDict.Add(TNFString.TNF_CURTAINPANEL_PARAMS_NAME, newTNFCurtainPanel.Id);
                else
                {
                    elementIdParamsDict[TNFString.TNF_CURTAINPANEL_PARAMS_NAME] = newTNFCurtainPanel.Id;
                }
            }
            else
            {
                SetTNFParameters(tnfCurtainPanel.Parameters, tnf.Parameters);
                if (!elementIdParamsDict.ContainsKey(TNFString.TNF_CURTAINPANEL_PARAMS_NAME))
                    elementIdParamsDict.Add(TNFString.TNF_CURTAINPANEL_PARAMS_NAME, tnfCurtainPanel.Id);
                else
                {
                    elementIdParamsDict[TNFString.TNF_CURTAINPANEL_PARAMS_NAME] = tnfCurtainPanel.Id;
                }
            }

        }
        /// <summary>
        /// Tạo mới 1 cải tạo đơn lẻ
        /// </summary>
        /// <param name="tnf"></param>
        /// <param name="doc"></param>
        /// <param name="elementIdParamsDict"></param>
        private void CreateRoofType(TNFBase tnf, Document doc, Dictionary<string, ElementId> elementIdParamsDict)
        {
            var collector = new FilteredElementCollector(doc);
            var roofPanelSymbols = collector.OfClass(typeof(RoofType)).ToElements();
            var defaultRoofType = roofPanelSymbols.FirstOrDefault(x => x.Name.Contains(TNFString.TNF_CURTAINPANEL_FAMILY_STRING));
            var existingElement = roofPanelSymbols.FirstOrDefault(x => x.Name == tnf.FullName);


            ElementType roofType = null;

            if (existingElement == null)
            {
                roofType = ((ElementType)defaultRoofType).Duplicate(tnf.FullName);
            }
            else
            {
                roofType = existingElement as ElementType;
            }

            SetTNFParameters(roofType.Parameters, tnf.Parameters, elementIdParamsDict);
        }
        /// <summary>
        /// Create Foundation Instance
        /// </summary>
        private List<Element> CreateFoundationInstance(List<TNFBase> tnfIntances, Document doc, Dictionary<string, FamilySymbol> foundationTypes)
        {
            var newElements = new List<Element>();

            var grids = GetGrids(doc);
            var firstAsk = true;
            var isUpdateLocation = false;

            foreach (var tnf in tnfIntances)
            {
                if (tnf is TNFFoundationInstance tnfIntance)
                {
                    var gridStrings = tnfIntance.GridIntersection.Split('-');
                    var gridStringX = gridStrings[0];
                    var gridStringY = gridStrings[1];
                    var gridPoint = GetGridIntersection(gridStringX, gridStringY, grids);
                    var insertPoint = new XYZ(gridPoint.X + tnfIntance.GridOffsetX.MmToInch(), gridPoint.Y + tnfIntance.GridOffsetY.MmToInch(), gridPoint.Z);

                    if (!string.IsNullOrEmpty(tnfIntance.FoundationId))
                    {
                        var famInstance = GetExistingFoundationInstance(tnfIntance.FoundationId, doc);
                        // Nếu đã tồn tại móng có sẵn thì
                        if (famInstance != null)
                        {
                            if (firstAsk)
                            {
                                firstAsk = false;
                                var td = new TaskDialog("LocationConfirm");
                                td.MainContent = "Keep or update foundation instance location";
                                td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Keep Location", "");
                                td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Update Location");
                                switch (td.Show())
                                {
                                    case TaskDialogResult.CommandLink1:
                                        isUpdateLocation = false;
                                        break;
                                    case TaskDialogResult.CommandLink2:
                                        isUpdateLocation = true;
                                        break;
                                }

                            }
                            if (isUpdateLocation)
                            {
                                ((LocationPoint)famInstance.Location).Point = insertPoint;
                            }
                            else
                            {
                                var instanceLocation = ((LocationPoint)famInstance.Location).Point;
                                // update lại thông tin vị trí để lưu vào excel

                                if (instanceLocation != insertPoint)
                                {
                                    var offsetX = instanceLocation.X - gridPoint.X;
                                    var offsetY = instanceLocation.Y - gridPoint.Y;
                                    tnfIntance.GridOffsetX = offsetX.InchToMm();
                                    tnfIntance.GridOffsetY = offsetY.InchToMm();
                                }
                                insertPoint = instanceLocation;
                            }

                            // Update thông tin Type móng
                            var familyType = doc.GetElement(famInstance.GetTypeId()) as ElementType;
                            if (familyType != null)
                            {
                                var typeMark = familyType.GetParameters(TNFString.TNF_JTYPEMARK).FirstOrDefault();
                                if (typeMark == null)
                                {
                                    typeMark = familyType.GetParameters(TNFString.TNF_ETYPEMARK).FirstOrDefault();
                                }
                                if (typeMark != null)
                                {
                                    var typeMarkString = typeMark.AsString();
                                    // Nếu type móng trong Excel và revit giống nhau thì chỉ update lại thông tin mới cho móng
                                    if (typeMarkString == tnfIntance.Name)
                                    {
                                        SetTNFParameters(famInstance.Parameters, tnfIntance.Parameters);

                                    }
                                    // Nếu type móng trong Excel và Revit khác nhau thì xóa móng cũ đi, tạo móng mới
                                    else
                                    {
                                        doc.Delete(famInstance.Id);
                                        var newElement = CreateFamilyInstance(tnfIntance, doc, foundationTypes, insertPoint);
                                        if (newElement != null)
                                            newElements.Add(newElement);
                                    }
                                }
                            }


                        }
                        else
                        {
                            tnfIntance.FoundationId = string.Empty;
                            var newElement = CreateFamilyInstance(tnfIntance, doc, foundationTypes, insertPoint);
                            if(newElement!=null)
                            newElements.Add(newElement);
                        }

                    }
                    // chưa tồn tại thì tạo mới
                    else
                    {
                        var newElement = CreateFamilyInstance(tnfIntance, doc, foundationTypes, insertPoint);
                        if (newElement != null)
                            newElements.Add(newElement);
                    }
                }
            }
            return newElements;

        }
        private Element CreateFamilyInstance(TNFFoundationInstance tnfIntance, Document doc, Dictionary<string, FamilySymbol> foundationTypes, XYZ insertPoint)
        {
            foundationTypes.TryGetValue(tnfIntance.Name, out var tnfFoundation);
            if (tnfFoundation != null)
            {
                var famInstance = doc.Create.NewFamilyInstance(insertPoint, tnfFoundation, Autodesk.Revit.DB.Structure.StructuralType.Footing);
                SetTNFParameters(famInstance.Parameters, tnfIntance.Parameters);
                tnfIntance.FoundationId = famInstance.UniqueId;
                return famInstance;
            }
            return null;

        }

        private bool CheckInstanceIsFoundation(FamilyInstance instance,Document doc)
        {
            var familyTypeId = instance.GetTypeId();
            var familyType = doc.GetElement(familyTypeId);
            if(familyType is FamilySymbol fSymbol)
            {
                if (fSymbol.FamilyName == TNFString.TNF_FOUNDATION_DTYPE || fSymbol.FamilyName == TNFString.TNF_FOUNDATION_NORMAL)
                    return true;
                return false;
            }
            return false;
        }
        private FamilyInstance GetExistingFoundationInstance(string elementId, Document doc)
        {
            var collector = new FilteredElementCollector(doc);
            var element = doc.GetElement(elementId);
            if (element is FamilyInstance famInstance)
            {
                return famInstance;
            }
            else
            {
                return null;
            }
        }

        private XYZ GetGridIntersection(string xGrid, string yGrid, IList<Element> elements)
        {
            var gridX = elements.FirstOrDefault(x => x.Name == xGrid);
            var gridY = elements.FirstOrDefault(y => y.Name == yGrid);

            var xLine = ((Grid)gridX).Curve;
            var yLine = ((Grid)gridY).Curve;
            var intersection = xLine.Intersect(yLine, out var interSectionResult);
            if (intersection != SetComparisonResult.Disjoint)
                return interSectionResult.get_Item(0).XYZPoint;
            return null;

        }

        private IList<Element> GetGrids(Document doc)
        {
            var collector = new FilteredElementCollector(doc);
            return collector.OfClass(typeof(Grid)).WhereElementIsNotElementType().ToElements();

        }

        private void GetFoundations(Document doc, Dictionary<string, FamilySymbol> foudationTypes)
        {
            var collector = new FilteredElementCollector(doc);
            var symbols = collector.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().ToElements();

            foreach (var symbol in symbols)
            {
                if (symbol is ElementType elementType && (elementType.FamilyName == TNFString.TNF_FOUNDATION_DTYPE || elementType.FamilyName == TNFString.TNF_FOUNDATION_NORMAL))
                {
                    var typeMarkE = symbol.GetParameters(TNFString.TNF_ETYPEMARK).FirstOrDefault();
                    var typeMarkJ = symbol.GetParameters(TNFString.TNF_JTYPEMARK).FirstOrDefault();
                    var typeMark = "";
                    if (typeMarkE != null)
                    {
                        typeMark = typeMarkE.AsString();

                    }
                    else if (typeMarkJ != null)
                    {
                        typeMark = typeMarkJ.AsString();
                    }
                    if (!string.IsNullOrEmpty(typeMark))
                    {
                        if (!foudationTypes.ContainsKey(typeMark))
                            foudationTypes.Add(typeMark, symbol as FamilySymbol);
                    }

                }
            }
        }

    }
}
