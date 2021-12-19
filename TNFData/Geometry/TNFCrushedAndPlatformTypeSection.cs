using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Models.Section;

namespace TNFData.Geometry
{
    public class TNFCrushedAndPlatformTypeSection : TNFPlatformTypeSection
    {
        public TNFCrushedAndPlatformTypeSection(Point2D basePoint, TNFSection tnfSectionData) : base(basePoint, tnfSectionData)
        {
        }

    }
}
