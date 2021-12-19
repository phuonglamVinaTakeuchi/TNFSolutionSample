using Autodesk.AutoCAD.ApplicationServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.Runtime;
using TNFData.Models.Section;
using TNFForm.Views;
using MaterialDesignThemes;
using MaterialDesignColors;
using TNFData.Factory;
using devDept.Geometry;
using Autodesk.AutoCAD.EditorInput;
using TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationDraws;
using TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationSectionDraws;
using TNFSectionAddOn.Factory;
using TNFSectionAddOn.Drawings.BeamSectionDrawings;
using TNFSectionAddOn.Drawings;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;

namespace TNFSectionAddOn
{
    public class TNFSectionAddon
    {
        private Document _acDoc = Application.DocumentManager.MdiActiveDocument;

        public TNFSectionAddon()
        {
            var acDoc = Application.DocumentManager.MdiActiveDocument;
        }
        [CommandMethod("FCrossSection")]
        public void FCrossCommand()
        {
            var acDoc = Application.DocumentManager.MdiActiveDocument;
            var acCurrentDb = acDoc.Database;

            // Hack for assembly Loaded
            var material = new MaterialDesignThemes.Wpf.ColorZone();
            var tnfSection = new TNFSection();
            var dialog = new TNFWindow(tnfSection);
            var resul = Application.ShowModalWindow(dialog);
            if(resul.Value)
            {
                using (var transactions = acCurrentDb.TransactionManager.StartTransaction())
                {
                    GeneralDimStyle(transactions, acCurrentDb,tnfSection.TNFParameters.ScaleRatio);
                    transactions.Commit();
                }
                    var promptPointOption = new PromptPointOptions("Please enter point to insert your Section: ");
                var prompPointResult = _acDoc.Editor.GetPoint(promptPointOption);
                if (prompPointResult.Status != PromptStatus.OK)
                    return;

                var basePoint = new Point2D(prompPointResult.Value.X, prompPointResult.Value.Y);
                var tnfSectonG = TNFSectionGFactory.CreateSectionGeometry(basePoint,tnfSection);
                var sectionDraw = new TNFSectionDrawing(tnfSectonG);
                sectionDraw.Draw();


            }
        }
        private void GeneralDimStyle(Transaction transaction, Database currentDB,double scaleRatio)
        {
            var dimStyleTable = transaction.GetObject(currentDB.DimStyleTableId, OpenMode.ForWrite) as DimStyleTable;
            //var blockTable = transaction.GetObject(currentDB.BlockTableId, OpenMode.ForRead) as BlockTable;
            //var acBlockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
            var textStyleTable = transaction.GetObject(currentDB.TextStyleTableId, OpenMode.ForWrite) as TextStyleTable;
            TextStyleTableRecord textStyle = null;
            // neu chua co textStyle thi tao moi, co roi thi thoi
            if (!textStyleTable.Has("ＭＳ ゴシック"))
            {
                var textStyleRecord = new TextStyleTableRecord();
                textStyleRecord.Name = "ＭＳ ゴシック";
                textStyleRecord.Font = new FontDescriptor("MS Gothic", false, false, 0, 0);
                textStyleTable.Add(textStyleRecord);
                transaction.AddNewlyCreatedDBObject(textStyleRecord, true);
                textStyle = textStyleRecord;
            }
            else
            {
                var textStyleID = textStyleTable["ＭＳ ゴシック"];
                textStyle = transaction.GetObject(textStyleID, OpenMode.ForRead) as TextStyleTableRecord;
            }
            string dimStyleName = "1-300" + "(" + scaleRatio + " - DimStyle)";
            if (!dimStyleTable.Has(dimStyleName))
            {
                var dimStyleRecord = transaction.GetObject(dimStyleTable["1-300"], OpenMode.ForRead) as DimStyleTableRecord;

                dimStyleRecord = dimStyleRecord.Clone() as DimStyleTableRecord;

                dimStyleRecord.Name = dimStyleName;
                //dimStyleRecord.Dimtxsty = textStyle.Id;
                //dimStyleRecord.Dimtxt = 3;
                //dimStyleRecord.Dimclrt = Color.FromRgb(255,255,255);
                //dimStyleRecord.Dimtad = 3;
                //dimStyleRecord.Dimexo = 0.0625;
                //var dimBlk1 = GetArrowObjectId("DIMBLK1","_DOTSMALLBLANK");
                //dimStyleRecord.Dimblk1 =dimBlk1;
                //dimStyleRecord.Dimblk2 =dimBlk1;
                //dimStyleRecord.Dimasz = 1.2;
                //dimStyleRecord.Dimgap = 0.3;
                //dimStyleRecord.Dimtih = false;
                //dimStyleRecord.Dimtoh = false;
                dimStyleRecord.Dimscale = 60 * scaleRatio;
                dimStyleRecord.Dimlfac = 1 / scaleRatio;
                //dimStyleRecord.Dimlunit = 6;
                //dimStyleRecord.Dimaltu = 6;
                //dimStyleRecord.Dimaltd = 0;
                //dimStyleRecord.Dimdec = 1;
                //dimStyleRecord.Dimdec = 0;
                //dimStyleRecord.Dimadec = 1;
                //dimStyleRecord.Dimaltrnd = 5;
                dimStyleTable.Add(dimStyleRecord);
                transaction.AddNewlyCreatedDBObject(dimStyleRecord, true);
            }
        }
    }
}
