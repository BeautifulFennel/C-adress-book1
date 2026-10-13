using NUnit.Framework;
using System.Text.RegularExpressions;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactInformationTests : AuthTestBase
    {
        [Test]
        [Description("Задание №12: страница свойств контакта и форма редактирования")]
        public void TestContactDetailsInformation()
        {
            app.Navigator.OpenHomePage();

            if (!app.Contacts.IsThereAContact())
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

            const int index = 0;
            ContactData fromForm = app.Contacts.GetContactInformationFromEditForm(index);
            string fromDetails = app.Contacts.GetContactInformationFromDetails(index);
            app.Navigator.OpenHomePage();

            // Собираем ожидаемый блок из формы, а не разбираем текст страницы на поля.
            string expectedDetails = fromForm.GetAllDetails(DateTime.Today);
            NUnit.Framework.Assert.That(NormalizeWhitespace(fromDetails),
                Is.EqualTo(NormalizeWhitespace(expectedDetails)), "Детальная информация о контакте");
        }

        private static string NormalizeWhitespace(string text)
        {
            // WebDriver может добавлять переносы около ссылок и меток; сравниваем содержимое блока.
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        [Test]
        [Description("Задание №11: таблица контактов и форма редактирования")]
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
