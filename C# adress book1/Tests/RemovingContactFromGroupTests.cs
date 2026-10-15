using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class RemovingContactFromGroupTests : ContactTestBase
    {
        [Test]
        public void TestRemovingContactFromGroup()
        {
            if (GroupData.GetAll().Count == 0)
            {
                app.Navigator.OpenGroupsPage();
                app.Groups.Create(new GroupData("Group for removing a contact"));
            }

            GroupData group = GroupData.GetAll()[0];
            List<ContactData> oldContacts = group.GetContacts();
            if (oldContacts.Count == 0)
            {
                if (ContactData.GetAll().Count == 0)
                {
                    app.Navigator.OpenHomePage();
                    app.Contacts.Create(new ContactData("Contact", "For removing from a group"));
                }

                app.Contacts.AddContactToGroup(ContactData.GetAll()[0], group);
                oldContacts = group.GetContacts();
            }

            ContactData contact = oldContacts[0];
            List<ContactData> allContacts = ContactData.GetAll();

            app.Contacts.RemoveContactFromGroup(contact, group);

            oldContacts.RemoveAll(c => c.Id == contact.Id);
            AssertContactsEqual(group.GetContacts(), oldContacts);
            // Исключение из группы должно сохранить сам контакт и остальные записи.
            AssertContactsEqual(ContactData.GetAll(), allContacts);
        }
    }
}
