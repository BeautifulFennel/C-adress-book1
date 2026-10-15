using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactRemovalTests : ContactTestBase
    {
        [Test]
        public void ContactRemovalTest()
        {
            app.Navigator.OpenHomePage();

            if (ContactData.GetAll().Count == 0)
            {
                app.Contacts.Create(new ContactData("Contact", "For removal")
                {
                    BirthDay = "1",
                    BirthMonth = "January"
                });
            }

            const int index = 0;
            List<ContactData> oldContacts = ContactData.GetAll();

            app.Contacts.Remove(oldContacts[index].Id!);

            NUnit.Framework.Assert.That(app.Contacts.GetContactCount(), Is.EqualTo(oldContacts.Count - 1));

            List<ContactData> newContacts = ContactData.GetAll();
            oldContacts.RemoveAt(index);
            AssertContactsEqual(newContacts, oldContacts);
        }
    }
}
