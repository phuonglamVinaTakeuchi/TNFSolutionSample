using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Interface;

namespace TNFData.Models.Section
{
    public class TNFFoundationSectionParams : BindableBase,IReactionTNFParameterChanged
    {
        #region Properties
        public TNFGlobalInfo TNFParameters { get; set; }

        /// <summary>
        /// Móng cải tạo
        /// </summary>
        public TNFFoundationParams MainFoundation { get; set; }

        /// <summary>
        /// các thông số kỹ thuật móng dành cho Dạng Platform
        /// </summary>
        public TNFFoundationParams SubFoundation { get; set; }
        public TNFBeam CrushedBeamData { get; set; }

        public bool IsCheckFoundationParams => MainFoundation.IsCheckFoundationParams;

        #endregion
        #region construction
        public TNFFoundationSectionParams(TNFGlobalInfo tnfParams)
        {
            TNFParameters = tnfParams;
            MainFoundation = new TNFFoundationParams(TNFParameters) {
                Name = "F2",
                HashiraGataOffsetWithGL = 100,
                OffsetWithGL = 100
        };
            SubFoundation = new TNFFoundationParams(TNFParameters) {
                Name = "F1",
                FoundationType = Enums.TNFFoundationTypes.NormalType,
                HashiraGataOffsetWithGL = 100,
                OffsetWithGL = 200
            };
            TNFParameters.RegisterReactionStoneThicknessChanged(SubFoundation);
            TNFParameters.RegisterReactionThirdDepthChanged(SubFoundation) ;
            TNFParameters.PropertyChanged += TNFParameters_PropertyChanged;
            SubFoundation.PropertyChanged += SubFoundation_PropertyChanged;
            CrushedBeamData = new TNFBeam(tnfParams) {
                ZConcreateThickness = 30,
                BeamWidth = 300,
                OpenPitchDistance=300,
                BaseBeamDepth=0,
                OffsetWithGL=0,
            };

            MainFoundation.PropertyChanged += MainFoundation_PropertyChanged;
            TNFParameters.RegisterReactionStoneThicknessChanged(this);
        }

        private void MainFoundation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TNFFoundationParams.IsCheckFoundationParams))
            {
                RaisePropertyChanged(nameof(IsCheckFoundationParams));
            }
        }

        private void TNFParameters_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(TNFParameters.SubFoundationOffset))
            {
                SubFoundation.OffsetWithGL = TNFParameters.SubFoundationOffset;
            }
        }

        private void SubFoundation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TNFFoundationParams.OffsetWithGL):
                    SubFoundation.SetHashiraGataColumnHeight();
                    ReactChanged();
                    break;
                default:
                    break;
            }
        }

        public void ReactChanged()
        {
            CrushedBeamData.BeamUpDistance = (TNFParameters.ThirdRevonationDepth + TNFParameters.StoneThickness) - CrushedBeamData.OffsetWithGL;
            CrushedBeamData.BaseBeamDepth = this.SubFoundation.OffsetWithGL - CrushedBeamData.ZConcreateThickness;
        }


        #endregion
    }
}
