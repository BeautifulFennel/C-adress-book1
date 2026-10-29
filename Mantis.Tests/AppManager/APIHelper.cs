using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace mantis_tests;

// SOAP 1.1 RPC/encoded из WSDL MantisConnect, без зависимости от браузера.
public sealed class APIHelper : IDisposable
{
    private static readonly XNamespace Soap = "http://schemas.xmlsoap.org/soap/envelope/";
    private static readonly XNamespace Mantis = "http://futureware.biz/mantisconnect";
    private static readonly XNamespace Xsi = "http://www.w3.org/2001/XMLSchema-instance";
    private const string EncodingStyle = "http://schemas.xmlsoap.org/soap/encoding/";
    private readonly HttpClient client;
    private readonly AccountData account;

    public APIHelper(TestConfiguration configuration)
    {
        account = configuration.Account;
        client = new HttpClient
        {
            BaseAddress = new Uri(configuration.BaseUrl, "api/soap/mantisconnect.php"),
            Timeout = configuration.Timeout
        };
    }

    public List<ProjectData> GetProjectList()
    {
        XElement result = Call("mc_projects_get_user_accessible");
        return result.Elements("item")
            .Select(item => new ProjectData(
                item.Element("name")!.Value,
                (string?)item.Element("description") ?? "")
            {
                Id = ReadId(item.Element("id")!.Value)
            })
            .ToList();
    }

    public ProjectData CreateProject(ProjectData project)
    {
        XElement result = Call("mc_project_add",
            new XElement("project", new XAttribute(Xsi + "type", "mc:ProjectData"),
                new XElement("name", project.Name),
                new XElement("status", new XElement("id", 10)),
                new XElement("enabled", true),
                new XElement("view_state", new XElement("id", 10)),
                new XElement("description", project.Description)));
        return project with { Id = ReadId(result.Value) };
    }

    private XElement Call(string operation, params XElement[] parameters)
    {
        XDocument envelope = new(new XElement(Soap + "Envelope",
            new XAttribute(XNamespace.Xmlns + "soap", Soap),
            new XAttribute(XNamespace.Xmlns + "mc", Mantis),
            new XAttribute(XNamespace.Xmlns + "xsi", Xsi),
            new XAttribute(Soap + "encodingStyle", EncodingStyle),
            new XElement(Soap + "Body", new XElement(Mantis + operation,
                new XElement("username", account.Username),
                new XElement("password", account.Password), parameters))));

        using HttpRequestMessage request = new(HttpMethod.Post, client.BaseAddress);
        request.Headers.Add("SOAPAction", $"\"http://www.mantisbt.org/bugs/api/soap/mantisconnect.php/{operation}\"");
        request.Content = new StringContent(envelope.ToString(), Encoding.UTF8, "text/xml");
        using HttpResponseMessage response = client.Send(request);
        using Stream stream = response.Content.ReadAsStream();
        XDocument document;
        try
        {
            document = XDocument.Load(stream);
        }
        catch (XmlException error)
        {
            throw new InvalidOperationException(
                $"MantisConnect {operation}: ответ не является SOAP XML (HTTP {(int)response.StatusCode}). Проверьте адрес сервиса и расширение SOAP в PHP.", error);
        }

        XElement? body = document.Root?.Element(Soap + "Body");
        // SOAP Fault может приходить с HTTP 500 — читаем его до проверки HTTP-статуса.
        if (body?.Element(Soap + "Fault") is XElement fault)
        {
            throw new InvalidOperationException(
                $"MantisConnect {operation}: {(string?)fault.Element("faultcode")} — {(string?)fault.Element("faultstring")}");
        }

        response.EnsureSuccessStatusCode();
        return body?.Element(Mantis + (operation + "Response"))?.Element("return")
            ?? throw new InvalidOperationException($"MantisConnect {operation}: в SOAP-ответе отсутствует return.");
    }

    private static int ReadId(string value)
    {
        int id = int.Parse(value, CultureInfo.InvariantCulture);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        return id;
    }

    public void Dispose() => client.Dispose();
}
