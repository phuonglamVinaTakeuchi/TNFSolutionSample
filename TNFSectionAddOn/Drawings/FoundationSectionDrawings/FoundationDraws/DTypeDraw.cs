using Autodesk.AutoCAD.DatabaseServices;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.FoundationGeometry;

namespace TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationDraws
{
    public class DTypeDraw : FGeometryDrawBase
    {
        public DTypeDraw(FoundationGBase foundation) : base(foundation)
        {
        }

        protected override void CreateDimension(List<Entity> entities, Transaction transaction, Database acCurrentDB)
        {
            var tnfInfo = this.FoundationG.Foundation.TNFParams;
            var dType = FoundationG as DTypeG;
            var dist = 640 * tnfInfo.ScaleRatio;

            var startPoint = new Point2D(this.FoundationG.TopLeft.X,FoundationG.ConcreateBottomLeft.Y);
            var endPoint = new Point2D(FoundationG.TopRight.X,FoundationG.ConcreateBottomLeft.Y);
            var foundWidthdim = Helper.CreateDimension(startPoint, endPoint, dist, Helper.GetPointDownYByDistance, tnfInfo.ScaleRatio, transaction, acCurrentDB);
            entities.Add(foundWidthdim);

            dist = 330 * tnfInfo.ScaleRatio;

            var dimEndP = FoundationG.ConcreateBottomLeft;
            var dimStartP = startPoint;
            var leftFoundationDim = Helper.CreateDimension(dimStartP, dimEndP,dist,Helper.GetPointDownYByDistance,tnfInfo.ScaleRatio,transaction, acCurrentDB);
            entities.Add(leftFoundationDim);

            dimStartP = FoundationG.ConcreateBottomLeft;
            dimEndP = FoundationG.ConcreateBottomRight;
            var midFoundatioDim = Helper.CreateDimension(dimStartP, dimEndP, dist, Helper.GetPointDownYByDistance, tnfInfo.ScaleRatio, transaction, acCurrentDB);
            entities.Add(midFoundatioDim);

            dimStartP = FoundationG.ConcreateBottomRight;
            var rightFoundatioDim = Helper.CreateDimension(dimStartP, endPoint, dist, Helper.GetPointDownYByDistance, tnfInfo.ScaleRatio, transaction, acCurrentDB);
            entities.Add(rightFoundatioDim);




            dist = 295 * tnfInfo.ScaleRatio;
            if (FoundationG.OffsetPointRight != null)
                dimStartP = this.FoundationG.OffsetPointRight;
            else
                dimStartP = this.FoundationG.TopRight;

            dimEndP = dType.DTypeBottomRight;
            var foundationHeightDim = Helper.CreateDimension(dimStartP, dimEndP, dist, Helper.GetPointUpXByDistance, tnfInfo.ScaleRatio, transaction, acCurrentDB);
            entities.Add(foundationHeightDim);

            dimStartP = dType.DTypeBottomRight;
            var endX = dType.TopRight.X;
            var endY = dType.BottomLeft.Y;
            dimEndP = new Point2D(endX,endY);
            var bottomDim = Helper.CreateDimension(dimStartP, dimEndP, dist, Helper.GetPointUpXByDistance,tnfInfo.ScaleRatio, transaction, acCurrentDB);
            entities.Add(bottomDim);



            dimStartP = this.FoundationG.TopLeft;
            dimEndP = startPoint ;
            var foundationTotalHeight = Helper.CreateDimension(dimStartP, dimEndP, dist, Helper.GetPointDownXByDistance, tnfInfo.ScaleRatio, transaction, acCurrentDB);
            entities.Add(foundationTotalHeight);

            dist = 585 * tnfInfo.ScaleRatio;

            if (FoundationG.OffsetPointRight != null)
            {
                dimStartP = this.FoundationG.TopRight;
                dimEndP = this.FoundationG.OffsetPointRight;
                var dimPoint = Helper.GetPointUpYByDistance(dimStartP.Point2DtoPoint3d(), Helper.DIMPOINT_UP_Y_DISTANCE * tnfInfo.ScaleRatio);
                var dPoint = Helper.GetPointUpXByDistance(dimPoint, dist);
                var offsetDim = Helper.CreateDimension(dimStartP, dimEndP, dPoint, tnfInfo.ScaleRatio, transaction, acCurrentDB);
                entities.Add(offsetDim);
            }

            dimStartP = endPoint;
            dimEndP = new Point2D(endX, endY);
            {
                var dimPoint = Helper.GetPointDownYByDistance(endPoint.Point2DtoPoint3d(), Helper.DIMPOINT_DOWN_Y_DISTANCE * tnfInfo.ScaleRatio);
                var dPoint = Helper.GetPointUpXByDistance(dimPoint, dist);
                var xConcreateDim = Helper.CreateDimension(dimStartP, dimEndP, dPoint, tnfInfo.ScaleRatio, transaction, acCurrentDB);
                entities.Add(xConcreateDim);

            }

            if (dType.OffsetPointRight != null)
            {
                dimStartP = dType.OffsetPointRight;
            }
            else
                dimStartP = dType.TopRight;
            endPoint = new Point2D(endX, endY);
            var fHeightDim = Helper.CreateDimension(dimStartP,endPoint,dist,Helper.GetPointUpXByDistance,tnfInfo.ScaleRatio, transaction, acCurrentDB);
            entities.Add(fHeightDim);
        }
    }
}
