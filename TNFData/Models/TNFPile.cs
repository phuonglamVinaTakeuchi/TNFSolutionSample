using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFData.Models
{
    public class TNFPile
    {
        #region Properties
        /// <summary>
        /// Bán kính của cọc
        /// </summary>
        public int Radius { get; set; }
        /// <summary>
        /// Chiều dài cọc
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// Khoảng cách giữa các cọc
        /// </summary>
        public double Spacing { get; set; }
        public string Description => $"木杭 ∅{Radius} L={Length}m@{Spacing}";

        public TNFGlobalInfo TNFParams { get; private set; }
        #endregion
        public TNFPile(TNFGlobalInfo tnfParams)
        {
            TNFParams = tnfParams;
        }
    }
}
