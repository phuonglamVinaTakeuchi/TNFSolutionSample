using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.TNFDataAttribute;

namespace TNFDataModel.Block
{
    public class PlanBlock : BlockContentsBase
    {
        /// <summary>
        /// Ma trận nền đất
        /// </summary>
        [BlockContent("GLMatrix","*1", typeof(MatrixBlock<string>), "*")]
        public MatrixBlock<string> GLMatrix { get; set; }

        /// <summary>
        /// Ma trận vị trí móng trên plan
        /// </summary>
        [BlockContent("Foundation Location Matrix","*23", typeof(MatrixBlock<int>), "*")]
        public MatrixBlock<int> FoundationLocationMatrix { get; set; }

        /// <summary>
        /// ma trận offset móng lệch tâm, so với cột
        /// Lưu ý, khi set giá trị này cho revit family thì phải đảo dấu.
        /// Là giá trị Hashira Location
        /// </summary>
        [BlockContent("Foundation Offset X Matrix","*26", typeof(MatrixBlock<string>), "*")]
        public MatrixBlock<string> FoundationOfssetXMatrix { get; set; }
        /// <summary>
        /// ma trận offset móng lệch tâm, so với cột
        /// Lưu ý, khi set giá trị này cho revit family thì phải đảo dấu.
        /// </summary>
        [BlockContent("Foundation Offset Y Matrix","*27", typeof(MatrixBlock<string>), "*")]
        public MatrixBlock<string> FoundationOfssetYMatrix { get; set; }

        /// <summary>
        ///  Ma trận kích thước tạm hiểu là HashiraGata trên mặt bằng
        ///  GIá trị này dùng để set giá trị cho type của revit family, ko phải instance
        ///  Tuy nhiên, trên BF1, giá trị này lại dùng cho instance của móng, ko phải type
        /// </summary>
        [BlockContent("Column Dx Matrix","*31", typeof(MatrixBlock<string>), "*")]
        public MatrixBlock<string> HashiraGataDxMatrix { get; set; }
        /// <summary>
        ///  Ma trận kích thước Tạm hiểu là Hashira Gata trên mặt bằng
        ///  GIá trị này dùng để set giá trị cho type của revit family, ko phải instance
        ///  Tuy nhiên, trên BF1, giá trị này lại dùng cho instance của móng, ko phải type
        /// </summary>

        [BlockContent("Column Dy Matrix","*32", typeof(MatrixBlock<string>), "*")]
        public MatrixBlock<string> HashiraGataDyMatrix { get; set; }

        /// <summary>
        /// Ma trận offset của cột so với trục X, Y
        /// </summary>

        [BlockContent("Column Offset X Matrix","*36", typeof(MatrixBlock<string>), "*")]
        public MatrixBlock<string> ColumnOffsetXMatrix { get; set; }

        /// <summary>
        /// Ma trận offset của cột so với trục X, Y
        /// </summary>


        [BlockContent("Column Offset Y Matrix","*37", typeof(MatrixBlock<string>), "*")]
        public MatrixBlock<string> ColumnOffsetYMatrix { get; set; }



        public PlanBlock(string[] sourceContent,string blockName) : base(sourceContent,blockName)
        {
        }
    }
}
