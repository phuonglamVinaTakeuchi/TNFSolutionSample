using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BeamSection;
using TNFData.Interface;

namespace TNFSectionAddOn.Drawings.BeamSectionDrawings
{
    public abstract class BeamDrawingBase
    {
        public BeamSectionGBase BeamGeometry { get; }
        protected Document _acDoc;
        protected Database _acCurDb;
        public double RevoHatchScale => Helper.REVO_HATCH_SCALE * BeamGeometry.BeamSectionData.TNFParameters.ScaleRatio;
        public double StoneHatchScale => Helper.STONE_HATCH_SCALE * BeamGeometry.BeamSectionData.TNFParameters.ScaleRatio;
        public double FHatchScale => Helper.FHATCH_SCALE * BeamGeometry.BeamSectionData.TNFParameters.ScaleRatio;
        public double TextHeight => Helper.TextHeight * BeamGeometry.BeamSectionData.TNFParameters.ScaleRatio;
        public BeamDrawingBase(BeamSectionGBase beamGeometry)
        {
            _acDoc = Application.DocumentManager.MdiActiveDocument;
            _acCurDb = _acDoc.Database;
            BeamGeometry = beamGeometry;

        }
        public virtual void Draw()
        {
            var tnfdata = BeamGeometry.BeamSectionData.TNFParameters;
            using (var transaction = _acDoc.Database.TransactionManager.StartTransaction())
            {
                var acBlockTable = transaction.GetObject(_acCurDb.BlockTableId, OpenMode.ForWrite) as BlockTable;
                var acBlockTableRecord = transaction.GetObject(acBlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                var notes = new List<Entity>();
                CreateNotes(notes);
                Helper.AddEntities(notes, acBlockTableRecord, transaction);
                var dimension = new List<Entity>();
                CreateDimension(dimension,transaction,_acCurDb);
                Helper.AddEntities(dimension, acBlockTableRecord, transaction);
                var leaderNote = new List<Entity>();
                CreateLeaderNotes(leaderNote,transaction,_acCurDb);
                Helper.AddEntities(leaderNote, acBlockTableRecord, transaction);

                if (tnfdata.IsFirtRevoPile)
                {
                    var firstPiles = new List<Entity>();
                    CreateFirstPiles(firstPiles, transaction, _acCurDb);
                    Helper.AddEntities(firstPiles, acBlockTableRecord, transaction);
                }
                if (tnfdata.IsSecondRevoPile)
                {
                    var secondPiles = new List<Entity>();
                    CreateSecondPiles(secondPiles);
                    Helper.AddEntities(secondPiles, acBlockTableRecord, transaction);
                }




                transaction.Commit();
            }
        }

        protected virtual void CreateFirstPiles(List<Entity> entities, Transaction transaction, Database acCurrentDb)
        {
            var tnfdata = BeamGeometry.BeamSectionData.TNFParameters;
            var mainPile = Helper.CreatePiles(BeamGeometry.FirstRevonation, tnfdata.ScaleRatio, tnfdata.TNFPile, out var pilePoint);
            entities.AddRange(mainPile);
            var pileNote = tnfdata.TNFPile.Description;
            entities.Add(Helper.CreatePileLeaderNote(pileNote, pilePoint.Point2DtoPoint3d(), tnfdata.ScaleRatio, transaction, acCurrentDb, TextHeight));
        }
        protected virtual void CreateSecondPiles(List<Entity> entities)
        {
            var tnfdata = BeamGeometry.BeamSectionData.TNFParameters;
            var secondLeftPile = Helper.CreatePiles(BeamGeometry.BottomLeftSecondRevonation, tnfdata.ScaleRatio, tnfdata.TNFPile, out var secondLeftPilePoint);
            entities.AddRange(secondLeftPile);
            var secondRightPile = Helper.CreatePiles(BeamGeometry.BottomRightSecondRevonation, tnfdata.ScaleRatio, tnfdata.TNFPile, out var secondRightPilePoint);
            entities.AddRange(secondRightPile);
        }
        protected virtual void CreateNotes(List<Entity> entities)
        {
            var tnfParam = BeamGeometry.BeamSectionData.TNFParameters;
            entities.Add(Helper.CreateNoteForRect(Helper.FIRST_REVO_TEXT, this.BeamGeometry.FirstRevonation, TextHeight, tnfParam.ScaleRatio));
            entities.Add(Helper.CreateNoteForRect(Helper.SECOND_REVO_TEXT, this.BeamGeometry.BottomLeftSecondRevonation, TextHeight, tnfParam.ScaleRatio));
            entities.Add(Helper.CreateNoteForRect(Helper.SECOND_REVO_TEXT, this.BeamGeometry.BottomRightSecondRevonation, TextHeight, tnfParam.ScaleRatio));
            if(BeamGeometry is PlatformBeamSectionG platformBeam)
            {
                entities.Add(Helper.CreateNoteForRect(Helper.THIRD_REVO_TEXT,platformBeam.TopRightThirdRevonation,TextHeight, tnfParam.ScaleRatio));
            }
        }
        protected virtual void CreateDimension(List<Entity> entities,Transaction transaction,Database acCurrentDb)
        {
            var tnfParams = BeamGeometry.BeamSectionData.TNFParameters;
            var startP = BeamGeometry.FirstRevonation.BottomLeft;
            var endP = BeamGeometry.FirstRevonation.BottomRight;
            var dist = 460 * tnfParams.ScaleRatio;
            var firstDim = Helper.CreateDimension(startP, endP, dist, Helper.GetPointDownYByDistance, tnfParams.ScaleRatio,transaction,acCurrentDb, Helper.DIMENSION_LAYER_NAME, true);
            entities.Add(firstDim);
            var startx = BeamGeometry.BottomRightSecondRevonation.BottomRight.X;
            var starty = BeamGeometry.FirstRevonation.BottomRight.Y;
            dist = 350 * tnfParams.ScaleRatio;

            startP = new Point2D(startx, starty);
            endP = BeamGeometry.BottomRightSecondRevonation.BottomRight;
            var seconDim = Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParams.ScaleRatio, transaction, acCurrentDb);
            entities.Add(seconDim);

            startP = BeamGeometry.BottomRightSecondRevonation.BottomRight;
            endP = BeamGeometry.BottomRightSecondRevonation.TopRight;
            var thirdDim = Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParams.ScaleRatio, transaction, acCurrentDb);
            entities.Add(thirdDim);

