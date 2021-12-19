using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleSolution
{
    public class SampleSolution
    {
        private Document _acDoc;
        private Database _acCurDb;
        public SampleSolution()
        {
            _acDoc = Application.DocumentManager.MdiActiveDocument;
            _acCurDb = _acDoc.Database;
        }


        [CommandMethod("AutoDrawBoundaryCommand")]
        public void Solution2()
        {

            /// If fail when choose or user cancel, we return at there
            if (!SelectionGrid.Select(out var vLine, out var hLine, out var selectionResult, "TP 0-F 基準線"))
                return;

            /// vLine đường thẳng nằm đứng.
            /// hLine đường thẳng nằm ngang
            var maxRevo = Helper.GetMaxRevo(vLine, hLine);
            maxRevo.Add(maxRevo[maxRevo.Count - 1]);
            var maxRevoPline = Helper.CreatePolyLine(maxRevo, "TP 0-3 一次改良");
            maxRevoPline.Closed = true;
            Draw(maxRevoPline);
            return;
        }
        /// <summary>
        /// Add an entity to Cad database
        /// </summary>
        /// <param name="maxRevo"></param>
        private void Draw(Entity maxRevo)
        {
            using (var transaction = _acDoc.Database.TransactionManager.StartTransaction())
            {
                var acBlockTable = transaction.GetObject(_acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                var acBlockTableRecord = transaction.GetObject(acBlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                //var offsetCurve = ((Polyline)maxRevo).GetOffsetCurves();
                Helper.AddEntities(maxRevo, acBlockTableRecord, transaction);
                transaction.Commit();
            }


        }
    }
}
