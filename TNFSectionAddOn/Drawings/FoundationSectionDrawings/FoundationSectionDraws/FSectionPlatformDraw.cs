using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry;
using TNFData.Geometry.FoundationSection;

namespace TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationSectionDraws
{
    public class FSectionPlatformDraw : FSectionHasSubDrawBase
    {
        public FSectionPlatformDraw(FSectionGBase foundation) : base(foundation)
        {
        }

        protected override List<Entity> CreateOverRevoLines()
        {
            var entities = new List<Entity>();
            var platformG = FSectionGeometry as PlatformSectionG;
            var firstTopLine = Helper.CreateLine(platformG.SubFoundation.HashiraGataColumn.TopRight,
                platformG.MainFoundationSection.TopLeft, Helper.SECTION_LINE_LAYER_NAME);
            var secondTopLine = Helper.CreateLine(platformG.MainFoundationSection.TopRight,
                platformG.ThirdRightPrimaryRevo.TopRight,Helper.SECTION_LINE_LAYER_NAME);
            entities.Add(firstTopLine);
            entities.Add(secondTopLine);
            return entities;
        }
        protected override void CreateDimension(List<Entity> entities, Transaction transaction, Database acCurrentDb)
        {
            base.CreateDimension(entities, transaction, acCurrentDb);

            var tnfParam = this.FSectionGeometry.FSectionData.TNFParameters;
            var fSection = FSectionGeometry as PlatformSectionG;

            var dist = 355 * tnfParam.ScaleRatio;
            var startP = fSection.PlatformPoint;
            var endP = fSection.SubFoundation.TopRight;

            entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointDownXByDistance, tnfParam.ScaleRatio, transaction, acCurrentDb));
        }
    }
}
