using Autodesk.AutoCAD.DatabaseServices;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry;
using TNFData.Geometry.BaseGeometry;
using TNFData.Geometry.FoundationSection;
using TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationDraws;
using TNFSectionAddOn.Factory;

namespace TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationSectionDraws
{
    public class FSectionCrushedStoneDraw : FSectionHasMainBase
    {
        private FGeometryDrawBase _subFoudationDraw;
        public FSectionCrushedStoneDraw(FSectionGBase foundation) : base(foundation)
        {
            var crushedG = foundation as CrushedStoneSectionG;
            _subFoudationDraw = FGeometryDrawFactory.CreateFGeometryDraw(crushedG.SubFoundation);
        }
        public override void Draw()
        {
            base.Draw();
            _subFoudationDraw.Draw();
        }
        protected override void CreateAddditionEntities(List<Entity> entities)
        {
            base.CreateAddditionEntities(entities);

            var crushedG = FSectionGeometry as CrushedStoneSectionG;
            var subRevo = CreateSubRevoPLine(crushedG);
            entities.Add(subRevo);
            entities.AddRange(CreateOverSubRevoLine());
            CreateBottomFloorPline(entities);
            CreateSubBeamPline(entities);
            CreateStoneHatch(entities);
            CreateFloorHatch(entities);

        }
        private Polyline CreateSubRevoPLine(CrushedStoneSectionG crushedG)
        {
            return Helper.PlyLineFromPoint2D(crushedG.SubRevoPoints,Helper.SECTION_LINE_LAYER_NAME);
        }
        private List<Entity> CreateOverSubRevoLine()
        {
            var entities = new List<Entity>();
            var crushedG = FSectionGeometry as CrushedStoneSectionG;
            if (crushedG.SubFoundation.Foundation.OffsetWithGL == 0)
            {
                entities.Add(Helper.CreateLine(crushedG.SecondLeftSubRevo.TopLeft, crushedG.SecondRightSubRevo.TopRight, Helper.SECTION_LINE_LAYER_NAME));
            }
            else
            {
                var endX1 = crushedG.SubBeamGeometry.TopLeft.X;
                var endY1 = crushedG.SecondLeftPrimaryRevo.TopLeft.Y;
                var endP = new Point2D(endX1, endY1);
                entities.Add(Helper.CreateLine(crushedG.SecondLeftSubRevo.TopLeft,
                                                 endP, Helper.SECTION_LINE_LAYER_NAME));

                var segment1 = new Segment2D(endP, crushedG.SubFoundation.TopRight);
                var segment2 = new Segment2D(crushedG.SubBeamGeometry.BotomRight, crushedG.SubBeamGeometry.TopRight);
                Segment2D.Intersection(segment1, segment2, out var startP);
                entities.Add(Helper.CreateLine(startP, crushedG.SecondRightSubRevo.TopRight, Helper.SECTION_LINE_LAYER_NAME));

            }

            return entities;
        }
        protected override void CreateOverRevoLine(List<Entity> entities)
        {
            var crushedG = FSectionGeometry as CrushedStoneSectionG;
            entities.Add(Helper.CreateLine(crushedG.SecondLeftPrimaryRevo.TopLeft, crushedG.SecondRightPrimaryRevo.TopRight, Helper.SECTION_LINE_LAYER_NAME));


        }
        protected override void CreateDivideLines(List<Entity> entities)
        {
            var crushedG = FSectionGeometry as CrushedStoneSectionG;
            base.CreateDivideLines(entities);
            var firstLine = Helper.CreateLine(crushedG.SubRevoFirst.TopLeft,
                                              crushedG.SubRevoFirst.TopRight,
                                              Helper.DivideLineLayerName,
                                              Helper.DivideLineTypeName);
            var secondLine = Helper.CreateLine(crushedG.SecondLeftSubRevo.TopRight,
                                               crushedG.SecondLeftSubRevo.BottomRight,
                                               Helper.DivideLineLayerName,
                                               Helper.DivideLineTypeName);
            var thirLine =  Helper.CreateLine(crushedG.SecondRightSubRevo.TopLeft,
                                              crushedG.SecondRightSubRevo.BottomLeft,
                                              Helper.DivideLineLayerName,
                                              Helper.DivideLineTypeName);
            entities.Add(firstLine);
            entities.Add(secondLine);
            entities.Add(thirLine);
        }

