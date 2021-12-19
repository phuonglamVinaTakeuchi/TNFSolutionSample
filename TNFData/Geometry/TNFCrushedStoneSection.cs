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
    public class TNFCrushedStoneSection : TNFSectionGeometryBase
    {
        public TNFCrushedStoneSection(Point2D basePoint, TNFSection tnfSectionData) : base(basePoint, tnfSectionData)
        {
        }

        protected override void InitGeometry()
        {
            var tnfParameters = TNFSectionData.TNFParameters;
            var baseXPoint = BasePoint.X
                + (tnfParameters.SecondRevonationWidth
                + tnfParameters.FirstRevonationWith
                + tnfParameters.SecondRevonationWidth
                * 2
                + TNFGlobalInfo.SECTION_DISTANCE
                + (double)tnfParameters.FirstRevonationWith
                / 2)
                *tnfParameters.ScaleRatio;
            var basePoint = new Point2D(baseXPoint, BasePoint.Y);
            this.FSection = TNFFoundationSectionGFactory.CreateTNFFoundationSectionG(basePoint,
                TNFSectionData.FoundationSectionParams);
            MaxX = FSection.SecondRightPrimaryRevo.TopRight.X;
        }
    }
}
