using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.TNFDataAttribute;

namespace TNFDataModel.TNFModel
{

    public class TNFFoundationInstance
    {

        [TNFRevitFamilyName("Type Mark")]
        public string TypeMark
        {
            get;set;
        }

        [TNFRevitFamilyName("通り芯_ラベル")]
        public string GridIntersection { get; set; }

        [TNFRevitFamilyName("GridOffsetX")]
        public int FoundationOffsetX { get; set; }

        [TNFRevitFamilyName("GridOffsetY")]
        public int FoundationOffsetY { get; set; }

    }
}
