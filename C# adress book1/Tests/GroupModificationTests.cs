using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class GroupModificationTests : AuthTestBase
    {
        [Test]
        public void GroupModificationTest()
        {
            GroupData group = new GroupData("modified group")
            {
                Header = "modified header",
                Footer = "modified footer"
            };

            app.Navigator.OpenGroupsPage();

            if (!app.Groups.IsThereAGroup())
            {
                app.Groups.Create(new GroupData("group for modification"));
            }

            const int index = 0;
            List<GroupData> oldGroups = app.Groups.GetGroupList();

            app.Groups.Modify(index, group);

            List<GroupData> newGroups = app.Groups.GetGroupList();
            oldGroups[index] = group;
            oldGroups.Sort();
            newGroups.Sort();

            NUnit.Framework.Assert.That(newGroups, Is.EqualTo(oldGroups));
        }
    }
}
