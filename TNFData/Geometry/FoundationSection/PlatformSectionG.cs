using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Factory;
using TNFData.Geometry.BaseGeometry;
using TNFData.Geometry.FoundationGeometry;
using TNFData.Interface;
using TNFData.Models;
using TNFData.Models.Section;

namespace TNFData.Geometry.FoundationSection
{
    public class PlatformSectionG : FSectionGBase,IHasSubFoundation
    {
        public Point2D PlatformPoint { get; set; }
        public Rectangle ThirdPrimaryRevo { get; set; }
        public Rectangle ThirdLeftPrimaryRevo { get; set; }
        public Rectangle ThirdRightPrimaryRevo { get; set; }
        public FoundationGBase SubFoundation { get; set; }
        public List<Point2D> SubRevonationHatchPoints{get;}
        public Rectangle SubRevoFirst { get; set; }
        public Rectangle SecondSubRevo { get; set; }
        public PlatformSectionG(Point2D basePoint, TNFFoundationSectionParams fData) : base(basePoint, fData)
        {
            SubRevonationHatchPoints = new List<Point2D>();
            InitGeometry();
        }

        protected override void InitGeometry()
        {
            base.InitGeometry();
            var tnfParams = FSectionData.TNFParameters;
            var basePointForThirdRevo = Point2D.MidPoint(SecondPrimaryRevo.TopLeft, SecondPrimaryRevo.TopRight);

            ThirdPrimaryRevo = new Rectangle(basePointForThirdRevo, tnfParams.FirstRevonationWith*tnfParams.ScaleRatio, tnfParams.ThirdRevonationDepth*tnfParams.ScaleRatio);

            var basePointForMain = Point2D.MidPoint(ThirdPrimaryRevo.TopLeft,ThirdPrimaryRevo.TopRight);
            MainFoundationSection = TNFFoundationGFactory.CreateFoundationGBase(basePointForMain, FSectionData.MainFoundation);


            var basePointForLeftThird = Point2D.MidPoint(SecondLeftPrimaryRevo.TopLeft, SecondLeftPrimaryRevo.TopRight);
            var basePointForRightThird = Point2D.MidPoint(SecondRightPrimaryRevo.TopLeft, SecondRightPrimaryRevo.TopRight);
            ThirdLeftPrimaryRevo = new Rectangle(basePointForLeftThird, tnfParams.SecondRevonationWidth*tnfParams.ScaleRatio, tnfParams.ThirdRevonationDepth*tnfParams.ScaleRatio);
            ThirdRightPrimaryRevo = new Rectangle(basePointForRightThird, tnfParams.SecondRevonationWidth*tnfParams.ScaleRatio, tnfParams.ThirdRevonationDepth*tnfParams.ScaleRatio);

            var xLocationForSubFirstRevo = BasePoint.X - (tnfParams.FirstRevonationWith + tnfParams.SecondRevonationWidth)*tnfParams.ScaleRatio;
            var basePointForSubFirstRevo = new Point2D(xLocationForSubFirstRevo, BasePoint.Y);

            SubRevoFirst = new Rectangle(basePointForSubFirstRevo, tnfParams.FirstRevonationWith*tnfParams.ScaleRatio, tnfParams.FirstRevonationDepth*tnfParams.ScaleRatio);
            var basePointForSubSecondRevo = Point2D.MidPoint(SubRevoFirst.TopLeft, SubRevoFirst.TopRight);
            SecondSubRevo = new Rectangle(basePointForSubSecondRevo, tnfParams.FirstRevonationWith*tnfParams.ScaleRatio, tnfParams.SecondRevonationDepth*tnfParams.ScaleRatio);

            var xLocationBaseForSubF = basePointForSubSecondRevo.X;
            var yLocationBaseForSubF = SecondSubRevo.TopLeft.Y;
            var basePointForSubF = new Point2D(xLocationBaseForSubF, yLocationBaseForSubF);

            SubFoundation = TNFFoundationGFactory.CreateFoundationGBase(basePointForSubF, FSectionData.SubFoundation);

            var baseFloorX = (SubFoundation.HashiraGataColumn.TopLeft.X + ThirdRightPrimaryRevo.TopRight.X)/2 ;
            var baseFloorY = SubFoundation.HashiraGataColumn.TopLeft.Y;

            var basePointForFloor = new Point2D(baseFloorX, baseFloorY);
            var floorWidth = (int) (ThirdRightPrimaryRevo.TopRight.X - SubFoundation.HashiraGataColumn.TopLeft.X);
            Floor = new Rectangle(basePointForFloor, floorWidth, tnfParams.FloorThickness*tnfParams.ScaleRatio);

            var xAxistForPlatformPoint = SubFoundation.TopRight.X;
            var yAxistForPlatformPoint = ThirdPrimaryRevo.TopLeft.Y;
            PlatformPoint = new Point2D(xAxistForPlatformPoint, yAxistForPlatformPoint);


            GeneralPoints(tnfParams);
        }
        protected virtual void GeneralPoints(TNFGlobalInfo tnfParams)
        {

            GeneralPrimaryRevoPline();
            GeneralNoneRevoList();
            GeneralRevoHatchPoints();
            GeneralFoundationHatch();

        }
        private void GeneralPrimaryRevoPline()
        {
            PrimaryRevoPline.Clear();
            PrimaryRevoPline.Add(SecondSubRevo.TopLeft);
            PrimaryRevoPline.Add(SubRevoFirst.BottomLeft);
            PrimaryRevoPline.Add(SubRevoFirst.BottomRight);
            PrimaryRevoPline.Add(SubRevoFirst.TopRight);
            PrimaryRevoPline.Add(FirstPrimaryRevo.TopLeft);
            PrimaryRevoPline.Add(FirstPrimaryRevo.BottomLeft);
            PrimaryRevoPline.Add(FirstPrimaryRevo.BottomRight);
            PrimaryRevoPline.Add(FirstPrimaryRevo.TopRight);
            PrimaryRevoPline.Add(SecondRightPrimaryRevo.BottomRight);
        }
        private void GeneralNoneRevoList()
        {
            NoneRevoRectangles.Clear();
            NoneRevoRectangles.Add(SecondLeftPrimaryRevo);
            NoneRevoRectangles.Add(SecondRightPrimaryRevo);
            NoneRevoRectangles.Add(ThirdLeftPrimaryRevo);
            NoneRevoRectangles.Add(ThirdRightPrimaryRevo);
        }
        private void GeneralRevoHatchPoints()
        {
            if (this.SubFoundation == null)
            {
                return;
            }
            RevoHatchPoints.Clear();

            var mainRevo = new List<Point2D>();
            mainRevo.Add(ThirdPrimaryRevo.TopLeft);
            mainRevo.AddRange(MainFoundationSection.FoundationPoints);
            mainRevo.Add(ThirdPrimaryRevo.TopRight);
            mainRevo.Add(FirstPrimaryRevo.BottomRight);
            mainRevo.Add(FirstPrimaryRevo.BottomLeft);
            mainRevo.Add(ThirdPrimaryRevo.TopLeft);

            RevoHatchPoints.Add(mainRevo);
            var subRevo = new List<Point2D>();
            subRevo.Add(SecondSubRevo.TopLeft);
            subRevo.AddRange(SubFoundation.FoundationPoints);
            subRevo.Add(PlatformPoint);
            subRevo.Add(ThirdLeftPrimaryRevo.TopLeft);
            subRevo.Add(SubRevoFirst.BottomRight);
            subRevo.Add(SubRevoFirst.BottomLeft);
            subRevo.Add(SecondSubRevo.TopLeft);
            RevoHatchPoints.Add(subRevo);

        }
        protected virtual void GeneralFoundationHatch()
        {
            if (SecondSubRevo == null)
                return;
            FoundationHatchPoints.Clear();
            var mainFOundation = new List<Point2D>();
            mainFOundation.Add(Floor.TopLeft);
            mainFOundation.Add(Floor.TopRight);
            mainFOundation.Add(Floor.BottomRight);
            var fPoints = new List<Point2D>(MainFoundationSection.FoundationPoints);
            fPoints.Reverse();
            mainFOundation.AddRange(fPoints);
            mainFOundation.Add(SubFoundation.HashiraGataColumn.TopRight);
            mainFOundation.Add(SubFoundation.HashiraGataColumn.BottomRight);
            mainFOundation.Add(SubFoundation.HashiraGataColumn.BottomLeft);
            mainFOundation.Add(SubFoundation.HashiraGataColumn.TopLeft);
            FoundationHatchPoints.Add(mainFOundation);
            GeneralSubFoundationHatch();
        }
        protected void GeneralSubFoundationHatch()
        {
            var subFoundation = new List<Point2D>();
            if (SubFoundation.Foundation.OffsetWithGL > 0)
            {
                subFoundation.Add(SubFoundation.OffsetPointLeft);
                subFoundation.AddRange(SubFoundation.BottomFoundationPoints);
                subFoundation.Add(SubFoundation.OffsetPointRight);
                subFoundation.Add(SubFoundation.OffsetPointLeft);
            }
            else
            {
                subFoundation.Add(SubFoundation.TopLeft);
                subFoundation.AddRange(SubFoundation.BottomFoundationPoints);
                subFoundation.Add(SubFoundation.TopRight);
                subFoundation.Add(SubFoundation.TopLeft);
            }
            FoundationHatchPoints.Add(subFoundation);
        }
    }
}
