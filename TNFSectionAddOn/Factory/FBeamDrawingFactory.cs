using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BeamSection;
using TNFSectionAddOn.Drawings.BeamSectionDrawings;

namespace TNFSectionAddOn.Factory
{
    public static class FBeamDrawingFactory
    {
        public static BeamDrawingBase CreateBeamDrawing(BeamSectionGBase beamGeometry)
        {
            switch (beamGeometry.BeamSectionData.TNFParameters.SectionType)
            {
                case TNFData.Enums.TNFSectionTypes.NormalType:
                    return new NormalBeamDraw(beamGeometry);
                case TNFData.Enums.TNFSectionTypes.PlatformType:
                    return new PlatformBeamDrawing(beamGeometry);
                case TNFData.Enums.TNFSectionTypes.CrushedStoneAndPlatform:
                    return new CrushedAndPlatformBeamDraw(beamGeometry);
                default:
                    return null;
            }
        }
    }
}
