using System.Globalization;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace mantis_tests;

public sealed class ProjectManagementHelper : HelperBase
{
    private static readonly By ProjectLinks = By.CssSelector("a[href*='manage_proj_edit_page.php?project_id=']");
    private static readonly By CreateSubmit = By.CssSelector("form[action$='manage_proj_create.php'] input[type='submit'], form[action$='manage_proj_create.php'] button");
    private const string DeleteSubmitSelector = "form[action$='manage_proj_delete.php'] input[type='submit'], form[action$='manage_proj_delete.php'] button, button[formaction$='manage_proj_delete.php']";

    public ProjectManagementHelper(ApplicationManager manager) : base(manager) { }

    public ProjectData Create(ProjectData project)
    {
        manager.ManagementMenu.OpenProjects();
        List<ProjectData> before = ReadProjectList();
        Click(By.CssSelector("form[action$='manage_proj_create_page.php'] input[type='submit'], form[action$='manage_proj_create_page.php'] button"));
        FillProjectForm(project);
        Click(CreateSubmit);
        WaitForOperationResult("manage_proj_create.php");

        List<ProjectData> after = GetProjectList();
        return after.Single(candidate => candidate.Name == project.Name
            && before.All(existing => existing.Id != candidate.Id)) with { Description = project.Description };
    }

    public void Remove(int projectId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(projectId);
        manager.ManagementMenu.OpenProjects();
        IWebElement link = driver.FindElements(ProjectLinks).First(element => ParseId(element) == projectId);
        link.Click();
        Click(By.CssSelector(DeleteSubmitSelector));
        // Удаление подтверждается отдельной HTML-формой, а не JavaScript alert.
        wait.Until(browser => browser.FindElements(By.Name("_confirmed")).Count > 0);
        Click(By.CssSelector("form:has(input[name='_confirmed']) input[type='submit'], form:has(input[name='_confirmed']) button"));
        WaitForOperationResult("manage_proj_delete.php");
        manager.ManagementMenu.OpenProjects();
        wait.Until(_ => ReadProjectList().All(project => project.Id != projectId));
    }

    public List<ProjectData> GetProjectList()
    {
        manager.ManagementMenu.OpenProjects();
        return ReadProjectList();
    }

    public ProjectData GetProject(int projectId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(projectId);
        manager.ManagementMenu.OpenProjects();
        driver.FindElements(ProjectLinks).First(element => ParseId(element) == projectId).Click();
        return new ProjectData(
            FindVisible(By.Name("name")).GetAttribute("value") ?? "",
            FindVisible(By.Name("description")).GetAttribute("value") ?? "") { Id = projectId };
    }

    private void FillProjectForm(ProjectData project)
    {
        Type(By.Name("name"), project.Name);
        Type(By.Name("description"), project.Description);
    }

    private List<ProjectData> ReadProjectList()
    {
        return driver.FindElements(ProjectLinks)
            .Select(element => new ProjectData(element.Text.Trim()) { Id = ParseId(element) })
            .DistinctBy(project => project.Id)
            .ToList();
    }

    private static int ParseId(IWebElement link)
    {
        Match match = Regex.Match(link.GetAttribute("href") ?? "", @"[?&]project_id=(\d+)");
        return int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
    }

    private void WaitForOperationResult(string action)
    {
        // Ожидаем ответа POST перед следующим переходом, не перекрывая сохранение.
        wait.Until(browser => browser.FindElements(By.CssSelector("div.alert-success, div.alert-danger, .error-info")).Count > 0
            || new Uri(browser.Url).AbsolutePath.EndsWith("/manage_proj_page.php", StringComparison.Ordinal));
        if (driver.FindElements(By.CssSelector("div.alert-danger, .error-info")).Count > 0)
        {
            throw new InvalidOperationException($"MantisBT отклонил операцию {action}: {driver.FindElement(By.TagName("body")).Text}");
        }
    }
}
