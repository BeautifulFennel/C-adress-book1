using addressbook_tests;
using System.Globalization;

namespace TestDataGenerator
{
    internal static class TestDataFactory
    {
        public static List<GroupData> CreateGroups(int count)
        {
            List<GroupData> groups = new List<GroupData>();

            for (int i = 0; i < count; i++)
            {
                groups.Add(new GroupData(TestBase.GenerateRandomString(30))
                {
                    Header = TestBase.GenerateRandomString(100),
                    Footer = TestBase.GenerateRandomString(100)
                });
            }

            return groups;
        }

        public static List<ContactData> CreateContacts(int count)
        {
            List<ContactData> contacts = new List<ContactData>();

            for (int i = 0; i < count; i++)
            {
                // Дни 1–28 подходят для любого месяца, включая февраль.
                DateTime birthday = CreateRandomDate(1950, 2011);
                DateTime anniversary = CreateRandomDate(2011, 2026);

                contacts.Add(new ContactData(TestBase.GenerateRandomString(30), TestBase.GenerateRandomString(30))
                {
                    MiddleName = TestBase.GenerateRandomString(30),
                    Nickname = TestBase.GenerateRandomString(30),
                    Title = TestBase.GenerateRandomString(30),
                    Company = TestBase.GenerateRandomString(50),
                    Address = TestBase.GenerateRandomString(100),
                    HomePhone = CreateRandomPhone(),
                    MobilePhone = CreateRandomPhone(),
                    WorkPhone = CreateRandomPhone(),
                    Email = CreateRandomEmail(),
                    Email2 = CreateRandomEmail(),
                    Email3 = CreateRandomEmail(),
                    Homepage = "https://site" + TestBase.GenerateRandomString(20) + ".example.com",
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

        private static DateTime CreateRandomDate(int firstYear, int lastYearExclusive)
        {
            return new DateTime(Random.Shared.Next(firstYear, lastYearExclusive),
                Random.Shared.Next(1, 13), Random.Shared.Next(1, 29));
        }

        private static string CreateRandomPhone()
        {
            return Random.Shared.Next(1000000, 10000000).ToString(CultureInfo.InvariantCulture);
        }

        private static string CreateRandomEmail()
        {
            return "user" + TestBase.GenerateRandomString(20) + "@example.com";
        }
    }
}
