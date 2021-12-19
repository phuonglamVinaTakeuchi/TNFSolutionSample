using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry;
using TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationSectionDraws;

namespace TNFSectionAddOn.Factory
{
    public class FSectionFoundationGDrawFactory
    {
        public static FSectionDrawBase CreateSectionDraw(FSectionGBase foundation)
        {
            switch (foundation.FSectionData.TNFParameters.SectionType)
            {
                case TNFData.Enums.TNFSectionTypes.NormalType:
                    return new FSectionNomalDraws(foundation);
                case TNFData.Enums.TNFSectionTypes.PlatformType:
                    return new FSectionPlatformDraw(foundation);
                case TNFData.Enums.TNFSectionTypes.CrushedStone:
                    return new FSectionCrushedStoneDraw(foundation);
                case TNFData.Enums.TNFSectionTypes.CrushedStoneAndPlatform:
                    return new FSectionCrushedAndPlatformDraw(foundation);
                default:
                    return null;
            }
        }
    }
}
