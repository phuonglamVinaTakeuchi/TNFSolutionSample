using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Enums;

namespace TNFData.Models.Section
{
    /// <summary>
    /// TNF Solution Section
    /// </summary>
    public class TNFSection : BindableBase
    {
        #region Properties

        public TNFGlobalInfo TNFParameters { get; set; }

        /// <summary>
        /// Thông số kỹ thuật cho Footing Beam (dầm)
        /// </summary>
        public TNFFootingBeamParams FootingBeamSectionParams { get; set; }
        /// <summary>
        /// Thông số kỹ thuật cho móng
        /// </summary>
        public TNFFoundationSectionParams FoundationSectionParams { get; set; }

        public bool IsProcessDrawing => TNFParameters.IsCheckParamsForSectionType && FoundationSectionParams.IsCheckFoundationParams;
        #endregion
        #region Contruction
        public TNFSection()
        {
            TNFParameters = new TNFGlobalInfo() {
                // Set some default value for this
                FloorThickness = 150,
                FirstRevonationDepth = 1500,
                SecondRevonationDepth = 1000,
                FirstRevonationWith = 4800,
                FirstRevonationLength = 2400,
                SecondRevonationWidth = 1665,
                IsDrawBeamSection = true,
                IsDrawFoundationSection = true,
                IsFirtRevoPile = false,
                IsSecondRevoPile = false,
                SubFoundationOffset = 0,
                CrushedBeamBottomWidth = 300
            };
            FootingBeamSectionParams = new TNFFootingBeamParams(TNFParameters);
            FoundationSectionParams = new TNFFoundationSectionParams(TNFParameters);
            TNFParameters.PropertyChanged += TNFParameters_PropertyChanged;
            FoundationSectionParams.PropertyChanged += FoundationSectionParams_PropertyChanged; ;

        }

        private void FoundationSectionParams_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TNFFoundationSectionParams.IsCheckFoundationParams))
            {
                RaisePropertyChanged(nameof(IsProcessDrawing));
            }
        }

        private void TNFParameters_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(TNFParameters.IsCheckParamsForSectionType))
            {
                RaisePropertyChanged(nameof(IsProcessDrawing));
            }
        }



        #endregion
        #region Event Trigger Method


        #endregion

    }
}
