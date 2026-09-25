namespace addressbook_tests
{
    public class GroupData : IEquatable<GroupData>, IComparable<GroupData>
    {
        public GroupData(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public string Header { get; set; } = "";

        public string Footer { get; set; } = "";

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
