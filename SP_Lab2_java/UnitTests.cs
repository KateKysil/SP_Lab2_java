using JavaLexer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_Lab2_java
{
    [TestFixture]
    public class UnitTests
    {
        //основна функція Tokenize яка по суті визначає всі токени
        //в ній є функція NextToken, найбільша функція з основною логікою, думаю треба протестувати тут всі можливі варіанти
        //функція Peek() дивиться на наступний символ і повертає його
        //функція MakeToken просто створює новий об'єкт класу, не думаю, що треба тестувати
        //функція Get() дивиться на наступний символ і переміщає позицію на один символ вперед
        //функція StartsWith(string) просто порівняння сабстрінгом, не думаю, що треба тестувати
        //функція IsHexDigit(char) коли обробляємо число, то за допомогою цієї функції перевідяємо кожен наступний символ чи це вважається символом 16-кової системи

        private Lexer def_lexer;
        [SetUp]
        public void Setup()
        {
            def_lexer = new Lexer("int a = 42;");
        }
        [Test]
        public void TestPeek1()
        {
            char res = def_lexer.Peek();
            Assert.That(res, Is.EqualTo('i'));
        }
        [Test]
        public void TestPeek2()
        {
            def_lexer.pos = 3;
            char res = def_lexer.Peek(15);
            Assert.That(res, Is.EqualTo('\0'));
        }

        [Test]
        public void TestGet1()
        {
            def_lexer.pos = 3;
            char res = def_lexer.Get();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.EqualTo(' '));
                Assert.That(def_lexer.pos, Is.EqualTo(4));
            });
            
        }
        [Test]
        public void TestGet2()
        {
            def_lexer.pos = 15;
            char res = def_lexer.Get();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.EqualTo('\0'));
                Assert.That(def_lexer.pos, Is.EqualTo(15));
            });

        }
        [TestCase('A', true)]
        [TestCase('f', true)]
        [TestCase('5', true)]
        [TestCase('G', false)]
        [TestCase('-', false)]
        public void TestIsHexDigit(char input, bool expected)
        {
            bool result = def_lexer.IsHexDigit(input);
            Assert.That(result, Is.EqualTo(expected));
        }
        [Test]
        public void TestNullInput()
        {
            Lexer brokenLexer = new Lexer(null);
            Assert.Throws<NullReferenceException>(() => brokenLexer.Tokenize());
        }
        [Test]
        public void TestTokenize1()
        {
            List<Token> tokens = def_lexer.Tokenize();

            Assert.Multiple(() =>
            {
                Assert.That(tokens, Has.Count.EqualTo(9));
                Assert.That(tokens[0].Lexeme, Does.StartWith("in").And.EndsWith("t"));
            });
        }
        [Test]
        public void TestTokenize2()
        {
            string complexInput = @"
                // single line comment
                /* multi line 
                   comment */
                String str = ""hello\n"";
                char c = '\'';
                float f = 3.14f;
                int hex = 0x1A;
                if (a == b && c != d) { 
                    a += 1; 
                }
            ";
            Lexer coverageLexer = new Lexer(complexInput);
            List<Token> tokens = coverageLexer.Tokenize();
            Assert.That(tokens, Is.Not.Null);
            bool hasErrors = tokens.Any(t => t.Type == TokenType.Error);
            Assert.That(hasErrors, Is.False, "Lexer failed to parse valid Java code and threw an error token.");
        }
    }
}
