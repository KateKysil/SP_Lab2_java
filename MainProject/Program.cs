using System.Diagnostics.Metrics;
using System.Drawing;
using JavaLexer;

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
        Console.Write("Enter path to Java file: ");
        string filepath = Console.ReadLine();

        if (!File.Exists(filepath))
        {
            Console.Error.WriteLine($"Cannot open file: {filepath}");
            Environment.Exit(2);
        }

        string input;
        try
        {
            input = File.ReadAllText(filepath);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error reading file: {ex.Message}");
            Environment.Exit(2);
            return;
        }

        Lexer lexer = new Lexer(input);
        List<Token> tokens = lexer.Tokenize();

        foreach (Token t in tokens)
        {
            if (t.Type == TokenType.EndOfFile) break;
            string col = ColorFor(t.Type);

            if (!string.IsNullOrEmpty(col))
            {
                Console.Write($"{col}{t.Lexeme}{JavaLexer.Color.Reset}");
            }
            else
            {
                Console.Write(t.Lexeme);
            }
        }
    }

}
