using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.Block;
using TNFDataModel.TNFDataAttribute;
using TNFDataModel.TNFModel;

namespace TNFDataModel.Factory
{
    public class TNFBlockContentFactory : TNFFactory
    {
        public override void GenerationData(TNFBaseModel tnfModel, string[] contentSource,PropertyInfo propInfo,TNFAttributeBase tnfAttribute)
        {
                    var blockContent = CreateInstanceObject(tnfAttribute, contentSource);
                    if (blockContent != null)
                    {
                        propInfo.SetValue(tnfModel, blockContent);
                    }

        }
    }
}
