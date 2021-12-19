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
    public class TNFCol
    {
        private Document _acDoc;
        private Database _acCurDb;
        public TNFCol()
        {
            _acDoc = Application.DocumentManager.MdiActiveDocument;
            _acCurDb = _acDoc.Database;
        }
        [CommandMethod("TNFKol")]
        public void TNFColDraw()
        {
            PromptStringOptions pStrOpts = new PromptStringOptions("\nCol Offset:<900>: ");
            pStrOpts.DefaultValue = "900";
            pStrOpts.UseDefaultValue = true;
            PromptResult pStrRes = _acDoc.Editor.GetString(pStrOpts);
            if (string.IsNullOrEmpty(pStrRes.StringResult))
                return;
            double.TryParse(pStrRes.StringResult, out var offsetDist);
            if (offsetDist == 0) offsetDist = 900;

            if (!SelectionGrid.Select(out var vLine, out var hLine, out var selectionResult, "TP 0-F 基準線"))
                return;

            if (vLine.Count > 0 && hLine.Count > 0)
            {
                var xGridPoints = TNFUtilities.GetMatrixPoints(vLine);
                var yGridPoints = TNFUtilities.GetMatrixPoints(hLine, false);

                var minXLine = vLine.OrderBy(x => x.StartPoint.X).ToList()[0];
                var minYLine = hLine.OrderBy(y => y.StartPoint.Y).ToList()[0];

                var xColListMatrixs = TNFUtilities.GetColList(xGridPoints.ToList(), offsetDist);
                var yColListMatrixs = TNFUtilities.GetColList(yGridPoints.ToList(), offsetDist);

                // Lấy tất cả điểm so le
                var colPoints = TNFUtilities.GetColDrawPointsFromGrids(xColListMatrixs, yColListMatrixs);
                // Detect các điểm theo trục
                var drawPoints = new HashSet<Point3d>();
                // trục x.
                TNFUtilities.GetColFollowGrids(colPoints.ToList(), xGridPoints.ToList(),drawPoints, offsetDist );

                // Truc y.
                TNFUtilities.GetColFollowGrids(colPoints.ToList(), yGridPoints.ToList(),drawPoints, offsetDist, false);

                //var xDict = GetMatrixPointDict(xGridPoints.ToArray(), xColListMatrixs);
                //var yDict = GetMatrixPointDict(yGridPoints.ToArray(), yColListMatrixs);

                var maxX = GetXMax(hLine);
                var maxY = GetYMax(vLine);
                var startPVline = GetMinPointOfVline(minXLine);
                var endPVline = new Point3d(startPVline.X, maxY,0);

                var verLine = new Line( endPVline, startPVline);

                var startPHline = GetMinPointOfHLine(minYLine);
                var endPVHline = new Point3d(maxX, startPHline.Y, 0);
                var horLine = new Line(startPHline, endPVHline);

                if (!SelectionFoundation.Select(out var foundationSelectResult))
                    return;
                var tnfFoundation = TNFUtilities.GetEntities(foundationSelectResult.Value);

                SelectionFoundation.SelectMaxRevo(tnfFoundation,out var maxRevo);

                var tempDrawPoint = TNFUtilities.FoundPointInside(drawPoints.ToList(), new List<Entity>() { maxRevo });
                drawPoints.Clear();
                foreach(var p in tempDrawPoint)
                {
                    drawPoints.Add(p);
                }

                DrawColMatrix(verLine, offsetDist, xColListMatrixs[0], xColListMatrixs[xColListMatrixs.Length - 1]);
                DrawColMatrix(horLine, offsetDist, yColListMatrixs[0], yColListMatrixs[yColListMatrixs.Length - 1]);

                TNFUtilities.GetColForRevo(tnfFoundation, xColListMatrixs, yColListMatrixs,drawPoints);
                //var xCol = col.Item1.ToArray();
                //var yCol = col.Item2.ToArray();
                //var ps = GetPointsFromGrid(xCol,yCol);


                // Thêm cột cho các ô móng
                //var addedPointInsideFoundation = new HashSet<Point3d>();
                //GetPrimaryDrawPoint(xDict, yDict, addedPointInsideFoundation);
                //var colDrawPoint = TNFUtilities.FoundPointInside(ps.ToList(), tnfFoundation);
                //foreach (var p in colDrawPoint)
                //{
                //    drawPoints.Add(p);
                //}

                //TNFUtilities.DrawColEnt(drawXPoints.ToArray());
                //TNFUtilities.DrawColEnt(drawYPoints.ToArray());
                TNFUtilities.DrawColEnt(drawPoints.ToArray());

            }

        }
        public Dictionary<double,double[]> GetMatrixPointDict(double[] gridPoints,double[] colMatrix)
        {
            var dict = new Dictionary<double, double[]>();

            foreach (var poinLocation in gridPoints)
            {
                var founded = 0;
                var cols = new double[2];
                foreach (var colLocation in colMatrix)
                {
                    if (Math.Abs(poinLocation - colLocation) < 900)
                    {
                        cols[founded] = colLocation;
                        founded++;
                    }
                    if(founded == 1)
                    {
                        dict.Add(poinLocation, cols);
                        continue;
                    }
                    if(founded == 2)
                    {
                        break;
                    }
                }
            }
            return dict;
        }

        private void GetDrawPoints(Dictionary<double,double[]> pointDict,double[] points,HashSet<Point3d> drawpoints,bool swap = false)
        {
            foreach(var poinkey in pointDict)
            {
                var pointvalue = poinkey.Value;
                var tempDrawPoints = TNFUtilities.GetColDrawPointsFromGrids(pointvalue, points,swap);
                foreach(var temPoint in tempDrawPoints)
                {
                    drawpoints.Add(temPoint);
                }

            }
        }


        private void GetPrimaryDrawPoint(Dictionary<double, double[]> xPoints, Dictionary<double, double[]> yPoints,HashSet<Point3d> points)
        {
            var listXPoints = new List<double>();
            var listYPoints = new List<double>();
            foreach (var xKey in xPoints)
            {
                listXPoints.AddRange(xKey.Value);
            }
            foreach(var yKey in yPoints)
            {
                listYPoints.AddRange(yKey.Value);
            }

            foreach(var x in listXPoints)
            {
                foreach(var y in listYPoints)
                {
                    var drawPoint = new Point3d(x, y, 0);
                    points.Add(drawPoint);
                }
            }

        }
        private HashSet<Point3d> GetPointsFromGrid(double[] xPoints,double[] yPoints)
        {
            var resutl = new HashSet<Point3d>();
            foreach (var x in xPoints)
            {
                foreach (var y in yPoints)
                {
                    var drawPoint = new Point3d(x, y,0);
                    resutl.Add(drawPoint);
                }
            }
            return resutl;
        }

        private Point3d GetMinPointOfVline(Line line)
        {
            if (line.StartPoint.Y < line.EndPoint.Y)
                return line.StartPoint;
            return line.EndPoint;
        }
        private Point3d GetMinPointOfHLine(Line line)
        {
            if (line.StartPoint.X < line.EndPoint.X)
                return line.StartPoint;
            return line.EndPoint;
        }

        private double GetXMax(List<Line> lines)
        {
            var maxX = lines[0].StartPoint.X;
            foreach(var line in lines)
            {
                if (line.StartPoint.X > maxX)
                    maxX = line.StartPoint.X;
                if (line.EndPoint.X > maxX)
                    maxX = line.EndPoint.X;
            }
            return maxX;
        }
        private double GetYMax(List<Line> lines)
        {
            var maxX = lines[0].StartPoint.Y;
            foreach (var line in lines)
            {
                if (line.StartPoint.Y > maxX)
                    maxX = line.StartPoint.Y;
                if (line.EndPoint.Y > maxX)
                    maxX = line.EndPoint.Y;
            }
            return maxX;
        }


        public void DrawColMatrix(Line lines,double offsetDist,double minValue,double maxvalue)
        {
            using(var transaction = _acDoc.Database.TransactionManager.StartTransaction())
            {
                var acBlockTable = transaction.GetObject(_acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                var acBlockTableRecord = transaction.GetObject(acBlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                var offsetLines = lines.GetOffsetCurves(-offsetDist / 2)[0] as Line;
                offsetLines.Layer = "Defpoints";
                TNFUtilities.AddEntities(offsetLines, acBlockTableRecord, transaction);

                while (minValue < maxvalue)
                {
                    offsetLines = offsetLines.GetOffsetCurves(offsetDist)[0] as Line;
                    offsetLines.Layer = "Defpoints";
                    TNFUtilities.AddEntities(offsetLines, acBlockTableRecord, transaction);
                    minValue += offsetDist;
                }
                transaction.Commit();
            }

        }

    }
}
