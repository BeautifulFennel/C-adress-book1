using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactInformationTests : ContactTestBase
    {
        [Test]
        [Description("Задание №12: страница свойств контакта и форма редактирования")]
        public void TestContactDetailsInformation()
        {
            app.Navigator.OpenHomePage();

            if (ContactData.GetAll().Count == 0)
            {
                app.Contacts.Create(new ContactData("Contact", "For details check")
                {
                    MiddleName = "Middle",
                    Nickname = "Tester",
                    Title = "Engineer",
                    Company = "Test company",
                    Address = "Test street, 1\nNovosibirsk",
                    HomePhone = "+7 (383) 123-45-67",
                    MobilePhone = "+7 (913) 765-43-21",
                    WorkPhone = "383 111-22-33",
                    Email = "home@example.com",
                    Email2 = "work@example.com",
                    Email3 = "other@example.com",
                    Homepage = "https://example.com",
                    BirthDay = "17",
                    BirthMonth = "November",
                    BirthYear = "1991",
                    AnniversaryDay = "1",
                    AnniversaryMonth = "January",
                    AnniversaryYear = "2010"
                });
            }

            string id = ContactData.GetAll()[0].Id!;
            ContactData fromForm = app.Contacts.GetContactInformationFromEditForm(id);
            string fromDetails = app.Contacts.GetContactInformationFromDetails(id);
            app.Navigator.OpenHomePage();

            // Собираем ожидаемый блок из формы, а не разбираем текст страницы на поля.
            string expectedDetails = fromForm.GetAllDetails(DateTime.Today);
            NUnit.Framework.Assert.That(fromDetails,
                Is.EqualTo(expectedDetails), "Детальная информация о контакте, включая пробелы и переносы строк");
        }

        [Test]
        [Description("Задание №11: таблица контактов и форма редактирования")]
        public void TestContactInformation()
        {
            app.Navigator.OpenHomePage();

            if (ContactData.GetAll().Count == 0)
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

            string id = ContactData.GetAll()[0].Id!;
            ContactData fromTable = app.Contacts.GetContactInformationFromTable(id);
            ContactData fromForm = app.Contacts.GetContactInformationFromEditForm(id);
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
