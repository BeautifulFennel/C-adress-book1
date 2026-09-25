namespace addressbook_tests
{
    public class ContactData : IEquatable<ContactData>, IComparable<ContactData>
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
