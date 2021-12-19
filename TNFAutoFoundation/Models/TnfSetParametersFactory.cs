using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using TNFAutoFoundation.Enums;

namespace TNFAutoFoundation.Models
{
    public static class TnfSetParametersFactory
    {
        public static SetTnfParameters CreateSetTnf(TnfFoundation tnfFoundation,FamilySymbol symbol)
        {
            switch (tnfFoundation.FoundationType)
            {
                case FoundationTypes.DType:

                    return new SetDTypeParameters(tnfFoundation,symbol);
                case FoundationTypes.NormalType:

                    return new SetTnfNormalParameters(tnfFoundation,symbol);
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}
