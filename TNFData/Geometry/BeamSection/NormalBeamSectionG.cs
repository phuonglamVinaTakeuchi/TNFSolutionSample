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
    public class NormalBeamSectionG : BeamSectionGBase
    {
        public NormalBeamSectionG(Point2D basePoint, TNFFootingBeamParams beamData) : base(basePoint, beamData)
        {
        }
        protected override void InitGeometry()
        {
            base.InitGeometry();
            if(this.BottomRightSecondRevonation != null)
            CreateFloorAndWall(this.BottomRightSecondRevonation.TopRight);
        }
    }
}
