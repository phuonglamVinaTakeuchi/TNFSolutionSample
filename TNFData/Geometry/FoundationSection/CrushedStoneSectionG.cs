using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Factory;
using TNFData.Geometry.BaseGeometry;
using TNFData.Geometry.BeamSection;
using TNFData.Geometry.FoundationGeometry;
using TNFData.Interface;
using TNFData.Models;
using TNFData.Models.Section;

namespace TNFData.Geometry.FoundationSection
{
    public class CrushedStoneSectionG : NormalSectionG , INeedDrawStoneHatch,IHasSubFoundation
    {
        public Rectangle StoneS { get; set; }
        public Trapezium FloorV { get; set; }

        public Rectangle SubRevoFirst { get; set; }
        public Rectangle SubRevoSecond { get; set; }
        public FoundationGBase SubFoundation { get; set; }
        public Rectangle SubStone { get; set; }
        public Rectangle SubFloor { get; set; }
        public Rectangle SecondRightSubRevo { get; set; }
        public Rectangle SecondLeftSubRevo { get; set; }
        public BeamGeometry SubBeamGeometry { get; set; }
        public List<Point2D> SubRevoPoints { get; }
        public List<List<Point2D>> FloorHatchPoints { get; }

        public List<List<Point2D>> StoneHatchs { get; }

