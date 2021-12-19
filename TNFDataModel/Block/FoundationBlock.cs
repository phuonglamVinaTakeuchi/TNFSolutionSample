using TNFDataModel.TNFDataAttribute;
using TNFDataModel.TNFModel;

namespace TNFDataModel.Block
{
    public class FoundationBlock : BlockContentsBase
    {
        [CollectionDataAttribute("Foundation Types",typeof(TNFCollectionBlock<TNFFoundationType>),new string[] { "基礎No" },3)]
        public TNFCollectionBlock<TNFFoundationType> FoundationCollection { get; set; }
        public FoundationBlock(string[] sourceContent,string blockName) :base(sourceContent,blockName)
        {

        }
    }
}
