using NUnit.Framework;

namespace addressbook_tests
{
    [TestFixture]
    public class ContactDataTests
    {
        [Test]
        public void DetailsIncludeAllFieldsInDisplayOrder()
        {
            ContactData contact = new ContactData("First", "Last")
            {
                MiddleName = "Middle",
                Nickname = "Tester",
                Title = "Engineer",
                Company = "Company",
                Address = "Street, 1\nCity",
                HomePhone = "+7 (383) 123-45-67",
                MobilePhone = "222",
                WorkPhone = "333",
                Email = "same@example.com",
                Email2 = "same@example.com",
                Email3 = "other@example.com",
                Homepage = "https://example.com",
                BirthDay = "17",
                BirthMonth = "November",
                BirthYear = "1991",
                AnniversaryDay = "1",
                AnniversaryMonth = "January",
                AnniversaryYear = "2010"
            };

            string expected = "First Middle Last\nTester\nEngineer\nCompany\nStreet, 1\nCity\n"
                + "H: +7 (383) 123-45-67\nM: 222\nW: 333\n"
                + "same@example.com\nsame@example.com\nother@example.com\nHomepage: example.com\n"
                + "Birthday 17. November 1991 (34)\nAnniversary 1. January 2010 (16)";

            NUnit.Framework.Assert.That(contact.GetAllDetails(new DateTime(2026, 10, 13)), Is.EqualTo(expected));
        }

        [Test]
        public void DetailsOmitEmptyOptionalFields()
        {
            ContactData contact = new ContactData("First", "Last");

            NUnit.Framework.Assert.That(contact.GetAllDetails(new DateTime(2026, 10, 13)),
                Is.EqualTo("First Last"));
        }

        [TestCase("13", "October", "1991", "Birthday 13. October 1991 (35)")]
        [TestCase("14", "October", "1991", "Birthday 14. October 1991 (34)")]
        [TestCase("12", "October", "1991", "Birthday 12. October 1991 (35)")]
        [TestCase("-", "-", "", "")]
        [TestCase("0", "-", "", "")]
        [TestCase("17", "November", "", "Birthday 17. November")]
        [TestCase("-", "November", "", "Birthday November")]
        [TestCase("17", "-", "", "Birthday 17.")]
        [TestCase("-", "-", "1991", "Birthday 1991 (35)")]
        [TestCase("-", "November", "1991", "Birthday November 1991 (34)")]
        [TestCase("1", "January", "1800", "Birthday 1. January 1800")]
        [TestCase("1", "January", "2027", "Birthday 1. January 2027")]
        public void DetailsFormatBirthdayAndAge(string day, string month, string year, string expectedDate)
        {
            ContactData contact = new ContactData("First", "Last")
            {
                BirthDay = day,
                BirthMonth = month,
                BirthYear = year
            };

            string expected = "First Last" + (expectedDate == "" ? "" : "\n" + expectedDate);
            NUnit.Framework.Assert.That(contact.GetAllDetails(new DateTime(2026, 10, 13)), Is.EqualTo(expected));
        }

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