        public CrushedStoneSectionG(Point2D basePoint, TNFFoundationSectionParams fData) : base(basePoint, fData)
        {
            FloorHatchPoints = new List<List<Point2D>>();
            SubRevoPoints = new List<Point2D>();
            StoneHatchs = new List<List<Point2D>>();
            InitGeometry();
        }
        protected override void InitGeometry()
        {
            base.InitGeometry();
            var tnfParams = FSectionData.TNFParameters;
            var basePointStone = Point2D.MidPoint(SecondLeftPrimaryRevo.TopLeft, SecondRightPrimaryRevo.TopRight);

            var basePointForF = new Point2D(basePointStone.X, basePointStone.Y);
            MainFoundationSection = TNFFoundationGFactory.CreateFoundationGBase(basePointForF, FSectionData.MainFoundation);
            var floorWidth = (tnfParams.FirstRevonationWith + tnfParams.SecondRevonationWidth * 2)* tnfParams.ScaleRatio;

            StoneS = new Rectangle(basePointStone, floorWidth, tnfParams.StoneThickness*tnfParams.ScaleRatio);

            var baseFloor = new Point2D(BasePoint.X, StoneS.TopLeft.Y);
            Floor = new Rectangle(baseFloor, floorWidth, tnfParams.FloorThickness*tnfParams.ScaleRatio);

            FloorV = new Trapezium(basePointStone, tnfParams.CrushedStoneVBottomWidth*tnfParams.ScaleRatio,
                                   (tnfParams.CrushedStoneVBottomWidth + tnfParams.CrushedStoneOpenHolePitchDistance * 2)*tnfParams.ScaleRatio,
                                   tnfParams.StoneThickness*tnfParams.ScaleRatio);

            var baseXForSub = BasePoint.X - (tnfParams.FirstRevonationWith + 2*tnfParams.SecondRevonationWidth + TNFGlobalInfo.SECTION_DISTANCE)*tnfParams.ScaleRatio;
            var baseYForSub = BasePoint.Y;

            var basePointForSubRevo = new Point2D(baseXForSub, baseYForSub);
            SubRevoFirst = new Rectangle(basePointForSubRevo, tnfParams.FirstRevonationWith*tnfParams.ScaleRatio, tnfParams.FirstRevonationDepth*tnfParams.ScaleRatio);
            var basePointForSecond = Point2D.MidPoint(SubRevoFirst.TopLeft, SubRevoFirst.TopRight);

            SubRevoSecond = new Rectangle(basePointForSecond, tnfParams.FirstRevonationWith*tnfParams.ScaleRatio, tnfParams.SecondRevonationDepth*tnfParams.ScaleRatio);


            var baseYForSubFoundation = SubRevoSecond.TopLeft.Y;
            var basePointForSubFoundation = new Point2D(baseXForSub, baseYForSubFoundation);
            SubFoundation = TNFFoundationGFactory.CreateFoundationGBase(basePointForSubFoundation, this.FSectionData.SubFoundation);

            var baseXForSubBeam = baseXForSub - tnfParams.ScaleRatio* FSectionData.CrushedBeamData.BeamWidth/2;
            var basePointForSubBeam = new Point2D(baseXForSubBeam, baseYForSubFoundation- FSectionData.SubFoundation.OffsetWithGL*tnfParams.ScaleRatio);

            // Tính toán tọa độ cho Beam thuộc Crushed Beam
            SubBeamGeometry = new BeamGeometry(basePointForSubBeam, FSectionData.CrushedBeamData);

            var baseXForRightSub = SubRevoFirst.TopRight.X + tnfParams.ScaleRatio*(double)tnfParams.SecondRevonationWidth / 2;
            var basePointForRightSub = new Point2D(baseXForRightSub, SubRevoFirst.TopRight.Y);
            SecondRightSubRevo = new Rectangle(basePointForRightSub, tnfParams.SecondRevonationWidth* tnfParams.ScaleRatio, tnfParams.SecondRevonationDepth*tnfParams.ScaleRatio);
            var baseXForLeftSub = SubRevoFirst.TopLeft.X - tnfParams.ScaleRatio* (double)tnfParams.SecondRevonationWidth / 2;
            var basePointForLeftSub = new Point2D(baseXForLeftSub, SubRevoFirst.TopRight.Y);
            SecondLeftSubRevo = new Rectangle(basePointForLeftSub, tnfParams.ScaleRatio*tnfParams.SecondRevonationWidth,tnfParams.ScaleRatio* tnfParams.SecondRevonationDepth);

            var baseXForSubStone = (baseXForSubBeam + SecondRightSubRevo.TopRight.X) / 2;

            var basePointForSubStone = new Point2D(baseXForSubStone, SecondRightSubRevo.TopLeft.Y);
            var subStoneWidth = (int)(SecondRightSubRevo.TopRight.X - baseXForSubBeam);
            SubStone = new Rectangle(basePointForSubStone, subStoneWidth, tnfParams.StoneThickness* tnfParams.ScaleRatio);
            var subFloorBasePoint = Point2D.MidPoint(SubStone.TopLeft, SubStone.TopRight);
            SubFloor = new Rectangle(subFloorBasePoint, subStoneWidth, tnfParams.FloorThickness* tnfParams.ScaleRatio);
            GeneralPoints(tnfParams);
        }
        private void GeneralPoints(TNFGlobalInfo tnfParams)
        {
            GeneralPrimaryRevoPoints();
            GeneralNoneRevoRect();
            GeneralSubRevoPoints();
            GeneralMainRevoHatch();
            GeneralMainFoundationHatchs();
            GeneralFloorHatchPoints();
            GeneralStoneHatchs();
        }
        protected override void GeneralMainRevoHatch()
        {
            if (SubRevoSecond == null)
                return;
            RevoHatchPoints.Clear();
            base.GeneralMainRevoHatch();
            var subRevoHatch = new List<Point2D>();
            subRevoHatch.Add(SubRevoSecond.TopLeft);
            subRevoHatch.AddRange(SubFoundation.FoundationPoints);
            subRevoHatch.Add(SubRevoSecond.TopRight);
            subRevoHatch.Add(SubRevoFirst.BottomRight);
            subRevoHatch.Add(SubRevoFirst.BottomLeft);
            subRevoHatch.Add(SubRevoSecond.TopLeft);
            RevoHatchPoints.Add(subRevoHatch);

        }
        private void GeneralMainFoundationHatchs()
        {
            if (SubFoundation == null)
                return;
            FoundationHatchPoints.Clear();
            var mainFoundation = new List<Point2D>();

            mainFoundation.Add(MainFoundationSection.TopLeft);
            mainFoundation.AddRange(MainFoundationSection.BottomFoundationPoints);
            mainFoundation.Add(MainFoundationSection.TopRight);
            mainFoundation.Add(MainFoundationSection.TopLeft);
            FoundationHatchPoints.Add(mainFoundation);

            var subF = new List<Point2D>();
            subF.Add(SubFoundation.TopLeft);
            subF.AddRange(SubFoundation.BottomFoundationPoints);
            subF.Add(SubFoundation.TopRight);
            subF.Add(SubFoundation.TopLeft);
            FoundationHatchPoints.Add(subF);

        }
        private void GeneralPrimaryRevoPoints()
        {
            PrimaryRevoPline.Clear();
            PrimaryRevoPline.Add(SecondLeftPrimaryRevo.BottomLeft);
            PrimaryRevoPline.Add(FirstPrimaryRevo.TopLeft);
            PrimaryRevoPline.Add(FirstPrimaryRevo.BottomLeft);
            PrimaryRevoPline.Add(FirstPrimaryRevo.BottomRight);
            PrimaryRevoPline.Add(FirstPrimaryRevo.TopRight);
            PrimaryRevoPline.Add(SecondRightPrimaryRevo.BottomRight);
        }
        private void GeneralNoneRevoRect()
        {
            NoneRevoRectangles.Clear();
            NoneRevoRectangles.Add(SecondLeftPrimaryRevo);
            NoneRevoRectangles.Add(SecondRightPrimaryRevo);
            NoneRevoRectangles.Add(SecondLeftSubRevo);
            NoneRevoRectangles.Add(SecondRightSubRevo);
        }
        private void GeneralSubRevoPoints()
        {
            if (SubRevoPoints == null)
                return;
            SubRevoPoints.Clear();
            SubRevoPoints.Add(SecondLeftSubRevo.BottomLeft);
            SubRevoPoints.Add(SecondLeftSubRevo.BottomRight);
            SubRevoPoints.Add(SubRevoFirst.BottomLeft);
            SubRevoPoints.Add(SubRevoFirst.BottomRight);
            SubRevoPoints.Add(SecondRightSubRevo.BottomLeft);
            SubRevoPoints.Add(SecondRightSubRevo.BottomRight);
        }

