using System;
using System.Reflection;
using TNFDataModel.TNFDataAttribute;
using TNFDataModel.TNFModel;
using TNFDataModel.Utilities;

namespace TNFDataModel.Factory
{
    public class TNFPropertyDataFactory : TNFFactory
    {
        public override void GenerationData(TNFBaseModel tnfModel, string[] contentSource, PropertyInfo propertyInfo, TNFAttributeBase tnfAttribute)
        {
            if (tnfAttribute is PropertyDataAttribute propertyAttribute)
            {
                SetPropertyValue(tnfModel, propertyInfo, contentSource, propertyAttribute);
            }
        }
        public void SetPropertyValue(TNFBaseModel instanceObject, PropertyInfo propertyInfo, string[] lineStrings, PropertyDataAttribute propAttribute)
        {
            if (!propAttribute.IsNestedProperties)
            {
                var propValue = propAttribute.GetContentFromSource(lineStrings, propAttribute.PropertyIndexes[0]);
                propertyInfo.SetValue(instanceObject, DataConverter.ConvertTo(propertyInfo.PropertyType,propValue ));
                return;
            }
            else
            {
                var propValues = propAttribute.GetContentFromSource(lineStrings, propAttribute.PropertyIndexes);
                var propertyType = propertyInfo.PropertyType;
                var newinstantPropertyValue = Activator.CreateInstance(propertyType, new object[] {  propValues,propAttribute.BlockName });
                propertyInfo.SetValue(instanceObject, newinstantPropertyValue);


            }

        }
    }
}
