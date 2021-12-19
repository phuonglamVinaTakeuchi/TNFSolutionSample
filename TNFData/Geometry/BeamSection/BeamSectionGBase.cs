using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BaseGeometry;
using TNFData.Models.Section;

namespace TNFData.Geometry.BeamSection
{
    public abstract class BeamSectionGBase : GeometryBase
    {
        public Rectangle FirstRevonation { get; set; }
        public Rectangle SecondRevonation { get; set; }
        public Rectangle BottomRightSecondRevonation { get; set; }
        public Rectangle BottomLeftSecondRevonation { get; set; }
        public Rectangle Floor { get; set; }
        public Rectangle Wall { get; set; }
        public BeamGeometry Beam { get; set; }
        public BeamHoleG BeamHole { get; set; }
        public TNFFootingBeamParams BeamSectionData { get;  }

        public BeamSectionGBase(Point2D basePoint, TNFFootingBeamParams beamData) : base(basePoint)
        {
            BeamSectionData = beamData;
            InitGeometry();
        }

        protected override void InitGeometry()
        {
            if (BeamSectionData == null)
                return;
            var tnfParams = BeamSectionData.TNFParameters;
            FirstRevonation = new Rectangle(BasePoint, tnfParams.FirstRevonationWith*tnfParams.ScaleRatio, tnfParams.FirstRevonationDepth*tnfParams.ScaleRatio);
            var basePointForSecond = Point2D.MidPoint(FirstRevonation.TopLeft, FirstRevonation.TopRight);
            SecondRevonation = new Rectangle(basePointForSecond, tnfParams.FirstRevonationWith*tnfParams.ScaleRatio, tnfParams.SecondRevonationDepth*tnfParams.ScaleRatio);

            var yBaseRightRevo = SecondRevonation.BottomLeft.Y;
            var xBaseRightRevo = SecondRevonation.BottomRight.X + tnfParams.ScaleRatio*tnfParams.SecondRevonationWidth/2;
            var rightPointBase = new Point2D(xBaseRightRevo, yBaseRightRevo);
            BottomRightSecondRevonation = new Rectangle(rightPointBase, tnfParams.SecondRevonationWidth*tnfParams.ScaleRatio, tnfParams.SecondRevonationDepth*tnfParams.ScaleRatio);

            var yBaseLeftRevo = SecondRevonation.BottomLeft.Y;
            var xBaseLeftRevo = SecondRevonation.BottomLeft.X - tnfParams.ScaleRatio*tnfParams.SecondRevonationWidth/2;
            var leftPointBase = new Point2D(xBaseLeftRevo, yBaseLeftRevo);
            BottomLeftSecondRevonation = new Rectangle(leftPointBase, tnfParams.SecondRevonationWidth*tnfParams.ScaleRatio, tnfParams.ScaleRatio* tnfParams.SecondRevonationDepth);

            var baseBeamX = BasePoint.X - tnfParams.ScaleRatio* (double)BeamSectionData.Wall.Thickness / 2;
            var baseBeamY = SecondRevonation.TopLeft.Y - tnfParams.ScaleRatio*BeamSectionData.BeamHoleDepth;

            var beamBasePoint = new Point2D(baseBeamX, baseBeamY);

            Beam = new BeamGeometry(beamBasePoint, BeamSectionData.Beam);
            BeamHole = new BeamHoleG(beamBasePoint, BeamSectionData);

        }
        protected void CreateFloorAndWall(Point2D floorBottomRight)
        {
            var tnfParams = BeamSectionData.TNFParameters;
            var baseFloorP = Point2D.MidPoint(Beam.TopLeft, floorBottomRight);
            var floorWidth = (int)Point2D.Distance(Beam.TopLeft, floorBottomRight);
            Floor = new Rectangle(baseFloorP, floorWidth, BeamSectionData.TNFParameters.FloorThickness* tnfParams.ScaleRatio);
            var baseWallPointY = Floor.TopLeft.Y;
            var baseWallPointX = BasePoint.X;
            var wallBasePoint = new Point2D(baseWallPointX, baseWallPointY);
            Wall = new Rectangle(wallBasePoint, BeamSectionData.Wall.Thickness*tnfParams.ScaleRatio, BeamSectionData.Wall.Height*tnfParams.ScaleRatio);
        }
    }
}
