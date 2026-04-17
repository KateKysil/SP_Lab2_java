using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_Lab2_java.Lab3
{
    [TestFixture]
    public class WebFormTests
    {
        private IWebDriver _driver;
        private readonly string _formUrl = "https://www.selenium.dev/selenium/web/web-form.html";

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            _driver.Manage().Window.Maximize();
        }

        [TearDown]
        public void Teardown()
        {
            _driver.Quit();
        }

        [Test]
        public void Test_SubmitForm_WithValidText_ReturnsSuccessMessage()
        {
            _driver.Navigate().GoToUrl(_formUrl);
            IWebElement textInput = _driver.FindElement(By.Id("my-text-id"));
            textInput.SendKeys("Привіт, Selenium!");
            IWebElement submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
            submitButton.Click();
            IWebElement messageElement = _driver.FindElement(By.Id("message"));
            Assert.That(messageElement.Text, Is.EqualTo("Received!"));
            Assert.That(_driver.Url, Does.Contain("submitted-form.html"));
        }

        [Test]
        public void Test_CheckboxesAndRadioButtons_StateChangesOnClick()
        {
            _driver.Navigate().GoToUrl(_formUrl);
            IWebElement checkbox2 = _driver.FindElement(By.Id("my-check-2"));
            checkbox2.Click();
            IWebElement radio2 = _driver.FindElement(By.Id("my-radio-2"));
            radio2.Click();

            Assert.That(checkbox2.Selected, Is.True, "Чекбокс 2 мав бути відміченим.");
            Assert.That(radio2.Selected, Is.True, "Радіокнопка 2 мала бути обраною.");
        }

        [Test]
        public void Test_SelectDropdownAndDate_ValuesAreSetCorrectly()
        {
            _driver.Navigate().GoToUrl(_formUrl);
            IWebElement selectElement = _driver.FindElement(By.Name("my-select"));
            SelectElement dropdown = new SelectElement(selectElement);
            dropdown.SelectByText("Two"); 
            IWebElement datePicker = _driver.FindElement(By.Name("my-date"));
            datePicker.SendKeys("10/24/2023");

            Assert.That(dropdown.SelectedOption.Text, Is.EqualTo("Two"));
            Assert.That(datePicker.GetAttribute("value"), Is.EqualTo("10/24/2023"));
        }
    }
}
