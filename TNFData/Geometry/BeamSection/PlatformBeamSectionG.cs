using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BaseGeometry;
using TNFData.Models.Section;

namespace TNFData.Geometry.BeamSection
{
    public class PlatformBeamSectionG : BeamSectionGBase
    {
        public Rectangle ThirdRevonation { get; set; }
        public Rectangle TopRightThirdRevonation { get; set; }
        public PlatformBeamSectionG(Point2D basePoint, TNFFootingBeamParams beamData) : base(basePoint, beamData)
        {
            InitGeometry();
        }
        protected override void InitGeometry()
        {
            base.InitGeometry();
            var tnfParams = BeamSectionData.TNFParameters;
            var baseForThird = Point2D.MidPoint(SecondRevonation.TopLeft, SecondRevonation.TopRight);
            ThirdRevonation = new Rectangle(baseForThird, tnfParams.FirstRevonationWith*tnfParams.ScaleRatio,
                                            tnfParams.ThirdRevonationDepth*tnfParams.ScaleRatio);
            var baseForThirdRight = Point2D.MidPoint(BottomRightSecondRevonation.TopLeft, BottomRightSecondRevonation.TopRight);
            TopRightThirdRevonation = new Rectangle(baseForThirdRight, tnfParams.SecondRevonationWidth*tnfParams.ScaleRatio, tnfParams.ThirdRevonationDepth*tnfParams.ScaleRatio);
            CreateFloorAndWall(TopRightThirdRevonation.TopRight);

        }
    }
}
