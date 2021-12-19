using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Enums;
using TNFData.Interface;

namespace TNFData.Models
{
    /// <summary>
    /// Móng
    /// </summary>
    public class TNFFoundationParams : BindableBase, IReactionTNFParameterChanged
    {
        #region Fields
        private TNFFoundationTypes _foundationType;
        private int _topDepth;
        private int _bottomDepth;
        private int _offsetWithGL;
        private int _bottomWidth;
        private string _name;

        #endregion
        #region Properties
        /// <summary>
        /// Type of Foundation DType or normal type
        /// </summary>
        public TNFFoundationTypes FoundationType
        {
            get => _foundationType;
            set
            {
                SetProperty(ref _foundationType, value);
                this.ChangeDimensionForFoundation();
                RaisePropertyChanged(nameof(XConcreateThickness));
                RaisePropertyChanged(nameof(ZConcreateThickness));
                RaisePropertyChanged(nameof(IsDepthEditable));
                RaisePropertyChanged(nameof(IsBottomWidthEditable));
                RaisePropertyChanged(nameof(IsCheckFoundationParams));
            }
        }
        /// <summary>
        /// Name of Foundation
        /// </summary>
        public string Name { get=>_name; set=>SetProperty(ref _name,value); }

        public string Description
        {
            get {
                if (string.IsNullOrEmpty(Name))
                {
                    return "独立基礎";
                }
                else
                {
                    return $"独立基礎({Name})";

                }
            }
        }
        /// <summary>
        /// Kích thước của móng theo trục XYZ
        /// </summary>
        public Dimension3d FDimension { get; set; }

        /// <summary>
        /// Chiều sâu trên của móng D
        /// </summary>
        public int TopDepth
        {
            get => _topDepth; set
            {
                SetProperty(ref _topDepth, value);
                SetDepthDimention();
            }
        }

        /// <summary>
        /// Chiều sâu dưới của móng D
        /// </summary>
        public int BottomDepth
        {
            get => _bottomDepth;
            set
            {
                SetProperty(ref _bottomDepth, value);
                SetDepthDimention();
            }
        }
        /// <summary>
        /// Kích thước đáy của móng D
        /// </summary>
        public int BottomWidth
        {
            get => _bottomWidth; set
            {
                SetProperty(ref _bottomWidth, value);
                RaisePropertyChanged(nameof(IsCheckFoundationParams));
            }
        }

        /// <summary>
        /// Độ dày mở 2 bên của lớp bê tông lót
        /// Yobori width
        /// 基礎_余掘り
        /// </summary>
        public int XConcreateThickness => FoundationType == TNFFoundationTypes.DType ? 50 : 0;

        /// <summary>
        /// Độ dày của lớp bê tông lót cho móng
        /// Với móng thường chỉ cần thông số độ dày theo phương Z
        /// Đối với mô hình 3D thuộc tính này chỉ hướng theo phương Z
        /// Đối với mô hình 2D section, thuộc tính này chỉ hướng treo phương Y
        /// Đây chính thà thông số LearnConcreateThickness trong Revit
        /// Nếu móng thường thì bằng 30, nếu móng D thì bằng 100
        /// </summary>
        public int ZConcreateThickness => FoundationType == TNFFoundationTypes.DType ? 100 : 30;

        /// <summary>
        /// HakamaRebar Diameter
        /// </summary>
        public RebarDiameter HakamaRebar { get; set; }

        /// <summary>
        /// Besu Rebar Diameter
        /// </summary>
        public RebarDiameter BesuRebar { get; set; }

        /// <summary>
        /// Kích thước của cọc trong cột
        /// </summary>
        public Dimension3d HashiraGataDimension { get; set; }
        /// <summary>
        /// 24-D25
        /// 24 là số lượng, 25 là bán kính rebar
        /// </summary>
        public RebarDiameter HashiraGataMainRebar { get; set; }

