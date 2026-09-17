using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace addressbook_tests
{
    public class ApplicationManager
    {
        private const string BaseUrl = "http://localhost";
        private static readonly object SyncRoot = new object();
        private static ApplicationManager? instance;
        private readonly IWebDriver driver;
        private bool isStopped;

        private ApplicationManager()
        {
            driver = new FirefoxDriver();
            Auth = new LoginHelper(this);
            Navigator = new NavigationHelper(this, BaseUrl);
            Groups = new GroupHelper(this);
            Contacts = new ContactHelper(this);
        }

        ~ApplicationManager()
        {
            Stop();
        }

        public static ApplicationManager GetInstance()
        {
            lock (SyncRoot)
            {
                instance ??= new ApplicationManager();
                return instance;
            }
        }

        public IWebDriver Driver => driver;

        public LoginHelper Auth { get; }

        public NavigationHelper Navigator { get; }

        public GroupHelper Groups { get; }

        public ContactHelper Contacts { get; }

        public void Stop()
        {
            if (isStopped)
            {
                return;
            }

            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            finally
            {
                isStopped = true;
            }
        }
    }
}
