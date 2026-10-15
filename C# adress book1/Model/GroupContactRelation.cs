using LinqToDB.Mapping;

namespace addressbook_tests
{
    [Table(Name = "address_in_groups")]
    public class GroupContactRelation
    {
        [Column(Name = "group_id"), PrimaryKey(1)]
        public string GroupId { get; set; } = "";

        [Column(Name = "id"), PrimaryKey(2)]
        public string ContactId { get; set; } = "";
    }
}
