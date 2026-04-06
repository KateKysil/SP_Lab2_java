using JavaLexer;
using SP_Lab2_java;
using System.Diagnostics.Metrics;
using System.Drawing;

class Program
{
    static string ColorFor(TokenType t)
    {
        switch (t)
        {
            case TokenType.Keyword: return JavaLexer.Color.Blue + JavaLexer.Color.Bold;
            case TokenType.Identifier: return JavaLexer.Color.White;
            case TokenType.Number: return JavaLexer.Color.Magenta;
            case TokenType.String: return JavaLexer.Color.Green;
            case TokenType.Char: return JavaLexer.Color.Green;
            case TokenType.Comment: return JavaLexer.Color.Cyan;
            case TokenType.Operator: return JavaLexer.Color.Yellow;
            case TokenType.Punctuator: return JavaLexer.Color.Yellow;
            case TokenType.Error: return JavaLexer.Color.Red + JavaLexer.Color.Bold;
            case TokenType.Whitespace: return "";
            default: return "";
        }
    }
    static void Main(string[] args)
    {
        IFileReader realReader = new RealFileReader();
        CodeAnalyzer analyzer = new CodeAnalyzer(realReader);

        string filePath = "input.java";

        try
        {
            List<Token> tokens = analyzer.Analyze(filePath);

            Console.WriteLine($"\nУспішно знайдено токенів: {tokens.Count}");
            foreach (var token in tokens)
            {
                Console.WriteLine($"Тип: {token.Type,-15} Лексема: '{token.Lexeme}'");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nСталася помилка під час виконання: {ex.Message}");
        }
        Console.ReadLine();
    }

}
