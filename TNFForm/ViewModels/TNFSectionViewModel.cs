using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TNFData.Models;
using TNFData.Models.Section;

namespace TNFForm.ViewModels
{
    public class TNFSectionViewModel: BindableBase
    {
        private TNFSection _tnfSection;
        public TNFSection TNFSection { get => _tnfSection; set => SetProperty(ref _tnfSection, value); }
        public ICommand UpdateScaleCommand { get;private set;  }
        public ObservableCollection<string> ScaleOptions { get; private set; }
        public TNFSectionViewModel()
        {
            TNFSection = new TNFSection();
            ScaleOptions = new ObservableCollection<string>() { "1/1", "2/1", "3/1", "4/1", "5/1", "6/1", "10:1", "1/2", "1/3", "1/4", "1/5","1/10","1/25","1/50","1/75","1/100" };
            UpdateScaleCommand = new DelegateCommand<ComboBox>(this.OnUpdateScaleList, this.CanUpdateScaleList);
        }

        private bool CanUpdateScaleList(ComboBox parameter)
        {
            var canExcute = false;
            if (parameter is ComboBox inforView)
            {
                if (string.IsNullOrEmpty(inforView.Text))
                {
                    canExcute = false;
                }
                else
                {
                    if (inforView.Text.Contains("/"))
                        canExcute = true;
                    else if (Double.TryParse(inforView.Text,out var result))
                    {
                        canExcute = true;
                    }
                    else
                        canExcute = false;
                }
            }

            return canExcute;
        }

        private void OnUpdateScaleList(ComboBox parameter)
        {
            if (!(parameter is ComboBox inforView)) return;
            if (ScaleOptions.Contains(inforView.Text) || string.IsNullOrEmpty(inforView.Text))
            {
                return;
            }
            this.ScaleOptions.Add(inforView.Text);
        }
    }
}
