using TNFDataModel.TNFDataAttribute;

namespace TNFDataModel.Block
{
    public class BasicInfoBlock : BlockContentsBase
    {
        [ArrayData("XAxistName", typeof(string), 7)]
        public string[] XAxistName { get; set; }

        [ArrayData("XAxistSpacing", typeof(int), 8)]
        public int[] XAxistSpacing { get; set; }

        [ArrayData("YAxistName", typeof(string), 9)]
        public string[] YAxistName { get; set; }

        [ArrayData("YAxistSpacing", typeof(int), 10)]
        public int[] YAxistSpacing { get; set; }

        public BasicInfoBlock(string[] sourceContent,string blockName) : base(sourceContent,blockName)
        {
        }
    }
}