        private void CreateBottomFloorPline(List<Entity> entities)
        {
            var crushedG = FSectionGeometry as CrushedStoneSectionG;
            var bottomPoints = new List<Point2D>();
            bottomPoints.Add(crushedG.Floor.BottomLeft);
            bottomPoints.Add(crushedG.FloorV.TopLeft);
            bottomPoints.Add(crushedG.FloorV.BottomLeft);
            bottomPoints.Add(crushedG.FloorV.BottomRight);
            bottomPoints.Add(crushedG.FloorV.TopRight);
            bottomPoints.Add(crushedG.Floor.BottomRight);
            var bottomPline = Helper.PlyLineFromPoint2D(bottomPoints, Helper.SECTION_LINE_LAYER_NAME);
            entities.Add(bottomPline);
        }
        private void CreateSubBeamPline(List<Entity> entities)
        {
            var crushedG = FSectionGeometry as CrushedStoneSectionG;
            var subBeamPoints = new List<Point2D>();
            subBeamPoints.Add(crushedG.SubFloor.TopRight);
            subBeamPoints.Add(crushedG.SubFloor.TopLeft);
            subBeamPoints.Add(crushedG.SubBeamGeometry.ConcreateBottomLeft);
            subBeamPoints.Add(crushedG.SubBeamGeometry.ConcreateBottomRight);
            subBeamPoints.Add(crushedG.SubBeamGeometry.BotomRight);
            subBeamPoints.Add(crushedG.SubBeamGeometry.TopRight);
            subBeamPoints.Add(crushedG.SubFloor.BottomRight);
            var subBeamPLine = Helper.PlyLineFromPoint2D(subBeamPoints,Helper.SECTION_LINE_LAYER_NAME);
            entities.Add(subBeamPLine);
            var beamBottomLine = Helper.CreateLine(crushedG.SubBeamGeometry.BottomLeft,
                                                   crushedG.SubBeamGeometry.BotomRight,
                                                   Helper.SECTION_LINE_LAYER_NAME);
            entities.Add(beamBottomLine);
        }
        private void CreateStoneHatch(List<Entity> entities)
        {
            var crushedStoneG = FSectionGeometry as CrushedStoneSectionG;
            foreach(var stoneHatchPoints in crushedStoneG.StoneHatchs)
            {
                var stoneHatch = Helper.CreateHatch(stoneHatchPoints, null, Helper.STONE_HATCH_NAME, StoneHatchScale);
                entities.Add(stoneHatch);
            }
        }
        public void CreateFloorHatch(List<Entity> entities)
        {
            var crushedStoneG = FSectionGeometry as CrushedStoneSectionG;
            foreach (var floorHatchPoints in crushedStoneG.FloorHatchPoints)
            {
                var floorHatch = Helper.CreateHatch(floorHatchPoints, null, Helper.FOUNDATION_HATCH_NAME, FHatchScale);
                entities.Add(floorHatch);
            }
        }

        protected override void CreateDimension(List<Entity> entities, Transaction transaction, Database acCurrentDb)
        {
            base.CreateDimension(entities, transaction, acCurrentDb);
            var tnfParam = this.FSectionGeometry.FSectionData.TNFParameters;
            var crushedStone = FSectionGeometry as CrushedStoneSectionG;
            var dist = 350 * tnfParam.ScaleRatio;

            var startP = crushedStone.StoneS.BottomRight;
            var endP = crushedStone.StoneS.TopRight;
            entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParam.ScaleRatio, transaction, acCurrentDb));


