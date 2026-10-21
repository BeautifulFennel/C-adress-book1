using OpenQA.Selenium;

namespace mantis_tests;

public sealed class LoginHelper : HelperBase
{
    private static readonly By LogoutLink = By.CssSelector("a[href*='logout_page.php']");
    private static readonly By Submit = By.CssSelector("input[type='submit'], button[type='submit']");

    public LoginHelper(ApplicationManager manager) : base(manager) { }

    public void Login(AccountData account)
    {
        OpenPage("login_page.php");
        if (driver.FindElements(LogoutLink).Count > 0)
        {
            Logout();
        }

        Type(By.Name("username"), account.Username);
        // MantisBT 2.x запрашивает пароль на втором экране.
        // В старых версиях оба поля находятся на одной странице.
        if (!driver.FindElements(By.Name("password")).Any(element => element.Displayed))
        {
            Click(Submit);
        }

        Type(By.Name("password"), account.Password);
        Click(Submit);
        wait.Until(browser => browser.FindElements(LogoutLink).Count > 0);
    }

    public void Logout()
    {
        OpenPage("logout_page.php");
        FindVisible(By.Name("username"));
    }
}
