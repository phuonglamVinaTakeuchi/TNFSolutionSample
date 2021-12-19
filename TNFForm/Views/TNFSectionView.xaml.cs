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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TNFData.Models.Section;
using TNFForm.ViewModels;
using MaterialDesignThemes;
using MaterialDesignColors;

namespace TNFForm.Views
{
    /// <summary>
    /// Interaction logic for TNFSectionViews.xaml
    /// </summary>
    public partial class TNFSectionView : UserControl
    {
        public TNFSectionView()
        {
            InitializeComponent();
            var _ = new Microsoft.Xaml.Behaviors.DefaultTriggerAttribute(typeof(Trigger), typeof(Microsoft.Xaml.Behaviors.TriggerBase), null);
            var tnfFormViewModel = new TNFSectionViewModel();
            this.DataContext = tnfFormViewModel;
        }


    }
}
