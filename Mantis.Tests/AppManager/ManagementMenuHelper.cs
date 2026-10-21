using OpenQA.Selenium;

namespace mantis_tests;

public sealed class ManagementMenuHelper : HelperBase
{
    public ManagementMenuHelper(ApplicationManager manager) : base(manager) { }

    public void OpenProjects()
    {
        // Переходы по меню, локаторы не зависят от языка интерфейса.
        OpenPage("my_view_page.php");
        Click(By.CssSelector("a[href$='manage_overview_page.php'], a[href$='manage_proj_page.php']"));
        if (!IsProjectsPage())
        {
            Click(By.CssSelector("a[href$='manage_proj_page.php']"));
        }

        wait.Until(_ => IsProjectsPage());
        FindVisible(By.CssSelector("form[action$='manage_proj_create_page.php']"));
    }

    private bool IsProjectsPage() => new Uri(driver.Url).AbsolutePath.EndsWith("/manage_proj_page.php", StringComparison.Ordinal);
}
