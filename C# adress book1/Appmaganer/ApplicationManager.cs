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
            Auth = new LoginHelper(this);
            Navigator = new NavigationHelper(this, BaseUrl);
            Groups = new GroupHelper(this);
            Contacts = new ContactHelper(this);
        }

        public IWebDriver Driver => driver;

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
