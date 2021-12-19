using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFData.Geometry.BaseGeometry
{
    /// <summary>
    /// Hình Thang
    /// </summary>
    public class Trapezium : GeometryBase
    {

        private double _bottomWidth;
        private double _topWidth;
        private double _height;

        public Point2D BottomRight { get; set; }
        public Point2D TopRight { get; set; }
        public Point2D BottomLeft { get; set; }
        public Point2D TopLeft { get; set; }

        /// <summary>
        /// Chiều cao của hình chữ nhật
        /// </summary>
        public double BottomWidth
        {
            get => _bottomWidth;
            set
            {
                _bottomWidth = value;
                //InitPoint();
            }
        }
        /// <summary>
        /// Chiều rộng của hình chữ nhật
        /// Thay đổi chiều rộng sẽ tự động tính toán lại tọa độ các điểm của hình chữ nhật
        /// </summary>
        public double TopWidth
        {
            get => _topWidth;
            set
            {
                _topWidth = value;
                //InitPoint();
            }
        }

        /// <summary>
        /// Chiều cao của hình chữ nhật
        /// Thay đổi chiều cao sẽ tự động tính toán lại tọa độ các điểm của hình chữ nhật
        /// </summary>
        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                //InitPoint();
            }
        }


        public Trapezium(Point2D basePoint,double bottomWidth,double topWidth,double height):base(basePoint)
        {
            BottomWidth = bottomWidth;
            TopWidth = topWidth;
            Height = height;
            InitGeometry();

        }
        protected override void InitGeometry()
        {
            var yTop = BasePoint.Y + Height;
            var xBottomLeft = BasePoint.X - (double)BottomWidth / 2;
            var xBottomRight = BasePoint.X + (double)BottomWidth / 2;
            var xTopLeft = BasePoint.X - (double)TopWidth / 2;
            var xTopRight = BasePoint.X + (double)TopWidth / 2;

            TopLeft = new Point2D(xTopLeft, yTop);
            TopRight = new Point2D(xTopRight, yTop);
            BottomLeft = new Point2D(xBottomLeft, BasePoint.Y);
            BottomRight = new Point2D(xBottomRight, BasePoint.Y);

        }
    }
}
