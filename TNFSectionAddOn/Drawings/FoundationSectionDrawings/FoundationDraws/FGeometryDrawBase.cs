using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.FoundationGeometry;

namespace TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationDraws
{
    public abstract class FGeometryDrawBase
    {
        private Document _acDoc;
        private Database _acCurDb;
        public bool IsSubDim { get; set; }
        public double TextHeight => Helper.TextHeight * FoundationG.Foundation.TNFParams.ScaleRatio;
        public FoundationGBase FoundationG { get; }
        public FGeometryDrawBase(FoundationGBase foundation)
        {
            _acDoc = Application.DocumentManager.MdiActiveDocument;
            _acCurDb = _acDoc.Database;
            FoundationG = foundation;
            IsSubDim = false;
        }
        public virtual void Draw()
        {
            var normalFoundation = FoundationG;
            if (normalFoundation == null)
            {
                return;
            }

            var outLineFoundationPline = Helper.PlyLineFromPoint2D(normalFoundation.FoundationPoints, Helper.SECTION_LINE_LAYER_NAME);
            var foundationPline = Helper.PlyLineFromPoint2D(normalFoundation.BottomFoundationPoints, Helper.SECTION_LINE_LAYER_NAME);
            Line offsetLine = null;
            if (normalFoundation.OffsetPointLeft != null && normalFoundation.OffsetPointRight != null)
            {
                offsetLine = Helper.CreateLine(normalFoundation.OffsetPointLeft, normalFoundation.OffsetPointRight, Helper.OFFSET_LINE_LAYER_NAME,Helper.OFFSET_LINE_TYPE);
            }

            using (var transaction = _acDoc.Database.TransactionManager.StartTransaction())
            {
                var acBlockTable = transaction.GetObject(_acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                var acBlockTableRecord = transaction.GetObject(acBlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                Helper.AddEntity(outLineFoundationPline, acBlockTableRecord, transaction);
                Helper.AddEntity(foundationPline, acBlockTableRecord, transaction);
                if(offsetLine!=null)
                Helper.AddEntity(offsetLine, acBlockTableRecord, transaction);

                var rebarNote = CreateNotes();
                Helper.AddEntity(rebarNote,acBlockTableRecord,transaction);

                var dimensions = new List<Entity>() ;
                CreateDimension(dimensions, transaction, _acCurDb);
                Helper.AddEntities(dimensions,acBlockTableRecord,transaction) ;

                transaction.Commit();
            }
        }
        private MText CreateNotes()
        {
            var fRebarNote = FoundationG.Foundation.HakamaRebar.FullName +"\\P " + FoundationG.Foundation.BesuRebar.FullName;
            var fRebarPoint = FoundationG.GetCenterPoint();
            var fRebarText = new MText();
            fRebarText.SetDatabaseDefaults();
            fRebarText.Location = fRebarPoint.Point2DtoPoint3d();
            fRebarText.TextHeight = TextHeight;
            fRebarText.Layer = Helper.NoteInfoLayerName;
            fRebarText.Attachment = AttachmentPoint.MiddleCenter;
            var textStyle = Helper.GetTextStyleId(Helper.NoteTextStyle);
            fRebarText.TextStyleId = textStyle;
            fRebarText.Contents = fRebarNote;
            return fRebarText;

        }

        protected abstract void CreateDimension(List<Entity> entities, Transaction transaction, Database acCurrentDB);
    }
}
