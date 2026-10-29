using NUnit.Framework;

namespace mantis_tests;

[TestFixture]
public sealed class ProjectRemovalTests : TestBase
{
    [Test]
    public void TestProjectRemoval()
    {
        List<ProjectData> oldProjects = app.Projects.GetProjectList();
        if (oldProjects.Count == 0)
        {
            // Подготовка через SOAP; само удаление по-прежнему проверяем через UI.
            app.API.CreateProject(NewProject("for removal", "Проект создан через MantisConnect."));
            oldProjects = app.Projects.GetProjectList();
        }

        Assert.That(oldProjects, Is.Not.Empty, "Должен существовать проект для удаления.");
        ProjectData project = oldProjects[0];
        Assert.That(oldProjects.Count(item => item.Id == project.Id), Is.EqualTo(1));

        app.Projects.Remove(project.Id!.Value);

        List<ProjectData> newProjects = app.Projects.GetProjectList();
        Assert.That(newProjects.Count, Is.EqualTo(oldProjects.Count - 1));
        Assert.That(newProjects.Any(item => item.Id == project.Id), Is.False);
        oldProjects.RemoveAll(item => item.Id == project.Id);
        Assert.That(newProjects, Is.EquivalentTo(oldProjects));
    }
}
