using NUnit.Framework;

namespace mantis_tests;

[TestFixture]
public sealed class ProjectRemovalTests : TestBase
{
    [Test]
    public void TestProjectRemoval()
    {
        // Собственное предусловие: тест работает и при пустом списке проектов.
        ProjectData retainedProject = app.Projects.Create(NewProject("must remain", "Контрольный проект должен сохраниться."));
        ProjectData project = app.Projects.Create(NewProject("for removal", "Удаляется только этот тестовый проект."));
        List<ProjectData> oldProjects = app.Projects.GetProjectList();
        Assert.That(oldProjects.Count(item => item.Id == project.Id), Is.EqualTo(1));

        app.Projects.Remove(project.Id!.Value);

        List<ProjectData> newProjects = app.Projects.GetProjectList();
        Assert.That(newProjects.Count, Is.EqualTo(oldProjects.Count - 1));
        Assert.That(newProjects.Any(item => item.Id == project.Id), Is.False);
        Assert.That(newProjects.Any(item => item.Id == retainedProject.Id), Is.True);
        oldProjects.RemoveAll(item => item.Id == project.Id);
        Assert.That(newProjects, Is.EquivalentTo(oldProjects));
    }
}
