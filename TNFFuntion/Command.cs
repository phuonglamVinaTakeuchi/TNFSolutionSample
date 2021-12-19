using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Polybool.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFFuntion
{
    public class Commands

    {

        [CommandMethod("USOLS")]

        static public void UniteSolids()

        {

            var doc = Application.DocumentManager.MdiActiveDocument;



            var psr =

              SelectSolids(

                doc.Editor,

                "\nSelect solid objects to unite"

              );

            if (psr.Status != PromptStatus.OK)

                return;



            if (psr.Value.Count > 1)

            {

                BooleanSolids(

                  doc, psr.Value[0].ObjectId, AllButFirst(psr),

                  BooleanOperationType.BoolUnite

                );

            }

        }



        [CommandMethod("ISOLS")]

        static public void IntersectSolids()

        {

            var doc = Application.DocumentManager.MdiActiveDocument;



            var psr =

              SelectSolids(

                doc.Editor,

                "\nSelect solid objects to intersect"

              );

            if (psr.Status != PromptStatus.OK)

                return;



            if (psr.Value.Count > 1)

            {

                BooleanSolids(

                  doc, psr.Value[0].ObjectId, AllButFirst(psr),

                  BooleanOperationType.BoolIntersect

                );

            }

        }



        [CommandMethod("SSOLS")]

        static public void SubtractSolids()

        {

            var doc = Application.DocumentManager.MdiActiveDocument;

            var ed = doc.Editor;



            var first = SelectSingleSolid(ed, "\nSelect primary solid");

            if (first == ObjectId.Null)

                return;



            var psr = SelectSolids(ed, "\nSelect solids to subtract");

            if (psr.Status != PromptStatus.OK)

                return;



            if (psr.Value.Count > 0)

            {

                BooleanSolids(

                  doc, first, psr.Value.GetObjectIds(),

                  BooleanOperationType.BoolSubtract

                );

            }

        }



        private static ObjectId SelectSingleSolid(

          Editor ed, string prompt

        )

        {

            var peo = new PromptEntityOptions(prompt);

            peo.SetRejectMessage("\nMust be a Region");

            peo.AddAllowedClass(typeof(Polyline), false);



            var per = ed.GetEntity(peo);

            if (per.Status != PromptStatus.OK)

                return ObjectId.Null;



            return per.ObjectId;

        }



        private static ObjectId[] AllButFirst(PromptSelectionResult psr)

        {

            // Use LINQ to skip the first item in the IEnumerable

            // and then return the results as an ObjectId array



            return

              psr.Value.Cast<SelectedObject>().Skip(1).

                Select(o => { return o.ObjectId; }).ToArray();

        }



        private static PromptSelectionResult SelectSolids(

          Editor ed, string prompt

        )

        {

            // Set up our selection to only select 3D solids



            var pso = new PromptSelectionOptions();

            pso.MessageForAdding = prompt;



            var sf =

              new SelectionFilter(

                new TypedValue[]

                {

            new TypedValue((int)DxfCode.Start, "LWPOLYLINE")

                }

              );



            return ed.GetSelection(pso, sf);

        }



        private static void BooleanSolids(

          Document doc, ObjectId first, ObjectId[] others,

          BooleanOperationType op

        )

        {

            var tr = doc.TransactionManager.StartTransaction();
            var acCurDb = doc.Database;

            using (tr)

            {
                var acBlockTable = tr.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                var acBlockTableRecord = tr.GetObject(acBlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var sol =

                  tr.GetObject(first, OpenMode.ForWrite) as Polyline;
                var s1 = TNFUtilities.RegionFromClosedCurve(sol);



                if (sol != null)

                {

                    foreach (ObjectId id in others)

                    {

                        var sol2 =

                          tr.GetObject(id, OpenMode.ForWrite) as Polyline;
                        var s2 = TNFUtilities.RegionFromClosedCurve(sol2);


                        if (sol2 != null)

                        {

                            s1.BooleanOperation(op, s2);

                        }
}
}

                var point = TNFUtilities.GetPointsFromRegion(s1);
                var pline = new Polyline(point.Count);
                pline.SetDatabaseDefaults();
                pline.Layer = "TP 0-3 一次改良";
                var verticeIndex = 0;
                foreach (var vertice in point)
                {
                    pline.AddVertexAt(verticeIndex, vertice, 0, 0, 0);
                    verticeIndex++;
                }
                pline.Closed = true;
                TNFUtilities.AddEntities(pline, acBlockTableRecord, tr);
                tr.Commit();

            }

        }

    }
}
