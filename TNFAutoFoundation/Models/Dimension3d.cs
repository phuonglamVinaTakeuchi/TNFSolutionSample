using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFAutoFoundation.Models
{
    public class Dimension3d : Dimension2d
    {
        public int ZDimension
        {
            get
            {
                var strings = DimensionString.Split('x');
                return strings.Length > 2 ? Convert.ToInt32(strings[2]) : 0;
            }
        }

        public Dimension3d(string dimensionString) : base(dimensionString)
        {
        }
    }
}
