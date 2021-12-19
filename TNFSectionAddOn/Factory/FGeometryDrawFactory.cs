using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.FoundationGeometry;
using TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationDraws;

namespace TNFSectionAddOn.Factory
{
    public static class FGeometryDrawFactory
    {
        public static FGeometryDrawBase CreateFGeometryDraw(FoundationGBase foundation)
        {
            switch (foundation.Foundation.FoundationType)
            {
                case TNFData.Enums.TNFFoundationTypes.DType:
                    return new DTypeDraw(foundation);
                case TNFData.Enums.TNFFoundationTypes.NormalType:
                    return new NormalTypeDraw(foundation);
                default:
                    return null;
            }
        }
    }
}