            var subDimY = crushedStone.BasePoint.Y;
            var subDimX = crushedStone.SecondRightSubRevo.TopRight.X;
            startP = new Point2D(subDimX, subDimY);
            endP = crushedStone.SecondRightSubRevo.BottomRight;
            entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParam.ScaleRatio, transaction, acCurrentDb));

            startP = endP;
            endP = crushedStone.SecondRightSubRevo.TopRight;
            entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParam.ScaleRatio, transaction, acCurrentDb));
            startP = endP;
            endP = crushedStone.SubFloor.BottomRight;
            entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParam.ScaleRatio, transaction, acCurrentDb));
            startP = endP;
            endP = crushedStone.SubFloor.TopRight;
            entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParam.ScaleRatio, transaction, acCurrentDb));

            dist = 275 * tnfParam.ScaleRatio;

            startP = crushedStone.SubBeamGeometry.ConcreateBottomLeft;
            endP = crushedStone.SubBeamGeometry.ConcreateBottomRight;
            entities.Add(Helper.CreateDimension(startP,endP,dist,Helper.GetPointDownYByDistance,tnfParam.ScaleRatio, transaction, acCurrentDb));

            startP = crushedStone.SubBeamGeometry.ConcreateBottomLeft;
            endP = crushedStone.SubBeamGeometry.BottomLeft;

            var dimPoit = Helper.GetPointDownYByDistance(startP.Point2DtoPoint3d(), 300 * tnfParam.ScaleRatio);
            var dP = Helper.GetPointDownXByDistance(dimPoit, dist);
            entities.Add(Helper.CreateDimension(startP,endP,dP,tnfParam.ScaleRatio, transaction, acCurrentDb));
            startP = endP;
            endP = crushedStone.SubBeamGeometry.TopLeft;
            entities.Add(Helper.CreateDimension(startP,endP,dist,Helper.GetPointDownXByDistance,tnfParam.ScaleRatio, transaction, acCurrentDb));

            dist = 870*tnfParam.ScaleRatio;
            startP = crushedStone.SubBeamGeometry.ConcreateBottomLeft;
            entities.Add(Helper.CreateDimension(startP,endP,dist,Helper.GetPointDownXByDistance,tnfParam.ScaleRatio, transaction,   acCurrentDb));


        }
        protected override void CreateNotes(List<Entity> entities)
        {
            base.CreateNotes(entities);
            var tnfParam = FSectionGeometry.FSectionData.TNFParameters;
            var fSection = FSectionGeometry as CrushedStoneSectionG;
            var basep = Point2D.MidPoint(fSection.FloorV.BottomRight, fSection.StoneS.BottomRight);
            var width = fSection.FloorV.BottomRight.DistanceTo(fSection.StoneS.BottomRight);
            var firstStoneRect = new Rectangle(basep, width, (double)tnfParam.StoneThickness * tnfParam.ScaleRatio);
            entities.Add(Helper.CreateStoneNote(Helper.STONE_NOTE, firstStoneRect, TextHeight, tnfParam.ScaleRatio));

            basep = Point2D.MidPoint(fSection.StoneS.BottomLeft, fSection.FloorV.BottomLeft);
            width = fSection.StoneS.BottomLeft.DistanceTo(fSection.FloorV.BottomLeft);
            var secondStoneRect = new Rectangle(basep, width, (double)tnfParam.StoneThickness * tnfParam.ScaleRatio);
            entities.Add(Helper.CreateStoneNote(Helper.STONE_NOTE, secondStoneRect, TextHeight, tnfParam.ScaleRatio));

            var baseX = Point2D.MidPoint(fSection.SubFloor.BottomLeft, fSection.SubFloor.BottomRight).X;
            var baseY = basep.Y;
            basep = new Point2D(baseX, baseY);
            width = fSection.SubFloor.BottomLeft.DistanceTo(fSection.SubFloor.BottomRight);
            var subStoneRect = new Rectangle(basep, width, (double)tnfParam.StoneThickness*tnfParam.ScaleRatio);
            entities.Add(Helper.CreateStoneNote(Helper.STONE_NOTE, subStoneRect, TextHeight, tnfParam.ScaleRatio));

        }
    }
}
