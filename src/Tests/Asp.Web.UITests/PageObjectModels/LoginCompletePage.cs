using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace Asp.Web.UITests.PageObjectModels
{
    internal class LoginCompletePage
    {
        public IWebDriver Driver { get; }

        public LoginCompletePage(IWebDriver driver)
        {
            Driver = driver;

            PageFactory.InitElements(driver, this);
        }
    }
}