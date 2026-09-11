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
            app.Groups.RemoveSelectedGroup();
        }
    }
}
