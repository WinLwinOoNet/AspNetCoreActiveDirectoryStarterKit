using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace Asp.Web.UITests.PageObjectModels
{
    internal class LoginPage
    {
        public IWebDriver Driver { get; }

        private const string PagePath = "account/login";

        public LoginPage(IWebDriver driver)
        {
            Driver = driver;

            PageFactory.InitElements(driver, this);
        }

        public void NavigateTo()
        {
            var root = new Uri(Driver.Url).GetLeftPart(UriPartial.Authority);

            var url = $"{root}/{PagePath}";

            Driver.Navigate().GoToUrl(url);
        }

        [FindsBy(How = How.Name, Using = "Domain")]
        private IWebElement Domain { get; set; }
        
        [FindsBy(How = How.Name, Using = "UserName")]
        private IWebElement UserName { get; set; }
        
        [FindsBy(How = How.Name, Using = "Password")]
        private IWebElement Password { get; set; }
        
        [FindsBy(How = How.Id, Using = "submit-button")]
        private IWebElement SubmitButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".field-validation-error #UserName-error")]
        private IWebElement UserNameError { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".field-validation-error #Password-error")]
        private IWebElement PasswordError { get; set; }

        public string UserNameErrorMessage => UserNameError.Text;

        public string PasswordErrorMessage => PasswordError.Text;

        public void SelectDomain(string domain)
        {
            Domain.SendKeys(domain);
        }

        public void EnterUserName(string username)
        {
            UserName.SendKeys(username);
        }

        public void EnterPassword(string password)
        {
            Password.SendKeys(password);
        }

        public LoginCompletePage SubmitApplication()
        {
            SubmitButton.Click();

            return new LoginCompletePage(Driver);
        }
    }
}