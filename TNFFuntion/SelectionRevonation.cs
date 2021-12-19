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
    public static class SelectionRevonation
    {
        private static Document _acDoc = Application.DocumentManager.MdiActiveDocument;
        private static Database _acCurDb = Application.DocumentManager.MdiActiveDocument.Database;
        public static bool Select(out PromptSelectionResult selectionResult,string layerName)
        {
            var prompSelect = new PromptSelectionOptions();

            prompSelect.MessageForAdding = "Please select TNF Revonation\n";

            var typeValues2 = new TypedValue[2];
            typeValues2.SetValue(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"), 0);
            typeValues2.SetValue(new TypedValue((int)DxfCode.LayerName, layerName), 1);
            var newfilter = new SelectionFilter(typeValues2);

            selectionResult = _acDoc.Editor.GetSelection(prompSelect, newfilter);
            if (selectionResult.Status == PromptStatus.OK)
                return true;
            return false;
        }
    }
}
