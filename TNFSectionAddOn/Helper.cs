using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BaseGeometry;
using TNFData.Models;

namespace TNFSectionAddOn
{
    public delegate Point3d GetNewPointDelegate(Point3d basePoint, double length);
    public static  class Helper
    {
        public const string HatchingLayerName = "TP 0-3 一次改良";
        public static Color HatchingColor = Color.FromRgb(206, 204, 230);
        public const string FlooringGLLayerName = "TP 0-F 基準線";
        public const string SECTION_LINE_LAYER_NAME = "TP 0-3 一次改良";
        public const string NoteInfoLayerName = "TP 0-7 寸法";
        public const string OFFSET_LINE_LAYER_NAME = "TP 0-1 基礎";
        public const string DimensionLineLayerName = "TP 0-7 寸法";
        public const string GLBaseLineLayerName = "TP 0-F 基準線";
        public const string PileLayerName = "TP 2-1";
        public const string DivideLineLayerName = "TP 0-2 FW";
        public const string DivideLineTypeName = "JWW_DASHED1";
        public const string OFFSET_LINE_TYPE = "JWW_DASHED1";
        public const double TextHeight = 180;
        public const double REVO_HATCH_SCALE = 100;
        public const double FHATCH_SCALE = 50;
        public const double STONE_HATCH_SCALE = 20;
        public const string NoteTextStyle = "1-200";
        public const string NoneRevoLineLayerName = "TP 0-4二次改良";
        public const string NoneRevoLineTypeName = "ByLayer";
        public const double HASHIRAGATA_COLUMN_NOTE_DISTANCE = 35;
        public const string REVO_HATCH_NAME = "ANSI31";
        public const string FOUNDATION_HATCH_NAME = "JIS_RC_15";
        public const string STONE_HATCH_NAME = "GRAVEL";
        public const string DIM_STYLE_NAME = "1-300";
        public const double DIMPOINT_UP_Y_DISTANCE = 1200;
        public const double DIMPOINT_DOWN_Y_DISTANCE = 900;
        public const string FIRST_REVO_TEXT = "一次改良";
        public const string SECOND_REVO_TEXT = "ニ次改良";
        public const string THIRD_REVO_TEXT = "ニ次改良";
        public const string STONE_NOTE = "砕石";
        public const string DIMENSION_LAYER_NAME = "TP 0-7 寸法";
        public const string GL_LAYER_NAME = "TP 0-F 基準線";
        public const string GL_LINE_TYPE_NAME = "ByLayer";
        public const string PILE_LAYER_NAME = "TP 2-1";
        public const string PILE_LINE_TYPE = "ByLayer";
        public static Point3d Point2DtoPoint3d(this Point2D point)
        {
            return new Point3d(point.X, point.Y, 0);
        }
        public static Point2d Point2DtoPoint2d(this Point2D point)
        {
            return new Point2d(point.X, point.Y);
        }
        public static Point3d Point2dtoPoint3d(this Point2d point)
        {
            return new Point3d(point.X, point.Y, 0);
        }
        public static Point2d Point3dtoPoint2d(this Point3d point)
        {
            return new Point2d(point.X, point.Y);
        }
        public static List<Point2d> ListPoint2DToPoint2d(List<Point2D> points)
        {
            var returnList = new List<Point2d>();
            foreach(var point in points)
            {
                returnList.Add(point.Point2DtoPoint2d());
            }
            return returnList;

        }

