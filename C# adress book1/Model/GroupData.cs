using LinqToDB.Mapping;

namespace addressbook_tests
{
    [Table(Name = "group_list")]
    public class GroupData : IEquatable<GroupData>, IComparable<GroupData>
    {
        // XmlSerializer создаёт объект через конструктор без параметров.
        public GroupData() : this("")
        {
        }

        public GroupData(string name)
        {
            Name = name;
        }

        [Column(Name = "group_name")]
        public string Name { get; set; }

        [Column(Name = "group_id"), PrimaryKey, Identity]
        public string? Id { get; set; }

        [Column(Name = "group_header")]
        public string Header { get; set; } = "";

        [Column(Name = "group_footer")]
        public string Footer { get; set; } = "";

        public static List<GroupData> GetAll()
        {
            using AdressBookDB db = new AdressBookDB();
            return (from g in db.Groups select g).ToList();
        }

        public List<ContactData> GetContacts()
        {
            ArgumentException.ThrowIfNullOrEmpty(Id);
            using AdressBookDB db = new AdressBookDB();
            return (from contact in db.Contacts
                    from relation in db.GCR
                    where relation.GroupId == Id && relation.ContactId == contact.Id
                        && contact.Deprecated == null
                    select contact).Distinct().ToList();
        }

        public bool Equals(GroupData? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as GroupData);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public int CompareTo(GroupData? other)
        {
            if (other is null)
            {
                return 1;
            }

            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return $"Name={Name}";
        }
    }
}
