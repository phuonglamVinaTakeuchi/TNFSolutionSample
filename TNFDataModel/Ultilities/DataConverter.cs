using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFDataModel.Utilities
{
    public static class DataConverter
    {
        public static object ConvertTo(Type propertyType,string propertyDataValue)
        {
            switch (propertyType)
            {
                case var type when type == typeof(int):
                    return int.Parse(propertyDataValue);
                case var type when type == typeof(double):
                    return double.Parse(propertyDataValue);
                case var type when type == typeof(bool):
                    return propertyDataValue.Equals("1");
                case var type when type == typeof(string):
                default:
                    return propertyDataValue;
            }
        }
        public static T[,] StringToMatrix<T>(string[] sourceContents)
        {
            var rows = sourceContents.Count();
            var columns = sourceContents[0].Split(',').Count();
            var newMatrix = new T[rows, columns];
            for (var row = 0; row < rows; row++)
            {
                var rowString = sourceContents[row].Split(',');
                for (var col = 0; col < columns; col++)
                {

                    newMatrix[row, col] = (T)ConvertTo(typeof(T), rowString[col]);
                }
            }
            return newMatrix;
        }
        public static T[] StringToArray<T>(string sourceString)
        {
            var list = new List<T>();
            var stringList = sourceString.Split(',');
            foreach (var stringMember in stringList)
            {
                var resultValue = ConvertTo(typeof(T), stringMember);
                list.Add((T)resultValue);
            }
            return list.ToArray();
        }

    }
}
