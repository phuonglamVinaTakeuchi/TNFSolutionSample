using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using TNFAutoFoundation.TNFModel;

namespace TNFAutoFoundation.ReadingExcel
{
    public class TNFExcelReading
    {
        public List<TNFBase> TNFFoundationTypes { get; private set; }
        public List<TNFBase> TNFFoundationIntances { get; private set; }
        public Dictionary<TNFBase,TNFBase> TNFImproves { get; private set; }
        public List<TNFBase> TNFBeams { get; private set; }
        public TNFExcelReading(string filePath)
        {
            var excel = new Application();
            var workbook = excel.Workbooks.Open(filePath);
            TNFFoundationTypes = ReadSheet<TNFFoundation>(workbook, TNFString.TNF_FTYPESHEET);
            TNFFoundationIntances = ReadSheet<TNFFoundationInstance>(workbook, TNFString.TNF_FINSTANCESHEET);
            TNFImproves = ReadSheet<TNFImprovement, TNFCurtainPanel>(workbook, TNFString.TNF_IMPROVESHEET);
            TNFBeams = ReadSheet<TNFBeamType>(workbook, TNFString.TNF_BEAMTYPESHEET);


            excel.Quit();
            Marshal.ReleaseComObject(excel);

        }
        private List<TNFBase> ReadSheet<T>(Workbook workBook, string sheetName) where T : TNFBase, new()
        {
            var tnfItems = new List<TNFBase>();

            var workSheet = workBook.Worksheets[sheetName] as Worksheet;
            var sheetRange = workSheet?.UsedRange;
            var maxColumn = sheetRange?.Columns.Count;
            var maxRow = sheetRange?.Rows.Count;

            var firstDataColumnIndex = GetFirstIndexForData(sheetRange);
            // Tìm vị trí cột đầu tiên cho data

            var jNameIndex = firstDataColumnIndex - 1;
            var eNameIndex = jNameIndex - 1;
            var nameIndex = eNameIndex - 1;

            var hasNext = true;
            for (var columnIndex = firstDataColumnIndex; columnIndex <= maxColumn; columnIndex++)
            {
                if (hasNext == false)
                    break;
                var tnfItem = new T();
                var firstRow = true;
                for (var rowIndex = 3; rowIndex <= maxRow; rowIndex++)
                {
                    var cell = sheetRange.Cells[rowIndex, columnIndex] as Range;
                    if(firstRow)
                    {
                        firstRow = false;
                        if (string.IsNullOrEmpty(cell?.Value.ToString()))
                        {
                            hasNext = false;
                            break;

                        }
                    }

                        var value = string.Empty;
                        if(cell.Value!= null)
                        {
                            value = cell.Value.ToString();
                        }

                        var jName = sheetRange.Cells[rowIndex, jNameIndex].Value;
                        var eName = sheetRange.Cells[rowIndex, eNameIndex].Value;
                        var name = sheetRange.Cells[rowIndex, nameIndex].Value;
                        if (name == "TypeMark" )
                        {
                            tnfItem.Name = value;
                        }
                        if(name == "FoundationId")
                        {
                            if(tnfItem is TNFFoundationInstance tnfFoundation)
                            {
                                tnfFoundation.IdRowIndex = rowIndex;
                                tnfFoundation.IdColumnIndex = columnIndex;
                                if(!string.IsNullOrEmpty(value))
                                tnfFoundation.FoundationId = value;
                            }
                        }
                        CreateParams(jName, eName, name,value, tnfItem);
                }
                if (tnfItem.Parameters.Count > 0)
                {
                    tnfItem.InnitTNFPackageName();
                    tnfItems.Add(tnfItem);
                }
            }
            return tnfItems;

        }
        private int GetFirstIndexForData(Range sheetRange)
        {
            var searchRow = 2;
            var searchCol = 1;

            do
            {
                var cell = sheetRange.Cells[searchRow, searchCol] as Range;
                if (cell?.Value == "RevitNameJP")
                {
                    return searchCol + 1;
                }
                searchCol++;
            } while (true);
        }
        private Dictionary<TNFBase, TNFBase> ReadSheet<T1, T2>(Workbook workBook, string sheetName) where T1 : TNFBase, new() where T2 : TNFBase, new()
        {

            var tnfImproveDict = new Dictionary<TNFBase, TNFBase>();
            var workSheet = workBook.Worksheets[sheetName] as Worksheet;
            var sheetRange = workSheet?.UsedRange;
            var maxColumn = sheetRange?.Columns.Count;
            var maxRow = sheetRange?.Rows.Count;
            var firstDataColumnIndex = GetFirstIndexForData(sheetRange);
            var jNameIndex = firstDataColumnIndex - 1;
            var eNameIndex = jNameIndex - 1;
            var nameIndex = eNameIndex - 1;
            var familyTypeIndex = nameIndex - 3;
            var hasNext = true;
            for (var columnIndex = firstDataColumnIndex; columnIndex <= maxColumn; columnIndex++)
            {
                if (hasNext == false)
                    break;
                var tnfRoof = new T1();
                var tnfCurtainPanel = new T2();

                for (var rowIndex = 3; rowIndex <= maxRow; rowIndex++)
                {
                    var cell = sheetRange.Cells[rowIndex, columnIndex] as Range;
                    if (string.IsNullOrEmpty(cell?.Value.ToString()))
                    {
                        hasNext = false;
                        break;

                    }
                    else
                    {
                        var value = cell.Value.ToString();
                        var jName = sheetRange.Cells[rowIndex, jNameIndex].Value;
                        var eName = sheetRange.Cells[rowIndex, eNameIndex].Value;
                        var name = sheetRange.Cells[rowIndex, nameIndex].Value;
                        var familyType = sheetRange.Cells[rowIndex, familyTypeIndex].Value.ToString();
                        if (name == "TypeName")
                        {
                            tnfRoof.Name = value;
                            tnfCurtainPanel.Name = value;
                        }
                        switch (familyType)
                        {
                            case "(Roof)":
                                CreateParams(jName, eName,name,value, tnfRoof);
                                break;
                            case "(Curtain Panel)":
                                CreateParams(jName,  eName, name, value, tnfCurtainPanel);
                                break;
                            default:
                                break;
                        }
                    }

                }
                if (tnfRoof.Parameters.Count > 0 && tnfCurtainPanel.Parameters.Count>0)
                {
                    tnfRoof.InnitTNFPackageName();
                    tnfCurtainPanel.InnitTNFPackageName();
                    tnfImproveDict.Add(tnfRoof,tnfCurtainPanel);
                }
            }
            return tnfImproveDict;
        }

        private void CreateParams(string jName, string eName, string name, string value, TNFBase tnfItem)
        {
            if (string.IsNullOrEmpty(jName))
            {
                var property = tnfItem.GetType().GetProperty(name) as PropertyInfo;
                if (property != null && property.CanWrite)
                {
                    switch (property.PropertyType)
                    {
                        case Type _ when property.PropertyType == typeof(int):
                            property.SetValue(tnfItem, Convert.ToInt32(value));
                            break;
                        case Type _ when property.PropertyType == typeof(double):
                            property.SetValue(tnfItem, Convert.ToDouble(value));
                            break;
                        case Type _ when property.PropertyType == typeof(string):
                            property.SetValue(tnfItem, value);
                            break;
                        default:
                            break;
                    }

                }
            }
            else
            {
                var tnfParam = new TNFParameter()
                {
                    Value = value,
                    JName = jName,
                    EName = eName,
                    Name = name,
                };
                tnfItem.Parameters.Add(tnfParam);
            }
        }
    }
}
