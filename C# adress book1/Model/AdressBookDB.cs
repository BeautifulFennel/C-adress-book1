using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.MySql;

namespace addressbook_tests
{
    public class AdressBookDB : DataConnection
    {
        private static readonly DataOptions DbOptions = new DataOptions().UseMySql(
            TestConfiguration.ConnectionString, MySqlVersion.AutoDetect, MySqlProvider.MySqlConnector);

        public AdressBookDB() : base(DbOptions)
        {
        }

        public ITable<GroupData> Groups => this.GetTable<GroupData>();

        public ITable<ContactData> Contacts => this.GetTable<ContactData>();

        public ITable<GroupContactRelation> GCR => this.GetTable<GroupContactRelation>();
    }
}
