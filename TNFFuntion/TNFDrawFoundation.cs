using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFFuntion
{

    public class TNFDrawFoundation
    {
        private Document _acDoc;
        private Database _acCurDb;
        public TNFDrawFoundation()
        {
            _acDoc = Application.DocumentManager.MdiActiveDocument;
            _acCurDb = _acDoc.Database;
        }
        [CommandMethod("TNFDrawFoudnation")]
        public void DrawFoundation()
        {
            PromptStringOptions pStrOpts = new PromptStringOptions("\nEnter Foundation Size(seperated by commas):<2400,2400>: ");
            pStrOpts.DefaultValue = "2400,2400";
            pStrOpts.UseDefaultValue = true;
            PromptResult pStrRes = _acDoc.Editor.GetString(pStrOpts);
            if (string.IsNullOrEmpty(pStrRes.StringResult))
                return;

            var dimensions = pStrRes.StringResult.Split(',');
            if (dimensions.Length > 1)
            {
                var checkA = double.TryParse(dimensions[0], out var width);
                var checkB = double.TryParse(dimensions[1], out var height);
                if (checkA && checkB)
                {
                    if (!SelectionGrid.Select(out var vLine, out var hLine, out var selectionResult, "TP 0-F 基準線"))
                        return;
                    var points = TNFUtilities.GerInterSectionPoint(vLine,hLine);
                    //TNFUtilities.DrawColEnt(points.ToArray());

                    using (var transaction = _acDoc.Database.TransactionManager.StartTransaction())
                    {
                        var acBlockTable = transaction.GetObject(_acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                        var acBlockTableRecord = transaction.GetObject(acBlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                        foreach (var point in points)
                        {

                            var tnfRect = new TNFRect(point, width, height);
                            var pline = new Polyline(tnfRect.Vertices.Count);
                            pline.SetDatabaseDefaults();
                            pline.Layer = "TP 0-1 基礎";
                            var verticeIndex = 0;
                            foreach (var vertice in tnfRect.Vertices)
                            {
                                pline.AddVertexAt(verticeIndex, vertice, 0, 0, 0);
                                verticeIndex++;
                            }
                            pline.Closed = true;

                            TNFUtilities.AddEntities(pline, acBlockTableRecord, transaction);
                        }

                        transaction.Commit();
                    }

                }

            }
        }




    }
}
