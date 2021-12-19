using System;
using System.Collections.Generic;

namespace TNFDataModel.TNFDataAttribute
{
    /// <summary>
    /// Su dung de lay noi dung mot khoi du lieu trong file
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class BlockContentAttribute : TNFAttributeBase
    {
        public string StartBlockHeader { get;  }
        public string EndBlockSymbol { get; }

        public BlockContentAttribute(string blockName,string header,Type typeOf,string endBlockSymbol = "@") : base(blockName,typeOf)
        {
            StartBlockHeader = header;
            EndBlockSymbol = endBlockSymbol;
        }

        public override string[] GetContentFromSource( string[] sourceContents)
        {
            var resutlContents = new List<string>();
            var startRead = false;
            foreach (var line in sourceContents)
            {
                if (line.Equals(this.StartBlockHeader))
                {
                    startRead = true;
                    continue;
                }

                if (startRead == true && !line.Contains(this.EndBlockSymbol))
                {
                    resutlContents.Add(line);
                }
                else if (startRead == true && line.Contains(this.EndBlockSymbol))
                    break;
            }
            return resutlContents.ToArray();
        }
    }
}
