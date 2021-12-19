using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFData.Geometry.BaseGeometry
{
    public class Rectangle : GeometryBase
    {
        private double _height;
        private double _width;
        public Point2D BottomRight { get; set; }
        public Point2D TopRight { get; set; }
        public Point2D BottomLeft { get; set; }
        public Point2D TopLeft { get; set; }

        /// <summary>
        /// Chiều cao của hình chữ nhật
        /// Thay đổi chiều cao sẽ tự động tính toán lại tọa độ các điểm của hình chữ nhật
        /// </summary>
        public double Height
        {
            get => _height;
            set {
                _height = value;
                //InitPoint();
            }
         }
        /// <summary>
        /// Chiều rộng của hình chữ nhật
        /// Thay đổi chiều rộng sẽ tự động tính toán lại tọa độ các điểm của hình chữ nhật
        /// </summary>
        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                //InitPoint();
            }
        }

        /// <summary>
        /// An Rectangle Geometry
        /// BasePoint là điểm nằm giữa thuộc cạnh dưới của Rectangle
        /// tọa độ các điểm còn lại của rectangle sẽ được tính toán từ điểm này.
        /// Khi khởi tạo lưu ý BasePoint ko được null.
        /// </summary>
        /// <param name="basePoint"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rectangle(Point2D basePoint,double width, double height): base(basePoint)
        {
            Width = width;
            Height = height;
            InitGeometry();

        }

        /// <summary>
        /// Hàm khởi tạo để tính toán tọa độ các điểm của hình chữ nhật
        /// </summary>
        protected override void InitGeometry()
        {
            // Check BasePoint null or not before Calculator
            if (BasePoint == null) return;

            var baseX = BasePoint.X;
            var baseY = BasePoint.Y;
            var halfWidth = (double)Width / 2;
            var leftX = baseX - halfWidth;
            var rightX = baseX + halfWidth;
            var topY = baseY + Height;

            BottomLeft = new Point2D(leftX, baseY);
            BottomRight = new Point2D(rightX, baseY);
            TopLeft = new Point2D(leftX, topY);
            TopRight = new Point2D(rightX, topY);
        }
        public Point2D GetCenterPoint()
        {
            return Point2D.MidPoint(TopLeft, BottomRight);
        }
    }
}
