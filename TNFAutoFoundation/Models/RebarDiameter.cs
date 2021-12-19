using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFAutoFoundation.Models
{
    public class RebarDiameter
    {
        public string RebarSymbol => "D";
        public int ReBarRadius { get;  }
        public int RebarSpacing { get;  }
        public string RebarString { get;  }
        public int RebarQuantities { get;  }
        private bool _isConstrucQty;

        public RebarDiameter(string rebarString, bool isConstrucQty = false)
        {
            RebarString = rebarString;
            _isConstrucQty = isConstrucQty;

            var strings = RebarString.Split('@');
            if (strings.Length < 2) return;
            if (strings[0].Contains('D'))
            {
                var radius = Convert.ToInt32(strings[0].Trim('D'));
                ReBarRadius = radius;
                RebarSpacing = Convert.ToInt32(strings[1]);
            }
            else
            {
                RebarQuantities = Convert.ToInt32(strings[0]);
                ReBarRadius = Convert.ToInt32(strings[1].Trim('D'));
            }

        }

    }
}
