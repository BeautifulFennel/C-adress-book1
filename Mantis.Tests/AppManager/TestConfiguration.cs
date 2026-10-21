using System.Globalization;
using System.Xml.Linq;

namespace mantis_tests;

public sealed class TestConfiguration
{
    public Uri BaseUrl { get; private init; } = null!;
    public AccountData Account { get; private init; } = null!;
    public bool Headless { get; private init; }
    public TimeSpan Timeout { get; private init; }

    public static TestConfiguration Load()
    {
        // В .NET 10 App.config читается явно, без System.Configuration.
        XDocument document = XDocument.Load(Path.Combine(AppContext.BaseDirectory, "App.config"));
        string Read(string key, string environmentVariable)
        {
            return Environment.GetEnvironmentVariable(environmentVariable)
                ?? (string?)document.Root?.Element("appSettings")?.Elements("add")
                    .SingleOrDefault(element => (string?)element.Attribute("key") == key)?
                    .Attribute("value")
                ?? throw new InvalidOperationException($"Не задан параметр {key} в App.config.");
        }

        string url = Read("BaseUrl", "MANTIS_BASE_URL");
        if (!Uri.TryCreate(url.TrimEnd('/') + "/", UriKind.Absolute, out Uri? baseUrl)
            || (baseUrl.Scheme != Uri.UriSchemeHttp && baseUrl.Scheme != Uri.UriSchemeHttps)
            || !string.IsNullOrEmpty(baseUrl.Query) || !string.IsNullOrEmpty(baseUrl.Fragment))
        {
            throw new InvalidOperationException("MANTIS_BASE_URL должен указывать на каталог MantisBT, например http://localhost/mantisbt/.");
        }

        if (!bool.TryParse(Read("Headless", "MANTIS_HEADLESS"), out bool headless)
            || !int.TryParse(Read("TimeoutSeconds", "MANTIS_TIMEOUT_SECONDS"),
                NumberStyles.Integer, CultureInfo.InvariantCulture, out int seconds)
            || seconds <= 0)
        {
            throw new InvalidOperationException("Headless должен быть true/false, TimeoutSeconds — положительным целым числом.");
        }

        return new TestConfiguration
        {
            BaseUrl = baseUrl,
            Account = new AccountData(Read("Username", "MANTIS_USERNAME"), Read("Password", "MANTIS_PASSWORD")),
            Headless = headless,
            Timeout = TimeSpan.FromSeconds(seconds)
        };
    }
}
