using System;

namespace addressbook_tests_white
{
    public class GroupData : IEquatable<GroupData>, IComparable<GroupData>
    {
        public string Name { get; set; }

        public GroupData(string name)
        {
            Name = name;
        }

        public bool Equals(GroupData other)
        {
            return other != null && string.Equals(Name, other.Name, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GroupData);
        }

        public override int GetHashCode()
        {
            return Name == null ? 0 : StringComparer.Ordinal.GetHashCode(Name);
        }

        public int CompareTo(GroupData other)
        {
            return other == null ? 1 : string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
