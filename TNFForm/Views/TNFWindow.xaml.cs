using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TNFData.Models.Section;
using TNFForm.ViewModels;

namespace TNFForm.Views
{
    /// <summary>
    /// Interaction logic for TNFWindow.xaml
    /// </summary>
    public partial class TNFWindow : Window
    {
        public TNFWindow(TNFSection tnfSection)
        {
            InitializeComponent();
            var tnfWindowViewModel = new TNFWindowViewModel(tnfSection);
            this.DataContext = tnfWindowViewModel;
            var tnfViewModel = this.TNFParamsInput.DataContext as TNFSectionViewModel;
            if(tnfViewModel != null)
            {
                tnfViewModel.TNFSection = tnfSection;
            }
        }
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
