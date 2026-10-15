using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class GroupModificationTests : GroupTestBase
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

            if (GroupData.GetAll().Count == 0)
            {
                app.Groups.Create(new GroupData("group for modification"));
            }

            const int index = 0;
            List<GroupData> oldGroups = GroupData.GetAll();
            // Сохраняем Id выбранной из БД группы для изменения через UI.
            string? modifiedGroupId = oldGroups[index].Id;
            NUnit.Framework.Assert.That(modifiedGroupId, Is.Not.Null.And.Not.Empty);

            app.Groups.Modify(modifiedGroupId!, group);

            NUnit.Framework.Assert.That(app.Groups.GetGroupCount(), Is.EqualTo(oldGroups.Count));

            List<GroupData> newGroups = GroupData.GetAll();
            group.Id = modifiedGroupId;
            oldGroups[index] = group;
            AssertGroupsEqual(newGroups, oldGroups);

            GroupData? modifiedGroup = newGroups.Find(item => item.Id == modifiedGroupId);
            NUnit.Framework.Assert.That(modifiedGroup, Is.Not.Null,
                $"Группа с Id={modifiedGroupId} не найдена после изменения.");
            NUnit.Framework.Assert.That(modifiedGroup!.Name, Is.EqualTo(group.Name));
        }
    }
}
