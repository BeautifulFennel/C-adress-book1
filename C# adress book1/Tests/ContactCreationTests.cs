using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactCreationTests : AuthTestBase
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
