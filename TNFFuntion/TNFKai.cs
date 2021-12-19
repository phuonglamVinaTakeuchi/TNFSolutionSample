using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TNFFuntion
{

    public class TNFKai
    {
        //private int _test = 0;
        private Document _acDoc;
        private Database _acCurDb;
        public TNFKai()
        {
            _acDoc = Application.DocumentManager.MdiActiveDocument;
            _acCurDb = _acDoc.Database;
        }
        [CommandMethod("TNFKai")]
        public void TNFKaiDraw()
        {

            PromptStringOptions pStrOpts = new PromptStringOptions("\nOffsets(seperated by commas):<2600,1200>: ");
            pStrOpts.DefaultValue = "2600,1200";
            pStrOpts.UseDefaultValue = true;


            PromptResult pStrRes = _acDoc.Editor.GetString(pStrOpts);
            if (string.IsNullOrEmpty(pStrRes.StringResult))
                return;

            var dimensions = pStrRes.StringResult.Split(',');
            if (dimensions.Length > 1)
            {
                var checkA = double.TryParse(dimensions[0], out var outSizeOffset);
                var checkB = double.TryParse(dimensions[1], out var innerSizeOffset);
                if (checkA && checkB)
                {

                    if (!SelectionGrid.Select(out var vLine, out var hLine, out var selectionResult, "TP 0-F 基準線"))
                        return;

                    /// vLine đường thẳng nằm đứng.
                    /// hLine đường thẳng nằm ngang
                    var maxRevo = TNFUtilities.GetMaxRevo(vLine,hLine);
                    maxRevo.Add(maxRevo[maxRevo.Count - 1]);
                    var maxRevoPline = TNFUtilities.CreatePolyLine(maxRevo, "TP 0-3 一次改良");
                    maxRevoPline.Closed = true;

                    var sortedXs = TNFUtilities.GetMatrixPoints(vLine);
                    var sortedYs = TNFUtilities.GetMatrixPoints(hLine, false);

                    var tnfRectList = GetTNFRectList(sortedXs.ToArray(), sortedYs.ToArray(), innerSizeOffset, maxRevoPline);

                    if (!SelectionFoundation.Select(out var foundationSelectResult))
                        return;

                    if (foundationSelectResult == null || foundationSelectResult.Value == null || foundationSelectResult.Value.Count < 1)
                    {
                        var offsetRevo = maxRevoPline.GetOffsetCurves(-outSizeOffset);
                        Polyline revoPl = null;

                        foreach(var ent in offsetRevo)
                        {
                            revoPl = ent as Polyline;
                            break;

                        }
                        DrawRectList(tnfRectList, null, revoPl);
                    }
                    else
                    {
                        var offsetRevo = maxRevoPline.GetOffsetCurves(-outSizeOffset);
                        Polyline revoPl = null;

                        foreach (var ent in offsetRevo)
                        {
                            revoPl = ent as Polyline;
                            break;

                        }
                        var tbfFoundation = TNFUtilities.GetEntities(foundationSelectResult.Value);
                        DrawRectList(tnfRectList, tbfFoundation, revoPl);
                    }


                }

                return;
            }
            return;

        }
        private bool IsInterSect(TNFRevonation tnfRevonation, Polyline tnfFoundation)
        {
            var minA = tnfRevonation.BottomLeft;
            var maxA = tnfRevonation.TopRight;

            var minB = tnfFoundation.GeometricExtents.MinPoint;
            var maxB = tnfFoundation.GeometricExtents.MaxPoint;

            if (minA.X >= maxB.X || maxA.X <= minB.X || minA.Y >= maxB.Y || maxA.Y <= minB.Y)
                return false;
            return true;
        }
        private List<Polyline> GetOffsetFoundation(List<Entity> tnfFoundations)
        {
            if (tnfFoundations == null) return null;
            var result = new List<Polyline>();
            foreach (Polyline tnfFoundation in tnfFoundations)
            {
                var orignalPlength = TNFUtilities.GetLength(tnfFoundation);
                var tnfOffset = tnfFoundation.GetOffsetCurves(-800);
                var newPLength = TNFUtilities.GetLength(tnfOffset[0] as Polyline);
                if (newPLength < orignalPlength)
                {
                    tnfOffset = tnfFoundation.GetOffsetCurves(800);
                }
                foreach (var tnfPline in tnfOffset)
                {
                    result.Add((Polyline)tnfPline);
                }
                ///result.Add(tnfOffset);
            }
            return result;
        }
        private void DrawRectList(List<TNFRevonation> rectList, List<Entity> tnfFoundations,Entity maxRevo)
        {
            var offsetTBFFoundation = GetOffsetFoundation(tnfFoundations);
            foreach (var tnfRect in rectList)
            {
                if (offsetTBFFoundation != null)
                {
                    foreach (var tnfFound in offsetTBFFoundation)
                    {
                        if (IsInterSect(tnfRect, tnfFound))
                        {
                            tnfRect.AddFoundationCross(tnfFound);
                            tnfRect.SetCrossFoundation(tnfFound);

                        }

                    }
                }

                //tnfRect.GeneralVertices();

            }


            // Kiểm tra cải tạo có giao với móng không


            using (var transaction = _acDoc.Database.TransactionManager.StartTransaction())
            {
                var acBlockTable = transaction.GetObject(_acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                var acBlockTableRecord = transaction.GetObject(acBlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                //var offsetCurve = ((Polyline)maxRevo).GetOffsetCurves();
                TNFUtilities.AddEntities(maxRevo, acBlockTableRecord, transaction);


                foreach (var tnfRect in rectList)
                {
                    // Draw TNFRect
                    var pline = new Polyline(tnfRect.Vertices.Count);
                    pline.SetDatabaseDefaults();
                    pline.Layer = "TP 0-3 一次改良";
                    var verticeIndex = 0;
                    foreach (var vertice in tnfRect.Vertices)
                    {
                        pline.AddVertexAt(verticeIndex, vertice, 0, 0, 0);
                        verticeIndex++;
                    }
                    pline.Closed = true;

                    TNFUtilities.AddEntities(pline, acBlockTableRecord, transaction);

                    /// Draw Cross
                    var crossLine1 = new Line(tnfRect.BottomLeftCrossPoint.ToPoint3d(), tnfRect.TopRightCrossPoint.ToPoint3d());
                    crossLine1.Layer = "TP 0-4二次改良";
                    TNFUtilities.AddEntities(crossLine1, acBlockTableRecord, transaction);

                    var crossLine2 = new Line(tnfRect.TopLeftCrossPoint.ToPoint3d(), tnfRect.BottomRightCrossPoint.ToPoint3d());
                    crossLine2.Layer = "TP 0-4二次改良";
                    TNFUtilities.AddEntities(crossLine2, acBlockTableRecord, transaction);


                }
                transaction.Commit();
            }


        }

        private List<TNFRevonation> GetTNFRectList(double[] xPoints, double[] yPoints, double translate,Polyline maxRevo )
        {
            var tnfRectList = new List<TNFRevonation>();
            var xLength = xPoints.Length;
            var yLength = yPoints.Length;
            for (int i = 0; i < xLength - 1; i++)
            {
                for (int j = 0; j < yLength - 1; j++)
                {

                    var x1 = xPoints[i] + translate;
                    var x2 = xPoints[i + 1] - translate;
                    var y1 = yPoints[j] + translate;
                    var y2 = yPoints[j + 1] - translate;
                    var bottomLeft = new Point2d(x1, y1);
                    var bottomRight = new Point2d(x2, y1);
                    var topLeft = new Point2d(x1, y2);
                    var topRight = new Point2d(x2, y2);
                    var tnfRect = new TNFRevonation(topLeft,topRight,bottomLeft,bottomRight);
                    if( TNFUtilities.IsPointInside( tnfRect.BasePoint.ToPoint3d(), maxRevo))
                    {
                        /// xóa dòng này khi làm cách móng;
                        //tnfRect.GeneralVertices();
                        tnfRectList.Add(tnfRect);
                    }




                }
            }
            return tnfRectList;
        }


    }
}
