using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class GroupModificationTests : AuthTestBase
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

            const int index = 0;
            List<GroupData> oldGroups = app.Groups.GetGroupList();
            // Сохраняем Id до изменения ожидаемого списка и его сортировки.
            string? modifiedGroupId = oldGroups[index].Id;
            NUnit.Framework.Assert.That(modifiedGroupId, Is.Not.Null.And.Not.Empty);

            app.Groups.Modify(index, group);

            NUnit.Framework.Assert.That(app.Groups.GetGroupCount(), Is.EqualTo(oldGroups.Count));

            List<GroupData> newGroups = app.Groups.GetGroupList();
            oldGroups[index].Name = group.Name;
            oldGroups.Sort();
            newGroups.Sort();

            NUnit.Framework.Assert.That(newGroups, Is.EqualTo(oldGroups));

            GroupData? modifiedGroup = newGroups.Find(item => item.Id == modifiedGroupId);
            NUnit.Framework.Assert.That(modifiedGroup, Is.Not.Null,
                $"Группа с Id={modifiedGroupId} не найдена после изменения.");
            NUnit.Framework.Assert.That(modifiedGroup!.Name, Is.EqualTo(group.Name));
        }
    }
}
