using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Geometry.BeamSection;
using TNFData.Models.Section;

namespace TNFData.Factory
{
    public static class TNFBeamSectionFactory
    {
        public static BeamSectionGBase CreateBeamSection(Point2D basePoint, TNFFootingBeamParams beamSectionData)
        {
            switch (beamSectionData.TNFParameters.SectionType)
            {
                case Enums.TNFSectionTypes.NormalType:
                    return new NormalBeamSectionG(basePoint, beamSectionData);
                case Enums.TNFSectionTypes.PlatformType:
                    return new PlatformBeamSectionG(basePoint, beamSectionData);
                case Enums.TNFSectionTypes.CrushedStoneAndPlatform:
                    return new CrushedStondAndPlatformBeamG(basePoint, beamSectionData);
                default: 
                    return null;
            }
        }
    }
}