        public static Polyline PlyLineFromPoint2D(List<Point2D> points,string layerName)
        {
            var acadPoints = ListPoint2DToPoint2d(points);
            var pline = new Polyline(points.Count);
            pline.SetDatabaseDefaults();
            pline.Layer = layerName;
            var i = 0;
            foreach (var point in acadPoints)
            {
                pline.AddVertexAt(i, point, 0, 0, 0);
                i++;
            }
            return pline;
        }
        public static Line CreateLine(Point2D startPoint,Point2D endPoint,string layerName,string lineType = "Continuous")
        {
            var acadStartPoint =startPoint.Point2DtoPoint3d();
            var acadEndPoint = endPoint.Point2DtoPoint3d();
            var line = new Line(acadStartPoint, acadEndPoint);
            line.SetDatabaseDefaults();
            line.Layer= layerName;
            line.Linetype = lineType;
            return line;
        }
        public static void AddEntity(Entity entity, BlockTableRecord acBlockTableRecord, Transaction transaction)
        {
            acBlockTableRecord.AppendEntity(entity);
            transaction.AddNewlyCreatedDBObject(entity, true);
        }
        public static void AddEntities<T>(List<T> entities, BlockTableRecord acBlockTableRecord, Transaction transaction) where T : Entity
        {
            foreach(var entity in entities)
            {
                AddEntity(entity,acBlockTableRecord,transaction);
            }
        }
        public static void AddEntities(List<Entity> entities, BlockTableRecord acBlockTableRecord, Transaction transaction)
        {
            foreach (var entity in entities)
            {
                AddEntity(entity, acBlockTableRecord, transaction);
            }
        }
        public static Point3d MidPoint(Point2D p1,Point2D p2)
        {
            var midP = Point2D.MidPoint(p1, p2);
            return new Point3d(midP.X, midP.Y, 0);
        }
        public static ObjectId GetTextStyleId(string textStyleName)
        {
            var acDoc = Application.DocumentManager.MdiActiveDocument;
            var acCurDb = acDoc.Database;
            var textStyleId = acCurDb.Textstyle;
            using (var transaction = acDoc.Database.TransactionManager.StartTransaction())
            {
                var textStyleTable = transaction.GetObject(acCurDb.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                if (textStyleTable.Has(textStyleName))
                {
                    textStyleId = textStyleTable[textStyleName];
                }
            }
            return textStyleId;

        }

        public static List<Entity> CreateNoneRevo(List<Rectangle> rectangles)
        {
            var noneRevoLines = new List<Entity>();
            foreach(var rectangle in rectangles)
            {
                var firstLine = CreateLine(rectangle.TopLeft, rectangle.BottomRight, NoneRevoLineLayerName, NoneRevoLineTypeName);
                var secondLine = CreateLine(rectangle.TopRight, rectangle.BottomLeft, NoneRevoLineLayerName, NoneRevoLineTypeName);
                noneRevoLines.Add(firstLine);
                noneRevoLines.Add(secondLine);
            }
            return noneRevoLines;
        }
        public static Hatch CreateHatch(List<Point2D> points,Color color,string hatchName,double hatchScale)
        {
            if(color == null)
            {
                color = Color.FromRgb(206, 204, 230);
            }

            var hatch = new Hatch();
            hatch.SetDatabaseDefaults();
            hatch.Normal = Vector3d.ZAxis;
            hatch.Elevation = 0.0;
            hatch.PatternScale = hatchScale;
            hatch.SetHatchPattern(HatchPatternType.PreDefined, hatchName);
            hatch.Color = color;
            var pline = PlyLineFromPoint2D(points,SECTION_LINE_LAYER_NAME);
            pline.Closed = true;

            var acDoc = Application.DocumentManager.MdiActiveDocument;
            var acCurrentDb = acDoc.Database;
            using (var transaction = acCurrentDb.TransactionManager.StartTransaction())
            {
                var acBlockTable = transaction.GetObject(acCurrentDb.BlockTableId, OpenMode.ForWrite) as BlockTable;
                var acBlockTableRecord = transaction.GetObject(acBlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var oid1 = acBlockTableRecord.AppendEntity(pline);
                transaction.AddNewlyCreatedDBObject(pline, true);
                transaction.Commit();
            }

            var oid = pline.ObjectId;
            var oidCol = new ObjectIdCollection();
            oidCol.Add(oid);
            hatch.AppendLoop(HatchLoopTypes.External, oidCol);
            pline.Erase();
            return hatch;
        }
        public static AlignedDimension CreateDimension(Point2D startPoint,Point2D endPoint, Point3d dimPoint, double scaleRatio, Transaction transaction, Database acCurrentDB,  string layerName = "TP 0-7 寸法", bool isOverRideText = false)
        {
            var dimension = new AlignedDimension();
            dimension.SetDatabaseDefaults();
            dimension.XLine1Point = startPoint.Point2DtoPoint3d();
            dimension.XLine2Point = endPoint.Point2DtoPoint3d();
            dimension.Layer = layerName;
            dimension.DimLinePoint = dimPoint;
            dimension.TextPosition = dimPoint;
            if (isOverRideText)
                dimension.DimensionText = "W=<>";
            var dimStyleTable = transaction.GetObject(acCurrentDB.DimStyleTableId, OpenMode.ForWrite) as DimStyleTable;
            var dimStyleName = "1-300(" + scaleRatio + " - DimStyle)";
            var objectId = dimStyleTable[dimStyleName];
            dimension.DimensionStyle = objectId;
            return dimension;

        }
        public static AlignedDimension CreateDimension(Point2D startPoint, Point2D endPoint, double distMove, GetNewPointDelegate getNewPointByDistance, double scaleRatio, Transaction transaction, Database acCurrentDB,   string layerName = "TP 0-7 寸法", bool isOverRideText = false)
        {
            var dimPoint = Point2D.MidPoint(startPoint, endPoint);
            var dPoint = getNewPointByDistance(dimPoint.Point2DtoPoint3d(), distMove);
            return CreateDimension(startPoint, endPoint, dPoint, scaleRatio, transaction, acCurrentDB,layerName,isOverRideText);
        }

        public static Point3d GetPointByDistance(Point3d basePoint, Point3d trackingPoint, double distance)
        {
            var directDisc = trackingPoint.DistanceTo(basePoint);
            var vector = basePoint.GetVectorTo(trackingPoint);
            var factorDistacne = distance / directDisc;
            var x = factorDistacne * vector.X + basePoint.X;
            var y = factorDistacne * vector.Y + basePoint.Y;
            var z = factorDistacne * vector.Z + basePoint.Z;
            return new Point3d(x, y, z);
        }
        public static Point3d GetPointUpXByDistance(Point3d basePoint, double distance)
        {
            return GetPointByDistance(basePoint, new Point3d(basePoint.X + 1, basePoint.Y, basePoint.Z), distance);
        }
        public static Point3d GetPointDownXByDistance(Point3d basePoint, double distance)
        {
            return GetPointByDistance(basePoint, new Point3d(basePoint.X - 1, basePoint.Y, basePoint.Z), distance);
        }
        public static Point3d GetPointUpYByDistance(Point3d basePoint, double distance)
        {
            return GetPointByDistance(basePoint, new Point3d(basePoint.X, basePoint.Y + 1, basePoint.Z), distance);
        }
        public static Point3d GetPointDownYByDistance(Point3d basePoint, double distance)
        {
            return GetPointByDistance(basePoint, new Point3d(basePoint.X, basePoint.Y - 1, basePoint.Z), distance);
        }
        public static Point3d GetPointMoveByXYDistance(Point3d basePoint, double distanceX, double distanceY)
        {
            var x = basePoint.X + distanceX;
            var y = basePoint.Y + distanceY;
            var z = basePoint.Z;
            return new Point3d(x, y, z);
        }

        public static MText CreateText(string notes, Point2D notePoint,double textHeight, AttachmentPoint attachmentPoint)
        {
            return CreateText(notes,notePoint.Point2DtoPoint3d(),textHeight,attachmentPoint);
        }
        public static MText CreateText(string notes, Point3d notePoint, double textHeight, AttachmentPoint attachmentPoint)
        {
            var mText = new MText();
            mText.SetDatabaseDefaults();
            mText.Location = notePoint;
            mText.TextHeight = textHeight;
            mText.Layer = Helper.NoteInfoLayerName;
            mText.Attachment = attachmentPoint;
            var textStyle = Helper.GetTextStyleId(Helper.NoteTextStyle);
            mText.TextStyleId = textStyle;
            mText.Contents = notes;
            return mText;
        }
        public static MText CreateNoteForRect(string notes, Rectangle rect,double textHeight,double scaleRatio)
        {
            var tempNotePoint = Point2D.MidPoint(rect.BottomRight, rect.BottomLeft);
            var notePoint = GetPointUpYByDistance(tempNotePoint.Point2DtoPoint3d(),165*scaleRatio);
            return CreateText(notes, notePoint, textHeight,AttachmentPoint.MiddleCenter);
        }
        public static MText CreateStoneNote(string notes, Rectangle rect, double textHeight, double scaleRatio)
        {
            var tempNotePoint = rect.GetCenterPoint().Point2DtoPoint3d();
            return CreateText(notes,tempNotePoint,textHeight,AttachmentPoint.MiddleCenter);
        }

        public static MLeader CreateLeaded(Point3d basePoint,double leaderHeight,double textHeight, double scaleRatio, Transaction transaction,Database currentDb, string noteString,AttachmentPoint attachmentPoint,string layerName = "TP 0-7 寸法")
        {
            var arrowSize = 150 * scaleRatio;
            var leaderStylesDict = transaction.GetObject(currentDb.MLeaderStyleDictionaryId, OpenMode.ForRead) as DBDictionary;
            var blockTable = transaction.GetObject(currentDb.BlockTableId, OpenMode.ForRead) as BlockTable;
            var dotSmall = blockTable["_DOTSMALL"];
            ObjectId leaderStyleId = ObjectId.Null;
            if (leaderStylesDict.Contains("Standard"))
            {
                var leaderStyle = transaction.GetObject((ObjectId)leaderStylesDict["Standard"], OpenMode.ForWrite) as MLeaderStyle;
                if (leaderStyle != null)
                {
                    leaderStyleId = leaderStyle.Id;
                    leaderStyle.ArrowSize = 3.0;
                    leaderStyle.ArrowSymbolId = dotSmall;

                }

            }

            var leaderNote = new MLeader();
            leaderNote.SetDatabaseDefaults();
            if (leaderStyleId != ObjectId.Null)
            {
                leaderNote.MLeaderStyle = leaderStyleId;
            }

            leaderNote.ContentType = ContentType.MTextContent;

            var textNote = CreateText(noteString,basePoint,textHeight,attachmentPoint);

            leaderNote.MText = textNote;

            var footingBeamMidlePoint = GetPointDownXByDistance(textNote.Location, 330 * scaleRatio);

            var idx = leaderNote.AddLeaderLine(footingBeamMidlePoint);
            var footingBeamLeaderPoint = GetPointMoveByXYDistance(footingBeamMidlePoint, -240 * scaleRatio, -leaderHeight);

            leaderNote.AddFirstVertex(idx, footingBeamLeaderPoint);
            leaderNote.ArrowSize = arrowSize;
            leaderNote.Layer = layerName;
            leaderNote.Linetype = "ByLayer";
            leaderNote.LandingGap = 50;
            leaderNote.Scale = 1;
            return leaderNote;
        }

        public static MLeader CreatePileLeaderNote(string noteString, Point3d basePoint, double scaleRatio, Transaction transaction,Database currentDb,double textHeight,AttachmentPoint attachmentPoint = AttachmentPoint.MiddleLeft, string layerName ="TP 0-7 寸法" )
        {
            var arrowSize = 150 * scaleRatio;
            var leaderStylesDict = transaction.GetObject(currentDb.MLeaderStyleDictionaryId, OpenMode.ForRead) as DBDictionary;
            var blockTable = transaction.GetObject(currentDb.BlockTableId, OpenMode.ForRead) as BlockTable;
            var dotSmall = blockTable["_DOTSMALL"];
            ObjectId leaderStyleId = ObjectId.Null;
            if (leaderStylesDict.Contains("Standard"))
            {
                var leaderStyle = transaction.GetObject((ObjectId)leaderStylesDict["Standard"], OpenMode.ForWrite) as MLeaderStyle;
                if (leaderStyle != null)
                {
                    leaderStyleId = leaderStyle.Id;
                    leaderStyle.ArrowSize = 3.0;
                    leaderStyle.ArrowSymbolId = dotSmall;

                }

            }

            var leaderNote = new MLeader();
            leaderNote.SetDatabaseDefaults();
            if (leaderStyleId != ObjectId.Null)
            {
                leaderNote.MLeaderStyle = leaderStyleId;
            }

            leaderNote.ContentType = ContentType.MTextContent;
            var textPoint = new Point3d(basePoint.X + 580 * scaleRatio, basePoint.Y - 830 * scaleRatio, 0);
            var textNote = CreateText(noteString, textPoint, textHeight, attachmentPoint);

            leaderNote.MText = textNote;

            var footingBeamMidlePoint = GetPointDownXByDistance(textNote.Location, 330 * scaleRatio);

            var idx = leaderNote.AddLeaderLine(footingBeamMidlePoint);
            var footingBeamLeaderPoint = GetPointMoveByXYDistance(footingBeamMidlePoint, -240 * scaleRatio, 830*scaleRatio);

            leaderNote.AddFirstVertex(idx, footingBeamLeaderPoint);
            leaderNote.ArrowSize = arrowSize;
            leaderNote.Layer = layerName;
            leaderNote.Linetype = "ByLayer";
            leaderNote.LandingGap = 50;
            leaderNote.Scale = 1;
            return leaderNote;
        }

        public static List<Entity> CreatePiles(Rectangle rectangle,double scaleRatio,TNFPile pileData, out Point2D basePointPile2)
        {
            var entities = new List<Entity>();

            var baseX = rectangle.BasePoint.X;
            var firstPileX = baseX - scaleRatio * pileData.Spacing / 2;
            var secondPileX = baseX + scaleRatio * pileData.Spacing / 2;
            var pileY = 280 * scaleRatio + rectangle.BasePoint.Y;
            var base1 = new Point2D(firstPileX,pileY);
            var base2 = new Point2D(secondPileX,pileY);
            var pile1 = new PileGeometry(base1,pileData);
            var pile2 = new PileGeometry(base2,pileData);
            var pileEntity1 = CreatePile(pile1);
            var pileEntity2 = CreatePile(pile2);
            basePointPile2 = pile2.MidPoint;
            entities.AddRange(pileEntity1);
            entities.AddRange(pileEntity2);
            return entities;
        }
        public static List<Entity> CreatePile(PileGeometry pileG)
        {
            var entities = new List<Entity>();
            var plinePs1 = new List<Point2D>() { pileG.TopLeft,pileG.TopRight,pileG.BottomRight,pileG.BottomPoint,pileG.BottomLeft,pileG.TopLeft};
            var plinePs2 = new List<Point2D>() { pileG.BottomLeft, pileG.MidPoint, pileG.BottomRight };
            entities.Add(PlyLineFromPoint2D(plinePs1, PILE_LAYER_NAME));
            entities.Add(PlyLineFromPoint2D(plinePs2, PILE_LAYER_NAME));
            entities.Add(CreateLine(pileG.MidPoint, pileG.BottomPoint, PILE_LAYER_NAME));
            return entities;
        }
    }
}
