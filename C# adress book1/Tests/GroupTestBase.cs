using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace addressbook_tests
{
    public class GroupTestBase : AuthTestBase
    {
        [TearDown]
        public void CompareGroupsUI_DB()
        {
            if (!PERFORM_LONG_UI_CHECKS)
            {
                return;
            }

            List<GroupData> fromUI = app.Groups.GetGroupList(forceReload: true);
            List<GroupData> fromDB = GroupData.GetAll();

            // На странице списка отображаются Id и название группы.
            Assert.That(fromUI.OrderBy(g => g.Id).Select(g => (g.Id, g.Name)),
                Is.EqualTo(fromDB.OrderBy(g => g.Id).Select(g => (g.Id, g.Name))),
                "Список групп в UI не совпадает с БД.");
            TestContext.WriteLine("UI/БД: список групп совпадает.");
        }

        protected static void AssertGroupsEqual(List<GroupData> actual, List<GroupData> expected)
        {
            Assert.That(actual.OrderBy(g => g.Id).Select(g => (g.Id, g.Name, g.Header, g.Footer)),
                Is.EqualTo(expected.OrderBy(g => g.Id).Select(g => (g.Id, g.Name, g.Header, g.Footer))));
        }
    }
}
