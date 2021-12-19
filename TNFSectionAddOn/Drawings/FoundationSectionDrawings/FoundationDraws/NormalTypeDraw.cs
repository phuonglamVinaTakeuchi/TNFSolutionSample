using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.FoundationGeometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using devDept.Geometry;

namespace TNFSectionAddOn.Drawings.FoundationSectionDrawings.FoundationDraws
{
    public class NormalTypeDraw : FGeometryDrawBase
    {
        public NormalTypeDraw(FoundationGBase foundation) : base(foundation)
        {

        }

        protected override void CreateDimension(List<Entity> entities,Transaction transaction, Database acCurrentDB)
        {
            var tnfInfo = this.FoundationG.Foundation.TNFParams;
            var dist = 460 * tnfInfo.ScaleRatio;
            var startPoint = this.FoundationG.ConcreateBottomLeft;
            var endPoint = this.FoundationG.ConcreateBottomRight;


            var foundWidthdim = Helper.CreateDimension(startPoint, endPoint,dist,Helper.GetPointDownYByDistance, tnfInfo.ScaleRatio,transaction,acCurrentDB);
            entities.Add(foundWidthdim);

            dist = 370 * tnfInfo.ScaleRatio;

            GetNewPointDelegate getPoint = null;
            if (IsSubDim)
            {
                if (FoundationG.OffsetPointRight != null)
                    startPoint = this.FoundationG.OffsetPointLeft;
                else
                    startPoint = this.FoundationG.TopLeft;

                getPoint = Helper.GetPointDownXByDistance;
                endPoint = this.FoundationG.BottomLeft;

            }
            else
            {
                if (FoundationG.OffsetPointRight != null)
                    startPoint = this.FoundationG.OffsetPointRight;
                else
                    startPoint = this.FoundationG.TopRight;
                getPoint = Helper.GetPointUpXByDistance;
                endPoint = this.FoundationG.BottomRight;
            }

            var foundationHeightDim = Helper.CreateDimension(startPoint, endPoint, dist, getPoint, tnfInfo.ScaleRatio, transaction, acCurrentDB);
            entities.Add(foundationHeightDim);


            // Do cao cua Foundation
            if (!IsSubDim)
            {
                startPoint = this.FoundationG.TopLeft;
                endPoint = this.FoundationG.ConcreateBottomLeft;
                var foundationTotalHeight = Helper.CreateDimension(startPoint, endPoint, dist, Helper.GetPointDownXByDistance, tnfInfo.ScaleRatio, transaction, acCurrentDB);
                entities.Add(foundationTotalHeight);
            }



            if (FoundationG.OffsetPointRight != null)
            {
                Point3d dimPoint;
                Point3d dPoint;
                if (IsSubDim)
                {
                    startPoint = this.FoundationG.TopLeft;
                    endPoint = this.FoundationG.OffsetPointLeft;
                    dimPoint = Point2D.MidPoint(startPoint,endPoint).Point2DtoPoint3d();
                    dPoint = getPoint(dimPoint, dist);
                }
                else
                {
                    startPoint = this.FoundationG.TopRight;
                    endPoint = this.FoundationG.OffsetPointRight;
                    dimPoint = Helper.GetPointUpYByDistance(startPoint.Point2DtoPoint3d(), Helper.DIMPOINT_UP_Y_DISTANCE * tnfInfo.ScaleRatio);
                    dPoint = getPoint(dimPoint, dist);
                }

                var offsetDim = Helper.CreateDimension(startPoint,endPoint,dPoint,tnfInfo.ScaleRatio, transaction, acCurrentDB);
                entities.Add(offsetDim);
            }
            {
                Point3d dimPoint;
                Point3d dPoint;
                if (IsSubDim)
                {
                    startPoint = FoundationG.BottomLeft;
                    endPoint = FoundationG.ConcreateBottomLeft;
                }
                else
                {
                    startPoint = FoundationG.BottomRight;
                    endPoint = FoundationG.ConcreateBottomRight;
                }

                {
                    dimPoint = Helper.GetPointDownYByDistance(endPoint.Point2DtoPoint3d(), Helper.DIMPOINT_DOWN_Y_DISTANCE * tnfInfo.ScaleRatio);
                    dPoint = getPoint(dimPoint, dist);
                    var xConcreateDim = Helper.CreateDimension(startPoint, endPoint, dPoint, tnfInfo.ScaleRatio, transaction, acCurrentDB);

                    entities.Add(xConcreateDim);

                }

            }


        }
    }
}
