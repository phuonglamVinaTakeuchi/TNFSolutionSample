using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TNFDataModel.TNFFile;
using Application = Microsoft.Office.Interop.Excel.Application;
using Window = System.Windows.Window;
using Action = System.Action;
using System.ComponentModel;

namespace BF1ReadingApp
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _excelFilePath;
        private BF1FileDataModel _bf1Data;
        private bool _isUpdate;
        private ReportWorking _reportWorking;
        public MainWindow()        {
            InitializeComponent();
            _reportWorking = StartReport;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BF1ToExcel();
        }

        public void BF1ToExcel()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "bf1 files (*.bf1)|*.bf1";
            if (openFileDialog.ShowDialog() == true)
            {
                var bf1ReadingFile = new TNFReading(openFileDialog.FileName);
                this._bf1Data = new BF1FileDataModel(openFileDialog.FileName, bf1ReadingFile.Contents, "BF1 Data Blog");
                openFileDialog.Filter = "excel file(*.xlsx)|*.xlsx";
                _isUpdate = (bool)this.UpdateCheckBox.IsChecked;
                if (openFileDialog.ShowDialog() == true)
                {
                    _excelFilePath = openFileDialog.FileName;

                    var backgroundWorker = new BackgroundWorker();
                    backgroundWorker.WorkerReportsProgress = true;
                    backgroundWorker.DoWork += BackgroundWorker_DoWork;
                    //backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
                    backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
                    backgroundWorker.RunWorkerAsync();
                    //Start();

                }
            }
        }

        //private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    //Dispatcher.Invoke((Action)(() =>
        //    //{
        //    //    this.Start();
        //    //    this.PSB.Value = e.ProgressPercentage;

        //    //}));
        //    this.StartReport();
        //    this.PSB.Value = e.ProgressPercentage;
        //}

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                Complete(this);

            }));

        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Dispatcher.Invoke((Action)(() =>
            //{
                var excelFile = new TNFExcelFile(_excelFilePath, _bf1Data);
                var excel = new Application();
                var workbook = excel.Workbooks.Open(_excelFilePath);
                excelFile.WriteToExcel(_isUpdate, workbook,_reportWorking);
                workbook.Save();
                excel.Quit();
                Marshal.ReleaseComObject(excel);

            //}));
        }

        private void StartReport(int reportPercent)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                this.UpdateCheckBox.Visibility = Visibility.Collapsed;
            this.StatusTextBox.Text = "Running";
            this.PSB.Visibility = Visibility.Visible;
            this.PSB.Value = reportPercent;
            }));
        }
        private static void Complete(MainWindow mainWindow)
        {
            mainWindow.PSB.Visibility = Visibility.Collapsed;
            mainWindow.StatusTextBox.Visibility = Visibility.Visible;
            mainWindow.StatusTextBox.Text = "Complete";
            mainWindow.StatusTextBox.Foreground = Brushes.Green;
        }



    }
}
