using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Models;

namespace TNFData.Geometry.BaseGeometry
{
    public class PileGeometry : GeometryBase
    {
        public TNFPile PileData { get;private set;  }
        private const double _pileHeight = 1715;
        private const double _pileDst = 965;
        private const double _pileDst2 = 1215;
        public Point2D TopLeft { get; set; }
        public Point2D TopRight { get; set; }
        public Point2D BottomPoint { get; set; }
        public Point2D MidPoint { get; set; }
        public Point2D BottomLeft { get; set; }
        public Point2D BottomRight { get; set; }
        public PileGeometry(Point2D basePoint,TNFPile pileData) : base(basePoint)
        {
            PileData = pileData;
            InitGeometry();
        }

        protected override void InitGeometry()
        {
            var tnfParam = PileData.TNFParams;
            var leftX = BasePoint.X - tnfParam.ScaleRatio* PileData.Radius / 2;
            var rightX = BasePoint.X + tnfParam.ScaleRatio*PileData.Radius / 2;
            TopLeft = new Point2D(leftX, BasePoint.Y);
            TopRight = new Point2D(rightX, BasePoint.Y);
            var bottomY = BasePoint.Y - _pileHeight*tnfParam.ScaleRatio;
            BottomPoint = new Point2D(BasePoint.X, bottomY);
            var midPointY = BasePoint.Y - _pileDst*tnfParam.ScaleRatio;
            MidPoint = new Point2D(BasePoint.X, midPointY);
            var btY = BasePoint.Y - _pileDst2*tnfParam.ScaleRatio;
            BottomLeft = new Point2D(leftX, btY);
            BottomRight = new Point2D(rightX, btY);
        }
    }
}
