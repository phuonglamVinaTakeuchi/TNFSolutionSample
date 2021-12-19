using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFDataModel.TNFDataAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TNFExcelSheetAttribute : Attribute
    {
        public string SheetName { get; }
        public TNFExcelSheetAttribute(string sheetName)
        {
            SheetName = sheetName;
        }
    }
}
