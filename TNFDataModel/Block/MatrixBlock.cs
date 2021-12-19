using TNFDataModel.Utilities;

namespace TNFDataModel.Block
{
    public class MatrixBlock<T> : BlockContentsBase
    {
        public T[,] Matrixs { get; set; }
        public MatrixBlock(string[] sourceContent,string blockName) : base(sourceContent,blockName)
        {
            if (sourceContent.Length > 0)
                InitMatrix();

        }
        private void InitMatrix()
        {
            var t = typeof(T);
            var matrixMethodName = nameof(DataConverter.StringToMatrix);
            var matrixconverterMethod = typeof(DataConverter).GetMethod(matrixMethodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var genericmethod = matrixconverterMethod.MakeGenericMethod(t);
            Matrixs = (T[,])genericmethod.Invoke(null, new object[] { SourceContents });
        }

    }
}
