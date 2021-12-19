using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace TNFAutoFoundation.Models
{
    public class SetTnfNormalParameters : SetTnfParameters
    {
        public SetTnfNormalParameters(TnfFoundation tnfFoundation, FamilySymbol symbol) : base(tnfFoundation, symbol)
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
                    // IsEccentric
                    case "偏芯":
                        parameter.Set(TnfFoundation.IsEccentric ? 1 : 0);
                        break;
                    // FDimension Z
                    case "Foundation Thickness":
                        parameter.Set(TnfFoundation.FDimension.ZDimension.MmToInch());
                        break;
                    // HashiraGataLocation
                    case "柱型_オフセット_X方向":
                        parameter.Set(TnfFoundation.HashiraGataLocation.XDimension.MmToInch());
                        break;
                    case "柱型_オフセット_Y方向":
                        parameter.Set(TnfFoundation.HashiraGataLocation.YDimension.MmToInch());
                        break;
                    // AnchorLean
                    case "アンカー_下のステコン":
                        parameter.Set(TnfFoundation.AnchorLean ? 1 : 0);
                        break;

                    default:

                        break;
                }
            }

        }
    }
}
