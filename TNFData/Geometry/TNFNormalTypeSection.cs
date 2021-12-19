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
    public class TNFNormalTypeSection : TNFSectionGeometryBase
    {
        public TNFNormalTypeSection(Point2D basePoint, TNFSection tnfSectionData) : base(basePoint, tnfSectionData)
        {
        }

        protected override void InitGeometry()
        {
            if (TNFSectionData == null)
                return;

            var tnfParameters = TNFSectionData.TNFParameters;

            var baseXForBeamS = BasePoint.X + (tnfParameters.SecondRevonationWidth + (double)tnfParameters.FirstRevonationWith/2)* tnfParameters.ScaleRatio;
            var basePointForBeam = new Point2D(baseXForBeamS, BasePoint.Y);
            if (tnfParameters.IsDrawBeamSection)
            {
                BeamSection = TNFBeamSectionFactory.CreateBeamSection(basePointForBeam, TNFSectionData.FootingBeamSectionParams);
                var baseXFoundation = baseXForBeamS + (tnfParameters.FirstRevonationWith + tnfParameters.SecondRevonationWidth * 2 + TNFGlobalInfo.SECTION_DISTANCE)*tnfParameters.ScaleRatio;
                var baseFPoint = new Point2D(baseXFoundation, BasePoint.Y);
                if(tnfParameters.IsDrawFoundationSection)
                {
                    FSection = TNFFoundationSectionGFactory.CreateTNFFoundationSectionG(
                    baseFPoint,
                    TNFSectionData.FoundationSectionParams
                    );
                    MaxX = FSection.SecondRightPrimaryRevo.TopRight.X;
                }
                else
                {
                    MaxX = BeamSection.BottomRightSecondRevonation.TopRight.X;
                }

            }
            else
            {
                this.FSection = TNFFoundationSectionGFactory.CreateTNFFoundationSectionG(
                    basePointForBeam,
                    TNFSectionData.FoundationSectionParams);
                MaxX = FSection.SecondRightPrimaryRevo.TopRight.X;

            }

        }
    }
}
