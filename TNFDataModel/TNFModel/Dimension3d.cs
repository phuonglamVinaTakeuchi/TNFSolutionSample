using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.TNFDataAttribute;
using TNFDataModel.TNFModel;

namespace TNFDataModel.TNFModel
{
    public class Dimension3d : TNFBaseModel
    {
        [TNFRevitFamilyName("Length")]
        [PropertyData("Lx",typeof(int),0)]
        public int Lx { get; set; }
        [TNFRevitFamilyName("Width")]
        [PropertyData("Ly", typeof(int), 1)]
        public int Ly { get; set; }

        [TNFRevitFamilyName("Depth")]
        [PropertyData("Lz", typeof(int), 2)]
        public int Lz { get; set; }
        public Dimension3d(string[] sourceContents, string blockName) : base(sourceContents, blockName)
        {
        }
        public Dimension3d()
        {

        }


    }
}
