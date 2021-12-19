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
    public class TNFGlobalInfo : BindableBase
    {
        #region Fields
        private TNFSectionTypes _sectionType;
        private int _thirdRevonationDepth;
        private int _stoneThickness;
        private List<IReactionTNFParameterChanged> _reactionThirdDepthChangeds = new List<IReactionTNFParameterChanged>();
        private List<IReactionTNFParameterChanged> _reactionStoneThicknessChangeds = new List<IReactionTNFParameterChanged>();
        private int _subFoundationOffset;
        private int _crushedStoneBottomWidth;
        private int _crushedBeamBottomWidth;
        private bool _isDrawFoundationSection;
        private string _scaleOption;
        #endregion

        #region Properties

        public TNFSectionTypes SectionType { get=>_sectionType; set=>SetProperty(ref _sectionType,value); }
        /// <summary>
        /// Độ dày sàn, nếu set bằng 0 thì sẽ ko vẽ sàn trên mặt cắt
        /// </summary>
        public int FloorThickness { get; set; }

        /// <summary>
        /// Chiều sâu lớp cải tạo thứ nhất.
        /// Đối với mô hình 3D, thông số này thể hiện chiều Z.
        /// Đối với mô hình 2D, thông số này thẻ hiện chiều Y
        /// </summary>
        public int FirstRevonationDepth { get; set; }

        /// <summary>
        /// Chiều sâu lớp cải tạo thứ 2
        /// </summary>
        public int SecondRevonationDepth { get; set; }

        /// <summary>
        /// Chiều sâu lớp cải tạo thứ 3
        /// </summary>
        public int ThirdRevonationDepth {
            get=>_thirdRevonationDepth;
            set {
                this.SetProperty(ref _thirdRevonationDepth, value);
                NotifyThirdDepthChanged();
            }
        }

        /// <summary>
        /// Độ dày của lớp đá rải
        /// </summary>
        public int StoneThickness {
            get=>_stoneThickness;
            set {
                this.SetProperty(ref _stoneThickness, value);
                NotifyStoneThicknessChanged();
            }
        }

        /// <summary>
        /// Chiều rộng của hố cải tạo
        /// </summary>
        public int FirstRevonationWith { get; set; }

        /// <summary>
        /// Bề dài hố cải tạo thứ 1
        /// </summary>
        public int FirstRevonationLength { get; set; }

        /// <summary>
        /// Kich thước chiều rộng của lớp cải tạo thứ 2, dùng để vẽ trên mặt cắt 2d
        /// CHỉ mang kích thước tượng trưng
        /// </summary>
        public int SecondRevonationWidth { get; set; }

        /// <summary>
        /// Chiều rộng miệng hố V của Crushed Stone
        /// </summary>
        public int CrushedStoneVBottomWidth { get=>_crushedStoneBottomWidth; set=>SetProperty( ref _crushedStoneBottomWidth,value); }

        /// <summary>
        /// chiều rộng Beam cho section dạng crushed Stone
        /// </summary>
        public int CrushedBeamBottomWidth { get => _crushedBeamBottomWidth; set => SetProperty(ref _crushedBeamBottomWidth, value); }

        /// <summary>
        /// Kích thước độ mở cho miệng hố
        /// </summary>
        public int CrushedStoneOpenHolePitchDistance { get; set; }

        /// <summary>
        /// Chiều cao của dầm dùng cho trường hợp mặt cắt dạng nornal
        /// </summary>
        public int CrushedBeamDepth => StoneThickness + SubFoundationOffset;

        /// <summary>
        /// offset with GL distance subFoundation
        /// Changed this for Crushet Stone Type Section and Platform Section and Crushed Stone and platform
        /// </summary>
        public int SubFoundationOffset {
            get=>_subFoundationOffset;
            set {
                    SetProperty(ref _subFoundationOffset,value);
                RaisePropertyChanged(nameof(CrushedBeamDepth));
            }
        }

        /// <summary>
        /// Có vẽ section cho mặt cắt dầm không
        /// </summary>
        public bool IsDrawBeamSection { get; set; }

        /// <summary>
        /// Có vẽ section cho mặt cắt Beam ko
        /// </summary>
        public bool IsDrawFoundationSection { get=>_isDrawFoundationSection; set=>SetProperty(ref _isDrawFoundationSection,value); }

        /// <summary>
        /// Có vẽ cọc cho lớp cải tạo 1 không
        /// </summary>
        public bool IsFirtRevoPile { get; set; }

        /// <summary>
        /// Có vẽ cọc cho lớp cải tạo 2 không
        /// </summary>
        public bool IsSecondRevoPile { get; set; }

        /// <summary>
        /// Có vẽ sàn ko, biến này chỉ để kiếm tra có cần vẽ sàn hay ko và được tự động get từ độ dày của sàn
        /// </summary>
        public bool IsDrawFloor { get=>this.FloorThickness==0?false:true; }

        /// <summary>
        /// Đây là khoảng cách giữa các mặt cắt, để mặc định là 1000
        /// </summary>
        public const int SECTION_DISTANCE = 1000;

        /// <summary>
        /// Dùng để set Scale Option cho bản vẽ mặt cắt
        /// </summary>
        public string ScaleOption { get => _scaleOption; set => SetProperty(ref _scaleOption, value); }

        /// <summary>
        /// TÍnh toán tỉ lệ vẽ từ ScaleOption
        /// </summary>
        public double ScaleRatio => CalculatorScaleRatioFromString(ScaleOption);

        public RebarDiameter FloorRebar { get; private set; }
        public TNFPile TNFPile { get;private set;  }
        #endregion

        #region Properties cho việc hiển thị dữ liệu
        public bool IsStoneThicknessEditable
        {
            get
            {
                switch (SectionType)
                {
                    case TNFSectionTypes.NormalType:
                    case TNFSectionTypes.PlatformType:
                        return false;
                    case TNFSectionTypes.CrushedStone:
                    case TNFSectionTypes.CrushedStoneAndPlatform:
                        return true;
                    default:
                        return false;
                }
            }
        }
        public bool IsThirdDepthEditable
        {
            get
            {
                switch (SectionType)
                {
                    case TNFSectionTypes.NormalType:
                    case TNFSectionTypes.CrushedStone:
                        return false;
                    case TNFSectionTypes.PlatformType:
                    case TNFSectionTypes.CrushedStoneAndPlatform:
                        return true;
                    default:
                        return false;
                }
            }
        }
        public bool IsCheckParamsForSectionType { get => CheckingGeometryParams(); }
        #endregion
        public TNFGlobalInfo()
        {
            PropertyChanged += TNFGlobalInfo_PropertyChanged;
            ScaleOption = "1/1";
            FloorRebar = new RebarDiameter("Floor", 13, 200) { SubfixSymbol = "W"};
            TNFPile = new TNFPile(this) { Radius = 150,Length = 6,Spacing = 900};
        }

        private void TNFGlobalInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SectionType))
            {
                switch (SectionType)
                {
                    case TNFSectionTypes.NormalType:
                        this.ThirdRevonationDepth = 0;
                        break;
                    case TNFSectionTypes.PlatformType:
                        this.StoneThickness = 0;
                        break;
                    case TNFSectionTypes.CrushedStone:
                        this.ThirdRevonationDepth= 0;
                        AutoSetCrushedVBottomWidth();
                        break;
                    case TNFSectionTypes.CrushedStoneAndPlatform:
                        CrushedStoneVBottomWidth = 345;
                        CrushedStoneOpenHolePitchDistance = 205;
                        break;
                    default:
                        break;
                }
                RaisePropertyChanged(nameof(IsStoneThicknessEditable));
                RaisePropertyChanged(nameof(IsThirdDepthEditable));
                RaisePropertyChanged(nameof(IsCheckParamsForSectionType));
            }
            if(e.PropertyName == nameof(ScaleOption))
            {
                RaisePropertyChanged(nameof(ScaleRatio));
            }
            if(e.PropertyName == nameof(StoneThickness)|| e.PropertyName == nameof(ThirdRevonationDepth)){
                RaisePropertyChanged(nameof(IsCheckParamsForSectionType));
            }
        }
        private void AutoSetCrushedVBottomWidth()
        {
            CrushedStoneVBottomWidth = CrushedBeamBottomWidth * 2;
            CrushedStoneOpenHolePitchDistance = 300;
        }

        public void RegisterReactionThirdDepthChanged(IReactionTNFParameterChanged reactioner)
        {
            if (_reactionThirdDepthChangeds == null)
            {
                _reactionThirdDepthChangeds = new List<IReactionTNFParameterChanged>();
            }
            if (!_reactionThirdDepthChangeds.Contains(reactioner))
            {
                _reactionThirdDepthChangeds.Add(reactioner);
            }
        }
        public void RegisterReactionStoneThicknessChanged(IReactionTNFParameterChanged reactioner)
        {
            if (_reactionStoneThicknessChangeds == null)
            {
                _reactionStoneThicknessChangeds = new List<IReactionTNFParameterChanged>();
            }
            if (!_reactionStoneThicknessChangeds.Contains(reactioner))
            {
                _reactionStoneThicknessChangeds.Add(reactioner);
            }
        }
        private void NotifyThirdDepthChanged()
        {
            foreach(var thirdDepthChangedListener in _reactionThirdDepthChangeds)
            {
                thirdDepthChangedListener.ReactChanged();
            }
        }
        private void NotifyStoneThicknessChanged()
        {
            foreach (var stoneThicknessListener in _reactionStoneThicknessChangeds)
            {
                stoneThicknessListener.ReactChanged();
            }
        }
        private double CalculatorScaleRatioFromString(string scaleString)
        {
            var scaleRatio = 1.0;
            if (scaleString.Contains("/")){
                var scaleArr = scaleString.Split('/');
                if (scaleArr.Length < 2 || scaleArr[1] == "0")
                    return 1.0;
                scaleRatio = Double.Parse(scaleArr[0]) / Double.Parse(scaleArr[1]);
            }
            else
            {
                scaleRatio = Double.Parse(scaleString);
            }
            return scaleRatio;

        }
        private bool CheckingGeometryParams()
        {
            switch (SectionType)
            {
                case TNFSectionTypes.NormalType:
                    return true;
                case TNFSectionTypes.PlatformType:
                    return ThirdRevonationDepth > 0 ? true : false;
                case TNFSectionTypes.CrushedStone:
                    return StoneThickness > 0?true:false;
                case TNFSectionTypes.CrushedStoneAndPlatform:
                    return StoneThickness>0 && ThirdRevonationDepth>0 ? true:false;
                default:
                    return false;
            }
        }

    }
}
