using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFDataModel.TNFDataAttribute
{
    /// <summary>
    /// Dung de lay ra day cac doi tuong
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class ArrayDataAttribute : TNFAttributeBase
    {
        public int LineToRead { get; }

        public ArrayDataAttribute(string blockName, Type typeOf, int lineToRead) : base(blockName, typeOf)
        {
            LineToRead = lineToRead;

        }

        public override string[] GetContentFromSource(string[] source)
        {
            return new string[] { source[LineToRead] };
        }
    }
}
