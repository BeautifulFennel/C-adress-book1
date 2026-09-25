using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactModificationTests : AuthTestBase
    {
        [Test]
        public void ContactModificationTest()
        {
            ContactData contact = new ContactData("Modified first", "Modified last")
            {
                MiddleName = "Modified middle",
                Company = "Modified company",
                Email = "modified@example.com",
                BirthDay = "17",
                BirthMonth = "November",
                BirthYear = "1991"
            };

            app.Navigator.OpenHomePage();

            if (!app.Contacts.IsThereAContact())
            {
                app.Contacts.Create(new ContactData("Contact", "For modification")
                {
                    BirthDay = "1",
                    BirthMonth = "January"
                });
            }

            const int index = 0;
            List<ContactData> oldContacts = app.Contacts.GetContactList();

            app.Contacts.Modify(index, contact);

            List<ContactData> newContacts = app.Contacts.GetContactList();
            oldContacts[index] = contact;
            oldContacts.Sort();
            newContacts.Sort();

            NUnit.Framework.Assert.That(newContacts, Is.EqualTo(oldContacts));
        }
    }
}
