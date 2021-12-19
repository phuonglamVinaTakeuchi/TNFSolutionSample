using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BaseGeometry;
using TNFData.Models;

namespace TNFData.Geometry.FoundationGeometry
{
    public class NormalTypeG : FoundationGBase
    {

        #region Properties

        #endregion
        public NormalTypeG(Point2D basePoint, TNFFoundationParams tnfFoundation): base(basePoint,tnfFoundation)
        {
            InitGeometry();
        }
        protected override void InitGeometry()
        {
            base.InitGeometry();
            BottomFoundationPoints.Clear();
            OffsetPoints.Clear();
            FoundationPoints.Clear();

            FoundationPoints.Add(TopLeft);
            FoundationPoints.Add(ConcreateBottomLeft);
            FoundationPoints.Add(ConcreateBottomRight);
            FoundationPoints.Add(TopRight);

            BottomFoundationPoints.Add(BottomLeft);
            BottomFoundationPoints.Add(BottomRight);

            if (OffsetPointLeft != null && OffsetPointRight != null)
            {
                OffsetPoints.Add(OffsetPointLeft);
                OffsetPoints.Add(OffsetPointRight);
            }
        }

    }
}
