using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFAutoFoundation.TNFModel
{
    public class TNFFoundation : TNFBase
    {
        public string FoundationType { get; set; }


        public override void InnitTNFPackageName()
        {

            TNFParameter xDimension = null;
            TNFParameter yDimension = null;
            TNFParameter zDimension = null;
            TNFParameter fType = null;

            foreach (var param in Parameters)
            {
                switch (param.Name)
                {
                    case TNFString.TNF_XLENGTH:
                        xDimension = param;
                        break;
                    case TNFString.TNF_YLENGTH:
                        yDimension = param;
                        break;
                    case TNFString.TNF_ZLENGTH:
                        zDimension = param;
                        break;
                    case TNFString.TNF_FTYPE:
                        fType = param;
                        break;
                    default:
                        break;
                }
                if (xDimension != null && yDimension != null && zDimension != null && fType!=null)
                    break;
            }

            FullName = $"W{xDimension.Value}xL{yDimension.Value}xD{zDimension.Value}x{Name}";
        }
    }
}
