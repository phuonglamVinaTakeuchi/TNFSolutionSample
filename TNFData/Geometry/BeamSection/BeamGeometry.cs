using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BaseGeometry;
using TNFData.Models;

namespace TNFData.Geometry.BeamSection
{
    public class BeamGeometry : GeometryBase
    {
        public Point2D TopLeft { get; set; }
        public Point2D TopRight { get; set; }
        public Point2D BotomRight { get; set; }
        public Point2D BottomLeft { get; set; }
        public Point2D ConcreateBottomLeft { get; set; }
        public Point2D ConcreateBottomRight { get; set; }
        public Point2D OffsetGLLeft { get; set; }
        public Point2D OffsetGLRight { get; set; }
        public TNFBeam TNFBeam { get; }

        public BeamGeometry(Point2D basePoint,TNFBeam beam): base(basePoint)
        {
            TNFBeam = beam;
            InitGeometry();
        }
        protected override void InitGeometry()
        {
            if (TNFBeam == null)
                return;
            ConcreateBottomLeft = BasePoint;

            var tnfParams = TNFBeam.TNFParams;
            var concreateBaseX = BasePoint.X + tnfParams.ScaleRatio*TNFBeam.BeamWidth/2;
            var concreateBasePoint = new Point2D(concreateBaseX, BasePoint.Y);

            var concreateRectangle = new Rectangle(concreateBasePoint, tnfParams.ScaleRatio* TNFBeam.BeamWidth,tnfParams.ScaleRatio* TNFBeam.ZConcreateThickness);

            ConcreateBottomRight = concreateRectangle.BottomRight;
            BottomLeft = concreateRectangle.TopLeft;
            BotomRight = concreateRectangle.TopRight;

            var totalBeamWidth = (TNFBeam.BeamWidth + TNFBeam.OpenPitchDistance)*tnfParams.ScaleRatio;
            var totalBeamHeight = TNFBeam.BeamDepth*tnfParams.ScaleRatio;

            var xAxisForTopRight = BasePoint.X + totalBeamWidth;
            var yAxisForTop = BasePoint.Y + totalBeamHeight;

            TopLeft = new Point2D(BasePoint.X, yAxisForTop);
            TopRight = new Point2D(xAxisForTopRight, yAxisForTop);

            OffsetGLLeft = new Point2D(TopLeft.X, TopLeft.Y - TNFBeam.OffsetWithGL* tnfParams.ScaleRatio);
            var temOffsetPointRight = new Point2D(TopRight.X, TopRight.Y - TNFBeam.OffsetWithGL*tnfParams.ScaleRatio);
            var offsetBeamLine = new Segment2D(OffsetGLLeft,temOffsetPointRight);
            var beamLeftLint = new Segment2D(BotomRight, TopRight);
            Segment2D.IntersectionLine(offsetBeamLine, beamLeftLint, out var intersectPoint);
            OffsetGLRight = intersectPoint;



        }
    }
}
