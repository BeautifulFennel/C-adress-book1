using NUnit.Framework;
using System.Text.Json;
using System.Xml.Serialization;

namespace addressbook_tests
{
    [TestFixture]
    public class TestDataSerializationTests
    {
        [TestCase("xml")]
        [TestCase("json")]
        public void GroupsCanBeRestoredFromSerializedList(string format)
        {
            List<GroupData> original = new List<GroupData>
            {
                new GroupData("Группа <&> \"test\"")
                {
                    Header = "Первая строка\nВторая строка",
                    Footer = "footer \\ / ' &",
                    Id = "123"
                },
                new GroupData("")
            };

            List<GroupData> restored = RoundTrip(original, format);

            NUnit.Framework.Assert.Multiple(() =>
            {
                NUnit.Framework.Assert.That(restored, Has.Count.EqualTo(2));
                NUnit.Framework.Assert.That(restored[0].Name, Is.EqualTo(original[0].Name));
                NUnit.Framework.Assert.That(restored[0].Header, Is.EqualTo(original[0].Header));
                NUnit.Framework.Assert.That(restored[0].Footer, Is.EqualTo(original[0].Footer));
                NUnit.Framework.Assert.That(restored[0].Id, Is.EqualTo(original[0].Id));
                NUnit.Framework.Assert.That(restored[1].Name, Is.Empty);
                NUnit.Framework.Assert.That(restored[1].Id, Is.Null);
            });
        }

        [TestCase("xml")]
        [TestCase("json")]
        public void ContactsKeepFormFieldsAndRecalculateSummaryFields(string format)
        {
            ContactData original = new ContactData("Иван <&>", "Last \"name\"")
            {
                Id = "456",
                MiddleName = "Middle",
                Nickname = "Tester",
                Title = "Engineer",
                Company = "Company & Co",
                Address = "Street, 1\nCity",
                HomePhone = "+7 (383) 123-45-67",
                MobilePhone = "222",
                WorkPhone = "333",
                Email = "one@example.com",
                Email2 = "two@example.com",
                Email3 = "three@example.com",
                Homepage = "https://example.com/?a=1&b=2",
                BirthDay = "17",
                BirthMonth = "November",
                BirthYear = "1991",
                AnniversaryDay = "1",
                AnniversaryMonth = "January",
                AnniversaryYear = "2010",
                AllPhones = "Сводное поле из таблицы",
                AllEmails = "Сводное поле из таблицы"
            };

            ContactData restored = RoundTrip(new List<ContactData> { original }, format).Single();
            DateTime today = new DateTime(2026, 10, 14);

            NUnit.Framework.Assert.Multiple(() =>
            {
                NUnit.Framework.Assert.That(restored.Id, Is.EqualTo(original.Id));
                NUnit.Framework.Assert.That(restored.GetAllDetails(today), Is.EqualTo(original.GetAllDetails(today)));
                NUnit.Framework.Assert.That(restored.Homepage, Is.EqualTo(original.Homepage));
                NUnit.Framework.Assert.That(restored.AllPhones, Is.EqualTo("+73831234567\n222\n333"));
                NUnit.Framework.Assert.That(restored.AllEmails,
                    Is.EqualTo("one@example.com\ntwo@example.com\nthree@example.com"));
            });

            restored.HomePhone = "999";
            restored.Email = "changed@example.com";

            NUnit.Framework.Assert.Multiple(() =>
            {
                NUnit.Framework.Assert.That(restored.AllPhones, Is.EqualTo("999\n222\n333"));
                NUnit.Framework.Assert.That(restored.AllEmails,
                    Is.EqualTo("changed@example.com\ntwo@example.com\nthree@example.com"));
            });
        }

        [TestCase("xml")]
        [TestCase("json")]
        public void EmptyListsCanBeRestored(string format)
        {
            NUnit.Framework.Assert.That(RoundTrip(new List<GroupData>(), format), Is.Empty);
            NUnit.Framework.Assert.That(RoundTrip(new List<ContactData>(), format), Is.Empty);
        }

        private static List<T> RoundTrip<T>(List<T> data, string format)
        {
            if (format == "json")
            {
                string json = JsonSerializer.Serialize(data);
                return JsonSerializer.Deserialize<List<T>>(json)!;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            using StringWriter writer = new StringWriter();
            serializer.Serialize(writer, data);
            using StringReader reader = new StringReader(writer.ToString());
            return (List<T>)serializer.Deserialize(reader)!;
        }
    }
}
