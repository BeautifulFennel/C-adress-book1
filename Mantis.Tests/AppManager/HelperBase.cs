using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace mantis_tests;

public abstract class HelperBase
{
    protected readonly ApplicationManager manager;
    protected readonly IWebDriver driver;
    protected readonly WebDriverWait wait;

    protected HelperBase(ApplicationManager manager)
    {
        this.manager = manager;
        driver = manager.Driver;
        wait = new WebDriverWait(driver, manager.Configuration.Timeout);
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
    }

    protected IWebElement FindVisible(By locator)
    {
        return wait.Until(browser => browser.FindElements(locator)
            .FirstOrDefault(element => element.Displayed && element.Enabled))!;
    }

    protected void Click(By locator) => FindVisible(locator).Click();

    protected void Type(By locator, string text)
    {
        IWebElement element = FindVisible(locator);
        element.Clear();
        element.SendKeys(text);
    }

    protected void OpenPage(string path)
    {
        driver.Navigate().GoToUrl(new Uri(manager.Configuration.BaseUrl, path));
    }
}
