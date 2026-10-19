using System.Collections.Generic;
using NUnit.Framework;

namespace addressbook_tests_white
{
    [TestFixture]
    public class GroupCreationTests : TestBase
    {
        [TestCase("White")]
        [TestCase("Тестовая группа")]
        public void TestGroupCreation(string name)
        {
            List<GroupData> oldGroups = app.Groups.GetGroupList();
            GroupData newGroup = NewGroup(name);

            app.Groups.Add(newGroup);

            List<GroupData> newGroups = app.Groups.GetGroupList();
            Assert.That(newGroups.Count, Is.EqualTo(oldGroups.Count + 1));
            oldGroups.Add(newGroup);
            Assert.That(newGroups, Is.EquivalentTo(oldGroups));
        }
    }
}
