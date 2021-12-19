using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFData.Models
{
    public class Dimension3d : Dimension2d
    {
        private int _zLength;
        public int ZLength { get=>_zLength; set=>SetProperty(ref _zLength,value); }
    }
}
