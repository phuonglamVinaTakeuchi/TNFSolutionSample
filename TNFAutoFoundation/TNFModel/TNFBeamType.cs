using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFAutoFoundation.TNFModel
{
    public class TNFBeamType : TNFBase
    {

        public override void InnitTNFPackageName()
        {
            TNFParameter bDimension = null;
            TNFParameter hDimension = null;


            foreach (var param in Parameters)
            {
                switch (param.Name)
                {
                    case TNFString.TNF_BEAM_B:
                        bDimension = param;
                        break;
                    case TNFString.TNF_BEAM_H:
                        hDimension = param;
                        break;
                    default:
                        break;
                }
                if (bDimension != null && hDimension != null )
                    break;
            }

            var bPre = bDimension.Value.Length > 3 ? "" : "0";
            var hPre = hDimension.Value.Length > 3 ? "" : "0";
            FullName = $"B{bPre}{bDimension.Value}xH{hPre}{hDimension.Value}_{Name}";

        }
    }
}
