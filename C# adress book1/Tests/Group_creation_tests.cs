using NUnit.Framework;
using System.Text.Json;
using System.Xml.Serialization;

namespace addressbook_tests
{
    [TestFixture]
    public class Group_creation_tests : AuthTestBase
    {
        public static IEnumerable<GroupData> GroupDataFromXmlFile()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "groups.xml");
            using StreamReader reader = new StreamReader(fileName);

            return (List<GroupData>?)new XmlSerializer(typeof(List<GroupData>)).Deserialize(reader)
                ?? throw new InvalidDataException($"Файл '{fileName}' не содержит список групп.");
        }

        public static IEnumerable<GroupData> GroupDataFromJsonFile()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "groups.json");

            return JsonSerializer.Deserialize<List<GroupData>>(File.ReadAllText(fileName))
                ?? throw new InvalidDataException($"Файл '{fileName}' не содержит список групп.");
        }

        [TestCaseSource(nameof(GroupDataFromXmlFile), Category = "XML")]
        [TestCaseSource(nameof(GroupDataFromJsonFile), Category = "JSON")]
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
