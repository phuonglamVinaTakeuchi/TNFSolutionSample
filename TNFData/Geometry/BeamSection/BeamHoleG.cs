using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BaseGeometry;
using TNFData.Models;
using TNFData.Models.Section;

namespace TNFData.Geometry.BeamSection
{
    public class BeamHoleG : GeometryBase
    {
        public Point2D BottomRight { get; set; }
        public Point2D TopRight { get; set; }
        public Point2D BottomLeft { get; set; }
        public Point2D TopLeft { get; set; }
        public TNFFootingBeamParams TNFBeamData { get; set; }

        public BeamHoleG(Point2D basePoint, TNFFootingBeamParams tnfBeam) : base(basePoint)
        {
            TNFBeamData = tnfBeam;
            InitGeometry();
        }

        protected override void InitGeometry()
        {
            if (TNFBeamData == null)
                return;
            var tnfParams = TNFBeamData.TNFParameters;
            var bottomLeftX = BasePoint.X - TNFBeamData.BeamHoleAddWidth * tnfParams.ScaleRatio;

            var bottomRightX = BasePoint.X + TNFBeamData.Beam.BeamWidth * tnfParams.ScaleRatio;
            BottomRight = new Point2D(bottomRightX, BasePoint.Y);
            BottomLeft = new Point2D(bottomLeftX, BasePoint.Y);

            var topLeftX = bottomLeftX - TNFBeamData.BeamHoleOpenDistance * tnfParams.ScaleRatio;
            var topRightX = bottomRightX + TNFBeamData.Beam.OpenPitchDistance * tnfParams.ScaleRatio;
            var topHoleY = BasePoint.Y + TNFBeamData.BeamHoleDepth * tnfParams.ScaleRatio;
            TopLeft = new Point2D(topLeftX, topHoleY);
            TopRight = new Point2D(topRightX, topHoleY);

        }
    }
}
