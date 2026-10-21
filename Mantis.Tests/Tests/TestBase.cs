using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;

namespace mantis_tests;

[NonParallelizable]
public abstract class TestBase
{
    protected ApplicationManager app = null!;
    private readonly List<string> ownedProjectNames = new();

    [SetUp]
    public void SetUp()
    {
        ownedProjectNames.Clear();
        app = null!;
        app = new ApplicationManager(TestConfiguration.Load());
        try
        {
            app.Login.Login(app.Configuration.Account);
        }
        catch
        {
            SaveFailureScreenshot();
            app.Dispose();
            app = null!;
            throw;
        }
    }

    protected ProjectData NewProject(string name, string description = "")
    {
        ProjectData project = new($"hw20-{name}-{Guid.NewGuid():N}", description);
        ownedProjectNames.Add(project.Name);
        return project;
    }

    [TearDown]
    public void TearDown()
    {
        if (app is null)
        {
            return;
        }

        try
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                SaveFailureScreenshot();
            }

            // Удаляем только проекты с уникальными именами, созданными этим тестом.
            List<ProjectData> projects = app.Projects.GetProjectList();
            foreach (ProjectData project in projects.Where(project => ownedProjectNames.Contains(project.Name)))
            {
                app.Projects.Remove(project.Id!.Value);
            }
        }
        finally
        {
            app.Dispose();
        }
    }

    private void SaveFailureScreenshot()
    {
        try
        {
            string directory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults", "Screenshots");
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, $"failure-{Guid.NewGuid():N}.png");
            ((ITakesScreenshot)app.Driver).GetScreenshot().SaveAsFile(path);
            TestContext.AddTestAttachment(path, "Состояние браузера при ошибке");
        }
        catch (Exception error)
        {
            TestContext.Progress.WriteLine($"Не удалось сохранить скриншот: {error.Message}");
        }
    }
}
