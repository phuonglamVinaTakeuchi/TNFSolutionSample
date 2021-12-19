using Prism.Mvvm;

namespace TNFData.Models
{
    public class Dimension2d : BindableBase
    {
        private int _xLength;
        private int _yLength;
        public int XLength { get=>_xLength; set=>SetProperty(ref _xLength,value); }
        public int YLength { get=>_yLength; set=>SetProperty(ref _yLength,value); }

    }
}
