using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BaseGeometry;
using TNFData.Geometry.FoundationGeometry;

namespace TNFData.Interface
{
    public interface IHasSubFoundation
    {
        FoundationGBase SubFoundation { get; }
        Rectangle SubRevoFirst { get; }
    }
}
