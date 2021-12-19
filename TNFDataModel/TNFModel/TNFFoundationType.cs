using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.Enums;
using TNFDataModel.TNFDataAttribute;
using TNFDataModel.TNFModel;

namespace TNFDataModel.TNFModel
{
   [TypeData("Foundation Type",typeof(TNFFoundationType))]
   public class TNFFoundationType : TNFBaseModel
    {
        ///// <summary>
        ///// Type của móng
        ///// </summary>
        public TNFFoundationTypes FoundationType { get; set; }


        [PropertyData("Id",typeof(string),0)]
        public string ID { get; set; }
        /// <summary>
        /// Foundation type mark
        /// </summary>
        [TNFRevitFamilyName("Type Mark")]
        [PropertyData("TypeMark",typeof(string),1)]
        public string TypeMark { get; set; }


        /// <summary>
        /// Kích thước của móng theo trục XYZ
        /// </summary>
        [TNFRevitFamilyName("",true)]
        [PropertyData("FDimension",typeof(Dimension3d),new int[] {3,4,5})]
        public Dimension3d FDimension { get; private set; }

        [TNFRevitFamilyName("基礎_厚さ_上")]
        [PropertyData("TopDepth",typeof(int),6)]
        /// <summary>
        /// Chiều sâu trên của móng D
        /// </summary>
        public int TopDepth
        {
            get  ; set;

        }

        /// <summary>
        /// Chiều sâu dưới của móng D
        /// </summary>
        public int BottomDepth
        {
            get ;
            set;

        }
        /// <summary>
        /// Kích thước đáy của móng D
        /// </summary>
        public int BottomWidth
        {
            get ; set;

        }
        /// <summary>
        /// HakamaRebar Diameter
        /// </summary>
        [PropertyData("HakamaRebar X",typeof(RebarDiameter),new int[] {7,8})]
        public RebarDiameter HakamaRebarX { get; set; }
        /// <summary>
        /// HakamaRebar Diameter
        /// </summary>
        [PropertyData("HakamaRebar Y", typeof(RebarDiameter), new int[] { 9, 10 })]
        public RebarDiameter HakamaRebarY { get; set; }

        [TNFRevitFamilyName("鉄筋_ベース_直径")]
        public string HakamaRebarSize
        {
            get
            {
                if (HakamaRebarX == null) return "";
                else return HakamaRebarX.RebarSizeName;
            }
        }
        [TNFRevitFamilyName("鉄筋_ベース_距離_最大")]
        public int HakamaRebarSpacing
        {
            get
            {
                if (HakamaRebarX == null) return 0;
                else return HakamaRebarX.Spacing;
            }
        }
        /// <summary>
        /// Besu Rebar Diameter
        /// </summary>
        //public RebarDiameter BesuRebar { get; set; }

        /// <summary>
        /// Stirrup Rebar diameter
        /// </summary>
        //public RebarDiameter StirrupRebar { get; set; }

        /// <summary>
        /// Kích thước của cọc trong cột
        /// </summary>
        //public Dimension3d HashiraGataDimension { get; set; }
        /// <summary>
        /// 24-D25
        /// 24 là số lượng, 25 là bán kính rebar
        /// </summary>
        //public RebarDiameter HashiraGataMainRebar { get; set; }

        /// <summary>
        /// thông số kỹ thuật rebar cho cột hashiragata
        /// </summary>
        //public RebarDiameter HashiraGataHoopRebar { get; set; }

        /// <summary>
        /// Location of HashiraGata
        /// </summary>
        //public Dimension2d HashiraGataLocation { get; private set; }

        [TNFRevitFamilyName("柱型_オフセット_X方向")]
        public int HashiraGataLocationX { get; set; }
        [TNFRevitFamilyName("柱型_オフセット_Y方向")]
        public int HashiraGataLocationY { get; set; }

        [TNFRevitFamilyName("柱型_Lx")]
        public int HashiraGataLx { get; set; }
        [TNFRevitFamilyName("柱型_Ly")]
        public int HashiraGataLy { get; set; }
        //public Dimension2d ColumnDimension { get; private set; }


        public TNFFoundationType(string[] sourceContent, string blockName) : base(sourceContent, blockName)
        {
            InitHakamaSpacing();
            InitBottopDepth();
            InitFoundationType();
        }
        private void InitHakamaSpacing()
        {
            if (FDimension == null  )
            {
                return;
            }
            else
            {
                if(HakamaRebarX != null)
                {
                    var tempSpacing = FDimension.Lx / (HakamaRebarX.Quantities - 1);
                    var spacing = (tempSpacing / 100) * 100;
                    HakamaRebarX.Spacing = spacing;
                }
                if(HakamaRebarY != null)
                {
                    var tempSpacing = FDimension.Ly / (HakamaRebarY.Quantities - 1);
                    var spacing = (tempSpacing / 100) * 100;
                    HakamaRebarY.Spacing = spacing;
                }
            }


        }
        private void InitBottopDepth()
        {
            if (FDimension != null)
            {
                BottomDepth = FDimension.Lz - TopDepth;
            }
        }
        private void InitFoundationType()
        {
            if (BottomDepth == 0)
            {
                FoundationType = TNFFoundationTypes.DType;
            }
            else
            {
                FoundationType = TNFFoundationTypes.NormalType;
            }
        }
        public int GetHakamaSpacing()
        {
            if (HakamaRebarX == null)
            {
                return 100;
            }
            return HakamaRebarX.Spacing;

        }

    }
}
