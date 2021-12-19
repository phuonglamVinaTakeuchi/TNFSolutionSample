using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFAutoFoundation.TNFModel
{
    public class TNFFoundationInstance : TNFBase
    {
        public string GridIntersection { get; set;  }
        public double GridOffsetX { get; set; }
        public double GridOffsetY { get; set; }
        public string FoundationId { get; set; }
        public int IdRowIndex { get; set; }
        public int IdColumnIndex {  get; set; }
        public int ColumnInputIndex { get; set; }

        public override void InnitTNFPackageName()
        {

        }
    }
}
