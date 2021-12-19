using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry;
using TNFData.Geometry.FoundationSection;
using TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationDraws;
using TNFSectionAddOn.Factory;

namespace TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationSectionDraws
{
    public abstract class FSectionHasSubDrawBase : FSectionDrawBase
    {
        private FGeometryDrawBase _subFoudationDraw;
        protected FSectionHasSubDrawBase(FSectionGBase foundation) : base(foundation)
        {
            var platformG = foundation as PlatformSectionG;
            _subFoudationDraw = FGeometryDrawFactory.CreateFGeometryDraw(platformG.SubFoundation);
        }
        public override void Draw()
        {
            base.Draw();
            _subFoudationDraw.IsSubDim = true;
            _subFoudationDraw.Draw();
        }


        protected override List<Entity> CreateAddditionEntities()
        {
            var entities = new List<Entity>();
            CreateAdditionEntities(entities);

            //var firstLine = Helper.CreateLine();


            return entities;

        }
        protected virtual void CreateAdditionEntities(List<Entity> entities)
        {
            var beams = CreateSubBeam();
            var additionLines = CreateDivideLine();
            var overLines = CreateOverRevoLines();
            entities.AddRange(overLines);

            entities.AddRange(beams);
            entities.AddRange(additionLines);
            CreateRevoHatch(entities);
            CreateFoundationHatchs(entities);
        }
        private List<Entity> CreateDivideLine()
        {
            var entities = new List<Entity>();

            var platformG = FSectionGeometry as PlatformSectionG;

            var subRevoDevideLine = Helper.CreateLine(
                platformG.SubRevoFirst.TopLeft, platformG.SubRevoFirst.TopRight, Helper.DivideLineLayerName,
                Helper.DivideLineTypeName);
            var mainRevoDevideLine = Helper.CreateLine(
                platformG.FirstPrimaryRevo.TopLeft, platformG.FirstPrimaryRevo.TopRight, Helper.DivideLineLayerName,
                Helper.DivideLineTypeName);
            var noneRevoFirstLine = Helper.CreateLine(
                platformG.ThirdLeftPrimaryRevo.TopRight, platformG.SecondLeftPrimaryRevo.BottomRight,
                Helper.DivideLineLayerName, Helper.DivideLineTypeName);
            var noneRevoSecondLine = Helper.CreateLine(
                platformG.ThirdLeftPrimaryRevo.TopLeft, platformG.SecondLeftPrimaryRevo.BottomLeft,
                Helper.DivideLineLayerName, Helper.DivideLineTypeName);
            var noneRevoThirdLine = Helper.CreateLine(
                platformG.ThirdRightPrimaryRevo.TopLeft, platformG.SecondRightPrimaryRevo.BottomLeft,
                Helper.DivideLineLayerName, Helper.DivideLineTypeName);

            entities.Add(subRevoDevideLine);
            entities.Add(mainRevoDevideLine);
            entities.Add(noneRevoFirstLine);
            entities.Add(noneRevoSecondLine);
            entities.Add(noneRevoThirdLine);



            return entities;
        }

        private List<Entity> CreateSubBeam()
        {
            var entities = new List<Entity>();

            var platformG = FSectionGeometry as PlatformSectionG;

            var beamLeftLine = Helper.CreateLine(platformG.Floor.TopLeft,platformG.SubFoundation.HashiraGataColumn.BottomLeft,Helper.SECTION_LINE_LAYER_NAME);
            var beamRightLine = Helper.CreateLine(platformG.SubFoundation.HashiraGataColumn.BottomRight,
                platformG.SubFoundation.HashiraGataColumn.TopRight,Helper.SECTION_LINE_LAYER_NAME);
            if (platformG.SubFoundation.Foundation.HashiraGataOffsetWithGL > 0)
            {
                var hashiragataOffsetLine = Helper.CreateLine(platformG.SubFoundation.HashiraGataOffsetPointLeft,
                    platformG.SubFoundation.HashiraGataOFfsetPointRight, Helper.OFFSET_LINE_LAYER_NAME,Helper.OFFSET_LINE_TYPE);
                entities.Add(hashiragataOffsetLine);
            }
            var platformLine = Helper.CreateLine(platformG.SubFoundation.TopRight,platformG.PlatformPoint,Helper.SECTION_LINE_LAYER_NAME);
            var hashiraGataColumNotes = CreateHashiragataColumnNote();

            entities.Add(beamLeftLine);
            entities.Add(beamRightLine);
            entities.Add(platformLine);
            entities.Add(hashiraGataColumNotes);
            return entities;
        }

