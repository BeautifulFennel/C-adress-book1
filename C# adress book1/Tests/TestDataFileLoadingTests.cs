using NUnit.Framework;
using System.Text.Json;
using System.Xml.Linq;
using Assert = NUnit.Framework.Assert;

namespace addressbook_tests
{
    [TestFixture]
    public class TestDataFileLoadingTests
    {
        [TestCase("xml")]
        [TestCase("json")]
        public void GroupSourceLoadsAllFieldsFromGeneratedFile(string format)
        {
            List<GroupData> actual = (format == "xml"
                ? Group_creation_tests.GroupDataFromXmlFile()
                : Group_creation_tests.GroupDataFromJsonFile()).ToList();
            List<Dictionary<string, string?>> expected = ReadRawRecords("groups." + format);

            Assert.That(actual, Has.Count.EqualTo(expected.Count));
            for (int i = 0; i < actual.Count; i++)
            {
                GroupData group = actual[i];
                Dictionary<string, string?> record = expected[i];
                Assert.Multiple(() =>
                {
                    Assert.That(group.Name, Is.EqualTo(record["Name"]));
                    Assert.That(group.Header, Is.EqualTo(record["Header"]));
                    Assert.That(group.Footer, Is.EqualTo(record["Footer"]));
                    Assert.That(group.Id, Is.EqualTo(record.GetValueOrDefault("Id")));
                });
            }
        }

        [TestCase("xml")]
        [TestCase("json")]
        public void ContactSourceLoadsAllFieldsFromGeneratedFile(string format)
        {
            List<ContactData> actual = (format == "xml"
                ? ContactCreationTests.ContactDataFromXmlFile()
                : ContactCreationTests.ContactDataFromJsonFile()).ToList();
            List<Dictionary<string, string?>> expected = ReadRawRecords("contacts." + format);

            Assert.That(actual, Has.Count.EqualTo(expected.Count));
            for (int i = 0; i < actual.Count; i++)
            {
                ContactData contact = actual[i];
                Dictionary<string, string?> record = expected[i];
                Assert.Multiple(() =>
                {
                    Assert.That(contact.FirstName, Is.EqualTo(record["FirstName"]));
                    Assert.That(contact.MiddleName, Is.EqualTo(record["MiddleName"]));
                    Assert.That(contact.LastName, Is.EqualTo(record["LastName"]));
                    Assert.That(contact.Nickname, Is.EqualTo(record["Nickname"]));
                    Assert.That(contact.Title, Is.EqualTo(record["Title"]));
                    Assert.That(contact.Company, Is.EqualTo(record["Company"]));
                    Assert.That(contact.Address, Is.EqualTo(record["Address"]));
                    Assert.That(contact.HomePhone, Is.EqualTo(record["HomePhone"]));
                    Assert.That(contact.MobilePhone, Is.EqualTo(record["MobilePhone"]));
                    Assert.That(contact.WorkPhone, Is.EqualTo(record["WorkPhone"]));
                    Assert.That(contact.Email, Is.EqualTo(record["Email"]));
                    Assert.That(contact.Email2, Is.EqualTo(record["Email2"]));
                    Assert.That(contact.Email3, Is.EqualTo(record["Email3"]));
                    Assert.That(contact.Homepage, Is.EqualTo(record["Homepage"]));
                    Assert.That(contact.BirthDay, Is.EqualTo(record["BirthDay"]));
                    Assert.That(contact.BirthMonth, Is.EqualTo(record["BirthMonth"]));
                    Assert.That(contact.BirthYear, Is.EqualTo(record["BirthYear"]));
                    Assert.That(contact.AnniversaryDay, Is.EqualTo(record["AnniversaryDay"]));
                    Assert.That(contact.AnniversaryMonth, Is.EqualTo(record["AnniversaryMonth"]));
                    Assert.That(contact.AnniversaryYear, Is.EqualTo(record["AnniversaryYear"]));
                });
            }
        }

        // Проверяем исходные поля документа отдельно от десериализации моделей.
        // Количество берём из файла, чтобы тест работал и после новой генерации данных.
        private static List<Dictionary<string, string?>> ReadRawRecords(string fileName)
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", fileName);
            if (Path.GetExtension(path) == ".json")
            {
                using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
                return document.RootElement.EnumerateArray()
                    .Select(record => record.EnumerateObject()
                        .ToDictionary(field => field.Name, field => field.Value.GetString()))
                    .ToList();
            }

            return XDocument.Load(path).Root!.Elements()
                .Select(record => record.Elements()
                    .ToDictionary(field => field.Name.LocalName, field => (string?)field.Value))
                .ToList();
        }
    }
}
