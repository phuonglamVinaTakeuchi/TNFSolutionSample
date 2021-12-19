using System.Reflection;
using TNFDataModel.Block;
using TNFDataModel.Factory;
using TNFDataModel.TNFDataAttribute;

namespace TNFDataModel.TNFFile
{
    public abstract class TNFFileContentModel: BlockContentsBase
    {
        public string[] WriteContents { get; set; }
        public TNFFileContentModel(string[] contents,string blockName) : base(contents,blockName)
        {

        }
    }
}
