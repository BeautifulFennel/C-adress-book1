using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class Group_creation_tests : TestBase
    {
        [Test]
        public void GroupCreationTest()
        {
            GroupData group = new GroupData("new group")
            {
                Header = "new header",
                Footer = "new footer"
            };

            app.Navigator.OpenHomePage();
            app.Auth.Login("admin", "secret");
            app.Navigator.OpenGroupsPage();
            app.Groups.InitGroupCreation();
            app.Groups.FillGroupForm(group);
            app.Groups.SubmitGroupCreation();
            app.Navigator.ReturnToGroupsPage();
            app.Auth.Logout();
        }
    }
}
