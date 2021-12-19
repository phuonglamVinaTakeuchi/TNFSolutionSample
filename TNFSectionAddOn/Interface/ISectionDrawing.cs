using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry;
using Autodesk.AutoCAD;
using Autodesk.AutoCAD.Geometry;

namespace TNFSectionAddOn.Interface
{
    public interface ISectionDrawing
    {
        void DrawSection(Point3d basePoint, TNFSectionGeometryBase TNFSectionGeometry);
    }
}