        /// <summary>
        /// thông số kỹ thuật rebar cho cột hashiragata
        /// </summary>
        public RebarDiameter HashiraGataHoopRebar { get; set; }

        /// <summary>
        /// offset của cột với GL
        /// </summary>
        public int HashiraGataOffsetWithGL { get; set; }

        /// <summary>
        /// Khoảng cách từ mặt trên của móng so với đáy sàn.
        /// Đối với mặt cắt móng của báo giá sơ bộ, GL được hiểu ở đây là cao độ của dáy sàn
        /// Thể hiện vị trí tương đối của móng so với mặt bằng GL
        /// Thông số này nhóm báo giá gọi tên là độ dày mạch ngừng
        /// </summary>
        public int OffsetWithGL { get => _offsetWithGL; set => SetProperty(ref _offsetWithGL, value); }

        public bool IsCheckFoundationParams => CheckFoundationParams();
        public TNFGlobalInfo TNFParams { get; }
        #endregion

        #region Properties cho việc hiển thị dữ liệu
        public bool IsDepthEditable
        {
            get
            {
                switch (FoundationType)
                {
                    case TNFFoundationTypes.DType:
                        return false;
                    case TNFFoundationTypes.NormalType:
                        return true;
                    default:
                        return false;
                }
            }
        }
        public bool IsBottomWidthEditable
        {
            get
            {
                switch (FoundationType)
                {
                    case TNFFoundationTypes.DType:
                        return true;
                    case TNFFoundationTypes.NormalType:
                        return false;
                    default:
                        return false;
                }
            }
        }
        #endregion
        #region Constructor

        public TNFFoundationParams(TNFGlobalInfo tnfInfo)
        {
            TNFParams = tnfInfo;
            FDimension = new Dimension3d() { XLength = 3400, YLength = 3400 };
            _topDepth = 900;
            BottomDepth = 0;
            BottomWidth = FDimension.XLength;
            HakamaRebar = new RebarDiameter("はかま", 13, 600);
            BesuRebar = new RebarDiameter("ベ-ス", 16, 100);
            HashiraGataDimension = new Dimension3d() { XLength = 1200 };
            HashiraGataMainRebar = new RebarDiameter(24, 25);
            HashiraGataHoopRebar = new RebarDiameter("", 13, 100);
            FDimension.PropertyChanged += FDimension_PropertyChanged;
            // Set some default value


        }

        private void FDimension_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(FoundationType == TNFFoundationTypes.NormalType && e.PropertyName == nameof(FDimension.XLength))
            {
                BottomWidth = FDimension.XLength;
            }
        }
        #endregion

        #region method
        private void SetDepthDimention()
        {
            this.FDimension.ZLength = TopDepth + BottomDepth;
            RaisePropertyChanged(nameof(IsCheckFoundationParams));
        }
        private void ChangeDimensionForFoundation()
        {
            if (FoundationType == TNFFoundationTypes.NormalType)
            {
                BottomWidth = FDimension.XLength;
                this.TopDepth = FDimension.ZLength;
                this.BottomDepth = 0;
            }
        }

        public void SetHashiraGataColumnHeight()
        {
            HashiraGataDimension.ZLength = TNFParams.ThirdRevonationDepth + TNFParams.StoneThickness + this.OffsetWithGL;
        }
        public void SetHashiraGataColumnHeight(int height)
        {
            HashiraGataDimension.ZLength = height;
        }

        public void ReactChanged()
        {
            SetHashiraGataColumnHeight();
        }
        private bool CheckFoundationParams()
        {
            switch (FoundationType)
            {
                case TNFFoundationTypes.DType:
                    if (TopDepth > 0 && BottomDepth > 0 && BottomWidth < this.FDimension.XLength)
                        return true;
                    return false;
                case TNFFoundationTypes.NormalType:
                    return true;
                default:
                    return false;
            }
        }
        #endregion

    }
}
