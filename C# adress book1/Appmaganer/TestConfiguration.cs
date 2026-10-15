using System.Xml.Linq;

namespace addressbook_tests
{
    // В .NET 10 App.config читаем явно: автоматическая загрузка из урока
    // относится к .NET Framework. Файл лежит рядом с тестовой сборкой.
    internal static class TestConfiguration
    {
        private static readonly Lazy<XDocument> Configuration = new(() =>
            XDocument.Load(Path.Combine(
                Path.GetDirectoryName(typeof(TestConfiguration).Assembly.Location)!, "App.config")));

        public static string ConnectionString =>
            Environment.GetEnvironmentVariable("ADDRESSBOOK_CONNECTION_STRING")
            ?? (string?)Configuration.Value.Root?.Element("connectionStrings")?
                .Elements("add").SingleOrDefault(element => (string?)element.Attribute("name") == "AddressBook")?
                .Attribute("connectionString")
            ?? throw new InvalidOperationException("Не задана строка подключения AddressBook в App.config.");

        public static bool PerformLongUiChecks => bool.Parse(
            Environment.GetEnvironmentVariable("PERFORM_LONG_UI_CHECKS")
            ?? (string?)Configuration.Value.Root?.Element("appSettings")?
                .Elements("add").SingleOrDefault(element => (string?)element.Attribute("key") == "PERFORM_LONG_UI_CHECKS")?
                .Attribute("value")
            ?? "false");
    }
}
