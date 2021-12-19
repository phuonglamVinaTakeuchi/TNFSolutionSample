using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFAutoFoundation.TNFModel
{
    public abstract class TNFBase
    {
        public string Name { get; set; }
        public string FullName { get; protected set; }
        public List<TNFParameter> Parameters { get; private set; }
        public TNFBase()
        {
            Parameters = new List<TNFParameter>();
        }
        public abstract void InnitTNFPackageName();
    }
}
