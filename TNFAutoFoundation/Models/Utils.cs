using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace TNFAutoFoundation.Models
{
    public static class Utils
    {
        public static double MmToInch(this double number)
        {
            return UnitUtils.ConvertToInternalUnits(number, DisplayUnitType.DUT_MILLIMETERS);
        }
        public static double MmToInch(this int number)
        {
            return UnitUtils.ConvertToInternalUnits(number, DisplayUnitType.DUT_MILLIMETERS);
        }
        public static double InchToMm (this double number)
        {
            return UnitUtils.ConvertFromInternalUnits(number,DisplayUnitType.DUT_MILLIMETERS);
        }
    }
}
