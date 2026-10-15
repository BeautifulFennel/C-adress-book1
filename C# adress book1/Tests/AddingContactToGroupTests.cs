using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class AddingContactToGroupTests : ContactTestBase
    {
        [Test]
        public void TestAddingContactToGroup()
        {
            if (GroupData.GetAll().Count == 0)
            {
                app.Navigator.OpenGroupsPage();
                app.Groups.Create(new GroupData("Group for adding a contact"));
            }

            GroupData group = GroupData.GetAll()[0];
            List<ContactData> oldContacts = group.GetContacts();
            // Equals сравнивает имена; однофамильцев различаем по Id.
            ContactData? contact = ContactData.GetAll()
                .FirstOrDefault(c => oldContacts.All(member => member.Id != c.Id));
            if (contact == null)
            {
                app.Navigator.OpenHomePage();
                app.Contacts.Create(new ContactData("Contact", "For adding to a group"));
                contact = ContactData.GetAll()
                    .First(c => oldContacts.All(member => member.Id != c.Id));
            }

            List<ContactData> allContacts = ContactData.GetAll();

            app.Contacts.AddContactToGroup(contact, group);

            oldContacts.Add(contact);
            AssertContactsEqual(group.GetContacts(), oldContacts);
            AssertContactsEqual(ContactData.GetAll(), allContacts);
        }
    }
}
