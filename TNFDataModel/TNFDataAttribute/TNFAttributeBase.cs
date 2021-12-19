using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFDataModel.TNFDataAttribute
{
    public abstract class TNFAttributeBase : Attribute
    {
        public Type TypeOf { get; }
        public string BlockName { get; }
        public TNFAttributeBase(string blockName, Type typeOf)
        {
            BlockName = blockName;
            TypeOf = typeOf;
        }
        public abstract string[] GetContentFromSource(string[] source);
        public string GetContentFromSource(string source, int index)
        {
            var sourceArray = source.Split(',');
            var arrayLength = sourceArray.Length;
            if (arrayLength > 0 && index <= arrayLength - 1)
                return sourceArray[index];
            return string.Empty;
        }
        public string GetContentFromSource(string[] source, int index)
        {
            var arrayLength = source.Length;
            if (arrayLength > 0 && index <= arrayLength - 1)
                return source[index];
            return string.Empty;
        }
        public string[] GetContentFromSource(string[] source,int[] indexs)
        {
            var propertyValues = new List<string>();
            foreach (var propertyIndex in indexs)
            {
                var propertyValue = source[propertyIndex];
                propertyValues.Add(propertyValue);

            }
            return propertyValues.ToArray();
        }
        public string[] GetContentFromSource(string[] source,int startAt,int endAt)
        {
            var propertyValues = new List<string>();
            for(var i = startAt; i <= endAt; i++)
            {
                var propertyValue = source[i];
                propertyValues.Add(propertyValue);
            }
            return propertyValues.ToArray();
        }
    }
}
