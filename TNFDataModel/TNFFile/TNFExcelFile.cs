using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TNFDataModel.Block;
using TNFDataModel.TNFDataAttribute;
using TNFDataModel.TNFFile;
using TNFDataModel.TNFModel;

namespace TNFDataModel.TNFFile
{
    public delegate void ReportWorking(int reportPercent);
    public class TNFExcelFile
    {
        private BF1FileDataModel _bf1File;
        private string _excelFilePath;
        private const string PASSWORD = "123456";
        public TNFExcelFile(string filePath,BF1FileDataModel bf1File)
        {
            _bf1File = bf1File;
            _excelFilePath = filePath;
        }
        public void WriteToExcel(bool justUpdate,Workbook workbook, ReportWorking reportWorking)
        {
            var foundationTypes = (from foundation in ((FoundationBlock)_bf1File.FoundationBlock).FoundationCollection.CollectionData
                                   where !string.IsNullOrEmpty(foundation.TypeMark)
                                   select foundation).ToList();

            var foundationSheet = workbook.Worksheets["FoundationType"] as Worksheet;
            var foundationInstanceSheet = workbook.Worksheets["FoundationInstance"] as Worksheet;
            bool isProtected = false;
            if (workbook.HasPassword)
            {
                isProtected = true;
                workbook.Unprotect(PASSWORD);
            }
            var foundationInstance = _bf1File.FoundationInstances.OrderBy(x => x.GridIntersection).ToList();
            //var task1 =  Task.Delay(10000);
            WriteFoundationToExcelFile(justUpdate, foundationSheet, foundationTypes, reportWorking);
            WriteFoundationInstanceToExcelFile(justUpdate, foundationInstanceSheet, foundationInstance, reportWorking);
            //await Task.WhenAll(task1);

            //WriteFoundationToExcelFile(justUpdate,foundationSheet,foundationTypes);

            //WriteFoundationInstanceToExcelFile(justUpdate, foundationInstanceSheet,foundationInstance);

            if (isProtected)
            {
                workbook.Protect(PASSWORD);
            }

        }
        private void WriteFoundationToExcelFile(bool justUpdate,Worksheet fSheet,List<TNFFoundationType> foundationTypes,ReportWorking report)
        {
            if (!justUpdate)
            {
                fSheet.Range["D4", "CV31"].ClearContents();
            }

            var headerColum = 3;
            var startRow = 4;
            var endRow = 31;
            var startColum = 4;

            foreach (var foundation in foundationTypes)
            {
                SetProp(foundation, headerColum, startRow, endRow, startColum, fSheet);
                report?.Invoke(foundationTypes.IndexOf(foundation));
                startColum++;
            }

        }

        private  void SetProp(Object instanceObject,int headerCol,int startRow,int endRow,int startColum,Worksheet workingSheet)
        {
            var typeOfF = instanceObject.GetType();
            var properties = typeOfF.GetProperties();
            foreach (var prop in properties)
            {
                var propAtr = prop.GetCustomAttribute(typeof(TNFRevitFamilyNameAttribute)) as TNFRevitFamilyNameAttribute;
                if (propAtr != null)
                {
                    if (propAtr.IsNestedProperty)
                    {
                        var instanceOfProp = prop.GetValue(instanceObject);
                        SetProp(instanceOfProp, headerCol, startRow, endRow, startColum,workingSheet);
                    }
                    else
                    {
                        var rowIndex = GetRowIndex(propAtr.RevitParamName, headerCol, startRow, endRow, workingSheet);
                        if (rowIndex > 0)
                        {
                            workingSheet.Cells[rowIndex, startColum].Value = prop.GetValue(instanceObject);
                            continue;
                        }
                    }
                }
            }
        }



        private int GetRowIndex(string headerName,int headerCol,int start,int end,Worksheet workingSheet)
        {
            for(var i = start; i <= end; i++)
            {
                var headerString = workingSheet.Cells[i, headerCol].Value;
                if(headerString== headerName)
                {
                    return i;
                }
            }
            return -1;
        }


        private void WriteFoundationInstanceToExcelFile(bool justUpdate,Worksheet fInstanceSheet,List<TNFFoundationInstance> foundationInstances,ReportWorking report)
        {
            if (!justUpdate)
            {
                fInstanceSheet.Range["D3", "CV12"].ClearContents();
            }

            var headerColum = 3;
            var startRow = 3;
            var endRow = 12;
            var startColum = 4;
            foreach(var finStance in foundationInstances)
            {

                SetProp(finStance, headerColum, startRow, endRow, startColum, fInstanceSheet);
                report?.Invoke(foundationInstances.IndexOf(finStance));
                startColum++;
            }

        }

    }
}