            if (tnfParams.FloorThickness > 0)
            {
                startP = BeamGeometry.Floor.TopRight;
                endP = BeamGeometry.Floor.BottomRight;
                var floorP = Helper.GetPointUpYByDistance(startP.Point2DtoPoint3d(), Helper.DIMPOINT_UP_Y_DISTANCE* tnfParams.ScaleRatio);
                var fP = Helper.GetPointUpXByDistance(floorP, dist);

                var floorDim = Helper.CreateDimension(startP, endP, fP, tnfParams.ScaleRatio, transaction, acCurrentDb);
                entities.Add(floorDim);
            }
            if(BeamGeometry is PlatformBeamSectionG platform)
            {
                startP = platform.TopRightThirdRevonation.TopRight;
                endP = platform.TopRightThirdRevonation.BottomRight;
                var tDim = Helper.CreateDimension(startP,endP,dist,Helper.GetPointUpXByDistance,tnfParams.ScaleRatio,transaction, acCurrentDb);
                entities.Add(tDim);
            }
            if(BeamGeometry is CrushedStondAndPlatformBeamG crushed)
            {
                startP = crushed.TopRightThirdRevonation.TopRight;
                endP = crushed.Floor.BottomRight;
                var tD = Helper.CreateDimension(startP,endP,dist,Helper.GetPointUpXByDistance,tnfParams.ScaleRatio,transaction, acCurrentDb);
                entities.Add(tD);
            }

            // dim beam bottom
            startP = BeamGeometry.Beam.ConcreateBottomLeft;
            endP = BeamGeometry.Beam.ConcreateBottomRight;
            dist = 255 * tnfParams.ScaleRatio;
            var bD = Helper.CreateDimension(startP,endP,dist,Helper.GetPointDownYByDistance,tnfParams.ScaleRatio,transaction, acCurrentDb);
            entities.Add(bD);

            // dim open distance
            startx = BeamGeometry.Beam.TopRight.X;
            starty = BeamGeometry.Beam.ConcreateBottomRight.Y;
            endP = new Point2D(startx,starty);
            startP = BeamGeometry.Beam.ConcreateBottomRight;
            var dimP = Helper.GetPointUpXByDistance(endP.Point2DtoPoint3d(),1070*tnfParams.ScaleRatio);
            var dP = Helper.GetPointDownYByDistance(dimP, dist);
            entities.Add(Helper.CreateDimension(startP,endP,dP,tnfParams.ScaleRatio,transaction, acCurrentDb));

