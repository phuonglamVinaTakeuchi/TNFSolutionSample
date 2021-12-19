using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BaseGeometry;
using TNFData.Geometry.FoundationGeometry;
using TNFData.Models;
using TNFData.Models.Section;

namespace TNFData.Geometry
{
    public abstract class FSectionGBase : GeometryBase
    {
        #region Properties
        /// <summary>
        /// Hố cải tạo 1, hố chính
        /// </summary>
        public Rectangle FirstPrimaryRevo { get; set; }

        /// <summary>
        /// Hố cải tạo 2, hố chính
        /// </summary>
        public Rectangle SecondPrimaryRevo { get; set; }

        /// <summary>
        /// Hố trái hố cải tạo thứ 2, hố chính,
        /// </summary>
        public Rectangle SecondLeftPrimaryRevo { get; set; }

        /// <summary>
        /// Hố Phải hố cải tạo thứ 2, hố chính,
        /// </summary>
        public Rectangle SecondRightPrimaryRevo { get; set; }

        /// <summary>
        /// Tọa độ điểm cho mặt cắt móng
        /// </summary>
        public FoundationGBase MainFoundationSection { get; set; }

        /// <summary>
        /// Thông số cho mặt cắt móng
        /// </summary>
        public TNFFoundationSectionParams FSectionData { get; set; }

        /// <summary>
        /// Tọa độ điểm sàn cho mặt cắt móng
        /// </summary>
        public Rectangle Floor { get; set; }

        public List<Point2D> PrimaryRevoPline { get;  }
        public List<List<Point2D>> RevoHatchPoints { get; }
        public List<List<Point2D>> FoundationHatchPoints { get; }
        public List<Rectangle> NoneRevoRectangles { get; }

        #endregion
        protected FSectionGBase(Point2D basePoint, TNFFoundationSectionParams fData) : base(basePoint)
        {
            FSectionData = fData;
            PrimaryRevoPline = new List<Point2D>();
            RevoHatchPoints = new List<List<Point2D>>();
            FoundationHatchPoints = new List<List<Point2D>>();
            NoneRevoRectangles = new List<Rectangle> ();
            InitGeometry();
        }

        protected override void InitGeometry()
        {
            if (FSectionData == null)
                return;
            var tnfParams = FSectionData.TNFParameters;
            // Hố cải tạo dưới cùng
            FirstPrimaryRevo = new Rectangle(BasePoint, tnfParams.FirstRevonationWith*tnfParams.ScaleRatio, tnfParams.FirstRevonationDepth*tnfParams.ScaleRatio);

            // Hố cải tạo thứ 2
            var basePointForSecondRevo = Point2D.MidPoint(FirstPrimaryRevo.TopLeft, FirstPrimaryRevo.TopRight);
            SecondPrimaryRevo = new Rectangle(basePointForSecondRevo, tnfParams.FirstRevonationWith*tnfParams.ScaleRatio, tnfParams.SecondRevonationDepth*tnfParams.ScaleRatio);

            // Hố phải hố cải tạo 2
            var baseoffsetXLeft = BasePoint.X - ((double)tnfParams.FirstRevonationWith / 2 + (double)tnfParams.SecondRevonationWidth / 2)*tnfParams.ScaleRatio;
            var baseOffsetXRight = BasePoint.X + ((double)tnfParams.FirstRevonationWith / 2 + (double)tnfParams.SecondRevonationWidth / 2)*tnfParams.ScaleRatio;

            var basePointLeft = new Point2D(baseoffsetXLeft, FirstPrimaryRevo.TopLeft.Y);
            var basePointRight = new Point2D(baseOffsetXRight, FirstPrimaryRevo.TopLeft.Y);

            SecondLeftPrimaryRevo = new Rectangle(basePointLeft, tnfParams.SecondRevonationWidth*tnfParams.ScaleRatio, tnfParams.SecondRevonationDepth* tnfParams.ScaleRatio);
            SecondRightPrimaryRevo = new Rectangle(basePointRight, tnfParams.SecondRevonationWidth*tnfParams.ScaleRatio, tnfParams.SecondRevonationDepth*tnfParams.ScaleRatio);
        }
    }
}
