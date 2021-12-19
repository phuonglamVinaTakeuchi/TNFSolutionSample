using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace TNFFuntion
{
    public class TNFD1
    {
        [CommandMethod("TNFD1")]
        public static void AddLightweightPolyline()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                             OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create a polyline with two segments (3 points)
                Polyline acPoly = new Polyline();
                acPoly.SetDatabaseDefaults();
                acPoly.AddVertexAt(0, new Point2d(2, 4), 0, 0, 0);
                acPoly.AddVertexAt(1, new Point2d(4, 2), 0, 0, 0);
                acPoly.AddVertexAt(2, new Point2d(6, 4), 0, 0, 0);

                // Add the new object to the block table record and the transaction
                acBlkTblRec.AppendEntity(acPoly);
                acTrans.AddNewlyCreatedDBObject(acPoly, true);

                // Save the new object to the database
                acTrans.Commit();
            }
        }
        [CommandMethod("TNF_MONGD")]
        public static void GetPointsFromUser()
        {
            // Get the current database and start the Transaction Manager
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            #region　基礎中心入力

            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");

            // diem tam mong
            pPtOpts.Message = "\n基礎の中心を入力する: ";
            pPtRes = acDoc.Editor.GetPoint(pPtOpts);
            Point3d tamMong = pPtRes.Value;
            if (pPtRes.Status== PromptStatus.Cancel) return;
            #endregion
            #region 基礎名称入力

            // Nhap ten mong
            PromptStringOptions pStropts = new PromptStringOptions("");
            pStropts.Message = "\n基礎名称を入力する";
            pStropts.DefaultValue = "F1";
            PromptResult pStrRes = acDoc.Editor.GetString(pStropts);
            #endregion
            #region 基礎最大長さ・最低長さ入力

            PromptDoubleResult pDisRes;
            PromptDoubleOptions pDisOpts = new PromptDoubleOptions("");
            // nhap day lon
            pDisOpts.Message = "\n基礎の最大長さを入力する:";
            pDisOpts.DefaultValue = 3000;

            pDisRes = acDoc.Editor.GetDouble(pDisOpts);
            double dayLon = pDisRes.Value;
            if (pDisRes.Status == PromptStatus.Cancel) return;

            //nhap day be
            pDisOpts.Message = "\n基礎の最低長さを入力する:";
            pDisOpts.DefaultValue = 1000;
            pDisRes = acDoc.Editor.GetDouble(pDisOpts);
            double dayBe = pDisRes.Value;

            if (pDisRes.Status == PromptStatus.Cancel) return;
            #endregion



            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                BlockTableRecord acBlkTblRec;

                // Open Model space for write
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                             OpenMode.ForRead) as BlockTable;

                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;


                // Ve day lon ngoai
                bool success = CreatePoly(tamMong, dayLon, "continues", out Polyline miengMong);
                if (!success)
                {
                    return;
                }

                // Add the line to the drawing
                acBlkTblRec.AppendEntity(miengMong);
                acTrans.AddNewlyCreatedDBObject(miengMong, true);

                // ve day be trong
                success = CreatePoly(tamMong, dayBe, "点線(3)", out Polyline dayMong,20);
                if (!success)
                {
                    return;
                }

                // Add the line to the drawing
                acBlkTblRec.AppendEntity(dayMong);
                acTrans.AddNewlyCreatedDBObject(dayMong, true);

                // Ve duong line1 dakei
                Line acLine1 = new Line(new Point3d(tamMong.X - dayLon / 2, tamMong.Y + dayLon / 2,0),
                                       new Point3d(tamMong.X - dayBe / 2, tamMong.Y + dayBe / 2, 0) );
                acLine1.Linetype = "点線(3)";
                acLine1.LinetypeScale = 20;

                AddEntity(acLine1, acBlkTblRec, acTrans);

                // Ve duong line2 dakei
                Line acLine2 = new Line(new Point3d(tamMong.X + dayLon / 2, tamMong.Y + dayLon / 2, 0),
                                       new Point3d(tamMong.X + dayBe / 2, tamMong.Y + dayBe / 2, 0));
                acLine2.Linetype = "点線(3)";
                acLine2.LinetypeScale = 20;

                AddEntity(acLine2, acBlkTblRec, acTrans);

                // Ve duong line3 dakei
                Line acLine3 = new Line(new Point3d(tamMong.X + dayLon / 2, tamMong.Y - dayLon / 2, 0),
                                       new Point3d(tamMong.X + dayBe / 2, tamMong.Y - dayBe / 2, 0));
                acLine3.Linetype = "点線(3)";
                acLine3.LinetypeScale = 20;

                AddEntity(acLine3, acBlkTblRec, acTrans);

                // Ve duong line4 dakei
                Line acLine4 = new Line(new Point3d(tamMong.X - dayLon / 2, tamMong.Y - dayLon / 2, 0),
                                       new Point3d(tamMong.X - dayBe / 2, tamMong.Y - dayBe / 2, 0));

                acLine4.Linetype = "点線(3)";
                acLine4.LinetypeScale = 20;

                AddEntity(acLine4, acBlkTblRec, acTrans);


                // Text ten mong
                DBText acText = new DBText();
                acText.SetDatabaseDefaults();
                acText.Height = 500;
                acText.Justify = AttachmentPoint.BaseLeft;
                acText.TextString =pStrRes.StringResult;
                acText.Position = new Point3d(tamMong.X - dayLon / 2, tamMong.Y + dayLon / 2 + 200, 0);

                AddEntity(acText, acBlkTblRec, acTrans);

                // Zoom to the extents or limits of the drawing
                //acDoc.SendStringToExecute("._zoom _all ", true, false, false);


                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }

        }
        private static void AddEntity(Entity ent,BlockTableRecord blockTableRecord,Transaction trans)
        {
            blockTableRecord.AppendEntity(ent);
            trans.AddNewlyCreatedDBObject(ent, true);
        }

        private static bool CreatePoly(Point3d basePoint, double width, string lineType, out Polyline acPoly, int linetypeScale = 0)
        {
            acPoly = new Polyline();

            acPoly.SetDatabaseDefaults();
            try
            {
                acPoly.Linetype = lineType;
            }
            catch
            {
                return false;
            }

            if(linetypeScale != 0)
            {
                acPoly.LinetypeScale = linetypeScale;
            }

            var topLeft = new Point2d(basePoint.X - width / 2, basePoint.Y + width / 2);
            var topRight = new Point2d(basePoint.X + width / 2, basePoint.Y + width / 2);
            var bottomLeft = new Point2d(basePoint.X - width / 2, basePoint.Y - width / 2);
            var bottomRight = new Point2d(basePoint.X - width / 2, basePoint.Y + width / 2);


            acPoly.AddVertexAt(0, topLeft, 0, 0, 0);
            acPoly.AddVertexAt(1, topRight, 0, 0, 0);
            acPoly.AddVertexAt(2, bottomRight, 0, 0, 0);
            acPoly.AddVertexAt(3, bottomLeft, 0, 0, 0);
            acPoly.AddVertexAt(4, topLeft, 0, 0, 0);
            acPoly.Closed = true;

            return true;
        }

    }

}
