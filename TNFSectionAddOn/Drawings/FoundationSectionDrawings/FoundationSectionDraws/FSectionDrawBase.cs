using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry;
using TNFData.Geometry.FoundationSection;
using TNFData.Interface;
using TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationDraws;
using TNFSectionAddOn.Factory;

namespace TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationSectionDraws
{
    public abstract class FSectionDrawBase
    {
        private FGeometryDrawBase _mainFoudationDraw;
        private Document _acDoc;
        private Database _acCurDb;
        public double TextHeight =>Helper.TextHeight * FSectionGeometry.FSectionData.TNFParameters.ScaleRatio;
        public double RevoHatchScale => Helper.REVO_HATCH_SCALE * FSectionGeometry.FSectionData.TNFParameters.ScaleRatio;
        public double StoneHatchScale => Helper.STONE_HATCH_SCALE * FSectionGeometry.FSectionData.TNFParameters.ScaleRatio;
        public double FHatchScale=>Helper.FHATCH_SCALE* FSectionGeometry.FSectionData.TNFParameters.ScaleRatio;
        public FSectionGBase FSectionGeometry { get; }
        protected FSectionDrawBase(FSectionGBase foundation)
        {
            FSectionGeometry = foundation;
            _acDoc = Application.DocumentManager.MdiActiveDocument;
            _acCurDb = _acDoc.Database;
            _mainFoudationDraw = FGeometryDrawFactory.CreateFGeometryDraw(foundation.MainFoundationSection);

        }
        public virtual void Draw()
        {
            _mainFoudationDraw.Draw();
            var tnfdata = FSectionGeometry.FSectionData.TNFParameters;
            var revonationPline = Helper.PlyLineFromPoint2D(FSectionGeometry.PrimaryRevoPline,Helper.SECTION_LINE_LAYER_NAME);
            Line floorPline = null;
            if (FSectionGeometry.FSectionData.TNFParameters.FloorThickness > 0)
            {
                floorPline = Helper.CreateLine(FSectionGeometry.Floor.TopLeft,FSectionGeometry.Floor.TopRight, Helper.SECTION_LINE_LAYER_NAME);
            }
            var noneRevoLines = Helper.CreateNoneRevo(FSectionGeometry.NoneRevoRectangles);

            using (var transaction = _acDoc.Database.TransactionManager.StartTransaction())
            {
                var acBlockTable = transaction.GetObject(_acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                var acBlockTableRecord = transaction.GetObject(acBlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                Helper.AddEntity(revonationPline, acBlockTableRecord, transaction);
                if (floorPline != null){
                    Helper.AddEntity(floorPline ,acBlockTableRecord, transaction);
                }
                Helper.AddEntities(noneRevoLines, acBlockTableRecord, transaction);

                var addEntities = CreateAddditionEntities();
                Helper.AddEntities(addEntities,acBlockTableRecord, transaction);

                var dimensions = new List<Entity>();
                CreateDimension(dimensions, transaction, _acCurDb);
                Helper.AddEntities(dimensions,acBlockTableRecord, transaction);

                var notes = new List<Entity>();
                CreateNotes(notes);
                Helper.AddEntities(notes,acBlockTableRecord, transaction);
                var leaderNote = new List<Entity>();
                CreateLeaderText(leaderNote,transaction,_acCurDb);
                Helper.AddEntities(leaderNote,acBlockTableRecord, transaction);
                if (tnfdata.IsFirtRevoPile)
                {
                    var firstPiles = new List<Entity>();
                    CreateFirstPiles(firstPiles,transaction,_acCurDb);
                    Helper.AddEntities(firstPiles,acBlockTableRecord, transaction);
                }
                if (tnfdata.IsSecondRevoPile)
                {
                    var secondPiles = new List<Entity>();
                    CreateSecondPiles(secondPiles);
                    Helper.AddEntities(secondPiles,acBlockTableRecord, transaction);
                }
                transaction.Commit();
            }

        }
        protected virtual void CreateFirstPiles(List<Entity> entities,Transaction transaction,Database acCurrentDb)
        {
            var tnfdata = FSectionGeometry.FSectionData.TNFParameters;
            var mainPile = Helper.CreatePiles(FSectionGeometry.FirstPrimaryRevo,tnfdata.ScaleRatio,tnfdata.TNFPile,out var pilePoint);
            entities.AddRange(mainPile);
            var pileNote = tnfdata.TNFPile.Description;
            entities.Add(Helper.CreatePileLeaderNote(pileNote, pilePoint.Point2DtoPoint3d(), tnfdata.ScaleRatio, transaction, acCurrentDb, TextHeight));


            if(FSectionGeometry is IHasSubFoundation hasSubG)
            {
                var subPile = Helper.CreatePiles(hasSubG.SubRevoFirst,tnfdata.ScaleRatio,tnfdata.TNFPile,out var subpilePoint);
                entities.AddRange(subPile);
                entities.Add(Helper.CreatePileLeaderNote(pileNote, subpilePoint.Point2DtoPoint3d(), tnfdata.ScaleRatio, transaction, acCurrentDb, TextHeight));
            }
        }
        protected virtual void CreateSecondPiles(List<Entity> entities)
        {
            var tnfdata = FSectionGeometry.FSectionData.TNFParameters;
            var secondLeftPile = Helper.CreatePiles(FSectionGeometry.SecondLeftPrimaryRevo,tnfdata.ScaleRatio,tnfdata.TNFPile,out var secondLeftPilePoint);
            entities.AddRange(secondLeftPile);
            var secondRightPile = Helper.CreatePiles(FSectionGeometry.SecondRightPrimaryRevo,tnfdata.ScaleRatio,tnfdata.TNFPile,out var secondRightPilePoint);
            entities.AddRange(secondRightPile);
        }

        protected abstract List<Entity> CreateAddditionEntities();

        protected virtual void CreateDimension(List<Entity> entities,Transaction transaction,Database acCurrentDb)
        {
            var tnfParam = this.FSectionGeometry.FSectionData.TNFParameters;

            var dist = 460 * tnfParam.ScaleRatio;

            var startP = FSectionGeometry.FirstPrimaryRevo.BottomLeft;
            var endP = FSectionGeometry.FirstPrimaryRevo.BottomRight;
            var mainFDim = Helper.CreateDimension(startP, endP, dist, Helper.GetPointDownYByDistance, tnfParam.ScaleRatio,transaction,acCurrentDb, "TP 0-7 寸法", true);
            entities.Add(mainFDim);

            dist = 350 * tnfParam.ScaleRatio;

            var startX = FSectionGeometry.Floor.TopRight.X;
            var startY = FSectionGeometry.FirstPrimaryRevo.BottomRight.Y;

            startP = new devDept.Geometry.Point2D(startX, startY);
            endP = FSectionGeometry.SecondRightPrimaryRevo.BottomRight;
            var firstRevoDim = Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParam.ScaleRatio, transaction, acCurrentDb);
            entities.Add(firstRevoDim);

            startP = FSectionGeometry.SecondRightPrimaryRevo.BottomRight;
            endP = FSectionGeometry.SecondRightPrimaryRevo.TopRight;
            var secondRevoDim = Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParam.ScaleRatio, transaction, acCurrentDb);
            entities.Add(secondRevoDim);

            startP = FSectionGeometry.Floor.TopRight;
            endP = FSectionGeometry.Floor.BottomRight;

            var floorDimP = Helper.GetPointUpYByDistance(startP.Point2DtoPoint3d(), 480 * tnfParam.ScaleRatio);
            var floorDP = Helper.GetPointUpXByDistance(floorDimP, dist);
            var floorDIm = Helper.CreateDimension(startP,endP,floorDP,tnfParam.ScaleRatio, transaction, acCurrentDb);
            entities.Add(floorDIm);





        }

