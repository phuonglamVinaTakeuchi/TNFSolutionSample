using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BaseGeometry;
using TNFData.Models;

namespace TNFData.Geometry.FoundationGeometry
{
    public abstract class FoundationGBase : GeometryBase
    {
        #region Fields
        private Rectangle _hashiraGataColumn;
        #endregion
        #region Properties
        public Point2D TopLeft { get; set; }
        public Point2D TopRight { get; set; }
        public Point2D BottomLeft { get; set; }
        public Point2D BottomRight { get; set; }
        public Point2D OffsetPointLeft { get; set; }
        public Point2D OffsetPointRight { get; set; }
        public Point2D ConcreateBottomLeft { get; set; }
        public Point2D ConcreateBottomRight { get; set; }
        public Rectangle HashiraGataColumn
        {
            get=>_hashiraGataColumn;
            set {
                    _hashiraGataColumn =value;
                    InitHashiraGataOfsetPoint();
                }
        }
        public Point2D HashiraGataOffsetPointLeft { get; set; }
        public Point2D HashiraGataOFfsetPointRight { get; set; }

        public TNFFoundationParams Foundation { get; }

        public List<Point2D> FoundationPoints { get; }
        public List<Point2D> BottomFoundationPoints { get;  }
        public List<Point2D> OffsetPoints { get; }



        #endregion

        public FoundationGBase(Point2D basePoint, TNFFoundationParams tnfFoundation): base(basePoint)
        {
            Foundation = tnfFoundation;
            FoundationPoints = new List<Point2D>();
            BottomFoundationPoints = new List<Point2D>();
            OffsetPoints = new List<Point2D>();
            InitGeometry();
        }
        protected override void InitGeometry() {

            if (Foundation == null)
                return;

            // Tính tọa độ điểm từ điểm cơ sở
            // ĐIểm cơ sở là diểm trục giữa của móng tiếp xúc với sàn

            var baseX = BasePoint.X;
            var baseY = BasePoint.Y;
            var tnfParam = Foundation.TNFParams;

            // Chiều cao móng bao gồm phần ofset với GL
            var foundationTotalHeight = (Foundation.FDimension.ZLength + Foundation.OffsetWithGL)*tnfParam.ScaleRatio;

            //một nữa chiều rộng móng.
            var halfBottomWidth = tnfParam.ScaleRatio*((double)Foundation.BottomWidth) / 2;

            // Tọa độ trục Y của đáy móng
            var bottomY = baseY - foundationTotalHeight;

            // Tọa độ trục Y của đường concreate.
            var concreateY = bottomY - Foundation.ZConcreateThickness*tnfParam.ScaleRatio;

            // Tọa độ trục Y của đường offset của móng.
            var offsetHeightY = baseY - Foundation.OffsetWithGL*tnfParam.ScaleRatio;

            // tọa độ trục X của bottom left
            var bottomLeft = baseX - halfBottomWidth;

            // tọa độ trục x của bottom right
            var bottomRight = baseX + halfBottomWidth;

            var halfTopWidth = tnfParam.ScaleRatio* ((double)Foundation.FDimension.XLength) / 2;

            // Tọa độ trục x của Top Left
            var topLeft = baseX - halfTopWidth;

            // Tọa độ trục y của Top Right
            var topRight = baseX + halfTopWidth;

            TopLeft = new Point2D(topLeft, baseY);
            TopRight = new Point2D(topRight, baseY);
            BottomLeft = new Point2D(bottomLeft, bottomY);
            BottomRight = new Point2D(bottomRight, bottomY);
            ConcreateBottomLeft = new Point2D(bottomLeft, concreateY);
            ConcreateBottomRight = new Point2D(bottomRight, concreateY);
            if (Foundation.OffsetWithGL != 0)
            {
                OffsetPointLeft = new Point2D(topLeft, offsetHeightY);
                OffsetPointRight = new Point2D(topRight, offsetHeightY);
            }


            var basePointForHashiraGata = new Point2D(baseX, offsetHeightY);

            // trong trường hợp 2d và mặt cắt của nhóm Báo giá thì  x và y length của hashiraGata bằng nhau
            var hashiraGataWith = Foundation.HashiraGataDimension.XLength*tnfParam.ScaleRatio;
            // Đảm bảo chiều cao của Hashiragata sẽ là full chiều cao bao gồm cả phần offset with GL
            var hashiraGataHeight = Foundation.HashiraGataDimension.ZLength*tnfParam.ScaleRatio;

            HashiraGataColumn = new Rectangle(basePointForHashiraGata, hashiraGataWith, hashiraGataHeight);




        }
        private void InitHashiraGataOfsetPoint() {
            var tnfParam = Foundation.TNFParams;
            if (HashiraGataColumn == null) return;
            var offsety = HashiraGataColumn.TopLeft.Y - Foundation.HashiraGataOffsetWithGL * tnfParam.ScaleRatio;
            HashiraGataOffsetPointLeft = new Point2D(HashiraGataColumn.TopLeft.X, offsety);
            HashiraGataOFfsetPointRight = new Point2D(HashiraGataColumn.TopRight.X, offsety);

        }
        public Point2D GetCenterPoint()
        {
            if (TopLeft == null || TopRight == null || ConcreateBottomLeft == null || ConcreateBottomRight==null)
                return null;
            var midTop = Point2D.MidPoint(TopLeft, TopRight);
            var midBottom = Point2D.MidPoint(ConcreateBottomLeft, ConcreateBottomRight);
            return Point2D.MidPoint(midTop, midBottom);
        }
    }
}
