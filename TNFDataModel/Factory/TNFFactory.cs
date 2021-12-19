using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.TNFDataAttribute;
using TNFDataModel.TNFModel;

namespace TNFDataModel.Factory
{
    public abstract class TNFFactory
    {
        public TNFBaseModel CreateInstanceObject(TNFAttributeBase tnfAttribute, string[] sourceData)
        {
            var blockContentDataStrings = tnfAttribute.GetContentFromSource(sourceData);
            var blockContentInstance = Activator.CreateInstance(tnfAttribute.TypeOf, new object[] { blockContentDataStrings, tnfAttribute.BlockName });
            return (TNFBaseModel)blockContentInstance;
        }
        public TNFBaseModel CreateInstanceObject(TNFAttributeBase tnfAttribute, string sourceData)
        {
            var tnfObject = Activator.CreateInstance(tnfAttribute.TypeOf, new object[] { sourceData.Split(','), tnfAttribute.BlockName });
            return (TNFBaseModel)tnfObject;
        }
        public abstract void GenerationData(TNFBaseModel tnfModel, string[] contentSource,PropertyInfo propertyInfo,TNFAttributeBase tnfAttribute);
        //public abstract void GenerationData(TNFBaseModel tnfModel, string contentSource, PropertyInfo propertyInfo, TNFAttributeBase tnfAttribute);
        public static TNFFactory CreateTNFFactory(TNFAttributeBase tnfAttribute)
        {
            switch (tnfAttribute.GetType())
            {
                case var type when type == typeof(BlockContentAttribute):
                case var type2 when type2 == typeof(CollectionDataAttribute):
                    return new TNFBlockContentFactory();
                case var type when type == typeof(ArrayDataAttribute):
                    return new TNFArrayFactory();
                case var type when type == typeof(TypeDataAttribute):
                    return new TNFTypeDataFactory();
                case var type when type == typeof(PropertyDataAttribute):
                    return new TNFPropertyDataFactory();
                default:
                    return null;
            }
        }
    }
}