        protected virtual void CreateNotes(List<Entity> entities)
        {
            var tnfParams = this.FSectionGeometry.FSectionData.TNFParameters;
            var firstMainWidthNote = $"W={tnfParams.FirstRevonationLength.ToString("0,000")}";
            var firstMainPoint = Point2D.MidPoint(FSectionGeometry.FirstPrimaryRevo.BottomLeft, FSectionGeometry.FirstPrimaryRevo.BottomRight);
            var notePoint = Helper.GetPointDownYByDistance(firstMainPoint.Point2DtoPoint3d(), (460+175) * tnfParams.ScaleRatio);
            var firstNote = Helper.CreateText(firstMainWidthNote, notePoint, TextHeight, AttachmentPoint.MiddleCenter);
            entities.Add(firstNote);

            //var textFirst = Helper.CreateNoteForRect(Helper.FIRST_REVO_TEXT, FSectionGeometry.FirstPrimaryRevo, TextHeight, tnfParams.ScaleRatio);
            entities.Add(Helper.CreateNoteForRect(Helper.FIRST_REVO_TEXT, FSectionGeometry.FirstPrimaryRevo, TextHeight, tnfParams.ScaleRatio));
            entities.Add(Helper.CreateNoteForRect(Helper.SECOND_REVO_TEXT, FSectionGeometry.SecondLeftPrimaryRevo, TextHeight, tnfParams.ScaleRatio));
            entities.Add(Helper.CreateNoteForRect(Helper.SECOND_REVO_TEXT, FSectionGeometry.SecondRightPrimaryRevo, TextHeight, tnfParams.ScaleRatio));
        }
        protected virtual void CreateLeaderText(List<Entity> entities,Transaction transaction, Database currentDB)
        {
            var tnfParams = FSectionGeometry.FSectionData.TNFParameters;

            if (tnfParams.FloorThickness > 0)
            {
                var floorLeaderHeight = 400 * tnfParams.ScaleRatio;

                var floorY = (this.FSectionGeometry.Floor.TopRight.Y + FSectionGeometry.Floor.BottomRight.Y)/2 + floorLeaderHeight;
                var floorX = this.FSectionGeometry.FirstPrimaryRevo.TopLeft.X;
                var floorNotePoint = new Point2D(floorX, floorY);
                var floorNote = $"土間：t={tnfParams.FloorThickness} {tnfParams.FloorRebar.Description}";
                entities.Add(CreateLeader(floorNotePoint, floorLeaderHeight, floorNote, transaction, currentDB));
            }
            var fHeight = 1000* tnfParams.ScaleRatio;

            var mainFY = FSectionGeometry.MainFoundationSection.TopRight.Y - 200 * tnfParams.ScaleRatio + fHeight;
            var mainFX = FSectionGeometry.MainFoundationSection.TopRight.X;
            var mainPoint = new Point2D(mainFX, mainFY);
            var mainNote = FSectionGeometry.MainFoundationSection.Foundation.Description;
            entities.Add(CreateLeader(mainPoint,fHeight,mainNote, transaction, currentDB));

            if(FSectionGeometry is IHasSubFoundation hasSubFoundation)
            {
                var subHeight = 2200 * tnfParams.ScaleRatio;
                var subX = hasSubFoundation.SubFoundation.TopLeft.X + 800 * tnfParams.ScaleRatio;
                var subY = hasSubFoundation.SubFoundation.TopLeft.Y - 500*tnfParams.ScaleRatio + subHeight;
                var subPoint = new Point2D(subX, subY);
                var subNote = hasSubFoundation.SubFoundation.Foundation.Description;

                entities.Add(CreateLeader(subPoint,subHeight,subNote, transaction, currentDB));
            }
            if(FSectionGeometry is PlatformSectionG platform)
            {
                var extraConCreateHeight = 800 * tnfParams.ScaleRatio;
                var extraConcreateX = platform.SubFoundation.HashiraGataColumn.TopRight.X + 700 * tnfParams.ScaleRatio;
                var extraConcreateY = platform.SubFoundation.HashiraGataColumn.TopRight.Y - 240 * tnfParams.ScaleRatio + extraConCreateHeight;
                var extraConcreatePoint = new Point2D(extraConcreateX, extraConcreateY);
                var extraConcreateNote = $"コンクリート増打\\P又は改良掘削土埋戻し\\P（十分転圧する事";
                var leader = CreateLeader(extraConcreatePoint, extraConCreateHeight, extraConcreateNote, transaction, currentDB, AttachmentPoint.MiddleLeft);
                //leader.TextAlignmentType = TextAlignmentType.LeftAlignment;
                leader.TextAttachmentType = TextAttachmentType.AttachmentMiddleOfBottom;
                entities.Add(leader);
            }


        }
        private MLeader CreateLeader(Point2D basePoint,double leaderHeight, string noteString, Transaction transaction, Database currentDb,AttachmentPoint attachmentPoint = AttachmentPoint.MiddleCenter)
        {
            var tnfParams = FSectionGeometry.FSectionData.TNFParameters;
            return Helper.CreateLeaded(basePoint.Point2DtoPoint3d(), leaderHeight, TextHeight, tnfParams.ScaleRatio, transaction, currentDb, noteString, attachmentPoint);
        }
    }
}
