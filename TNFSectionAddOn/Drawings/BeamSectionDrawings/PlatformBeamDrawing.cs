using Autodesk.AutoCAD.DatabaseServices;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BaseGeometry;
using TNFData.Geometry.BeamSection;

namespace TNFSectionAddOn.Drawings.BeamSectionDrawings
{
    public class PlatformBeamDrawing : BeamDrawingBase
    {
        public PlatformBeamDrawing(BeamSectionGBase beamGeometry) : base(beamGeometry)
        {

        }
        public override void Draw()
        {
            base.Draw();
            var entities = new List<Entity>();
            CreateRevonationPLine(entities);

            using (var transaction = _acDoc.Database.TransactionManager.StartTransaction())
            {
                var acBlockTable = transaction.GetObject(_acCurDb.BlockTableId, OpenMode.ForWrite) as BlockTable;
                var acBlockTableRecord = transaction.GetObject(acBlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                CreateHatch(entities);
                Helper.AddEntities(entities, acBlockTableRecord, transaction);
                transaction.Commit();
            }
        }
        private void CreateRevonationPLine(List<Entity> entities)
        {
            var platformBeam = BeamGeometry as PlatformBeamSectionG;
            var revoPline = new List<Point2D>() {
                BeamGeometry.Floor.BottomRight,
                BeamGeometry.Beam.TopRight,
                BeamGeometry.Beam.BotomRight,
                BeamGeometry.Beam.ConcreateBottomRight,
                BeamGeometry.BeamHole.BottomLeft,
                BeamGeometry.BeamHole.TopLeft,
                BeamGeometry.BottomLeftSecondRevonation.TopLeft
            };
            entities.Add(Helper.PlyLineFromPoint2D(revoPline, Helper.SECTION_LINE_LAYER_NAME));


            var revoPline2 = new List<Point2D>()
            {
                BeamGeometry.BottomLeftSecondRevonation.BottomLeft,
                BeamGeometry.FirstRevonation.TopLeft,
                BeamGeometry.FirstRevonation.BottomLeft,
                BeamGeometry.FirstRevonation.BottomRight,
                BeamGeometry.FirstRevonation.TopRight,
                BeamGeometry.BottomRightSecondRevonation.BottomRight
            };
            entities.Add(Helper.PlyLineFromPoint2D(revoPline2, Helper.SECTION_LINE_LAYER_NAME));

            var floorPline = new List<Point2D>();
            floorPline.Add(BeamGeometry.Floor.TopRight);
            floorPline.Add(BeamGeometry.Wall.BottomRight);
            floorPline.Add(BeamGeometry.Wall.TopRight);
            floorPline.Add(BeamGeometry.Wall.TopLeft);
            floorPline.Add(BeamGeometry.Beam.ConcreateBottomLeft);
            entities.Add(Helper.PlyLineFromPoint2D(floorPline, Helper.SECTION_LINE_LAYER_NAME));
            entities.Add(Helper.CreateLine(BeamGeometry.Beam.BottomLeft, BeamGeometry.Beam.BotomRight, Helper.SECTION_LINE_LAYER_NAME));
            entities.Add(Helper.CreateLine(BeamGeometry.Beam.OffsetGLLeft, BeamGeometry.Beam.OffsetGLRight, Helper.OFFSET_LINE_LAYER_NAME, Helper.OFFSET_LINE_TYPE));

            var listNonRevo = new List<Rectangle>() {
                BeamGeometry.BottomLeftSecondRevonation,
                BeamGeometry.BottomRightSecondRevonation,
                platformBeam.TopRightThirdRevonation
            };

            entities.AddRange(Helper.CreateNoneRevo(listNonRevo));

            entities.Add(Helper.CreateLine(BeamGeometry.FirstRevonation.TopLeft,
                BeamGeometry.FirstRevonation.TopRight,
                Helper.DivideLineLayerName, Helper.DivideLineTypeName));
            entities.Add(Helper.CreateLine(BeamGeometry.BottomRightSecondRevonation.BottomLeft,
                platformBeam.TopRightThirdRevonation.TopLeft,
                Helper.DivideLineLayerName,
                Helper.DivideLineTypeName));
            entities.Add(Helper.CreateLine(BeamGeometry.BottomLeftSecondRevonation.TopRight,
               BeamGeometry.BottomLeftSecondRevonation.BottomRight,
               Helper.DivideLineLayerName,
               Helper.DivideLineTypeName));
        }
        private void CreateHatch(List<Entity> entities)
        {
            var platformBeam = BeamGeometry as PlatformBeamSectionG;
            var revoHatch = new List<Point2D>() {
                BeamGeometry.SecondRevonation.TopLeft,
                BeamGeometry.FirstRevonation.BottomLeft,
                BeamGeometry.FirstRevonation.BottomRight,
                platformBeam.ThirdRevonation.TopRight,
                BeamGeometry.Beam.TopRight,
                BeamGeometry.Beam.BotomRight,
                BeamGeometry.Beam.ConcreateBottomRight,
                BeamGeometry.BeamHole.BottomLeft,
                BeamGeometry.BeamHole.TopLeft,
                BeamGeometry.SecondRevonation.TopLeft
            };
            entities.Add(Helper.CreateHatch(revoHatch, null, Helper.REVO_HATCH_NAME, RevoHatchScale));

            var floorHatch = new List<Point2D>() {
                BeamGeometry.Floor.TopRight,
                BeamGeometry.Wall.BottomRight,
                BeamGeometry.Wall.TopRight,
                BeamGeometry.Wall.TopLeft,
                BeamGeometry.Beam.BottomLeft,
                BeamGeometry.Beam.BotomRight,
                BeamGeometry.Beam.TopRight,
                BeamGeometry.Floor.BottomRight,
                BeamGeometry.Floor.TopRight
            };
            entities.Add(Helper.CreateHatch(floorHatch, null, Helper.FOUNDATION_HATCH_NAME, FHatchScale));
        }
    }
}
