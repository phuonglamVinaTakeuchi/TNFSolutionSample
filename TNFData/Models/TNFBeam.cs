using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Interface;

namespace TNFData.Models
{
    /// <summary>
    /// Dầm móng
    /// </summary>
    public class TNFBeam: BindableBase
    {
        #region Properties
        private int _offsetWithGL;
        private int _zConcreateThickness;
        private int _beamBaseDepth;
        private int _beamUpDistance;
        private int _beamWidth;

        /// <summary>
        /// Chiều sâu của Beam
        /// ở đây được hiểu là chiều sâu của hố đào cho Beam tính từ miệng hố đến phần offset của beam so với mặt sàn.
        /// </summary>
        public int BeamDepth => ZConcreateThickness + BaseBeamDepth + BeamUpDistance + OffsetWithGL;

        /// <summary>
        /// Chiều sâu cơ bản của Beam
        /// Với normal Type Section Giá trị này bằng 350
        /// Với dạng nâng lên giá trị này bằng 500
        /// </summary>
        public int BaseBeamDepth { get=>_beamBaseDepth; set=>SetProperty(ref _beamBaseDepth,value); }

        /// <summary>
        /// Giá trị này được tính toán dựa trên giá trị của ThirdRevonation
        /// </summary>
        public int BeamUpDistance { get=>_beamUpDistance; set=>SetProperty(ref _beamUpDistance,value); }

        /// <summary>
        /// CHiều rộng của Beam
        /// Thông số này có thể lấy từ thuộc tính của Family Beam trên Revit
        /// </summary>
        public int BeamWidth { get=>_beamWidth; set=>SetProperty(ref _beamWidth,value); }
        /// <summary>
        /// Thông số bổ sung để xác định độ mở của beam
        /// </summary>
        public int OpenPitchDistance { get; set; }

        /// <summary>
        /// Độ dày của lớp bê tông lót cho
        /// Đối với mô hình 3D thuộc tính này chỉ hướng theo phương Z
        /// Đối với mô hình 2D section, thuộc tính này chỉ hướng treo phương Y
        /// Với móng thường, thông số này luôn bằng độ dày bê tông lót của móng.
        /// Với móng D, thông số này sẽ ko giống với độ dày của lớp bê tông lót
        /// </summary>
        public int ZConcreateThickness { get=>_zConcreateThickness; set=>SetProperty(ref _zConcreateThickness,value); }
        /// <summary>
        /// Khoảng cách từ mặt trên của Beam so với đáy sàn.
        /// Đối với mặt Beam của báo giá sơ bộ, GL được hiểu ở đây là cao độ của dáy sàn
        /// Thể hiện vị trí tương đối của móng so với mặt bằng GL
        /// </summary>
        public int OffsetWithGL { get=>_offsetWithGL; set=>SetProperty(ref _offsetWithGL,value); }

        public TNFGlobalInfo TNFParams { get; private set; }

        #endregion
        public TNFBeam(TNFGlobalInfo tnfParams)
        {
            TNFParams = tnfParams;
            ZConcreateThickness = 30;
            PropertyChanged += TNFBeam_PropertyChanged;
        }

        private void TNFBeam_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ZConcreateThickness):
                case nameof(BaseBeamDepth):
                case nameof(BeamUpDistance):
                case nameof(OffsetWithGL):
                    RaisePropertyChanged(nameof(BeamDepth));
                    break;
            }
        }
    }
}
