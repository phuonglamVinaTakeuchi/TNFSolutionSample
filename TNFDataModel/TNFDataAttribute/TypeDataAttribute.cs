using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFDataModel.TNFDataAttribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TypeDataAttribute : TNFAttributeBase
    {
        public TypeDataAttribute(string blockName, Type typeOf) : base(blockName, typeOf)
        {
        }

        public override string[] GetContentFromSource(string[] source)
        {
            return null;
        }
    }
}
