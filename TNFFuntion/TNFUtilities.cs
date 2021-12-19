using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using AcBr = Autodesk.AutoCAD.BoundaryRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polybool.Net.Objects;
using devDept.Geometry;

namespace TNFFuntion
{

    public static class TNFUtilities
    {
        private static Document _acDoc = Application.DocumentManager.MdiActiveDocument;
        private static Database _acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

        public static Point3d ToPoint3d(this Point2d point)
        {
            return new Point3d(point.X, point.Y, 0);
        }

        public static Point2d ToPoint2d(this Point3d point)
        {
            return new Point2d(point.X, point.Y);
        }
        public static void GetVLineAndHLine(SelectionSet selection, out List<Line> vLine, out List<Line> hLine)
        {
            var horizontalList = new List<Line>();
            var verticalList = new List<Line>();
            var acDoc = Application.DocumentManager.MdiActiveDocument;
            using (var tranSaction = acDoc.Database.TransactionManager.StartTransaction())
            {
                var objectIds = selection.GetObjectIds();
                foreach (var objectId in objectIds)
                {
                    var line = tranSaction.GetObject(objectId, OpenMode.ForRead) as Line;
                    if (line != null)
                    {
                        var startP = line.StartPoint;
                        var endP = line.EndPoint;
                        // StartX != EndX => Hline
                        if ((int)line.StartPoint.X != (int)line.EndPoint.X)
                        {
                            horizontalList.Add(line);

                        }
                        else
                        {

                            verticalList.Add(line);
                        }

                    }
                }

            }
            hLine = horizontalList;
            vLine = verticalList;
        }

        public static IOrderedEnumerable<double> GetMatrixPoints(List<Line> lineEnts, bool getX = true)
        {
            var xPoints = new HashSet<double>();
            foreach (var line in lineEnts)
            {
                if (getX)
                    xPoints.Add(line.StartPoint.X);
                else
                    xPoints.Add(line.StartPoint.Y);
            }
            return xPoints.OrderBy(x => x);
        }
        public static void AddEntities(Entity entity, BlockTableRecord acBlockTableRecord, Transaction transaction)
        {
            acBlockTableRecord.AppendEntity(entity);
            transaction.AddNewlyCreatedDBObject(entity, true);
        }

        public static double GetLength(Curve ent)
        {
            if (ent == null) return -1;

            return ent.GetDistanceAtParameter(ent.EndParam)
                - ent.GetDistanceAtParameter(ent.StartParam);
        }
        public static List<Entity> GetEntities(SelectionSet selection,OpenMode openMode = OpenMode.ForRead)
        {
            var resultEnties = new List<Entity>();
            using (var transaction = _acDoc.Database.TransactionManager.StartTransaction())
            {
                var objectIds = selection.GetObjectIds();
                foreach (var objectId in objectIds)
                {
                    var entity = transaction.GetObject(objectId, openMode) as Entity;
                    resultEnties.Add(entity);
                }

            }
            return resultEnties;
        }

        public static HashSet<Point3d> GetColDrawPointsFromGrids(double[] xPoints, double[] yPoints, bool swap = false)
        {
            HashSet<Point3d> points = new HashSet<Point3d>();
            var yIndex = 0;
            var xMax = xPoints.Length;
            var yMax = yPoints.Length;
            for (yIndex = 0; yIndex < yMax; yIndex++)
            {
                var xIndex = 0;
                if (yIndex % 2 != 0)
                {
                    xIndex = 1;
                }
                while (xIndex < xMax)
                {
                    Point3d point;
                    if (swap)
                    {
                        point = new Point3d(yPoints[yIndex], xPoints[xIndex], 0);
                    }
                    else point = new Point3d(xPoints[xIndex], yPoints[yIndex], 0);
                    points.Add(point);
                    xIndex += 2;
                }
            }
            return points;
        }

