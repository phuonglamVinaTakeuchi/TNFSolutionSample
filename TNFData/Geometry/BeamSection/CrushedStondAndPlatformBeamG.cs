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
    public class CrushedStondAndPlatformBeamG : PlatformBeamSectionG
    {
        public Rectangle Stone { get; set; }
        public CrushedStondAndPlatformBeamG(Point2D basePoint, TNFFootingBeamParams beamData) : base(basePoint, beamData)
        {
        }
        protected override void InitGeometry()
        {
            base.InitGeometry();
            var tnfParams = this.BeamSectionData.TNFParameters;
            var stoneYBase = ThirdRevonation.TopRight.Y;
            var stoneXBase = (double)(Beam.TopLeft.X + TopRightThirdRevonation.TopRight.X)/2;
            var stoneBasePoint = new Point2D(stoneXBase, stoneYBase);
            var stoneWidth = (int)(TopRightThirdRevonation.TopRight.X - Beam.TopLeft.X);
            var stoneHeight = this.BeamSectionData.TNFParameters.StoneThickness*tnfParams.ScaleRatio;
            Stone = new Rectangle(stoneBasePoint, stoneWidth, stoneHeight);
            CreateFloorAndWall(Stone.TopRight);
        }
    }
}
