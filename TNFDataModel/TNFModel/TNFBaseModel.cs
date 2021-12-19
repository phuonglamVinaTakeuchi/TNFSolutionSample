using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.Factory;
using TNFDataModel.TNFDataAttribute;

namespace TNFDataModel.TNFModel
{
    public abstract class TNFBaseModel
    {
        #region Property
        public string BlockName { get; private set; }
        public int Index { get; set; }
        public string[] SourceContents { get; private set; }
        //public string SourceContent { get; private set; }
        #endregion
        #region Constructor
        public TNFBaseModel(string[] sourceContents, string blockName)
        {
            SourceContents = sourceContents;
            BlockName = blockName;
            Init(SourceContents);
        }
        public TNFBaseModel()
        {

        }

        #endregion

        #region Method
        /// <summary>
        /// Init need to be call after constructor to init all nesscessery before use
        /// </summary>

        public virtual void Init(string[] sourceContents)
        {
            var typeOfthis = this.GetType();
            var properties = typeOfthis.GetProperties();
            foreach(var property in properties)
            {
                var tnfAttributes = property.GetCustomAttributes(typeof(TNFAttributeBase),true);
                if(tnfAttributes != null)
                {
                    foreach(var tnfAttribute in tnfAttributes)
                    {
                        var tnfFactory = TNFFactory.CreateTNFFactory((TNFAttributeBase)tnfAttribute);
                        if(tnfFactory!=null)
                        tnfFactory.GenerationData(this,SourceContents,property, (TNFAttributeBase)tnfAttribute);
                    }
                }
            }
        }

        #endregion


    }
}
