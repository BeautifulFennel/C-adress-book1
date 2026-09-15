using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class GroupModificationTests : TestBase
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

            app.Groups.Modify(0, group);
        }
    }
}
