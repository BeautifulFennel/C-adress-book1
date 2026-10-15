using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class GroupRemovalTest : GroupTestBase
    {
        [Test]
        public void TheDeletingAGroupTest()
        {
            app.Navigator.OpenGroupsPage();

            if (GroupData.GetAll().Count == 0)
            {
                app.Groups.Create(new GroupData("group for removal"));
            }

            const int index = 0;
            List<GroupData> oldGroups = GroupData.GetAll();

            app.Groups.Remove(oldGroups[index].Id!);

            NUnit.Framework.Assert.That(app.Groups.GetGroupCount(), Is.EqualTo(oldGroups.Count - 1));

            List<GroupData> newGroups = GroupData.GetAll();
            oldGroups.RemoveAt(index);
            AssertGroupsEqual(newGroups, oldGroups);
        }
    }
}
