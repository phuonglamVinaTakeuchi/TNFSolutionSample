using TNFDataModel.TNFFile;

namespace TestAttribute
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var filePath = @"D:\TNFSolution\TNFSolution\TestAttribute\bin\Debug\[江別製粉工]  2021.09.17.bf1";
            var bf1ReadingFile = new TNFReading(filePath);
            var bf1FileData = new BF1FileDataModel(filePath,bf1ReadingFile.Contents,"BF1 Data Block");
        }


    }

}
