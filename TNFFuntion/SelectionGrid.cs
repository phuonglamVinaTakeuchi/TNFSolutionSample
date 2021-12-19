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
    public static class SelectionGrid
    {
        private static Document _acDoc = Application.DocumentManager.MdiActiveDocument;
        private static Database _acCurDb = Application.DocumentManager.MdiActiveDocument.Database;
        public static bool Select(out List<Line> vLine,out List<Line> hLine,out PromptSelectionResult selectionResult,string layerName)
        {
            bool success = true;
            var prompSelect = new PromptSelectionOptions();
            prompSelect.MessageForAdding = "Please select the Grids";
            var typeValues = new TypedValue[2];
            typeValues.SetValue(new TypedValue((int)DxfCode.Start, "Line"), 0);
            typeValues.SetValue(new TypedValue((int)DxfCode.LayerName, layerName), 1);
            var filter = new SelectionFilter(typeValues);
            var firtTime = true;
            do
            {
                if (!firtTime)
                {
                    prompSelect.MessageForAdding = "You have not select Vertical Grids or Horizontal Grids please select againt.../n";
                }
                firtTime = false;
                do
                {
                    selectionResult = _acDoc.Editor.GetSelection(prompSelect, filter);
                    if (selectionResult.Status == PromptStatus.Cancel)
                    {
                        vLine = null;
                        hLine = null;
                        return false;
                    }    
                        
                    if (selectionResult == null || selectionResult.Value == null || selectionResult.Value.Count < 1)
                    {
                        prompSelect.MessageForAdding = "You have not select anything.../n Please Select Againt";
                    }

                } while (selectionResult == null || selectionResult.Value == null || selectionResult.Value.Count < 1);

                TNFUtilities.GetVLineAndHLine(selectionResult.Value, out vLine, out hLine);

            } while (vLine.Count < 1 || hLine.Count < 1);
            return success;
        }
    }
}
