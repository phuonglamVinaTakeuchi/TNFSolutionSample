using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Factory;
using TNFData.Geometry.BaseGeometry;
using TNFData.Models;
using TNFData.Models.Section;

namespace TNFData.Geometry.FoundationSection
{
    public class CrushedStoneAndPlatformG : PlatformSectionG
    {
        public Rectangle StoneS { get; set; }
        public Trapezium FloorV { get; set; }
        public List<List<Point2D>> FloorHatchPoints { get; }

        public List<List<Point2D>> StoneHatchs { get; }
        public CrushedStoneAndPlatformG(Point2D basePoint, TNFFoundationSectionParams fData) : base(basePoint, fData)
        {
            FloorHatchPoints = new List<List<Point2D>>();
            StoneHatchs = new List<List<Point2D>>();
            InitGeometry();
        }

        protected override void InitGeometry()
        {
            base.InitGeometry();

            var tnfParams = FSectionData.TNFParameters;
            var basePointForVFloor = Point2D.MidPoint(ThirdPrimaryRevo.TopLeft, ThirdPrimaryRevo.TopRight);
            FloorV = new Trapezium(basePointForVFloor, tnfParams.CrushedStoneVBottomWidth*tnfParams.ScaleRatio,
                                   (tnfParams.CrushedStoneVBottomWidth + tnfParams.CrushedStoneOpenHolePitchDistance * 2)*tnfParams.ScaleRatio,
                                   tnfParams.StoneThickness*tnfParams.ScaleRatio);

            var stoneMinX = SubFoundation.HashiraGataColumn.TopRight.X;
            var stoneMaxX = SecondRightPrimaryRevo.TopRight.X;

            var stoneBaseX = (stoneMinX + stoneMaxX) / 2;
            var stoneBaseY = basePointForVFloor.Y;
            var stoneWidth = (int)(stoneMaxX - stoneMinX);
            var stoneBasePoint = new Point2D(stoneBaseX, stoneBaseY);
            StoneS = new Rectangle(stoneBasePoint, stoneWidth, tnfParams.StoneThickness*tnfParams.ScaleRatio);

            GeneralPoints(tnfParams);
        }
        protected override void GeneralPoints(TNFGlobalInfo tnfParams)
        {
            base.GeneralPoints(tnfParams);
            CreateFloorHatchs();
            CreateStoneHatch();
        }
        protected override void GeneralFoundationHatch()
        {
            if (SubFoundation == null)
                return;
            FoundationHatchPoints.Clear();
            var mainFOundation = new List<Point2D>();
            mainFOundation.AddRange(MainFoundationSection.FoundationPoints);
            mainFOundation.Add(MainFoundationSection.TopLeft);
            FoundationHatchPoints.Add(mainFOundation);
            GeneralSubFoundationHatch();

        }
        private void CreateFloorHatchs()
        {
            if (SubFoundation == null || FloorV == null)
                return;
            if (FloorHatchPoints == null)
                return;
            FloorHatchPoints.Clear();
            var floorHath = new List<Point2D>() {
                Floor.TopLeft,
                Floor.TopRight,
                Floor.BottomRight,
                FloorV.TopRight,
                FloorV.BottomRight,
                FloorV.BottomLeft,
                FloorV.TopLeft,
                SubFoundation.HashiraGataColumn.TopRight,
                SubFoundation.HashiraGataColumn.BottomRight,
                SubFoundation.HashiraGataColumn.BottomLeft,
                Floor.TopLeft,
            };
            FloorHatchPoints.Add(floorHath);

        }
        private void CreateStoneHatch()
        {
            if(SubFoundation == null || FloorV == null)
            {
                return;
            }
            if (StoneHatchs == null)
                return;
            StoneHatchs.Clear();
            var mainLeftStone = new List<Point2D>()
            {
                FloorV.TopRight,
                StoneS.TopRight,
                StoneS.BottomRight,
                FloorV.BottomRight,
                FloorV.TopRight
            };
            StoneHatchs.Add(mainLeftStone);

            var mainRigthStone = new List<Point2D>() {
                StoneS.TopLeft,
                FloorV.TopLeft,
                FloorV.BottomLeft,
                StoneS.BottomLeft,
                StoneS.TopLeft
            };
            StoneHatchs.Add(mainRigthStone);
        }
    }
}
