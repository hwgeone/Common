using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FileHelper
{
    public class FileHelper
    {
        private static byte[] array;
        /// <summary>  
        /// 写字节数组到文件  
        /// </summary>  
        /// <param name="buff"></param>  
        /// <param name="filePath"></param>  
        public static void WriteBuffToFile(string filePath)
        {
            if (array != null && array.Length > 0)
            {
                WriteBuffToFile(0, array.Length, filePath);
            }
        }
        /// <summary>  
        /// 写字节数组到文件  
        /// </summary>  
        /// <param name="buff"></param>  
        /// <param name="offset">开始位置</param>  
        /// <param name="len"></param>  
        /// <param name="filePath"></param>  
        public static void WriteBuffToFile(int offset, int len, string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            FileStream output = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(array, offset, len);
            writer.Flush();
            writer.Close();
            output.Close();
        }


        public static void ReadBuffFromFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            //获取文件大小
            long size = fs.Length;

            array = new byte[size];

            //将文件读到byte数组中
            fs.Read(array, 0, array.Length);

            fs.Close();

        }
    }
}
