using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFAutoFoundation.Models
{
    public class Dimension2d
    {

        public int XDimension
        {
            get
            {
                var strings = DimensionString.Split('x');
                return strings.Length>=2 ? Convert.ToInt32(strings[0]) : 0;
            }
        }

        public int YDimension
        {
            get
            {
                var strings = DimensionString.Split('x');
                return strings.Length >= 2 ? Convert.ToInt32(strings[1]) : 0;
            }
        }

        public string DimensionString { get; }

        public Dimension2d(string dimensionString)
        {
            DimensionString = dimensionString;
        }
        

        

    }
}
