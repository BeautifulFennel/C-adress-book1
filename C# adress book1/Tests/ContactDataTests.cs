using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactDataTests
    {
        [TestCase("", "", "", "")]
        [TestCase("111", "222", "333", "111\n222\n333")]
        [TestCase("", "222", "333", "222\n333")]
        [TestCase("111", "", "333", "111\n333")]
        [TestCase("111", "222", "", "111\n222")]
        [TestCase("", "", "333", "333")]
        [TestCase("+7 (383) 123-45-67", "", "12.34/5'6", "+73831234567\n123456")]
        public void AllPhonesMatchTableFormat(string home, string mobile, string work, string expected)
        {
            ContactData contact = new ContactData("First", "Last")
            {
                HomePhone = home,
                MobilePhone = mobile,
                WorkPhone = work
            };

            NUnit.Framework.Assert.That(contact.AllPhones, Is.EqualTo(expected));
        }

        [TestCase("", "", "", "")]
        [TestCase("one@example.com", "two@example.com", "three@example.com",
            "one@example.com\ntwo@example.com\nthree@example.com")]
        [TestCase("", "two@example.com", "three@example.com", "two@example.com\nthree@example.com")]
        [TestCase("one@example.com", "", "three@example.com", "one@example.com\nthree@example.com")]
        [TestCase("one@example.com", "two@example.com", "", "one@example.com\ntwo@example.com")]
        [TestCase("", "", "three@example.com", "three@example.com")]
        [TestCase("same@example.com", "same@example.com", "", "same@example.com\nsame@example.com")]
        public void AllEmailsMatchTableFormat(string email, string email2, string email3, string expected)
        {
            ContactData contact = new ContactData("First", "Last")
            {
                Email = email,
                Email2 = email2,
                Email3 = email3
            };

            NUnit.Framework.Assert.That(contact.AllEmails, Is.EqualTo(expected));
        }
    }
}
