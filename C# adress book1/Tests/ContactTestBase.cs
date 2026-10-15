using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace addressbook_tests
{
    public class ContactTestBase : AuthTestBase
    {
        [TearDown]
        public void CompareContactsUI_DB()
        {
            if (!PERFORM_LONG_UI_CHECKS)
            {
                return;
            }

            List<ContactData> fromUI = app.Contacts.GetContactList(forceReload: true);
            List<ContactData> fromDB = ContactData.GetAll();

            Assert.That(fromUI.OrderBy(c => c.Id).Select(c =>
                    (c.Id, c.FirstName, c.LastName, c.Address, c.AllPhones, c.AllEmails)),
                Is.EqualTo(fromDB.OrderBy(c => c.Id).Select(c =>
                    (c.Id, c.FirstName, c.LastName, NormalizeLineEndings(c.Address), c.AllPhones, c.AllEmails))),
                "Список контактов в UI не совпадает с БД.");
            TestContext.WriteLine("UI/БД: список контактов совпадает.");
        }

        protected static void AssertContactsEqual(List<ContactData> actual, List<ContactData> expected)
        {
            Assert.That(actual.OrderBy(c => c.Id).Select(ContactFields),
                Is.EqualTo(expected.OrderBy(c => c.Id).Select(ContactFields)));
        }

        private static object ContactFields(ContactData c) => new
        {
            c.Id, c.FirstName, c.MiddleName, c.LastName, c.Nickname, c.Title, c.Company,
            Address = NormalizeLineEndings(c.Address),
            c.HomePhone, c.MobilePhone, c.WorkPhone, c.Email, c.Email2, c.Email3, c.Homepage,
            // В форме пустой день/месяц обозначен "-", в БД — 0, NULL или пустой строкой.
            BirthDay = int.TryParse(c.BirthDay, out int birthDay) ? birthDay : 0,
            BirthMonth = c.BirthMonth is null or "-" ? "" : c.BirthMonth,
            BirthYear = c.BirthYear ?? "",
            AnniversaryDay = int.TryParse(c.AnniversaryDay, out int anniversaryDay) ? anniversaryDay : 0,
            AnniversaryMonth = c.AnniversaryMonth is null or "-" ? "" : c.AnniversaryMonth,
            AnniversaryYear = c.AnniversaryYear ?? ""
        };

        private static string NormalizeLineEndings(string value) => value.Replace("\r\n", "\n");
    }
}
