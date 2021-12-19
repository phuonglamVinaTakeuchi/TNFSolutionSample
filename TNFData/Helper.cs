using devDept.Geometry;
using GeometRi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFData
{
    public static class Helper
    {
        public static Point3D Point2DtoPoint3D(this Point2D point)
        {
            return new Point3D(point.X, point.Y, 0);
        }
        public static Point2D Point3DtoPoint2D(this Point3D point)
        {
            return new Point2D(point.X, point.Y);
        }

        public static Point2D InterSection(Point2D p1,Point2D p2,Point2D p3,Point2D p4)
        {
            var startp1 = new Point3d(p1.X, p1.Y, 0);
            var endP1 = new Point3d(p2.X, p2.Y, 0);
            var line1 = new Segment3d(startp1, endP1);
            var startp2 = new Point3d(p3.X, p3.Y, 0);
            var endP2 = new Point3d(p4.X, p4.Y - 100, 0);
            var line2 = new Segment3d(startp2, endP2);
            var vector = new Vector3d(startp2, endP2);

            line1.IntersectionWith(line2);
            var ray = new Ray3d(startp2, vector);
            var intersectP = ray.IntersectionWith(line1) as Point3d;

            var intersect1 = new Point2D(intersectP.X, intersectP.Y);
            return intersect1;
        }
    }
}
