using System.Reflection;
using TNFDataModel.TNFDataAttribute;
using TNFDataModel.TNFModel;
using TNFDataModel.Utilities;

namespace TNFDataModel.Factory
{
    public class TNFArrayFactory : TNFFactory
    {
        public override void GenerationData(TNFBaseModel tnfModel, string[] contentSource,PropertyInfo propertyInfo,TNFAttributeBase tnfAttribute)
        {

            var arrayContents = tnfAttribute.GetContentFromSource(contentSource);
            var arrayType = tnfAttribute.TypeOf;
            var arrayMethod = nameof(DataConverter.StringToArray);
            var converterMethod = typeof(DataConverter).GetMethod(arrayMethod, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var genericmethod = converterMethod.MakeGenericMethod(arrayType);
            var arrayResult = genericmethod.Invoke(null, new object[] { arrayContents[0] });
             propertyInfo.SetValue(tnfModel, arrayResult);


        }
    }
}
