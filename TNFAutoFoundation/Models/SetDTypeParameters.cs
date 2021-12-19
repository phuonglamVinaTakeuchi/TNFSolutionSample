using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace TNFAutoFoundation.Models
{
    public class SetDTypeParameters : SetTnfParameters
    {
        public SetDTypeParameters(TnfFoundation tnfFoundation, FamilySymbol symbol) : base(tnfFoundation, symbol)
        {
        }

        public override void SetParameters()
        {
            base.SetParameters();
            var parameters = TnfSymbol.Parameters.Cast<Parameter>().ToList();
            foreach (var parameter in parameters)
            {
                switch (parameter.Definition.Name)
                {
                    case "基礎_厚さ_上":
                        parameter.Set(TnfFoundation.TopDepth.MmToInch());
                        break;
                    case "基礎_厚さ_下":
                        parameter.Set(TnfFoundation.BottomDepth.MmToInch());
                        break;
                    case "基礎_幅_下":
                        parameter.Set(TnfFoundation.BottomWidth.MmToInch());
                        break;
                    case "柱_I_X方向":
                        parameter.Set(TnfFoundation.IsByX ? 1 : 0);
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
