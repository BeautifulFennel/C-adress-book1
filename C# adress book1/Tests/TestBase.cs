using System.Text;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace addressbook_tests
{
    public class TestBase
    {
        protected ApplicationManager app = null!;

        private readonly StringBuilder verificationErrors = new StringBuilder();

        [SetUp]
        public void SetupTest()
        {
            app = new ApplicationManager();
            app.Navigator.OpenHomePage();
            app.Auth.Login("admin", "secret");
        }

        [TearDown]
        public void TeardownTest()
        {
            app.Stop();
            Assert.AreEqual("", verificationErrors.ToString());
        }
    }
}
