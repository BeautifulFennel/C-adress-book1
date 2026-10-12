using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactRemovalTests : AuthTestBase
    {
        [Test]
        public void ContactRemovalTest()
        {
            app.Navigator.OpenHomePage();

            if (!app.Contacts.IsThereAContact())
            {
                app.Contacts.Create(new ContactData("Contact", "For removal")
                {
                    BirthDay = "1",
                    BirthMonth = "January"
                });
            }

            const int index = 0;
            List<ContactData> oldContacts = app.Contacts.GetContactList();

            app.Contacts.Remove(index);

            NUnit.Framework.Assert.That(app.Contacts.GetContactCount(), Is.EqualTo(oldContacts.Count - 1));

            List<ContactData> newContacts = app.Contacts.GetContactList();
            oldContacts.RemoveAt(index);
            oldContacts.Sort();
            newContacts.Sort();

            NUnit.Framework.Assert.That(newContacts, Is.EqualTo(oldContacts));
        }
    }
}
