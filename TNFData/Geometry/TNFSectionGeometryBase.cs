using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BaseGeometry;
using TNFData.Geometry.BeamSection;
using TNFData.Models.Section;

namespace TNFData.Geometry
{
    public abstract class TNFSectionGeometryBase : GeometryBase
    {

        public FSectionGBase FSection { get; set; }
        public BeamSectionGBase BeamSection { get; set; }
        public double MinX { get; set; }
        public double MaxX { get; set; }

        public TNFSection TNFSectionData { get; }

        public TNFSectionGeometryBase(Point2D basePoint, TNFSection tnfSectionData) : base(basePoint)
        {
            TNFSectionData = tnfSectionData;
            MinX = BasePoint.X;
            InitGeometry();
        }
        //protected abstract void InitGeometry();
    }
}
