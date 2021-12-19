using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry;
using TNFData.Geometry.FoundationSection;
using TNFData.Models.Section;

namespace TNFData.Factory
{
    public static class TNFFoundationSectionGFactory
    {
        public static FSectionGBase CreateTNFFoundationSectionG(Point2D basePoint, TNFFoundationSectionParams fData)
        {
            switch (fData.TNFParameters.SectionType)
            {
                case Enums.TNFSectionTypes.NormalType:
                    return new NormalSectionG(basePoint, fData);
                case Enums.TNFSectionTypes.PlatformType:
                    return new PlatformSectionG(basePoint, fData);
                case Enums.TNFSectionTypes.CrushedStone:
                    return new CrushedStoneSectionG(basePoint, fData);
                case Enums.TNFSectionTypes.CrushedStoneAndPlatform:
                    return new CrushedStoneAndPlatformG(basePoint, fData);
                default:
                    return null;
            }
        }
    }
}
