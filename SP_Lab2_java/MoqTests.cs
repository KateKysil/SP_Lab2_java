using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;

namespace SP_Lab2_java.Tests
{
    [TestFixture]
    public class MockTests
    {
        private Mock<IFileReader> _mockReader;
        private CodeAnalyzer _analyzer;

        [SetUp]
        public void Setup()
        {
            _mockReader = new Mock<IFileReader>();
            _analyzer = new CodeAnalyzer(_mockReader.Object);
        }

        [Test]
        public void Test_Analyze_CorrectOrder()
        {
            _mockReader.Setup(r => r.ReadCode("Test.java")).Returns("int x = 5;");
            var sequence = new MockSequence();
            _mockReader.InSequence(sequence).Setup(r => r.LogStatus("Розпочато зчитування"));
            _mockReader.InSequence(sequence).Setup(r => r.ReadCode("Test.java")).Returns("int x = 5;");
            _mockReader.InSequence(sequence).Setup(r => r.LogStatus("Зчитування завершено"));
            var tokens = _analyzer.Analyze("Test.java");
            _mockReader.Verify(r => r.ReadCode("Test.java"), Times.Once);
            _mockReader.Verify(r => r.LogStatus(It.IsAny<string>()), Times.Exactly(2));

            Assert.That(tokens, Is.Not.Null);
        }
        [Test]
        public void Test_Analyze_ExceptionForSecret()
        {
            _mockReader.Setup(r => r.ReadCode(It.Is<string>(path => path.EndsWith(".secret"))))
                       .Throws(new UnauthorizedAccessException("Доступ заборонено!"));
            Assert.Throws<UnauthorizedAccessException>(() => _analyzer.Analyze("passwords.secret"));
            _mockReader.Verify(r => r.LogStatus("Розпочато зчитування"), Times.Once);
            _mockReader.Verify(r => r.LogStatus("Зчитування завершено"), Times.Never);
        }
        [Test]
        public void Test_Analyze_DifferentAnswersForOne()
        {
            _mockReader.SetupSequence(r => r.ReadCode("Retry.java"))
                       .Returns("")  
                       .Returns("int a = 1;");  
            Assert.Throws<ArgumentException>(() => _analyzer.Analyze("Retry.java"));
            var tokens = _analyzer.Analyze("Retry.java");
            Assert.That(tokens, Is.Not.Null);
            _mockReader.Verify(r => r.ReadCode("Retry.java"), Times.Exactly(2));
        }
    }
}