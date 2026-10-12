using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class GroupRemovalTest : AuthTestBase
    {
        [Test]
        public void TheDeletingAGroupTest()
        {
            app.Navigator.OpenGroupsPage();

            if (!app.Groups.IsThereAGroup())
            {
                app.Groups.Create(new GroupData("group for removal"));
            }

            const int index = 0;
            List<GroupData> oldGroups = app.Groups.GetGroupList();

            app.Groups.Remove(index);

            NUnit.Framework.Assert.That(app.Groups.GetGroupCount(), Is.EqualTo(oldGroups.Count - 1));

            List<GroupData> newGroups = app.Groups.GetGroupList();
            oldGroups.RemoveAt(index);
            oldGroups.Sort();
            newGroups.Sort();

            NUnit.Framework.Assert.That(newGroups, Is.EqualTo(oldGroups));
        }
    }
}
