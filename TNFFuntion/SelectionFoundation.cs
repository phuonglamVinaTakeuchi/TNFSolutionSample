using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFFuntion
{
    public static class SelectionFoundation
    {
        private static Document _acDoc = Application.DocumentManager.MdiActiveDocument;
        private static Database _acCurDb = Application.DocumentManager.MdiActiveDocument.Database;
        public static bool Select(out PromptSelectionResult selectionResult)
        {
            var prompSelect = new PromptSelectionOptions();

            prompSelect.MessageForAdding = "Please select TNF Foundation\n";

            var typeValues2 = new TypedValue[1];
            typeValues2.SetValue(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"), 0);
            var newfilter = new SelectionFilter(typeValues2);

            selectionResult = _acDoc.Editor.GetSelection(prompSelect, newfilter);
            if (selectionResult.Status == PromptStatus.OK)
                return true;
            return false;
        }
        public static void SelectMaxRevo(List<Entity> polyLines,out Polyline maxRevo)
        {
            var maxDistance = 0.0;
            maxRevo = null;
            foreach(var pline in polyLines)
            {
                var length = ((Polyline)pline).Length;
                if (length> maxDistance)
                {
                    maxDistance = length;
                    maxRevo = pline as Polyline;
                }
            }
            polyLines.Remove(maxRevo);

        }
    }
}
