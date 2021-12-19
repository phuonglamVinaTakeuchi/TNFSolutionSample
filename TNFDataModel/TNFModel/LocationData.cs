using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFDataModel.TNFModel
{
    public class LocationData
    {
        public string XName { get; set; }
        public string YName { get; set; }
        public string InterSectionName => XName + "-" + YName;
        public Dimension3d HashiraGataDim { get; set; }
        public TNFFoundationType FoundationType{get;set;}
        public int GridOffsetX { get; set; }
        public int GridOffsetY { get; set; }

    }
}
