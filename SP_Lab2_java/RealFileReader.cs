using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_Lab2_java
{
    public class RealFileReader : IFileReader
    {
        public string ReadCode(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл не знайдено за шляхом: {filePath}");
            }
            return File.ReadAllText(filePath);
        }

        public void LogStatus(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] LOG: {message}");
        }
    }
}
