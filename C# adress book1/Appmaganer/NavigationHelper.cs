using OpenQA.Selenium;

namespace addressbook_tests
{
    public class NavigationHelper : HelperBase
    {
        private readonly string baseUrl;

        public NavigationHelper(IWebDriver driver, string baseUrl) : base(driver)
        {
            this.baseUrl = baseUrl;
        }

        public void OpenHomePage()
        {
            driver.Navigate().GoToUrl(baseUrl + "/addressbook/");
        }

        public void OpenGroupsPage()
        {
            driver.FindElement(By.LinkText("groups")).Click();
        }

        public void ReturnToGroupsPage()
        {
            driver.FindElement(By.LinkText("group page")).Click();
        }

        public void ReturnToHomePage()
        {
            driver.FindElement(By.LinkText("home page")).Click();
        }
    }
}
