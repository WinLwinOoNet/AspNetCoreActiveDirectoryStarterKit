using System;
using Asp.Web.UITests.PageObjectModels;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace Asp.Web.UITests
{
    public class LoginTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly LoginPage _loginPage;

        public LoginTests()
        {
            _driver = new ChromeDriver();

            _driver.Navigate().GoToUrl("http://localhost:51397/");

            _loginPage = new LoginPage(_driver);
            _loginPage.NavigateTo();
        }

        [Fact]
        public void LoadLoginPage_SmokeTest()
        {
            Assert.Equal("Login", _loginPage.Driver.Title);
        }

        [Fact]
        public void LoginPage_ValidateUserNameAndPassword()
        {
            _loginPage.EnterUserName("");
            _loginPage.EnterPassword("");
            _loginPage.SubmitApplication();

            Assert.Equal("Login", _loginPage.Driver.Title);
            Assert.Equal("Please enter your username.", _loginPage.UserNameErrorMessage);
            Assert.Equal("Please enter your password.", _loginPage.PasswordErrorMessage);
        }

        [Fact]
        public void LoginPage_RediretToDashboardAfterSuccessfulLogin()
        {
            _loginPage.SelectDomain("domain2.com");
            _loginPage.EnterUserName("johndoe");
            _loginPage.EnterPassword("Sw@*V9Rk");

            LoginCompletePage loginCompletePage = _loginPage.SubmitApplication();
            Assert.Equal("Dashboard", loginCompletePage.Driver.Title);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
