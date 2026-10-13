using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class Group_creation_tests : AuthTestBase
    {
        public static IEnumerable<GroupData> RandomDataProvider()
        {
            List<GroupData> groups = new List<GroupData>();
            for (int i = 0; i < 5; i++)
            {
                groups.Add(new GroupData(GenerateRandomString(30))
                {
                    Header = GenerateRandomString(100),
                    Footer = GenerateRandomString(100)
                });
            }

            return groups;
        }

        [TestCaseSource(nameof(RandomDataProvider))]
        public void GroupCreationTest(GroupData group)
        {
            app.Navigator.OpenGroupsPage();

            List<GroupData> oldGroups = app.Groups.GetGroupList();

            app.Groups.Create(group);

            NUnit.Framework.Assert.That(app.Groups.GetGroupCount(), Is.EqualTo(oldGroups.Count + 1));

            List<GroupData> newGroups = app.Groups.GetGroupList();
            oldGroups.Add(group);
            oldGroups.Sort();
            newGroups.Sort();

            NUnit.Framework.Assert.That(newGroups, Is.EqualTo(oldGroups));
        }

        [Test]
        public void EmptyGroupCreationTest()
        {
            GroupData group = new GroupData("")
            {
                Header = "",
                Footer = ""
            };

            app.Navigator.OpenGroupsPage();

            List<GroupData> oldGroups = app.Groups.GetGroupList();

            app.Groups.Create(group);

            NUnit.Framework.Assert.That(app.Groups.GetGroupCount(), Is.EqualTo(oldGroups.Count + 1));

            List<GroupData> newGroups = app.Groups.GetGroupList();
            oldGroups.Add(group);
            oldGroups.Sort();
            newGroups.Sort();

            NUnit.Framework.Assert.That(newGroups, Is.EqualTo(oldGroups));
        }
    }
}
