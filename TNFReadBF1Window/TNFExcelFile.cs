using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.TNFFile;

namespace TNFReadBF1Window
{
    public class TNFExcelFile
    {
        private BF1FileDataModel _bf1File;
        private string _excelFilePath;
        public TNFExcelFile(string filePath,BF1FileDataModel bf1File)
        {
            _bf1File = bf1File;
            _excelFilePath = filePath;
        }
        public void WriteToExcel()
        {


        }

    }
}
