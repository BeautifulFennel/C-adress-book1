namespace addressbook_tests
{
    public class ContactData
    {
        public ContactData(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; set; }

        public string MiddleName { get; set; } = "";

        public string LastName { get; set; }

        public string Company { get; set; } = "";

        public string Email { get; set; } = "";

        public string BirthDay { get; set; } = "";

        public string BirthMonth { get; set; } = "";

        public string BirthYear { get; set; } = "";
    }
}
