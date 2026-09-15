using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class GroupRemovalTest : TestBase
    {
        [Test]
        public void TheDeletingAGroupTest()
        {
            app.Navigator.OpenGroupsPage();

            if (!app.Groups.IsThereAGroup())
            {
                app.Groups.Create(new GroupData("group for removal"));
            }

            app.Groups.Remove(0);
        }
    }
}
