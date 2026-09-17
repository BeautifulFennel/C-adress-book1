using NUnit.Framework;

namespace addressbook_tests
{
    public class TestBase
    {
        protected ApplicationManager app = null!;

        [SetUp]
        public void SetupTest()
        {
            app = ApplicationManager.GetInstance();
            app.Navigator.OpenHomePage();
        }
    }
}
