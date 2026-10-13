using NUnit.Framework;
using System.Globalization;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactCreationTests : AuthTestBase
    {
        public static IEnumerable<ContactData> RandomDataProvider()
        {
            List<ContactData> contacts = new List<ContactData>();
            for (int i = 0; i < 5; i++)
            {
                // Первые 28 дней существуют в любом месяце, в том числе в феврале.
                DateTime birthday = new DateTime(rnd.Next(1950, 2011), rnd.Next(1, 13), rnd.Next(1, 29));
                DateTime anniversary = new DateTime(rnd.Next(2011, 2026), rnd.Next(1, 13), rnd.Next(1, 29));

                contacts.Add(new ContactData(GenerateRandomString(30), GenerateRandomString(30))
                {
                    MiddleName = GenerateRandomString(30),
                    Nickname = GenerateRandomString(30),
                    Title = GenerateRandomString(30),
                    Company = GenerateRandomString(50),
                    Address = GenerateRandomString(100),
                    HomePhone = rnd.Next(1000000, 10000000).ToString(CultureInfo.InvariantCulture),
                    MobilePhone = rnd.Next(1000000, 10000000).ToString(CultureInfo.InvariantCulture),
                    WorkPhone = rnd.Next(1000000, 10000000).ToString(CultureInfo.InvariantCulture),
                    Email = "user" + GenerateRandomString(20) + "@example.com",
                    Email2 = "user" + GenerateRandomString(20) + "@example.com",
                    Email3 = "user" + GenerateRandomString(20) + "@example.com",
                    Homepage = "https://site" + GenerateRandomString(20) + ".example.com",
                    BirthDay = birthday.ToString("%d", CultureInfo.InvariantCulture),
                    BirthMonth = birthday.ToString("MMMM", CultureInfo.InvariantCulture),
                    BirthYear = birthday.ToString("yyyy", CultureInfo.InvariantCulture),
                    AnniversaryDay = anniversary.ToString("%d", CultureInfo.InvariantCulture),
                    AnniversaryMonth = anniversary.ToString("MMMM", CultureInfo.InvariantCulture),
                    AnniversaryYear = anniversary.ToString("yyyy", CultureInfo.InvariantCulture)
                });
            }

            return contacts;
        }

        [TestCaseSource(nameof(RandomDataProvider))]
        public void ContactCreationTest(ContactData contact)
        {
            app.Navigator.OpenHomePage();

            List<ContactData> oldContacts = app.Contacts.GetContactList();

            app.Contacts.Create(contact);

            NUnit.Framework.Assert.That(app.Contacts.GetContactCount(), Is.EqualTo(oldContacts.Count + 1));

            List<ContactData> newContacts = app.Contacts.GetContactList();
            oldContacts.Add(contact);
            oldContacts.Sort();
            newContacts.Sort();

            NUnit.Framework.Assert.That(newContacts, Is.EqualTo(oldContacts));
        }
    }
}
