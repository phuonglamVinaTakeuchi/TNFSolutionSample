using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Enums;

namespace TNFData.Models.Section
{
    public class TNFFootingBeamParams
    {
        #region Properties

        public TNFGlobalInfo TNFParameters { get; set; }

        /// <summary>
        /// Wall Dimenstion
        /// </summary>
        public TNFWall Wall { get; set; }

        /// <summary>
        /// Dầm móng.
        /// Đối với Revit, các thông số cho Beam sẽ được lấy từ family cấu kiệm beam dầm.
        /// </summary>
        public TNFBeam Beam { get; set; }

        /// <summary>
        /// Khoảng cách mở miệng hố móng
        /// </summary>
        public int BeamHoleOpenDistance { get; set; } = 100;
        /// <summary>
        /// Chiều sâu hố móng
        /// </summary>
        public int BeamHoleDepth { get; set; }

        /// <summary>
        /// Khoảng các mở rộng thêm cho đáy móng so với Beam Width
        /// </summary>
        public int BeamHoleAddWidth { get; set; } = 450;

        #endregion
        /// <summary>
        ///
        /// </summary>
        /// <param name="tnfParam"></param>
        public TNFFootingBeamParams(TNFGlobalInfo tnfParam)
        {
            TNFParameters = tnfParam;
            Wall = new TNFWall() {Height = 275,Thickness = 210};
            Beam = new TNFBeam(tnfParam) { BeamWidth=300,OffsetWithGL = 100,OpenPitchDistance=100,BaseBeamDepth=350,BeamUpDistance=0};
            BeamHoleDepth = Beam.BeamDepth;
            TNFParameters.PropertyChanged += TNFParameters_PropertyChanged;
        }

        private void TNFParameters_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(TNFParameters.StoneThickness):
                case nameof(TNFParameters.ThirdRevonationDepth):
                case nameof(TNFParameters.SectionType):
                case nameof(TNFParameters.IsDrawFoundationSection):
                    ChangeBeamParams();
                    break;
                default:
                    break;
            }
        }
        private void ChangeBeamParams()
        {
            switch (TNFParameters.SectionType)
            {
                case TNFSectionTypes.NormalType:

                    Beam.BeamUpDistance = 0;
                    if (!TNFParameters.IsDrawFoundationSection)
                    {
                        Beam.BaseBeamDepth = 800;
                        Beam.BeamWidth = 1000;
                        Beam.OpenPitchDistance = 50;
                    }
                    else
                    {
                        Beam.BaseBeamDepth = 350;
                        Beam.BeamWidth = 300;
                        Beam.OpenPitchDistance = 100;
                    }

                    BeamHoleDepth = Beam.BeamDepth;
                    break;

                case TNFSectionTypes.PlatformType:
                case TNFSectionTypes.CrushedStoneAndPlatform:
                    Beam.BaseBeamDepth = 500;
                    Beam.BeamUpDistance = (TNFParameters.ThirdRevonationDepth + TNFParameters.StoneThickness) - Beam.OffsetWithGL;
                    Beam.BeamWidth = 200;
                    Beam.OpenPitchDistance = 100;
                    BeamHoleDepth = 500 + Beam.ZConcreateThickness;
                    break;
                case TNFSectionTypes.CrushedStone:
                    Beam.BaseBeamDepth = 0;
                    break;
                default:
                    break;
            }
        }
    }
}
