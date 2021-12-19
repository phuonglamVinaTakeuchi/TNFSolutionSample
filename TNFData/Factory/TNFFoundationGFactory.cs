using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Enums;
using TNFData.Geometry.FoundationGeometry;
using TNFData.Models;

namespace TNFData.Factory
{
    public static class TNFFoundationGFactory
    {
        public static FoundationGBase CreateFoundationGBase(Point2D basePoint, TNFFoundationParams tnfFoundation)
        {
            switch (tnfFoundation.FoundationType)
            {
                case TNFFoundationTypes.DType:
                    return new DTypeG(basePoint,tnfFoundation);
                case TNFFoundationTypes.NormalType:
                    return new NormalTypeG(basePoint,tnfFoundation);
                default:
                    return null;
            }
        }
    }
}
