namespace addressbook_tests
{
    public class ContactData : IEquatable<ContactData>, IComparable<ContactData>
    {
        private string? allPhones;
        private string? allEmails;

        public ContactData(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; set; }

        public string MiddleName { get; set; } = "";

        public string LastName { get; set; }

        public string Company { get; set; } = "";

        public string Address { get; set; } = "";

        public string HomePhone { get; set; } = "";

        public string MobilePhone { get; set; } = "";

        public string WorkPhone { get; set; } = "";

        public string Email { get; set; } = "";

        public string Email2 { get; set; } = "";

        public string Email3 { get; set; } = "";

        // Из таблицы получаем готовую строку, из формы — собираем её из отдельных полей.
        public string AllPhones
        {
            get
            {
                return allPhones ?? string.Join("\n",
                    new[] { HomePhone, MobilePhone, WorkPhone }
                        .Select(CleanUpPhone)
                        .Where(phone => !string.IsNullOrEmpty(phone)));
            }
            set { allPhones = value; }
        }

        public string AllEmails
        {
            get
            {
                return allEmails ?? string.Join("\n",
                    new[] { Email, Email2, Email3 }
                        .Where(email => !string.IsNullOrEmpty(email)));
            }
            set { allEmails = value; }
        }

        private static string CleanUpPhone(string? phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return "";
            }

            return phone.Replace(" ", "").Replace("-", "")
                .Replace("(", "").Replace(")", "")
                .Replace(".", "").Replace("/", "").Replace("'", "");
        }

        public string BirthDay { get; set; } = "";

        public string BirthMonth { get; set; } = "";

        public string BirthYear { get; set; } = "";

        public bool Equals(ContactData? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return FirstName == other.FirstName
                && LastName == other.LastName;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ContactData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstName, LastName);
        }

        public int CompareTo(ContactData? other)
        {
            if (other is null)
            {
                return 1;
            }

            int lastNameComparison = string.Compare(
                LastName,
                other.LastName,
                StringComparison.Ordinal);

            if (lastNameComparison != 0)
            {
                return lastNameComparison;
            }

            return string.Compare(
                FirstName,
                other.FirstName,
                StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return $"FirstName={FirstName}, LastName={LastName}";
        }
    }
}
