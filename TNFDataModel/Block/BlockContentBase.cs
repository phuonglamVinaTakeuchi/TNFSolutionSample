using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.Factory;
using TNFDataModel.TNFDataAttribute;
using TNFDataModel.TNFModel;

namespace TNFDataModel.Block
{
    public abstract class BlockContentsBase : TNFBaseModel
    {
        public BlockContentsBase(string[] sourceContent,string blockName) : base(sourceContent,blockName)
        {

        }

    }
}
