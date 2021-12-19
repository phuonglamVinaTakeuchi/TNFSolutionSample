using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.TNFDataAttribute;
using TNFDataModel.TNFModel;

namespace TNFDataModel.TNFModel
{
    public class RebarDiameter : TNFBaseModel
    {
        public string Symbol { get => "D"; }

        /// <summary>
        /// Số lượng rebar, ví dụ 100
        /// </summary>

        [PropertyData("Quantities",typeof(int), 0)]
        public int Quantities { get; set; }

        /// <summary>
        /// Duong kinh Rebar
        /// Rebar Diameter ví dụ D16
        /// </summary>
        [PropertyData("Diameter",typeof(int), 1)]
        public int Diameter { get; set; }

        public string RebarSizeName => Symbol + Diameter;
        /// <summary>
        /// Rebar Spacing ví dụ @600
        /// </summary>

        public int Spacing { get; set; }

        public RebarDiameter(string[] sourceContents, string blockName) : base(sourceContents, blockName)
        {
        }



    }
}
