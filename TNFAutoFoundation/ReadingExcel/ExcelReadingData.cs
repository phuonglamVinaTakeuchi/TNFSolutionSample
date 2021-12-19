using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using TNFAutoFoundation.Models;

namespace TNFAutoFoundation.ReadingExcel
{
    public class ExcelReadingData
    {
        public List<TnfFoundation> TnfList { get; } = new List<TnfFoundation>();
        public ExcelReadingData(string filePath, string sheetName = "FOutPut")
        {
            //FOutPut
            //FoundationDataOutput
            var excel = new Excel.Application();
            var workbook = excel.Workbooks.Open(filePath);
            var worksheet = workbook.Sheets[sheetName] as Excel.Worksheet;
            var sheetRange = worksheet?.UsedRange;
            ReadObject(sheetRange);
            excel.Quit();
            Marshal.ReleaseComObject(excel);


        }

        public void ReadObject(Excel.Range excelRange)
        {
            var maxColumn = excelRange?.Columns.Count;
            var maxRow = excelRange?.Rows.Count;
            for (var columnIndex = 2; columnIndex <= maxColumn; columnIndex++)
            {
                var tnfPoco = new TnfPoco();
                for (int rowIndex = 1; rowIndex <= maxRow; rowIndex++)
                {
                    var cell = excelRange.Cells[rowIndex, columnIndex] as Excel.Range;
                    if (string.IsNullOrEmpty(cell?.Value.ToString()))
                    {
                        return;
                    }
                    var properties = tnfPoco.GetType().GetProperties();

                    if (excelRange.Cells[rowIndex, 1] is Excel.Range cellHeader)
                    {
                        foreach (var propertyInfo in properties)
                        {
                            if (propertyInfo.Name == cellHeader.Value.ToString()&& propertyInfo.CanWrite)
                            {
                                propertyInfo.SetValue(tnfPoco, cell.Value.ToString());
                            }
                        }
                    }
                }

                var tFoundation = TnfList.FindAll(x => x.TypeMark == tnfPoco.TypeMark);
                var subIndex = 0;
                if (tFoundation.Count()>0)
                {
                    subIndex = tFoundation.Max(x => x.SubIndex);
                    subIndex += 1;
                }

                var tnfFoundation = new TnfFoundation(tnfPoco){SubIndex = subIndex};
                TnfList.Add(tnfFoundation);

            }
        }

    }
}
