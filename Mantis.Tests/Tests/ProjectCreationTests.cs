using NUnit.Framework;

namespace mantis_tests;

[TestFixture]
public sealed class ProjectCreationTests : TestBase
{
    [TestCase("Mantis project", "Project created by the UI test.")]
    [TestCase("Проект & QA", "Описание проекта: кириллица, <теги> и \"кавычки\".")]
    public void TestProjectCreation(string name, string description)
    {
        ProjectData project = NewProject(name, description);
        List<ProjectData> oldProjects = app.Projects.GetProjectList();

        ProjectData created = app.Projects.Create(project);

        List<ProjectData> newProjects = app.Projects.GetProjectList();
        Assert.That(newProjects.Count, Is.EqualTo(oldProjects.Count + 1));
        Assert.That(created.Id, Is.Not.Null.And.GreaterThan(0));
        Assert.That(oldProjects.Any(existing => existing.Id == created.Id), Is.False);
        oldProjects.Add(project with { Id = created.Id });
        Assert.That(newProjects.Select(item => (item.Id, item.Name)),
            Is.EquivalentTo(oldProjects.Select(item => (item.Id, item.Name))));
        Assert.That(app.Projects.GetProject(created.Id!.Value), Is.EqualTo(project with { Id = created.Id }));
    }
}
