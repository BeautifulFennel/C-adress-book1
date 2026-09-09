namespace addressbook_tests
{
    public class GroupData
    {
        public GroupData(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public string Header { get; set; } = "";

        public string Footer { get; set; } = "";
    }
}
