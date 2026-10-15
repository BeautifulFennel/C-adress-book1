using NUnit.Framework;
using System.Text.Json;
using System.Xml.Serialization;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactCreationTests : ContactTestBase
    {
        public static IEnumerable<ContactData> ContactDataFromXmlFile()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "contacts.xml");
            using StreamReader reader = new StreamReader(fileName);

            return (List<ContactData>?)new XmlSerializer(typeof(List<ContactData>)).Deserialize(reader)
                ?? throw new InvalidDataException($"Файл '{fileName}' не содержит список контактов.");
        }

        public static IEnumerable<ContactData> ContactDataFromJsonFile()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "contacts.json");

            return JsonSerializer.Deserialize<List<ContactData>>(File.ReadAllText(fileName))
                ?? throw new InvalidDataException($"Файл '{fileName}' не содержит список контактов.");
        }

        [TestCaseSource(nameof(ContactDataFromXmlFile), Category = "XML")]
        [TestCaseSource(nameof(ContactDataFromJsonFile), Category = "JSON")]
        public void ContactCreationTest(ContactData contact)
        {
            app.Navigator.OpenHomePage();

            List<ContactData> oldContacts = ContactData.GetAll();

            app.Contacts.Create(contact);

            NUnit.Framework.Assert.That(app.Contacts.GetContactCount(), Is.EqualTo(oldContacts.Count + 1));

            List<ContactData> newContacts = ContactData.GetAll();
            contact.Id = newContacts.Single(c => oldContacts.All(old => old.Id != c.Id)).Id;
            oldContacts.Add(contact);
            AssertContactsEqual(newContacts, oldContacts);
        }
    }
}
