using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactModificationTests : TestBase
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

            if (!app.Contacts.IsThereAContact())
            {
                app.Contacts.Create(new ContactData("Contact", "For modification")
                {
                    BirthDay = "1",
                    BirthMonth = "January"
                });
            }

            app.Contacts.Modify(0, contact);
        }
    }
}
