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
    public class NormalSectionG : FSectionGBase
    {

        public NormalSectionG(Point2D basePoint, TNFFoundationSectionParams fData) : base(basePoint, fData)
        {

        }
        protected override void InitGeometry()
        {
            var tnfParams = FSectionData.TNFParameters;
            base.InitGeometry();
            var floorBasePoint = Point2D.MidPoint(SecondPrimaryRevo.TopLeft, SecondPrimaryRevo.TopRight);
            var floorWidth = (tnfParams.FirstRevonationWith + tnfParams.SecondRevonationWidth * 2)*tnfParams.ScaleRatio;
            Floor = new Rectangle(floorBasePoint, floorWidth, tnfParams.FloorThickness*tnfParams.ScaleRatio);
            var basePointFoundation = new Point2D(floorBasePoint.X, floorBasePoint.Y);
            MainFoundationSection = TNFFoundationGFactory.CreateFoundationGBase(basePointFoundation, FSectionData.MainFoundation);

            GeneralPoints(tnfParams);
        }
        private void GeneralPoints(TNFGlobalInfo tnfParams)
        {
            GeneralMainRevoPlinePoints();
            GeneralMainRevoHatch();
            GeneralMainFoundationHatch(tnfParams);
            GeneralNoneRevoRectangle();
        }
        private void GeneralMainFoundationHatch(TNFGlobalInfo tnfParams)
        {
            FoundationHatchPoints.Clear();
            var mainFoundation = new List<Point2D>();

            if (tnfParams.FloorThickness > 0)
            {
                mainFoundation.Add(Floor.BottomLeft);
                mainFoundation.Add(MainFoundationSection.TopLeft);
                mainFoundation.AddRange(MainFoundationSection.BottomFoundationPoints);
                mainFoundation.Add(MainFoundationSection.TopRight);
                mainFoundation.Add(Floor.BottomRight);
                mainFoundation.Add(Floor.TopRight);
                mainFoundation.Add(Floor.TopLeft);
                mainFoundation.Add(Floor.BottomLeft);

            }
            else
            {
                mainFoundation.Add(MainFoundationSection.TopLeft);
                mainFoundation.AddRange(MainFoundationSection.BottomFoundationPoints);
                mainFoundation.Add(MainFoundationSection.TopRight);
                mainFoundation.Add(MainFoundationSection.TopLeft);
            }
            FoundationHatchPoints.Add(mainFoundation);
        }
        private void GeneralMainRevoPlinePoints()
        {
            PrimaryRevoPline.Clear();
            PrimaryRevoPline.Add(SecondLeftPrimaryRevo.BottomLeft);
            PrimaryRevoPline.Add(FirstPrimaryRevo.TopLeft);
            PrimaryRevoPline.Add(FirstPrimaryRevo.BottomLeft);
            PrimaryRevoPline.Add(FirstPrimaryRevo.BottomRight);
            PrimaryRevoPline.Add(FirstPrimaryRevo.TopRight);
            PrimaryRevoPline.Add(SecondRightPrimaryRevo.BottomRight);
        }
        protected virtual void GeneralMainRevoHatch(){
            RevoHatchPoints.Clear();
            var mainRevoHatch = new List<Point2D>();

            mainRevoHatch.Add(SecondPrimaryRevo.TopLeft);
            mainRevoHatch.AddRange(MainFoundationSection.FoundationPoints);
            mainRevoHatch.Add(SecondPrimaryRevo.TopRight);
            mainRevoHatch.Add(FirstPrimaryRevo.BottomRight);
            mainRevoHatch.Add(FirstPrimaryRevo.BottomLeft);
            mainRevoHatch.Add(SecondPrimaryRevo.TopLeft);

            RevoHatchPoints.Add(mainRevoHatch);

        }
        private void GeneralNoneRevoRectangle()
        {
            NoneRevoRectangles.Clear();
            NoneRevoRectangles.Add(SecondLeftPrimaryRevo);
            NoneRevoRectangles.Add(SecondRightPrimaryRevo);
        }

    }
}
