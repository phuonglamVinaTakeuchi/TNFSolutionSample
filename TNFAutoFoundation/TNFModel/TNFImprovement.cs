using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFAutoFoundation.TNFModel
{
    public class TNFImprovement: TNFBase
    {
        public string Alphabet { get; set; }


        public override void InnitTNFPackageName()
        {
            FullName = Name;

        }
    }
}