        private Entity CreateHashiragataColumnNote()
        {
            var platformG = FSectionGeometry as PlatformSectionG;
            var textNoteXLocation = platformG.SubFoundation.BasePoint.X ;

            var txtTempH = platformG.Floor.TopLeft.Y - platformG.SubFoundation.HashiraGataOffsetPointLeft.Y;

            var textNoteYLocation = platformG.Floor.TopLeft.Y-
                (txtTempH - TextHeight)/2;

            var foundationParam = platformG.SubFoundation.Foundation;
            var widthNote = $"ロ -{foundationParam.HashiraGataDimension.XLength}";
            var hMainRebar = foundationParam.HashiraGataMainRebar;
            var mainRebarNote = $"{hMainRebar.Quantities}-{hMainRebar.PrefixSymbol}{hMainRebar.Radius}";
            var hoopRebar = foundationParam.HashiraGataHoopRebar;
            var hoopNote = $"{hoopRebar.PrefixSymbol}{hoopRebar.Radius}@{hoopRebar.Spacing}";
            var hashiragataNote = $"{widthNote}\\P{mainRebarNote}\\P{hoopNote}";
            var hashiragataPoint = new Point3d(textNoteXLocation,textNoteYLocation,0);
            var hashiraGataNoteText = new MText();
            hashiraGataNoteText.SetDatabaseDefaults();
            hashiraGataNoteText.Location = hashiragataPoint;
            hashiraGataNoteText.TextHeight = TextHeight;
            hashiraGataNoteText.Layer = Helper.NoteInfoLayerName;
            hashiraGataNoteText.Attachment = AttachmentPoint.TopCenter;
            var textStyle = Helper.GetTextStyleId(Helper.NoteTextStyle);
            hashiraGataNoteText.TextStyleId = textStyle;
            hashiraGataNoteText.Contents = hashiragataNote;
            return hashiraGataNoteText;
        }

        protected abstract List<Entity> CreateOverRevoLines();
        private void CreateRevoHatch(List<Entity> entities)
        {
            foreach(var revoPoints in FSectionGeometry.RevoHatchPoints)
            {
                var revoHatch = Helper.CreateHatch(revoPoints, null, Helper.REVO_HATCH_NAME, RevoHatchScale);
                entities.Add(revoHatch);
            }
        }
        private void CreateFoundationHatchs(List<Entity> entities)
        {
            foreach (var foundationPoint in FSectionGeometry.FoundationHatchPoints)
            {
                var foundationHatch = Helper.CreateHatch(foundationPoint, null, Helper.FOUNDATION_HATCH_NAME, FHatchScale);
                entities.Add(foundationHatch);
            }
        }
        protected override void CreateDimension(List<Entity> entities, Transaction transaction, Database acCurrentDb)
        {
            base.CreateDimension(entities, transaction, acCurrentDb);
            var tnfParam = this.FSectionGeometry.FSectionData.TNFParameters;
            var fSection = FSectionGeometry as PlatformSectionG;
            var dist = 350 * tnfParam.ScaleRatio;

            var startP = fSection.ThirdRightPrimaryRevo.TopRight;
            var endP = fSection.ThirdRightPrimaryRevo.BottomRight;
            entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointUpXByDistance, tnfParam.ScaleRatio, transaction, acCurrentDb));

            dist = 460* tnfParam.ScaleRatio;
            startP = fSection.SubRevoFirst.BottomLeft;
            endP = fSection.SubRevoFirst.BottomRight;
            entities.Add(Helper.CreateDimension(startP, endP, dist,Helper.GetPointDownYByDistance,tnfParam.ScaleRatio, transaction, acCurrentDb, "TP 0-7 寸法", true));

            dist = 370 * tnfParam.ScaleRatio;
            var startX = fSection.SubFoundation.TopLeft.X;
            var startY = fSection.Floor.TopLeft.Y;
            startP = new devDept.Geometry.Point2D(startX, startY);
            endP = fSection.SubFoundation.TopLeft;
            entities.Add(Helper.CreateDimension(startP, endP, dist, Helper.GetPointDownXByDistance, tnfParam.ScaleRatio, transaction, acCurrentDb));

            startP = fSection.SubFoundation.ConcreateBottomRight;
            endP = fSection.PlatformPoint;
            entities.Add(Helper.CreateDimension(startP,endP,dist,Helper.GetPointUpXByDistance, tnfParam.ScaleRatio, transaction, acCurrentDb));

        }
        protected override void CreateNotes(List<Entity> entities)
        {
            base.CreateNotes(entities);
            var tnfParams = FSectionGeometry.FSectionData.TNFParameters;
            var fSection = FSectionGeometry as PlatformSectionG;

            entities.Add(Helper.CreateNoteForRect(Helper.THIRD_REVO_TEXT, fSection.ThirdLeftPrimaryRevo, TextHeight, tnfParams.ScaleRatio));
            entities.Add(Helper.CreateNoteForRect(Helper.THIRD_REVO_TEXT,fSection.ThirdRightPrimaryRevo,TextHeight, tnfParams.ScaleRatio));
        }
    }
}
