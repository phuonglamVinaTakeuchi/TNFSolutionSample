using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.Factory;
using TNFDataModel.TNFDataAttribute;
using TNFDataModel.TNFModel;

namespace TNFDataModel.Block
{
    public class TNFCollectionBlock<T> : BlockContentsBase where T: TNFBaseModel
    {
        public List<T> CollectionData { get; private set; }
        public TNFCollectionBlock(string[] sourceContent, string blockName) : base(sourceContent, blockName)
        {
            if (this.SourceContents.Count() > 0)
                InitDataCollection();

        }
        private void InitDataCollection()
        {

            CollectionData = new List<T>();
            var typeofT = typeof(T);
            var tnfAttribute = typeofT.GetCustomAttribute<TNFAttributeBase>();
            var tnfFactory = TNFFactory.CreateTNFFactory(tnfAttribute);
            if(tnfFactory==null)
            {
                return;
            }
            var index = 1;
            foreach (var itemString in this.SourceContents)
            {
                var item = (T)tnfFactory.CreateInstanceObject(tnfAttribute, itemString);
                item.Index = index;
                CollectionData.Add(item);
                index++;
            }


        }
    }
}
