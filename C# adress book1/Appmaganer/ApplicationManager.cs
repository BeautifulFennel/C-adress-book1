using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace addressbook_tests
{
    public class ApplicationManager
    {
        private const string BaseUrl = "http://localhost";
        private readonly IWebDriver driver;

        public ApplicationManager()
        {
            driver = new FirefoxDriver();
            Auth = new LoginHelper(driver);
            Navigator = new NavigationHelper(driver, BaseUrl);
            Groups = new GroupHelper(driver);
            Contacts = new ContactHelper(driver);
        }

        public LoginHelper Auth { get; }

        public NavigationHelper Navigator { get; }

        public GroupHelper Groups { get; }

        public ContactHelper Contacts { get; }

        public void Stop()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }
    }
}
