using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class GroupRemovalTest : TestBase
    {
        [Test]
        public void TheDeletingAGroupTest()
        {
            app.Navigator.OpenHomePage();
            app.Auth.Login("admin", "secret");
            app.Navigator.OpenGroupsPage();
            app.Groups.SelectGroup();
            app.Groups.RemoveSelectedGroups();
            app.Navigator.ReturnToGroupsPage();
            app.Auth.Logout();
        }
    }
}
