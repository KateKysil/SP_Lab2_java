using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaLexer
{
    public static class Color
    {
        public const string Reset = "\x1b[0m";
        public const string Black = "\x1b[30m";
        public const string Red = "\x1b[31m";
        public const string Green = "\x1b[32m";
        public const string Yellow = "\x1b[33m";
        public const string Blue = "\x1b[34m";
        public const string Magenta = "\x1b[35m";
        public const string Cyan = "\x1b[36m";
        public const string White = "\x1b[37m";
        public const string Bold = "\x1b[1m";
    }

    public enum TokenType
    {
        Number,
        String,
        Char,
        Identifier,
        Keyword,
        Operator,
        Punctuator,
        Comment,
        Whitespace,
        Error,
        EndOfFile
    }

    public struct Token
    {
        public TokenType Type;
        public string Lexeme;
        public string Message;

        public Token(TokenType type, string lexeme, string message = "")
        {
            Type = type;
            Lexeme = lexeme;
            Message = message;
        }
    }

    public class Lexer
    {
        private static readonly HashSet<string> JavaKeywords = new HashSet<string>
        {
            "abstract","assert","boolean","break","byte","case","catch","char","class","const",
            "continue","default","do","double","else","enum","extends","final","finally","float",
            "for","goto","if","implements","import","instanceof","int","interface","long","native",
            "new","package","private","protected","public","return","short","static","strictfp",
            "super","switch","synchronized","this","throw","throws","transient","try","void",
            "volatile","while","yield","record","sealed","non-sealed","permits","var","module",
            "opens","requires","exports","provides","uses","with","to","transitive"
        };

        private readonly string src;
        public int pos;
        private Token? pendingToken;

        public Lexer(string src)
        {
            this.src = src;
            this.pos = 0;
            this.pendingToken = null;
        }

        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();
            while (true)
            {
                Token t = NextToken();
                tokens.Add(t);
                if (t.Type == TokenType.EndOfFile) break;
            }
            return tokens;
        }

        public char Peek(int k = 0)
        {
            if (pos + k >= src.Length) return '\0';
            return src[pos + k];
        }

        public char Get()
        {
            if (pos >= src.Length) return '\0';
            return src[pos++];
        }

        public bool StartsWith(string s)
        {
            if (pos + s.Length > src.Length) return false;
            return src.Substring(pos, s.Length) == s;
        }

        public Token MakeToken(TokenType type, string lexeme, string msg = "")
        {
            return new Token(type, lexeme, msg);
        }

        public bool IsHexDigit(char c)
        {
            return char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        public Token NextToken()
        {
            if (pendingToken.HasValue)
            {
                Token t = pendingToken.Value;
                pendingToken = null;
                return t;
            }

            char c = Peek();
            if (c == '\0')
            {
                return MakeToken(TokenType.EndOfFile, "");
            }

            if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
            {
                StringBuilder lex = new StringBuilder();
                while (true)
                {
                    char p = Peek();
                    if (p == ' ' || p == '\t' || p == '\r' || p == '\n') lex.Append(Get());
                    else break;
                }
                return MakeToken(TokenType.Whitespace, lex.ToString());
            }

            c = Peek();

            // Comments
            if (StartsWith("//"))
            {
                StringBuilder lex = new StringBuilder();
                lex.Append(Get()); // '/'
                lex.Append(Get()); // '/'
                while (Peek() != '\n' && Peek() != '\0') lex.Append(Get());
                return MakeToken(TokenType.Comment, lex.ToString());
            }
            if (StartsWith("/*"))
            {
                StringBuilder lex = new StringBuilder();
                lex.Append(Get());
                lex.Append(Get());
                bool closed = false;
                while (Peek() != '\0')
                {
                    if (StartsWith("*/"))
                    {
                        lex.Append(Get());
                        lex.Append(Get());
                        closed = true;
                        break;
                    }
                    else
                    {
                        lex.Append(Get());
                    }
                }
                if (!closed)
                {
                    return MakeToken(TokenType.Error, lex.ToString(), "Unterminated block comment");
                }
                return MakeToken(TokenType.Comment, lex.ToString());
            }

            // Strings
            if (c == '"')
            {
                StringBuilder lex = new StringBuilder();
                lex.Append(Get());
                bool closed = false;
                while (Peek() != '\0')
                {
                    char p = Get();
                    lex.Append(p);
                    if (p == '\\')
                    {
                        if (Peek() != '\0') lex.Append(Get());
                    }
                    else if (p == '"')
                    {
                        closed = true;
                        break;
                    }
                }
                if (!closed) return MakeToken(TokenType.Error, lex.ToString(), "Error");
                return MakeToken(TokenType.String, lex.ToString());
            }

            // Char
            if (c == '\'')
            {
                StringBuilder lex = new StringBuilder();
                lex.Append(Get());
                bool closed = false;
                while (Peek() != '\0')
                {
                    char p = Get();
                    lex.Append(p);
                    if (p == '\\')
                    {
                        if (Peek() != '\0') lex.Append(Get());
                    }
                    else if (p == '\'')
                    {
                        closed = true;
                        break;
                    }
                }
                if (!closed) return MakeToken(TokenType.Error, lex.ToString(), "Error");
                return MakeToken(TokenType.Char, lex.ToString());
            }

            // Number
            if (char.IsDigit(c) || (c == '.' && char.IsDigit(Peek(1))))
            {
                StringBuilder lex = new StringBuilder();
                bool isFloat = false;

                if (c == '0' && (Peek(1) == 'x' || Peek(1) == 'X'))
                {
                    lex.Append(Get()); // 0
                    lex.Append(Get()); // x or X

                    bool anyHex = false;
                    bool isValid = true;

                    while (IsIdentifierPart(Peek()))
                    {
                        char p = Get();
                        lex.Append(p);
                        if (!IsHexDigit(p))
                        {
                            isValid = false;
                        }
                        else
                        {
                            anyHex = true;
                        }
                    }

                    if (!anyHex)
                    {
                        return MakeToken(TokenType.Error, lex.ToString(), "Invalid hex literal: no digits");
                    }

                    if (!isValid)
                    {
                        return MakeToken(TokenType.Error, lex.ToString(), "Invalid hex literal");
                    }

                    return MakeToken(TokenType.Number, lex.ToString());
                }

                while (char.IsDigit(Peek())) lex.Append(Get());
                if (Peek() == '.')
                {
                    if (Peek(1) == '.')
                    {
                        // handled by punctuator later
                    }
                    else
                    {
                        isFloat = true;
                        lex.Append(Get());
                        while (char.IsDigit(Peek())) lex.Append(Get());
                    }
                }
                // exponent
                if (Peek() == 'e' || Peek() == 'E' || Peek() == 'p' || Peek() == 'P')
                {
                    char e = Peek();
                    if (lex.Length > 0)
                    {
                        lex.Append(Get());
                        if (Peek() == '+' || Peek() == '-') lex.Append(Get());
                        bool anyDig = false;
                        while (char.IsDigit(Peek())) { anyDig = true; lex.Append(Get()); }
                        if (!anyDig) return MakeToken(TokenType.Error, lex.ToString(), "Malformed exponent in number");
                        isFloat = true;
                    }
                }
                if (Peek() == 'f' || Peek() == 'F' || Peek() == 'd' || Peek() == 'D' || Peek() == 'l' || Peek() == 'L')
                {
                    lex.Append(Get());
                }
                return MakeToken(TokenType.Number, lex.ToString());
            }

            // Identifier or keyword
            if (IsIdentifierStart(c))
            {
                StringBuilder lex = new StringBuilder();
                lex.Append(Get());
                while (IsIdentifierPart(Peek())) lex.Append(Get());
                char next = Peek();
                if (IsIdentifierPart(next))
                {
                    lex.Append(next);
                    return MakeToken(TokenType.Error, lex.ToString(), "Missing separator between identifiers");
                }

                string lexeme = lex.ToString();
                if (JavaKeywords.Contains(lexeme))
                    return MakeToken(TokenType.Keyword, lexeme);
                else
                    return MakeToken(TokenType.Identifier, lexeme);
            }

            // Operators and punctuators: attempt longest-match
            string[] multiOps = {
                ">>>=", ">>=", "<<=", ">>>", ">>", "<<",
                "==", "!=", "<=", ">=", "&&", "||", "++", "--",
                "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "->", "::", "..."
            };

            foreach (string op in multiOps)
            {
                if (StartsWith(op))
                {
                    StringBuilder lex = new StringBuilder();
                    for (int i = 0; i < op.Length; ++i) lex.Append(Get());
                    return MakeToken(TokenType.Operator, lex.ToString());
                }
            }

            // Single-char operators / punctuators
            char single = Peek();
            string operators_chars = "+-*/%&|^~!=<>?:";
            string punctuators_chars = "(){},;[].@";

            if (operators_chars.Contains(single))
            {
                return MakeToken(TokenType.Operator, Get().ToString());
            }
            if (punctuators_chars.Contains(single))
            {
                return MakeToken(TokenType.Punctuator, Get().ToString());
            }

            // Fallback error
            return MakeToken(TokenType.Error, Get().ToString(), "There is no such character");
        }

        public static bool IsIdentifierStart(char c)
        {
            return c == '_' || c == '$' || char.IsLetter(c);
        }

        public static bool IsIdentifierPart(char c)
        {
            return c == '_' || c == '$' || char.IsLetterOrDigit(c);
        }
    }

    
}