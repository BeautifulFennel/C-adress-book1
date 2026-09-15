using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactRemovalTests : TestBase
    {
        [Test]
        public void ContactRemovalTest()
        {
            if (!app.Contacts.IsThereAContact())
            {
                app.Contacts.Create(new ContactData("Contact", "For removal")
                {
                    BirthDay = "1",
                    BirthMonth = "January"
                });
            }

            app.Contacts.Remove(0);
        }
    }
}