        private void GeneralFloorHatchPoints()
        {
            if (FloorHatchPoints == null)
                return;
            FloorHatchPoints.Clear();

            var mainFloor = new List<Point2D>();

            mainFloor.Add(Floor.TopLeft);
            mainFloor.Add(Floor.TopRight);

            mainFloor.Add(Floor.BottomRight);
            mainFloor.Add(FloorV.TopRight);
            mainFloor.Add(FloorV.BottomRight);
            mainFloor.Add(FloorV.BottomLeft);
            mainFloor.Add(FloorV.TopLeft);
            mainFloor.Add(Floor.BottomLeft);
            mainFloor.Add(Floor.TopLeft);

            FloorHatchPoints.Add(mainFloor);

            var subFloor = new List<Point2D>() {
                SubBeamGeometry.BottomLeft,
                SubFloor.TopLeft,
                SubFloor.TopRight,
                SubFloor.BottomRight,
                SubBeamGeometry.TopRight,
                SubBeamGeometry.BotomRight,
                SubBeamGeometry.BottomLeft
            };
            FloorHatchPoints.Add(subFloor);

            //var subFoundation = new List<Point2D>();
            //subFoundation.Add(SubFoundation.TopLeft);
            //subFoundation.AddRange(SubFoundation.BottomFoundationPoints);
            //subFoundation.Add(SubFoundation.TopRight);
            //subFoundation.Add(SubFoundation.TopLeft);
            //FloorHatchPoints.Add(subFoundation);


        }

        public void GeneralStoneHatchs()
        {
            if (StoneHatchs == null)
                return;
            StoneHatchs.Clear();
            var mainLeftStoneHatch = new List<Point2D>();

            mainLeftStoneHatch.Add(StoneS.TopLeft);
            mainLeftStoneHatch.Add(FloorV.TopLeft);
            mainLeftStoneHatch.Add(FloorV.BottomLeft);
            mainLeftStoneHatch.Add(StoneS.BottomLeft);
            mainLeftStoneHatch.Add(StoneS.TopLeft);
            StoneHatchs.Add(mainLeftStoneHatch);

            var maintRightStoneHatch = new List<Point2D>();
            maintRightStoneHatch.Add(FloorV.TopRight);
            maintRightStoneHatch.Add(FloorV.BottomRight);
            maintRightStoneHatch.Add(StoneS.BottomRight);
            maintRightStoneHatch.Add(StoneS.TopRight);
            maintRightStoneHatch.Add(FloorV.TopRight);
            StoneHatchs.Add(maintRightStoneHatch);

            var subStoneHatch = new List<Point2D>();
            subStoneHatch.Add(SubBeamGeometry.TopRight);
            subStoneHatch.Add(SubFloor.BottomRight);
            subStoneHatch.Add(SubStone.BottomRight);
            if (SubFoundation.Foundation.OffsetWithGL == 0)
            {

                subStoneHatch.Add(SubBeamGeometry.ConcreateBottomRight);
                subStoneHatch.Add(SubBeamGeometry.BotomRight);

            }
            else
            {
                var segmen1 = new Segment2D(SubBeamGeometry.BotomRight, SubBeamGeometry.TopRight);
                var segmen2 = new Segment2D(StoneS.BottomLeft, StoneS.BottomRight);

                Segment2D.IntersectionLine(segmen1, segmen2, out var point);
                subStoneHatch.Add(point);

            }
            subStoneHatch.Add(SubBeamGeometry.TopRight);
            StoneHatchs.Add(subStoneHatch);
            //subStoneHatch.Add();
        }

    }
}