        public static void DrawColEnt(Point3d[] points)
        {
            using (var transaction = _acDoc.Database.TransactionManager.StartTransaction())
            {
                var acBlockTable = transaction.GetObject(_acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                var acBlockTableRecord = transaction.GetObject(acBlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                foreach (var point in points)
                {
                    var colEnt = new Circle(point, new Vector3d(0, 0, 1), 75);
                    colEnt.Center = point;
                    colEnt.Radius = 75;
                    colEnt.SetDatabaseDefaults();
                    colEnt.Layer = "T-001";
                    TNFUtilities.AddEntities(colEnt, acBlockTableRecord, transaction);
                }
                transaction.Commit();
            }
        }

        public static Point2d GetMidPoint(Point2d p1, Point2d p2)
        {
            var x = p1.X + p2.X;
            var y = p1.Y + p2.Y;
            return new Point2d(x / 2, y / 2);
        }

        public static double[] GetColList(List<double> baseP, double offsetDist)
        {
            var returnP = new List<double>();
            returnP.Add(baseP[0] - offsetDist / 2);
            var newP = returnP[0];
            var lastIndex = baseP.Count - 1;

            while (newP < baseP[lastIndex] || Math.Abs(newP - baseP[lastIndex]) < 0.01)
            {
                newP += offsetDist;
                returnP.Add(newP);
            }
            return returnP.ToArray();

        }

        public static PointContainment GetPointContainment(Autodesk.AutoCAD.DatabaseServices.Region region, Point3d point)
        {
            PointContainment result = PointContainment.Outside;

            /// Get a Brep object representing the region:
            using (Brep brep = new Brep(region))
            {
                if (brep != null)
                {
                    // Get the PointContainment and the BrepEntity at the given point:

                    using (BrepEntity ent = brep.GetPointContainment(point, out result))
                    {
                        /// GetPointContainment() returns PointContainment.OnBoundary
                        /// when the picked point is either inside the region's area
                        /// or exactly on an edge.
                        ///
                        /// So, to distinguish between a point on an edge and a point
                        /// inside the region, we must check the type of the returned
                        /// BrepEntity:
                        ///
                        /// If the picked point was on an edge, the returned BrepEntity
                        /// will be an Edge object. If the point was inside the boundary,
                        /// the returned BrepEntity will be a Face object.
                        //
                        /// So if the returned BrepEntity's type is a Face, we return
                        /// PointContainment.Inside:

                        if (ent is AcBr.Face)
                            result = PointContainment.Inside;
                        if (ent is AcBr.Edge)
                            result = PointContainment.OnBoundary;

                    }
                }
            }
            return result;
        }

        public static bool IsPointInside(Point3d point, Autodesk.AutoCAD.DatabaseServices.Polyline pLine)
        {
            var pointContain = GetPointContainment(pLine, point);
            if (pointContain == PointContainment.Inside || pointContain == PointContainment.OnBoundary)
                return true;
            return false;
        }

        public static void GetColFollowGrids(List<Point3d> points, List<double> pivots, HashSet<Point3d> resultPoints, double distance = 450, bool isAxistX = true)
        {


            while (points.Count > 0)
            {
                var point = points[0];
                foreach (var pivot in pivots)
                {
                    var distCompare = 0.0;
                    if (isAxistX)
                    {
                        distCompare = Math.Abs(pivot - point.X);

                    }
                    else
                    {
                        distCompare = Math.Abs(pivot - point.Y);
                    }

                    if (distCompare > distance)
                    {
                        continue;
                    }
                    else
                    {
                        resultPoints.Add(point);
                    }
                }

                //pivots.RemoveAt(0);
                points.RemoveAt(0);
            }

        }

        public static PointContainment GetPointContainment(Curve curve, Point3d point)
        {
            if (!curve.Closed)
                throw new ArgumentException("Curve must be closed.");
            Autodesk.AutoCAD.DatabaseServices.Region region = RegionFromClosedCurve(curve);
            if (region == null)
                throw new InvalidOperationException("Failed to create region");
            using (region)
            {
                return GetPointContainment(region, point);
            }
        }

        public static Autodesk.AutoCAD.DatabaseServices.Region RegionFromClosedCurve(Curve curve)
        {
            if (!curve.Closed)
                throw new ArgumentException("Curve must be closed.");
            DBObjectCollection curves = new DBObjectCollection();
            curves.Add(curve);
            using (DBObjectCollection regions = Autodesk.AutoCAD.DatabaseServices.Region.CreateFromCurves(curves))
            {
                if (regions == null || regions.Count == 0)
                    throw new InvalidOperationException("Failed to create regions");
                if (regions.Count > 1)
                    throw new InvalidOperationException("Multiple regions created");
                return regions.Cast<Autodesk.AutoCAD.DatabaseServices.Region>().First();
            }
        }

        public static List<Point3d> FoundPointInside(List<Point3d> points, List<Entity> revos)
        {
            var result = new List<Point3d>();
            foreach (var revo in revos)
            {
                if (revo is Autodesk.AutoCAD.DatabaseServices.Polyline polyLine)
                {
                    var region = TNFUtilities.RegionFromClosedCurve(polyLine);

                    var i = 0;
                    while (i < points.Count - 1)
                    {
                        var pointContain = TNFUtilities.GetPointContainment(region, points[i]);
                        if (pointContain == PointContainment.Inside || pointContain == PointContainment.OnBoundary)
                        {
                            result.Add(points[i]);
                            points.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
            }

            return result;
        }
        public static List<Point2d> GetPointsFromRegion(Autodesk.AutoCAD.DatabaseServices.Region region)
        {
            var result = new List<Point2d>();
            Autodesk.AutoCAD.DatabaseServices.Polyline pline = new Autodesk.AutoCAD.DatabaseServices.Polyline();//TODO:use Polyline3d instead.
            pline.TransformBy(region.Ecs);
            DBObjectCollection subEntities = new DBObjectCollection();
            region.Explode(subEntities);
            var i = 0;
            var points = new List<Point2d>();
            foreach (Curve curve in subEntities)
            {
                if (curve is Line line)
                {
                    var point1 = curve.StartPoint.ToPoint2d();
                    var point2 = curve.EndPoint.ToPoint2d();
                    if (!points.Contains(point1))
                    {
                        points.Add(point1);
                    }
                    if (!points.Contains(point2))
                    {
                        points.Add(point2);
                    }
                }
            }
            points.Add(points[0]);

            result.AddRange(points);
            return result;
        }
        public static Autodesk.AutoCAD.DatabaseServices.Polyline ConvertIntoPolyline(this Autodesk.AutoCAD.DatabaseServices.Region region)
        {
            Autodesk.AutoCAD.DatabaseServices.Polyline pline = new Autodesk.AutoCAD.DatabaseServices.Polyline();//TODO:use Polyline3d instead.
            pline.TransformBy(region.Ecs);
            DBObjectCollection subEntities = new DBObjectCollection();
            region.Explode(subEntities);
            var i = 0;
            var points = new List<Point2d>();
            foreach (Curve curve in subEntities)
            {
                if (curve is Line line)
                {
                    var point1 = curve.StartPoint.ToPoint2d();
                    var point2 = curve.EndPoint.ToPoint2d();
                    if (!points.Contains(point1))
                    {
                        points.Add(point1);
                    }
                    if (!points.Contains(point2))
                    {
                        points.Add(point2);
                    }
                }
            }
            points.Add(points[0]);
            foreach (var p in points)
            {
                pline.AddVertexAt(i, p, 0, 0, 0);
                i++;
            }

            return pline;
        }


        public static List<Point2d> GetPointsFromPolyLine(Autodesk.AutoCAD.DatabaseServices.Polyline region)
        {
            var result = new List<Point2d>();
            for (var i = 0; i < region.NumberOfVertices; i++)
            {
                result.Add(region.GetPoint2dAt(i));
            }
            //var normal = region.Normal;
            //var plane = new Plane(Point3d.Origin, normal);

            //using (var brep = new Brep(region))
            //{
            //    result = brep.Edges.Select(e => e.Vertex1.Point.Convert2d(plane)).ToList();
            //}
            return result;
        }

        public static Point ToPoint(this Point2d point)
        {
            return new Point((decimal)point.X, (decimal)point.Y);
        }
        public static Point2d ToPoint2d(this Point point)
        {
            return new Point2d((double)point.X, (double)point.Y);
        }

        public static Point3D Point2DtoPoint3D(this Point2D point)
        {
            return new Point3D(point.X, point.Y, 0);
        }
        public static Point2D Point3DtoPoint2D(this Point3D point)
        {
            return new Point2D(point.X, point.Y);
        }
        public static Point2D ToPoint2D(this Point2d point2d)
        {
            return new Point2D(point2d.X, point2d.Y);
        }
        public static Point3D ToPoint3D(this Point2d point2d)
        {
            return new Point3D(point2d.X, point2d.Y, 0);
        }
        public static Point3D ToPoint3D(this Node node)
        {
            return node.Vertice.ToPoint3D();
        }

        /// <summary>
        /// vLine đường thẳng nằm đứng.
        /// hLine đường thẳng nằm ngang
        /// </summary>
        /// <param name="vLine"></param>
        /// <param name="hLine"></param>
        /// <returns></returns>
        public static List<Point2d> GetMaxRevo(List<Line> vLines, List<Line> hLines)
        {

            var result = new List<Point2d>();
            var lineCross = new List<Line>();
            // Doc
            var sortedVLines = vLines.OrderBy(x => x.StartPoint.X).ToList();
            // Ngang
            var sortedHLines = hLines.OrderBy(x => x.StartPoint.Y).ToList();

            var searchingLine = sortedVLines[0];
            Line foundedLine = searchingLine;
            lineCross.Add(searchingLine);
            //var startSearchingIndex = 0;
            //var maxSearchingIndex = sortedHLines.Count();
            var firstLines = new List<Line>(sortedVLines);
            var secondLines = new List<Line>(sortedHLines);
            secondLines.Reverse();

            do
            {
                if (foundedLine != null)
                {
                    foundedLine = null;
                    foundedLine = GetLineForRevo(firstLines, secondLines, lineCross, ref searchingLine);


                }
                else
                {
                    var tempLine = firstLines;
                    firstLines = secondLines;
                    tempLine.Reverse();
                    secondLines = tempLine;
                    foundedLine = GetLineForRevo(firstLines, secondLines, lineCross, ref searchingLine);

                }

            } while (searchingLine != sortedVLines[0]);

            var cound = lineCross.Count - 1;
            for (var i = 0; i < cound; i++)
            {
                var line1 = lineCross[i];
                var line2 = lineCross[i + 1];
                var intersectPoint = new Point3dCollection();
                line1.IntersectWith(line2, Intersect.OnBothOperands, intersectPoint, IntPtr.Zero, IntPtr.Zero);
                if (intersectPoint.Count > 0)
                {
                    var point = intersectPoint[0];
                    result.Add(point.ToPoint2d());
                }
            }
            return result;
        }

        private static Line GetLineForRevo(List<Line> lines1, List<Line> lines2, List<Line> revoLines, ref Line searchingLine)
        {
            Line foundedLine = null;
            var searchingIndex = 0;
            var maxSearchIndex = lines2.Count;
            while (true)
            {
                foundedLine = null;
                // nếu searchingLine nằm trong danh sách 1 thì
                if (lines1.Contains(searchingLine))
                {
                    foundedLine = GetFirstInterectLineWith(searchingLine, lines2, 0, lines2.Count);
                    if (foundedLine != null)
                    {
                        revoLines.Add(foundedLine);
                        searchingIndex = lines1.IndexOf(searchingLine) + 1;
                        maxSearchIndex = lines2.IndexOf(foundedLine);
                        searchingLine = foundedLine;
                        if (maxSearchIndex == 0)
                        {
                            foundedLine = null;
                            break;
                        }


                    }

                }
                else
                {
                    var maxCount = lines1.Count;
                    for (var i = searchingIndex; i < maxCount; i++)
                    {
                        var line = lines1[i];
                        var intersectPoints = new Point3dCollection();
                        searchingLine.IntersectWith(line, Intersect.OnBothOperands, intersectPoints, IntPtr.Zero, IntPtr.Zero);
                        if (intersectPoints.Count > 0) {
                            var tempSearching = line;
                            var tempFounded = GetFirstInterectLineWith(tempSearching, lines2, 0, maxSearchIndex);
                            if (tempFounded != null) {
                                foundedLine = tempSearching;
                                revoLines.Add(foundedLine);
                                searchingLine = foundedLine;
                                break;
                            }
                        }
                    }

                    if (foundedLine == null)
                    {
                        break;
                    }

                }
            }

            return foundedLine;
        }

        private static Line GetFirstInterectLineWith(this Line searchingLine, List<Line> searchList, int searchingIndex, int maxSearch)
        {
            var maxCound = searchList.Count >= maxSearch ? maxSearch : searchList.Count;
            for (var i = searchingIndex; i < maxCound; i++)
            {
                var line = searchList[i];
                var intersectPoints = new Point3dCollection();
                searchingLine.IntersectWith(line, Intersect.OnBothOperands, intersectPoints, IntPtr.Zero, IntPtr.Zero);
                if (intersectPoints.Count > 0)
                {
                    return line;
                }
            }

            return null;
        }
        public static bool IsPointInsideRect(Point2d point, Point2d minPoint, Point2d maxPoint)
        {
            var x1 = minPoint.X;
            var y1 = minPoint.Y;
            var x2 = maxPoint.X;
            var y2 = maxPoint.Y;

            var px = point.X;
            var py = point.Y;

            if (px > x1 && px < x2 && py > y1 && py < y2)
                return true;
            return false;
        }

        public static Autodesk.AutoCAD.DatabaseServices.Polyline CreatePolyLine(List<Point2d> points, string layerName)
        {
            if (points == null || points.Count == 0)
                return null;


            var polyline = new Autodesk.AutoCAD.DatabaseServices.Polyline(points.Count);
            polyline.SetDatabaseDefaults();
            polyline.Layer = layerName;
            var i = 0;
            foreach (var point in points)
            {
                polyline.AddVertexAt(i, point, 0, 0, 0);
                i++;
            }
            return polyline;
        }
        public static void GetColForRevo(List<Entity> revos, double[] colXList, double[] colYList, HashSet<Point3d> drawPoints)
        {
            var colXs = new HashSet<double>();
            var colYs = new HashSet<double>();

            foreach (var revo in revos)
            {
                if (revo.Layer != "TP 0-1 基礎")
                    continue;
                var bottomLeft = revo.GeometricExtents.MinPoint;
                var topRight = revo.GeometricExtents.MaxPoint;
                var midPoint = TNFUtilities.GetMidPoint(bottomLeft.ToPoint2d(), topRight.ToPoint2d());
                var count = 0;
                foreach (var x in colXList)
                {
                    if (Math.Abs(x - midPoint.X) <= 900)
                    {
                        colXs.Add(x);
                        count++;
                    }
                    if (count == 2)
                        break;
                }
                count = 0;
                foreach (var y in colYList)
                {
                    if (Math.Abs(y - midPoint.Y) <= 900)
                    {
                        colYs.Add(y);
                        count++;
                    }
                    if (count == 2)
                        break;
                }
                foreach (var x in colXs)
                {
                    foreach (var y in colYs)
                    {
                        var newPoint = new Point3d(x, y, 0);
                        drawPoints.Add(newPoint);
                    }
                }
                colXs.Clear();
                colYs.Clear();

            }



            //return new Tuple<List<double>, List<double>>(colXs.ToList(), colYs.ToList());
        }
        public static List<Point3d> GerInterSectionPoint(List<Line> vLines, List<Line> hLines)
        {
            var intersectPoint = new List<Point3d>();
            foreach(var vline in vLines)
            {
                foreach (var hline in hLines)
                {
                    var points = new Point3dCollection();
                    vline.IntersectWith(hline, Intersect.OnBothOperands, points, IntPtr.Zero, IntPtr.Zero);
                    if (points.Count > 0)
                    {
                        foreach(var iPoint in points)
                        {
                            intersectPoint.Add((Point3d)iPoint);
                        }
                    }
                }
            }
            return intersectPoint;
        }
    }
}
