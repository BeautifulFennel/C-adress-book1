using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactModificationTests : ContactTestBase
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

            if (ContactData.GetAll().Count == 0)
            {
                app.Contacts.Create(new ContactData("Contact", "For modification")
                {
                    BirthDay = "1",
                    BirthMonth = "January"
                });
            }

            const int index = 0;
            List<ContactData> oldContacts = ContactData.GetAll();
            contact.Id = oldContacts[index].Id;

            app.Contacts.Modify(contact.Id!, contact);

            NUnit.Framework.Assert.That(app.Contacts.GetContactCount(), Is.EqualTo(oldContacts.Count));

            List<ContactData> newContacts = ContactData.GetAll();
            oldContacts[index] = contact;
            AssertContactsEqual(newContacts, oldContacts);
        }
    }
}
