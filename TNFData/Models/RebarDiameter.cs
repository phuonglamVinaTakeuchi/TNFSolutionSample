using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFData.Models
{
    public class RebarDiameter
    {
        #region Fields

        #endregion
        #region Properties
        /// <summary>
        /// Ký tự đầu tiên cho Tên của Rebar = > D
        /// </summary>
        public string PrefixSymbol => "D";
        public string SubfixSymbol { get; set; } = "";
        /// <summary>
        /// Ký tự cuối cùng cho tên của Rebar =>W
        /// </summary>
        //public string SubfixSymbol { get; set; }
        /// <summary>
        /// Bán kính của rebar
        /// </summary>
        public int Radius { get; set; }
        /// <summary>
        /// Khoảng cách của Rebar spacing của
        /// </summary>
        public int Spacing { get; set; }
        /// <summary>
        /// Tên của Rebar là Hakama/Besu
        /// Sẽ được khởi tạo lúc tạo ra Rebar
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Tên đầy đủ của Rebar
        /// Description
        /// Name:Descrioption
        /// </summary>
        public string FullName => $"{Name} : {Description}";

        /// <summary>
        /// D13@200;
        /// </summary>
        public string Description => $"{PrefixSymbol}{Radius}@{Spacing}{SubfixSymbol}";

        public string Info => $"{Quantities}-{PrefixSymbol}{Radius}";
        /// <summary>
        /// Số lượng của Rebar
        /// Thuộc tính này dùng riêng cho Revit dimeter để thiết lập cho Family
        /// </summary>
        public int Quantities { get; set; }
        #endregion
        #region Constructor
        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="radius"></param>
        /// <param name="spacing"></param>
        public RebarDiameter(string name,int radius,int spacing)
        {
            Name = name;
            Radius = radius;
            Spacing = spacing;
        }
        public RebarDiameter(int qTy,int radius)
        {
            Name = string.Empty;
            Radius = radius;
            Quantities = qTy;
        }
        #endregion
    }
}
