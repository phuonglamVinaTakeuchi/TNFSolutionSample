using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.Enums;

namespace TNFDataModel.TNFDataAttribute
{
    /// <summary>
    /// Dung de lay ra day cac doi tuong
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class CollectionDataAttribute : TNFAttributeBase
    {
        private enum ReadingStatus
        {
            None,
            StartRead,
            ContinueReading,
            EndReading
        }
        public string[] StartStringEntry { get; }
        public int CountLoop { get; }


        public CollectionDataAttribute(string blockName, Type typeOf, string[] startStringEntry, int countLoop) : base(blockName, typeOf)
        {
            StartStringEntry = startStringEntry;
            CountLoop = countLoop;

        }

        public override string[] GetContentFromSource(string[] sourceContents)
        {
            var listDataString = new List<string>();
            var fString = string.Empty;
            var readingStatus = ReadingStatus.None;
            var lineCount = 0;
            //var maxContentRead = sourceContents.Count();
            foreach (var line in sourceContents)
            {

                if (readingStatus == ReadingStatus.None)
                {
                    var beginRead = CheckLineToBeginRead(line, StartStringEntry);
                    if (beginRead)
                    {
                        readingStatus = ReadingStatus.StartRead;
                        lineCount = 1;
                    }
                    else
                    {
                        continue;
                    }
                }
                if(readingStatus == ReadingStatus.ContinueReading && lineCount == 1)
                {
                    var beginRead = CheckLineToBeginRead(line, StartStringEntry);
                    if (!beginRead)
                    {
                        readingStatus = ReadingStatus.EndReading;
                    }
                }
                if (readingStatus == ReadingStatus.EndReading)
                    break;

                if(lineCount< CountLoop)
                {
                    fString += line;
                    fString += ",";
                    readingStatus = ReadingStatus.ContinueReading;
                    lineCount++;
                    continue;
                }
                else if (lineCount == CountLoop)
                {
                    fString += line;
                    listDataString.Add(fString);
                    fString = string.Empty;
                    readingStatus = ReadingStatus.ContinueReading;
                    lineCount = 1;
                }


            }
            return listDataString.ToArray();

        }

        private bool CheckLineToBeginRead(string lineString, string[] startEntrys)
        {
            foreach (var entry in startEntrys)
            {
                if (lineString.Contains(entry))
                    return true;
            }
            return false;
        }
    }
}
