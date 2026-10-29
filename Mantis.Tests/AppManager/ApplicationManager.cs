using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace mantis_tests;

public sealed class ApplicationManager : IDisposable
{
    private bool disposed;

    public ApplicationManager(TestConfiguration configuration)
    {
        Configuration = configuration;
        FirefoxOptions options = new();
        if (configuration.Headless)
        {
            options.AddArgument("-headless");
        }

        Driver = new FirefoxDriver(options);
        try
        {
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
            Driver.Manage().Window.Size = new System.Drawing.Size(1440, 1000);
            Login = new LoginHelper(this);
            ManagementMenu = new ManagementMenuHelper(this);
            API = new APIHelper(configuration);
            Projects = new ProjectManagementHelper(this);
        }
        catch
        {
            Driver.Dispose();
            throw;
        }
    }

    public TestConfiguration Configuration { get; }
    public IWebDriver Driver { get; }
    public LoginHelper Login { get; }
    public ManagementMenuHelper ManagementMenu { get; }
    public ProjectManagementHelper Projects { get; }
    public APIHelper API { get; }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        try
        {
            Driver.Dispose();
        }
        finally
        {
            API.Dispose();
        }
    }
}
