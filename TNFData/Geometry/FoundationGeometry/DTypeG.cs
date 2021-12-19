using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BaseGeometry;
using TNFData.Models;
using GeometRi;

namespace TNFData.Geometry.FoundationGeometry
{
    public class DTypeG : FoundationGBase
    {
        #region Properties

        public Point2D ConCreatePointTopLeft { get; set; }
        public Point2D ConCreatePointTopRight { get; set; }
        public Point2D DTypeBottomLeft { get; set; }
        public Point2D DTypeBottomRight { get; set; }
        #endregion
        public DTypeG(Point2D basePoint,TNFFoundationParams tnfFoundation): base(basePoint,tnfFoundation)
        {
            InitGeometry();
        }

        protected override void InitGeometry()
        {
            base.InitGeometry();
            var tnfParams = Foundation.TNFParams;
            // Tọa độ trụ Y của đáy móng type D, phàn từ mặt móng tới đáy của TopDepth
            var dTypeY = BasePoint.Y - (Foundation.OffsetWithGL + Foundation.TopDepth) * tnfParams.ScaleRatio;

            DTypeBottomLeft = new Point2D(TopLeft.X, dTypeY);
            DTypeBottomRight = new Point2D(TopRight.X, dTypeY);

            var offsetXLeft = TopLeft.X - Foundation.XConcreateThickness* tnfParams.ScaleRatio;
            var offsetXRight = TopRight.X + Foundation.XConcreateThickness * tnfParams.ScaleRatio;


            var endPoint1 = new Point2D(offsetXLeft, dTypeY);
            var botomConcreateLineLeft = new Segment2D(BottomLeft, endPoint1);

            //var line1 = new Line3d();

            var endPoint2 = new Point2D(offsetXRight, dTypeY);
            var bottomConcreateLineRight = new Segment2D(BottomRight, endPoint2);
            var leftEdge = new Segment2D(TopLeft, DTypeBottomLeft);


            var rightEdge = new Segment2D(TopRight, DTypeBottomRight);



            Segment2D.IntersectionLine(botomConcreateLineLeft, leftEdge, out var concreateLeft);
            Segment2D.IntersectionLine(bottomConcreateLineRight, rightEdge, out var concreateRight);

            var intersect1 = Helper.InterSection(botomConcreateLineLeft.P0, botomConcreateLineLeft.P1, leftEdge.P0, leftEdge.P1);
            var intersect2 = Helper.InterSection(bottomConcreateLineRight.P0, bottomConcreateLineRight.P1, rightEdge.P0, rightEdge.P1);

            //ConCreatePointTopLeft = concreateLeft;
            //ConCreatePointTopRight = concreateRight;

            ConCreatePointTopLeft = intersect1;
            ConCreatePointTopRight = intersect2;


            BottomFoundationPoints.Clear();
            OffsetPoints.Clear();
            FoundationPoints.Clear();

            // Pline cho đáy móng
            FoundationPoints.Add(TopLeft);
            FoundationPoints.Add(ConCreatePointTopLeft);
            FoundationPoints.Add(ConcreateBottomLeft);
            FoundationPoints.Add(ConcreateBottomRight);
            FoundationPoints.Add(ConCreatePointTopRight);
            FoundationPoints.Add(TopRight);

            // Pline cho đáy phần giao giữa đáy móng và lớp bê tông lót
            BottomFoundationPoints.Add(DTypeBottomLeft);
            BottomFoundationPoints.Add(BottomLeft);
            BottomFoundationPoints.Add(BottomRight);
            BottomFoundationPoints.Add(DTypeBottomRight);

            if (OffsetPointLeft != null && OffsetPointRight != null)
            {
                OffsetPoints.Add(OffsetPointLeft);
                OffsetPoints.Add(OffsetPointRight);
            }
        }

    }
}
