using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFData.Models;
using TNFData.Models.Section;

namespace TNFForm.ViewModels
{
    public class TNFWindowViewModel : BindableBase
    {
        private TNFSection _tnfSection;
        public TNFSection TNFSection { get => _tnfSection; set => SetProperty(ref _tnfSection, value); }
        public TNFWindowViewModel(TNFSection tnfSection)
        {
            TNFSection = tnfSection;
        }
    }
}
