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

namespace TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationSectionDraws
{
    public class FSectionCrushedAndPlatformDraw : FSectionHasSubDrawBase
    {
        public FSectionCrushedAndPlatformDraw(FSectionGBase foundation) : base(foundation)
        {
        }

        protected override List<Entity> CreateOverRevoLines()
        {
            var entities = new List<Entity>();

            var crushedPlatformG = FSectionGeometry as CrushedStoneAndPlatformG;

            var topRevoLine = Helper.CreateLine(crushedPlatformG.StoneS.BottomLeft,
                crushedPlatformG.StoneS.BottomRight, Helper.SECTION_LINE_LAYER_NAME);
            entities.Add(topRevoLine);
            var points = new List<Point2D>();
            points.Add(crushedPlatformG.SubFoundation.HashiraGataColumn.TopRight);
            points.Add(crushedPlatformG.FloorV.TopLeft);
            points.Add(crushedPlatformG.FloorV.BottomLeft);
            points.Add(crushedPlatformG.FloorV.BottomRight);
            points.Add(crushedPlatformG.FloorV.TopRight);
            points.Add(crushedPlatformG.Floor.BottomRight);
            var bootFloorLine = Helper.PlyLineFromPoint2D(points, Helper.SECTION_LINE_LAYER_NAME);

            entities.Add(bootFloorLine);


            return entities;
        }
        protected override void CreateAdditionEntities(List<Entity> entities)
        {
            base.CreateAdditionEntities(entities);
            CreateFloorHatch(entities);
            CreateStoneHatch(entities);
        }
        private void CreateFloorHatch(List<Entity> entities)
        {
            var crushedStoneG = FSectionGeometry as CrushedStoneAndPlatformG;
            foreach (var stoneHatchPoints in crushedStoneG.StoneHatchs)
            {
                var stoneHatch = Helper.CreateHatch(stoneHatchPoints, null, Helper.STONE_HATCH_NAME, StoneHatchScale);
                entities.Add(stoneHatch);
            }
        }
        private void CreateStoneHatch(List<Entity> entities)
        {
            var crushedStoneG = FSectionGeometry as CrushedStoneAndPlatformG;
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
            var fSection = FSectionGeometry as CrushedStoneAndPlatformG;
            var dist = 350 * tnfParam.ScaleRatio;

            var startP = fSection.StoneS.BottomRight;
            var endP = fSection.StoneS.TopRight;
            entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParam.ScaleRatio, transaction, acCurrentDb));
        }
        protected override void CreateNotes(List<Entity> entities)
        {
            base.CreateNotes(entities);
            var tnfParam = FSectionGeometry.FSectionData.TNFParameters;
            var fSection = FSectionGeometry as CrushedStoneAndPlatformG;
            var basep = Point2D.MidPoint(fSection.FloorV.BottomRight, fSection.StoneS.BottomRight);
            var width = (int)fSection.FloorV.BottomRight.DistanceTo(fSection.StoneS.BottomRight);
            var firstStoneRect = new Rectangle(basep, width, (double)tnfParam.StoneThickness*tnfParam.ScaleRatio);
            entities.Add(Helper.CreateStoneNote(Helper.STONE_NOTE, firstStoneRect, TextHeight, tnfParam.ScaleRatio));

            basep = Point2D.MidPoint(fSection.StoneS.BottomLeft, fSection.FloorV.BottomLeft);
            width = (int)fSection.StoneS.BottomLeft.DistanceTo(fSection.FloorV.BottomLeft);
            var secondStoneRect = new Rectangle(basep, width, (double)tnfParam.StoneThickness * tnfParam.ScaleRatio);
            entities.Add(Helper.CreateStoneNote(Helper.STONE_NOTE,secondStoneRect,TextHeight,tnfParam.ScaleRatio));

        }
    }
}
