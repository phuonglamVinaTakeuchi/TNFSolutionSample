using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFData.Geometry.BaseGeometry
{
    public abstract class GeometryBase
    {
        #region Fields
        private Point2D _basePoint;
        #endregion
        #region Properties
        public Point2D BasePoint
        {
            get => _basePoint;
            set
            {
                _basePoint = value;
                //InitGeometry();
            }
        }
        #endregion


        public GeometryBase(Point2D basePoint)
        {
            BasePoint = basePoint;
        }
        protected abstract void InitGeometry();
    }
}
