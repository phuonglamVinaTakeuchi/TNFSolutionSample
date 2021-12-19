using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry;
using TNFData.Models.Section;

namespace TNFData.Factory
{
    public static class TNFSectionGFactory
    {
        public static TNFSectionGeometryBase CreateSectionGeometry(Point2D basePoint, TNFSection sectionData)
        {
            switch (sectionData.TNFParameters.SectionType)
            {
                case Enums.TNFSectionTypes.NormalType:
                    return new TNFNormalTypeSection(basePoint, sectionData);
                case Enums.TNFSectionTypes.PlatformType:
                    return new TNFPlatformTypeSection(basePoint, sectionData);
                case Enums.TNFSectionTypes.CrushedStone:
                    return new TNFCrushedStoneSection(basePoint, sectionData);
                case Enums.TNFSectionTypes.CrushedStoneAndPlatform:
                    return new TNFCrushedAndPlatformTypeSection(basePoint,sectionData);
                default:
                    return null;

            }
        }
    }
}
