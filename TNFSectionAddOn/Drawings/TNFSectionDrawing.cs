using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry;
using TNFData.Models.Section;
using TNFSectionAddOn.Drawings.BeamSectionDrawings;
using TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationSectionDraws;
using TNFSectionAddOn.Factory;

namespace TNFSectionAddOn.Drawings
{
    public class TNFSectionDrawing
    {
        private BeamDrawingBase _beamDraw ;
        private FSectionDrawBase _foundationDraw;
        private TNFSectionGeometryBase _tnfSectionGeometry;
        public TNFSectionDrawing(TNFSectionGeometryBase tnfSectionG)
        {
            _tnfSectionGeometry = tnfSectionG;
            if(tnfSectionG.TNFSectionData.TNFParameters.IsDrawBeamSection)
            {
                if (tnfSectionG.BeamSection != null)
                {
                    _beamDraw = FBeamDrawingFactory.CreateBeamDrawing(tnfSectionG.BeamSection);
                }
            }
            if(tnfSectionG.TNFSectionData.TNFParameters.IsDrawFoundationSection)
            _foundationDraw = FSectionFoundationGDrawFactory.CreateSectionDraw(tnfSectionG.FSection);
        }
        public void Draw()
        {
            var tnfParams = _tnfSectionGeometry.TNFSectionData.TNFParameters;
            if (tnfParams.IsDrawBeamSection)
            {
                if (_beamDraw!=null)
                {
                    _beamDraw.Draw();
                }
            }
            if(tnfParams.IsDrawFoundationSection)
            {
                if (_foundationDraw!=null)
                    _foundationDraw.Draw();
            }

            var yAxist = _tnfSectionGeometry.BasePoint.Y + ( tnfParams.FirstRevonationDepth + tnfParams.SecondRevonationDepth)*tnfParams.ScaleRatio;
            var xMinAxist = _tnfSectionGeometry.MinX - 450 * tnfParams.ScaleRatio;
            var xMaxAxist = _tnfSectionGeometry.MaxX+2*450* tnfParams.ScaleRatio;
            var minPoint = new Point2D(xMinAxist, yAxist);
            var maxPoint = new Point2D(xMaxAxist, yAxist);
            var glLine = Helper.CreateLine(minPoint, maxPoint, Helper.GL_LAYER_NAME,Helper.GL_LINE_TYPE_NAME);

            var acDoc = Application.DocumentManager.MdiActiveDocument;
            var acCurDb = acDoc.Database;
            using (var transaction = acDoc.Database.TransactionManager.StartTransaction())
            {
                var acBlockTable = transaction.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite) as BlockTable;
                var acBlockTableRecord = transaction.GetObject(acBlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                Helper.AddEntity(glLine,acBlockTableRecord,transaction);
                transaction.Commit();
            }

        }

    }
}
