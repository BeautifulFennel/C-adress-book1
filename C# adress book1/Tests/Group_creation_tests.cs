using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class Group_creation_tests : AuthTestBase
    {
        [Test]
        public void GroupCreationTest()
        {
            GroupData group = new GroupData("new group")
            {
                Header = "new header",
                Footer = "new footer"
            };

            app.Navigator.OpenGroupsPage();

            List<GroupData> oldGroups = app.Groups.GetGroupList();

            app.Groups.Create(group);

            List<GroupData> newGroups = app.Groups.GetGroupList();
            oldGroups.Add(group);
            oldGroups.Sort();
            newGroups.Sort();

            NUnit.Framework.Assert.That(newGroups, Is.EqualTo(oldGroups));
        }

        [Test]
        public void EmptyGroupCreationTest()
        {
            GroupData group = new GroupData("")
            {
                Header = "",
                Footer = ""
            };

            app.Navigator.OpenGroupsPage();

            List<GroupData> oldGroups = app.Groups.GetGroupList();

            app.Groups.Create(group);

            List<GroupData> newGroups = app.Groups.GetGroupList();
            oldGroups.Add(group);
            oldGroups.Sort();
            newGroups.Sort();

            NUnit.Framework.Assert.That(newGroups, Is.EqualTo(oldGroups));
        }
    }
}
