using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFFuntion
{
    public class TNFRect
    {
        public List<Point2d> Vertices { get; set; }

        public TNFRect(Point3d basePoint,double width,double height)
        {

            var left = basePoint.X - width / 2;
            var right = basePoint.X + width / 2;
            var top = basePoint.Y + height / 2;
            var bottom = basePoint.Y - height / 2;
            var topLeft = new Point2d(left, top);
            var bottomLeft = new Point2d(left, bottom);
            var topRight = new Point2d(right, top);
            var bottomRight = new Point2d(right, bottom);
            Vertices = new List<Point2d>() {
                topLeft,topRight,bottomRight,bottomLeft,topLeft
            };

        }
    }
}
