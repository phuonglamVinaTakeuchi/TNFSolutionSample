using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.Utilities;

namespace TNFDataModel.TNFModel
{
    public  class LocationBuilder
    {
        private LocationData _locationInfo;

        public LocationBuilder()
        {
            _locationInfo = new LocationData();
        }

        public LocationBuilder SetupGridIntersection(string xName,string yName)
        {
            _locationInfo.XName = xName;
            _locationInfo.YName = yName;
            return this;
        }

        public LocationBuilder SetFoundationData(TNFFoundationType foundationType)
        {
            _locationInfo.FoundationType = foundationType;
            return this;
        }
        public LocationBuilder SetHashiraGataDimension(string xDim, string yDim)
        {
            var hXDim = 0;
            if (xDim != string.Empty)
            {
                hXDim = (int)DataConverter.ConvertTo(typeof(int), xDim);
            }
            var hYDim = 0;
            if (yDim != string.Empty)
            {
                hYDim = (int)DataConverter.ConvertTo(typeof(int), yDim);
            }

            var hashiraGataDimension = new Dimension3d();
            hashiraGataDimension.Lx = hXDim;
            hashiraGataDimension.Ly = hYDim;
            _locationInfo.HashiraGataDim = hashiraGataDimension;
            _locationInfo.FoundationType.HashiraGataLx = hXDim;
            _locationInfo.FoundationType.HashiraGataLy = hYDim;
            return this;
        }
        public LocationBuilder SetupOffsetDim(string xOffset,string yOffset)
        {
            _locationInfo.GridOffsetX = string.IsNullOrEmpty(xOffset)?0:(int)DataConverter.ConvertTo(typeof(int), xOffset);
            _locationInfo.GridOffsetY = string.IsNullOrEmpty(yOffset)?0:(int)DataConverter.ConvertTo(typeof(int),yOffset);
            return this;
        }
        public LocationBuilder SetupHashiraGataColumnOffset(string xColOffset,string yColOffset)
        {
            var xColumOffsetWithGrid = string.IsNullOrEmpty(xColOffset)?0:(int)DataConverter.ConvertTo(typeof(int), xColOffset);
            var yColumOffsetWithGrid = string.IsNullOrEmpty(yColOffset)?0:(int)DataConverter.ConvertTo(typeof(int), yColOffset);

            var hashiraGataLocationX = xColumOffsetWithGrid - _locationInfo.GridOffsetX;
            var hashiraGataLocationY = yColumOffsetWithGrid - _locationInfo.GridOffsetY;
            if (_locationInfo.FoundationType != null)
            {
                _locationInfo.FoundationType.HashiraGataLocationX = hashiraGataLocationX;
                _locationInfo.FoundationType.HashiraGataLocationY = hashiraGataLocationY;
            }

            return this;
        }
        public LocationData Build()
        {
            return _locationInfo;
        }
    }
}
