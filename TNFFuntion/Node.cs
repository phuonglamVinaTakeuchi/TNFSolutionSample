using Autodesk.AutoCAD.Geometry;

namespace TNFFuntion
{
    public class Node
    {
        public bool IsConvex { get; set; }
        public Point2d Vertice { get; private set; }
        public Node(Point2d vertice)
        {
            Vertice =  vertice;
            IsConvex = false;
        }
    }
}
