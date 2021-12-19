using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Factory;
using TNFData.Models;
using TNFData.Models.Section;

namespace TNFData.Geometry
{
    public class TNFPlatformTypeSection : TNFSectionGeometryBase
    {
        public TNFPlatformTypeSection(Point2D basePoint, TNFSection tnfSectionData) : base(basePoint, tnfSectionData)
        {
        }

        protected override void InitGeometry()
        {
            var tnfParameters = TNFSectionData.TNFParameters;
            var baseXForBeamS = BasePoint.X + (tnfParameters.SecondRevonationWidth + (double)tnfParameters.FirstRevonationWith/2)*tnfParameters.ScaleRatio;
            var basePointForBeam = new Point2D(baseXForBeamS, BasePoint.Y);
            if (tnfParameters.IsDrawBeamSection)
            {
                this.BeamSection = TNFBeamSectionFactory.CreateBeamSection(basePointForBeam, TNFSectionData.FootingBeamSectionParams);
                var baseXForF = BasePoint.X
                    + (tnfParameters.SecondRevonationWidth
                    + tnfParameters.FirstRevonationWith
                    + tnfParameters.SecondRevonationWidth
                    + TNFGlobalInfo.SECTION_DISTANCE
                    + tnfParameters.FirstRevonationWith
                    + tnfParameters.SecondRevonationWidth
                    + ((double)tnfParameters.FirstRevonationWith/2))
                    *tnfParameters.ScaleRatio;
                var basePointForF = new Point2D(baseXForF, BasePoint.Y);
                FSection = TNFFoundationSectionGFactory.CreateTNFFoundationSectionG(basePointForF, TNFSectionData.FoundationSectionParams);
            }
            else
            {
                var baseXForF1 = BasePoint.X + (tnfParameters.FirstRevonationWith + tnfParameters.SecondRevonationWidth+(double)tnfParameters.FirstRevonationWith/2)*tnfParameters.ScaleRatio;
                var basePointForF2 = new Point2D(baseXForF1, BasePoint.Y);
                FSection = TNFFoundationSectionGFactory.CreateTNFFoundationSectionG(basePointForF2, TNFSectionData.FoundationSectionParams);
            }

            MaxX = FSection.SecondRightPrimaryRevo.TopRight.X;



        }
    }
}
