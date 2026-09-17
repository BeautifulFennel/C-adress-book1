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
            app.Groups.Create(group);
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
            app.Groups.Create(group);
        }
    }
}
