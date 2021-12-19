using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFDataModel.TNFDataAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TNFRevitFamilyNameAttribute : Attribute
    {
        public string RevitParamName { get; }
        public bool IsNestedProperty { get; }
        public TNFRevitFamilyNameAttribute(string paramName,bool isNested = false)
        {
            RevitParamName = paramName;
            IsNestedProperty = isNested;
        }
    }
}
