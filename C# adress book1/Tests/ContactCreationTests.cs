using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactCreationTests : TestBase
    {
        [Test]
        public void ContactCreationTest()
        {
            ContactData contact = new ContactData("First", "Last")
            {
                MiddleName = "Middle",
                Company = "123",
                Email = "test123@test123.com",
                BirthDay = "16",
                BirthMonth = "October",
                BirthYear = "1990"
            };

            app.Navigator.OpenHomePage();
            app.Auth.Login("admin", "secret");
            app.Contacts.InitContactCreation();
            app.Contacts.FillContactForm(contact);
            app.Contacts.SubmitContactCreation();
            app.Navigator.ReturnToHomePage();
            app.Auth.Logout();
        }
    }
}
