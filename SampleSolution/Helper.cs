using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleSolution
{
    public static class Helper
    {
        /// <summary>
        /// Convert Point3d to Point2d
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point2d ToPoint2d(this Point3d point)
        {
            return new Point2d((double)point.X, (double)point.Y);
        }
        /// <summary>
        /// Filter vline and hline from selection set
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="vLine"></param>
        /// <param name="hLine"></param>
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

        /// <summary>
        /// Get the points for boundary Pline
        /// </summary>
        /// <param name="vLines"></param>
        /// <param name="hLines"></param>
        /// <returns></returns>
        public static List<Point2d> GetMaxRevo(List<Line> vLines, List<Line> hLines)
        {


            var baseCrossLines = new List<Line>();
            // order vLine begin from left to right
            var sortedVLines = vLines.OrderBy(x => x.StartPoint.X).ToList();
            // order hLine begin from bottom to top
            var sortedHLines = hLines.OrderBy(x => x.StartPoint.Y).ToList();

            // Set the searchingbase line Line is VlineLine at zero
            var searchingLine = sortedVLines[0];
            // and the founded line is searching line
            Line foundedLine = searchingLine;
            baseCrossLines.Add(searchingLine);

            // We will make first lines as Vline and second Line as h line.
            var firstLines = new List<Line>(sortedVLines);
            var secondLines = new List<Line>(sortedHLines);

            // this one reverse HLine from top to bottom
            secondLines.Reverse();

            do
            {
                if (foundedLine != null)
                {
                    foundedLine = null;

                    // Begin filter all axis from firstLine and second line ad add them to baseCrossLine, the result we need is base cross lines

                    foundedLine = GetLineForRevo(firstLines, secondLines, baseCrossLines, ref searchingLine);

                }
                else
                {
                    // when founded line is reset to null from GetLineForRevo method,
                    // we switch chaned form vLines to hLines with current base searching line is the last founded line we founded in GetLineForRevo.
                    // Repeat the loop until searching line is the first firt searching line from the begining then break out with the result we need
                    var tempLine = firstLines;
                    firstLines = secondLines;
                    tempLine.Reverse();
                    secondLines = tempLine;
                    foundedLine = GetLineForRevo(firstLines, secondLines, baseCrossLines, ref searchingLine);

                }

            } while (searchingLine != sortedVLines[0]); // Break out the loop when searching line is searchingbaseline and here, we founded all the base axis for boundary

            return GetBoundaryPointFromBoundaryLineInList(baseCrossLines);
        }

        /// <summary>
        /// Get Boundary Point for Boundary Pline frome base crossLines
        /// </summary>
        /// <param name="baseCrossLines"></param>
        /// <returns></returns>
        public static List<Point2d> GetBoundaryPointFromBoundaryLineInList(List<Line> baseCrossLines)
        {
            var result = new List<Point2d>();
            var cound = baseCrossLines.Count - 1;
            for (var i = 0; i < cound; i++)
            {
                var line1 = baseCrossLines[i];
                var line2 = baseCrossLines[i + 1];
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

        /// <summary>
        /// Filter the main Line from vLines and hLines
        /// </summary>
        /// <param name="vLines"></param>
        /// <param name="hLines"></param>
        /// <param name="baseCrossLines"></param>
        /// <param name="searchingLine"></param>
        /// <returns></returns>
        private static Line GetLineForRevo(List<Line> vLines, List<Line> hLines, List<Line> baseCrossLines, ref Line searchingLine)
        {
            /// the searchingLine at begin alway Vertical Line

            Line foundedLine = null;
            var searchingIndex = 0;
            var maxSearchIndex = hLines.Count;
            while (true)
            {
                foundedLine = null;

                // if searchingLine is horizontalLine then find the firt cross line in vline
                if (vLines.Contains(searchingLine))
                {
                    // get the first horizontal line crossing with first line in hLines in case hLines is order from top to bottom
                    foundedLine = GetFirstInterectLineWith(searchingLine, hLines, 0, hLines.Count);
                    if (foundedLine != null)
                    {
                        baseCrossLines.Add(foundedLine);
                        searchingIndex = vLines.IndexOf(searchingLine) + 1;
                        maxSearchIndex = hLines.IndexOf(foundedLine);
                        // at there, we need to find the line
                        searchingLine = foundedLine;
                        if (maxSearchIndex == 0)
                        {
                            // incase the max searching index is zero, we wil rester the foundated line to null,
                            // then we swich verticalLines to horizontal Lines and try again with the same way, the current base searching line now is the last founded line
                            foundedLine = null;
                            break;
                        }


                    }

                }
                // else we need find cross line in hLine
                else
                {
                    var maxCount = vLines.Count;

                    // loop all the vline from left to right except the base line from begining
                    for (var i = searchingIndex; i < maxCount; i++)
                    {
                        var line = vLines[i];
                        var intersectPoints = new Point3dCollection();
                        // find if the line is crossing with searching linh
                        searchingLine.IntersectWith(line, Intersect.OnBothOperands, intersectPoints, IntPtr.Zero, IntPtr.Zero);


                        if (intersectPoints.Count > 0)
                        {
                            // if current line is crossing with searching line (in case, searching line is horizontol line
                            // then we need to use the tempt searching as base searching line
                            // then try to find are there available the horizontal line from 0 to the main searching line we founded before
                            // if yes, the line will be the we need will be current line, and break for loop, if not, try again with horizontal line until founded the horizontal line we need
                            var tempSearching = line;
                            var tempFounded = GetFirstInterectLineWith(tempSearching, hLines, 0, maxSearchIndex);
                            if (tempFounded != null)
                            {
                                foundedLine = tempSearching;
                                baseCrossLines.Add(foundedLine);
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

        /// <summary>
        /// Searching the first line is intersect with the searching line
        /// </summary>
        /// <param name="searchingLine"></param>
        /// <param name="searchList"></param>
        /// <param name="searchingIndex"></param>
        /// <param name="maxSearch"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Create Poly line from points
        /// </summary>
        /// <param name="points"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static Polyline CreatePolyLine(List<Point2d> points, string layerName)
        {
            if (points == null || points.Count == 0)
                return null;


            var polyline = new Polyline(points.Count);
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

        /// <summary>
        /// Add entities to autocad block table record
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="acBlockTableRecord"></param>
        /// <param name="transaction"></param>
        public static void AddEntities(Entity entity, BlockTableRecord acBlockTableRecord, Transaction transaction)
        {
            acBlockTableRecord.AppendEntity(entity);
            transaction.AddNewlyCreatedDBObject(entity, true);
        }
    }
}
