using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry;

namespace TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationSectionDraws
{
    public abstract class FSectionHasMainBase : FSectionDrawBase
    {
        protected FSectionHasMainBase(FSectionGBase foundation) : base(foundation)
        {
        }
        public override void Draw()
        {
            base.Draw();

        }
        protected override List<Entity> CreateAddditionEntities()
        {
            var entities = new List<Entity>();
            CreateAddditionEntities(entities);
            return entities;
        }

        protected virtual void CreateAddditionEntities(List<Entity> entities)
        {
            CreateOverRevoLine(entities);
            CreateDivideLines(entities);
            CreateMainRevoHatch(entities);
            CreateMainFoundationHatch(entities);
        }
        protected virtual void CreateOverRevoLine(List<Entity> entities)
        {
            var firstLine = Helper.CreateLine(
                FSectionGeometry.SecondLeftPrimaryRevo.TopLeft, FSectionGeometry.MainFoundationSection.TopLeft,
                Helper.SECTION_LINE_LAYER_NAME);
            entities.Add(firstLine);
            var secondLine = Helper.CreateLine(
                FSectionGeometry.SecondRightPrimaryRevo.TopRight, FSectionGeometry.MainFoundationSection.TopRight,
                Helper.SECTION_LINE_LAYER_NAME);
            entities.Add(secondLine);
        }
        protected virtual void CreateDivideLines(List<Entity> entities)
        {
            var thirdLine = Helper.CreateLine(
                FSectionGeometry.SecondLeftPrimaryRevo.TopRight, FSectionGeometry.SecondLeftPrimaryRevo.BottomRight,
                Helper.DivideLineLayerName, Helper.DivideLineTypeName);
            entities.Add(thirdLine);
            var fourthLine = Helper.CreateLine(
                FSectionGeometry.SecondRightPrimaryRevo.TopLeft, FSectionGeometry.SecondRightPrimaryRevo.BottomLeft,
                Helper.DivideLineLayerName, Helper.DivideLineTypeName);
            entities.Add(fourthLine);
            var fiveLine = Helper.CreateLine(FSectionGeometry.FirstPrimaryRevo.TopLeft, FSectionGeometry.FirstPrimaryRevo.TopRight,
                Helper.DivideLineLayerName, Helper.DivideLineTypeName);
            entities.Add(fiveLine);
        }
        private void CreateMainRevoHatch(List<Entity> entities)
        {
            foreach(var revoPoints in FSectionGeometry.RevoHatchPoints)
            {
                var mainHatch = Helper.CreateHatch(revoPoints, null, Helper.REVO_HATCH_NAME, RevoHatchScale);
                entities.Add(mainHatch);
            }

        }
        private void CreateMainFoundationHatch(List<Entity> entities)
        {
            foreach(var foundationPoints in FSectionGeometry.FoundationHatchPoints)
            {
                var foundationHatch = Helper.CreateHatch(foundationPoints, null, Helper.FOUNDATION_HATCH_NAME, FHatchScale);
                entities.Add(foundationHatch);
            }

        }
    }
}
