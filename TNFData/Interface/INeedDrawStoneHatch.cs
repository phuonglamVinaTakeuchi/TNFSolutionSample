using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFData.Interface
{
    public interface INeedDrawStoneHatch
    {
        List<List<Point2D>> StoneHatchs { get; }
        void GeneralStoneHatchs();
    }
}
