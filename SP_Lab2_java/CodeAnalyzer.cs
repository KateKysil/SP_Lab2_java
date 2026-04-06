using JavaLexer;
using System;
using System.Collections.Generic;

namespace SP_Lab2_java
{
    public interface IFileReader
    {
        string ReadCode(string filePath);
        void LogStatus(string message);
    }
    public class CodeAnalyzer
    {
        private readonly IFileReader _fileReader;

        // Впровадження залежності (Dependency Injection) - це і є "зазор" для мока!
        public CodeAnalyzer(IFileReader fileReader)
        {
            _fileReader = fileReader;
        }

        public List<Token> Analyze(string filePath)
        {
            _fileReader.LogStatus("Розпочато зчитування");

            string code = _fileReader.ReadCode(filePath);

            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("Файл порожній або не існує");
            }

            _fileReader.LogStatus("Зчитування завершено");

            // Викликаємо ваш існуючий Lexer
            Lexer lexer = new Lexer(code);
            return lexer.Tokenize();
        }
    }
}