            // dim beam

            dist = 540* tnfParams.ScaleRatio;
            startP = BeamGeometry.Beam.ConcreateBottomLeft;
            entities.Add(Helper.CreateDimension(startP,endP,dist,Helper.GetPointDownYByDistance,tnfParams.ScaleRatio,transaction, acCurrentDb));

            // dim concreate thickness
            dist = 565 * tnfParams.ScaleRatio;
            startP = endP;

            startx = startP.X;
            starty = BeamGeometry.Beam.BotomRight.Y;
            endP = new Point2D(startx,starty);
            var bemOfP = Helper.GetPointDownYByDistance(startP.Point2DtoPoint3d(), 380 * tnfParams.ScaleRatio);
            var beamP = Helper.GetPointUpXByDistance(bemOfP, dist);
            entities.Add(Helper.CreateDimension(startP,endP,beamP,tnfParams.ScaleRatio, transaction, acCurrentDb));

            if(BeamGeometry is PlatformBeamSectionG platform2)
            {
                startP = new Point2D(platform2.Beam.TopRight.X, platform2.Beam.BotomRight.Y);
                endP = new Point2D(platform2.Beam.TopRight.X, platform2.SecondRevonation.TopRight.Y);
                entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParams.ScaleRatio, transaction, acCurrentDb));

                startP = endP;
                endP = new Point2D(BeamGeometry.Beam.TopRight.X, platform2.TopRightThirdRevonation.TopLeft.Y);
                entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParams.ScaleRatio, transaction, acCurrentDb));

                dist = 1010 * tnfParams.ScaleRatio;
                startP = new Point2D(platform2.Beam.TopRight.X, platform2.Beam.ConcreateBottomLeft.Y);
                entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParams.ScaleRatio, transaction, acCurrentDb));
                startP = BeamGeometry.Beam.OffsetGLLeft;
                endP = BeamGeometry.Beam.BottomLeft;
                dist = 975 * tnfParams.ScaleRatio;
                entities.Add(Helper.CreateDimension(startP,endP,dist,Helper.GetPointDownXByDistance,tnfParams.ScaleRatio,transaction,acCurrentDb));

            }
            else
            {
                var yAxis = BeamGeometry.Beam.OffsetGLRight.Y;
                var xAxis = BeamGeometry.Beam.TopRight.X;

                startP = endP;
                endP = new Point2D(xAxis,yAxis);
                entities.Add(Helper.CreateDimension(startP,endP,dist,Helper.GetPointUpXByDistance,tnfParams.ScaleRatio,transaction,acCurrentDb));

                startP = endP;
                endP = BeamGeometry.Beam.TopRight;
                var tmpP = Helper.GetPointUpYByDistance(endP.Point2DtoPoint3d(), 450 * tnfParams.ScaleRatio);
                var tempP = Helper.GetPointUpXByDistance(tmpP, dist);
                entities.Add(Helper.CreateDimension(startP, endP, tempP, tnfParams.ScaleRatio, transaction, acCurrentDb));

                startP = new Point2D(BeamGeometry.Beam.TopRight.X,BeamGeometry.Beam.ConcreateBottomLeft.Y);
                dist = 870 * tnfParams.ScaleRatio;
                entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParams.ScaleRatio, transaction, acCurrentDb));
            }




        }
        protected virtual void CreateLeaderNotes(List<Entity> entities,Transaction transaction, Database acCurrentDb)
        {
            var tnfParams = BeamGeometry.BeamSectionData.TNFParameters;
            var beamNoteHeight = BeamGeometry.BeamSectionData.Beam.BeamDepth/2 * tnfParams.ScaleRatio;
            var beamNoteX = BeamGeometry.Beam.TopRight.X + 350* tnfParams.ScaleRatio;
            var beamNoteY = BeamGeometry.Beam.TopRight.Y + beamNoteHeight;
            var beamNotePoint = new Point2D(beamNoteX, beamNoteY);
            var beamNote = $"外周部基礎（FW)";
            entities.Add(Helper.CreateLeaded(beamNotePoint.Point2DtoPoint3d(), beamNoteHeight * 2, TextHeight, tnfParams.ScaleRatio, transaction, acCurrentDb, beamNote, AttachmentPoint.MiddleLeft));



        }
    }
}
