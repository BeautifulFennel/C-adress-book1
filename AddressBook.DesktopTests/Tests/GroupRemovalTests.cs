using System.Collections.Generic;
using NUnit.Framework;

namespace addressbook_tests_white
{
    [TestFixture]
    public class GroupRemovalTests : TestBase
    {
        [Test]
        public void TestGroupRemoval()
        {
            // Тест не зависит от запуска тестов создания и удаляет только свою группу.
            GroupData group = NewGroup("For removal");
            app.Groups.Add(group);
            List<GroupData> oldGroups = app.Groups.GetGroupList();

            app.Groups.Remove(group);

            List<GroupData> newGroups = app.Groups.GetGroupList();
            Assert.That(newGroups.Count, Is.EqualTo(oldGroups.Count - 1));
            oldGroups.Remove(group);
            Assert.That(newGroups, Is.EquivalentTo(oldGroups));
        }
    }
}
