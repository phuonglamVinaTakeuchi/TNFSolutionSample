using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFFuntion
{
    public class TNFSelectionFilters
    {
        private static Document _acDoc = Application.DocumentManager.MdiActiveDocument;
        private static Database _acCurDb = Application.DocumentManager.MdiActiveDocument.Database;
        [CommandMethod("SelectDimension")]
        public void SelectFilter()
        {

            var prompSelect = new PromptSelectionOptions();

            prompSelect.MessageForAdding = "Please select TNF Foundation\n";

            var typeValues2 = new TypedValue[1];
            typeValues2.SetValue(new TypedValue((int)DxfCode.Start, "DIMENSION"), 0);
            var newfilter = new SelectionFilter(typeValues2);

            var selectionResult = _acDoc.Editor.GetSelection(prompSelect, newfilter);
            if (selectionResult.Status == PromptStatus.OK)
            {


                using (var transaction = _acCurDb.TransactionManager.StartTransaction())
                {
                    var objectIds = selectionResult.Value.GetObjectIds();
                    foreach (var objectId in objectIds)
                    {
                        var entity = transaction.GetObject(objectId, OpenMode.ForWrite) as Entity;

                        entity.ColorIndex = 256;
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
