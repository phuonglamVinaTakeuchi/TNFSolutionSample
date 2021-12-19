using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFFuntion
{
    public class TNFKolF
    {
        private Document _acDoc;
        private Database _acCurDb;
        public TNFKolF()
        {
            _acDoc = Application.DocumentManager.MdiActiveDocument;
            _acCurDb = _acDoc.Database;
        }
        [CommandMethod("TNFKolF")]
        public void TNFKolFDraw()
        {
            PromptStringOptions pStrOpts = new PromptStringOptions("\nCol Offset:<900>: ");
            pStrOpts.DefaultValue = "900";
            pStrOpts.UseDefaultValue = true;
            PromptResult pStrRes = _acDoc.Editor.GetString(pStrOpts);
            if (string.IsNullOrEmpty(pStrRes.StringResult))
                return;
            double.TryParse(pStrRes.StringResult, out var offsetDist);
            if (offsetDist == 0) offsetDist = 900;
            // Select the base grid

            if (!SelectionGrid.Select(out var vLine, out var hLine, out var selectionResult, "TP 0-F 基準線"))
                return;


            var xGridPoints = TNFUtilities.GetMatrixPoints(vLine);
            var yGridPoints = TNFUtilities.GetMatrixPoints(hLine, false);

            var sortedvLine = vLine.OrderBy(x => x.StartPoint.X).ToList()[0];
            var sortedhLine = hLine.OrderBy(y => y.StartPoint.Y).ToList()[0];

            var xColListMatrixs = TNFUtilities.GetColList(xGridPoints.ToList(), offsetDist);
            var yColListMatrixs = TNFUtilities.GetColList(yGridPoints.ToList(), offsetDist);

            var colPoints = TNFUtilities.GetColDrawPointsFromGrids(xColListMatrixs, yColListMatrixs);



            PromptSelectionResult revoResult;

            do
            {
                var selectSuccess = SelectionRevonation.Select(out revoResult, "TP 0-3 一次改良");
                if (!selectSuccess)
                {
                    return;
                }
            } while (revoResult.Value == null || revoResult.Value.Count < 1);

            var tnfFoundation = TNFUtilities.GetEntities(revoResult.Value);
            SelectionFoundation.SelectMaxRevo(tnfFoundation,out var maxRevo);

            var colDrawPoint = TNFUtilities.FoundPointInside(colPoints.ToList(), tnfFoundation);

            TNFUtilities.DrawColEnt(colDrawPoint.ToArray());

        }


    }
}
