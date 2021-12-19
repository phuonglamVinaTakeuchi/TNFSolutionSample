using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TNFAutoFoundation.TNFModel;

namespace TNFAutoFoundation.ReadingExcel
{
    public class TNFExcelWriting
    {
        public TNFExcelWriting(string filePath, List<TNFBase> tnfIntances)
        {
            var excel = new Application();
            var workbook = excel.Workbooks.Open(filePath);

            WritingToExcel(workbook, tnfIntances);
            workbook.Save();
            excel.Quit();
            Marshal.ReleaseComObject(excel);
        }
        private void WritingToExcel(Workbook workBook, List<TNFBase> tnfIntances)
        {
            bool isProtected = false;
            if (workBook.HasPassword)
            {
                isProtected = true;
                workBook.Unprotect(TNFString.TNF_PASS_STRING);
            }

            var workSheet = workBook.Worksheets[TNFString.TNF_FINSTANCESHEET] as Worksheet;
            var inputWorkSheet = workBook.Worksheets[TNFString.TNF_FINSTANCESHEET_INPUT] as Worksheet;
            var offsetRowXIndex = FindOffsetXRowId(inputWorkSheet.UsedRange);
            var offsetRowYIndex = FindOffsetYRowId(inputWorkSheet.UsedRange);
            foreach (var intance  in tnfIntances)
            {
                if (intance is TNFFoundationInstance fIntance)
                {
                    workSheet.Cells[fIntance.IdRowIndex, fIntance.IdColumnIndex].Value = fIntance.FoundationId;
                    inputWorkSheet.Cells[offsetRowXIndex,fIntance.ColumnInputIndex].Value = fIntance.GridOffsetX;
                    inputWorkSheet.Cells[offsetRowYIndex,fIntance.ColumnInputIndex].Value = fIntance.GridOffsetY;
                }
            }
            if (isProtected)
            {
                workBook.Protect(TNFString.TNF_PASS_STRING);
            }

        }
        private int FindOffsetXRowId(Range sheetRange)
        {
            var columnIndex =3;
            var maxRow = sheetRange.Rows.Count;

            for (int i = 3; i <= maxRow; i++)
            {
                if (sheetRange.Cells[i, columnIndex].Value.ToString() =="GridOffsetX")
                {
                    return i;
                }
            }
            return 6;
        }
        private int FindOffsetYRowId(Range sheetRange)
        {
            var columnIndex = 3;
            var maxRow = sheetRange.Rows.Count;

            for (int i = 3; i <= maxRow; i++)
            {
                if (sheetRange.Cells[i, columnIndex].Value.ToString() == "GridOffsetY")
                {
                    return i;
                }
            }
            return 7;
        }
    }
}
