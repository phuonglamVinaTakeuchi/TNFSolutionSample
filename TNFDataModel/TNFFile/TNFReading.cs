using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TNFDataModel.TNFFile
{

    public class TNFReading
    {

        public string[] Contents {  get;}

        /// <summary>
        /// Đọc toàn bộ nội dung file qua List<string></string>
        /// </summary>
        public TNFReading(string filePath)
        {
            // Encoding cho file tiếng Nhật
            Encoding encoding = Encoding.GetEncoding(932);
            var bf1Strings = File.ReadAllLines(filePath, encoding);
            var clearBf2SString = new List<string>();

            // Làm sạch các ký tự ko mong muốn trong chuỗi
            foreach (var line in bf1Strings)
            {
                var newLine = line.Replace(@"/", "");
                var newLine2 = newLine.Replace("\"", "");
                clearBf2SString.Add(newLine2);
            }
            Contents = clearBf2SString.ToArray();
        }
    }
}
