using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using devDept.Geometry;
//using Autodesk.AutoCAD.GraphicsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFFuntion
{
    public class TNFRevonation
    {
        #region Field
        private Polyline _bottomLeftCross;
        private Polyline _bottomRightCross;
        private Polyline _topLeftCross;
        private Polyline _topRightCross;
        private List<Polyline> _crossPolyLines;
        #endregion

        #region Properties
        public Point2d BottomLeft { get; set; }
        public Point2d BottomRight { get; set; }
        public Point2d TopLeft { get; set; }
        public Point2d TopRight { get; set; }
        public Point2d BasePoint { get; set; }
        public Point2d BottomLeftCrossPoint
        {
            get
            {
                if (_bottomLeftCross != null)
                {
                    var max = _bottomLeftCross.GeometricExtents.MaxPoint;
                    if (Vertices.Contains(max.ToPoint2d()))
                    {
                        return max.ToPoint2d();
                    }
                }
                return ClosestPointWith(BottomLeft);
            }


        }
        public Point2d BottomRightCrossPoint {
            get
            {
                if (_bottomRightCross != null)
                {
                    var max = _bottomRightCross.GeometricExtents.MaxPoint;
                    var min = _bottomRightCross.GeometricExtents.MinPoint;
                    var point = new Point2d(min.X,max.Y);

                    if (Vertices.Contains(point))
                    {
                        return point;
                    }
                }
                return ClosestPointWith(BottomRight);
            }
        }

        public Point2d TopLeftCrossPoint
        {
            get
            {
                if (_topLeftCross != null)
                {
                    var max = _topLeftCross.GeometricExtents.MaxPoint;
                    var min = _topLeftCross.GeometricExtents.MinPoint;
                    var point = new Point2d(max.X, min.Y);

                    if (Vertices.Contains(point))
                    {
                        return point;
                    }
                }
                return ClosestPointWith(TopLeft);
            }
        }

        public Point2d TopRightCrossPoint
        {
            get
            {
                if (_topRightCross != null)
                {

                    var min = _topRightCross.GeometricExtents.MinPoint;


                    if (Vertices.Contains(min.ToPoint2d()))
                    {
                        return min.ToPoint2d();
                    }
                }
                return ClosestPointWith(TopRight);
            }
        }
        public List<Point2d> Vertices { get; private set; }
        public List<Node> Nodes { get; private set; }
        #endregion

        #region Constructor

        public TNFRevonation(Point2d basePoint, double width, double height)
        {
            BasePoint = basePoint;
            var left = basePoint.X - width / 2;
            var right = basePoint.X + width / 2;
            var top = basePoint.Y + height / 2;
            var bottom = basePoint.Y - height / 2;
            TopLeft = new Point2d(left, top);
            BottomLeft = new Point2d(left, bottom);
            TopRight = new Point2d(right, top);
            BottomRight = new Point2d(right, bottom);
            Initializer();
        }
        public TNFRevonation(Point2d topLeft, Point2d topRight, Point2d bottopLeft, Point2d bottomRight)
        {
            BasePoint = TNFUtilities.GetMidPoint(topLeft, bottomRight);
            TopLeft = topLeft;
            BottomLeft = bottopLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            Initializer();
        }
        #endregion

        private void Initializer()
        {
            _crossPolyLines = new List<Polyline>();
            Vertices = new List<Point2d>();
            Nodes = new List<Node>();
        }
        public void AddFoundationCross(Polyline foundation)
        {
            if (!_crossPolyLines.Contains(foundation))
            {
                _crossPolyLines.Add(foundation);
            }
            Regen();
        }

        public void RemoveFoundationCross(Polyline foundation)
        {
            if (_crossPolyLines.Contains(foundation))
            {
                _crossPolyLines.Remove(foundation);
            }
            Regen();

        }

        public void Regen()
        {
            //var region1 = new Polybool.Net.Objects.Region();
            var point1 = new List<Point2d>() {
                TopLeft,
                TopRight,
                BottomRight,
                BottomLeft,
                TopLeft
                };
            var polygon = new Autodesk.AutoCAD.DatabaseServices.Polyline(5);
            for (int i = 0; i < 5; i++)
            {
                polygon.AddVertexAt(i, point1[i], 0, 0, 0);
            }
            polygon.Closed = true;
            var region = TNFUtilities.RegionFromClosedCurve(polygon);



            foreach (Autodesk.AutoCAD.DatabaseServices.Polyline foundation in _crossPolyLines)
            {
                var subtracRegion = TNFUtilities.RegionFromClosedCurve(foundation);
                region.BooleanOperation(BooleanOperationType.BoolSubtract, subtracRegion);
            }

            var points = TNFUtilities.GetPointsFromRegion(region);
            //var regionResul = poly1.Regions.FirstOrDefault();
            Vertices.Clear();
            Nodes.Clear();
            Vertices.AddRange(points);
            foreach (var vertice in Vertices)
            {
                var node = new Node(vertice);
                Nodes.Add(node);
            }
            FixRevonationGeometry();
            //foreach(var p in regionResul.Points)
            //{
            //    Vertices.Add(p.ToPoint2d());
            //}

        }
        public void FixRevonationGeometry()
        {
            var vertices = new List<Node>(Nodes);

            vertices.RemoveAt(vertices.Count - 1);

            GetConverHuxVertices(vertices);
            vertices.Add(vertices[0]);
            Vertices.Clear();
            for (var i = 0; i < vertices.Count - 1; i++)
            {

                if (vertices[i].IsConvex)
                {
                    if (vertices[i + 1].IsConvex)
                    {
                        var distance = vertices[i].Vertice.GetDistanceTo(vertices[i + 1].Vertice);
                        if (distance >= 500)
                        {
                            Vertices.Add(vertices[i].Vertice);
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else
                    {
                        Vertices.Add(vertices[i].Vertice);
                    }
                }
                else
                {
                    Vertices.Add(vertices[i].Vertice);
                }
            }

        }

        public void GetConverHuxVertices(List<Node> points)
        {

            var max = points.Count();
            for (int i = 0; i < max; i++)
            {
                var point = points[i];
                Node prePoint;
                Node nextPoint;
                if (i == 0)
                {
                    prePoint = points[max - 1];
                    nextPoint = points[i + 1];
                }
                else if (i == max - 1)
                {
                    prePoint = points[i - 1];
                    nextPoint = points[0];
                }
                else
                {
                    prePoint = points[i - 1];
                    nextPoint = points[i + 1];
                }

                if (IsPointConvexHux(point, prePoint, nextPoint))
                    point.IsConvex = true;

            }
        }
        public bool IsPointConvexHux(Node point, Node prePoint, Node nextPoint)
        {
            var p1 = point.ToPoint3D();
            var p2 = prePoint.ToPoint3D();
            var p3 = nextPoint.ToPoint3D();
            var vector1 = new Vector3D(p1, p2);
            vector1.Normalize();
            var vector2 = new Vector3D(p1, p3);
            vector2.Normalize();
            var angleRadian = Vector3D.SignedAngleBetween(vector1, vector2);
            var angleDegre = Utility.RadToDeg(angleRadian);
            if (angleDegre > 180 || angleDegre < 0)
            {
                point.IsConvex = true;
                return true;

            }

            return false;

        }


        //public void GeneralVertices()
        //{
        //    Vertices.Clear();
        //    if (_bottomLeftCross != null)
        //    {
        //        var x1 = BottomLeft.X;
        //        var x2 = BottomLeftCrossPoint.X;
        //        var y1 = BottomLeft.Y;
        //        var y2 = BottomLeftCrossPoint.Y;

        //        var p1 = new Point2d(x1, y2);
        //        var p2 = new Point2d(x2, y1);
        //        Vertices.Add(p1);
        //        Vertices.Add(BottomLeftCrossPoint);
        //        Vertices.Add(p2);
        //    }
        //    else Vertices.Add(BottomLeft);

        //    if (_bottomRightCross != null)
        //    {
        //        var x1 = BottomRightCrossPoint.X;
        //        var x2 = BottomRight.X;
        //        var y1 = BottomRight.Y;
        //        var y2 = BottomRightCrossPoint.Y;
        //        var p1 = new Point2d(x1, y1);
        //        var p2 = new Point2d(x2, y2);
        //        Vertices.Add(p1);
        //        Vertices.Add(BottomRightCrossPoint);
        //        Vertices.Add(p2);
        //    }
        //    else Vertices.Add(BottomRight);
        //    if (_topRightCross != null)
        //    {
        //        var x1 = TopRightCrossPoint.X;
        //        var x2 = TopRight.X;
        //        var y1 = TopRightCrossPoint.Y;
        //        var y2 = TopRight.Y;
        //        var p1 = new Point2d(x2, y1);
        //        var p2 = new Point2d(x1, y2);
        //        Vertices.Add(p1);
        //        Vertices.Add(TopRightCrossPoint);
        //        Vertices.Add(p2);
        //    }
        //    else Vertices.Add(TopRight);
        //    if (_topLeftCross != null)
        //    {
        //        var x1 = TopLeft.X;
        //        var x2 = TopLeftCrossPoint.X;
        //        var y1 = TopLeftCrossPoint.Y;
        //        var y2 = TopLeft.Y;

        //        var p1 = new Point2d(x2, y2);
        //        var p2 = new Point2d(x1, y1);
        //        Vertices.Add(p1);
        //        Vertices.Add(TopLeftCrossPoint);
        //        Vertices.Add(p2);
        //    }
        //    else Vertices.Add(TopLeft);

        //}

        private Point2d ClosestPointWith(Point2d testPoint)
        {
            var dist = double.MaxValue;
            Point2d resultPoint = new Point2d();
            foreach (var p in Vertices)
            {
                if (testPoint == p)
                    return p;
                var tempDst = testPoint.GetDistanceTo(p);
                if (tempDst < dist)
                {
                    resultPoint = p;
                    dist = tempDst;
                }

            }
            return resultPoint;

        }

        public void SetCrossFoundation(Polyline crossFoundation)
        {
            var maxP = crossFoundation.GeometricExtents.MaxPoint;
            var minP = crossFoundation.GeometricExtents.MinPoint;

            if (TNFUtilities.IsPointInsideRect(BottomLeft, minP.ToPoint2d(), maxP.ToPoint2d()))
            {
                _bottomLeftCross = crossFoundation;
                return;
            }
            if (TNFUtilities.IsPointInsideRect(BottomRight, minP.ToPoint2d(), maxP.ToPoint2d()))
            {
                _bottomRightCross = crossFoundation;
                return;
            }
            if (TNFUtilities.IsPointInsideRect(TopLeft, minP.ToPoint2d(), maxP.ToPoint2d()))
            {
                _topLeftCross = crossFoundation;
                return;
            }
            if (TNFUtilities.IsPointInsideRect(TopRight, minP.ToPoint2d(), maxP.ToPoint2d()))
            {
                _topRightCross = crossFoundation;
                return;
            }
        }


    }
}
