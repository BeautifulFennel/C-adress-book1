using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactInformationTests : AuthTestBase
    {
        [Test]
        public void TestContactInformation()
        {
            app.Navigator.OpenHomePage();

            if (!app.Contacts.IsThereAContact())
            {
                app.Contacts.Create(new ContactData("Contact", "For information check")
                {
                    Address = "Test street, 1\nNovosibirsk",
                    HomePhone = "+7 (383) 123-45-67",
                    MobilePhone = "+7 (913) 765-43-21",
                    WorkPhone = "383 111-22-33",
                    Email = "home@example.com",
                    Email2 = "work@example.com",
                    Email3 = "other@example.com",
                    BirthDay = "1",
                    BirthMonth = "January"
                });
            }

            const int index = 0;
            ContactData fromTable = app.Contacts.GetContactInformationFromTable(index);
            ContactData fromForm = app.Contacts.GetContactInformationFromEditForm(index);
            app.Navigator.OpenHomePage();

            // Обратная проверка: превращаем поля формы в ожидаемое представление таблицы.
            NUnit.Framework.Assert.Multiple(() =>
            {
                NUnit.Framework.Assert.That(fromTable.FirstName, Is.EqualTo(fromForm.FirstName), "Имя");
                NUnit.Framework.Assert.That(fromTable.LastName, Is.EqualTo(fromForm.LastName), "Фамилия");
                NUnit.Framework.Assert.That(fromTable.Address, Is.EqualTo(fromForm.Address), "Адрес");
                NUnit.Framework.Assert.That(fromTable.AllPhones, Is.EqualTo(fromForm.AllPhones), "Телефоны");
                NUnit.Framework.Assert.That(fromTable.AllEmails, Is.EqualTo(fromForm.AllEmails), "E-mail");
            });
        }
    }
}
