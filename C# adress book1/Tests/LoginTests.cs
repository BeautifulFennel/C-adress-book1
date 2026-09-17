using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace addressbook_tests
{
    [TestFixture]
    public class LoginTests : TestBase
    {
        [Test]
        public void LoginWithValidCredentials()
        {
            app.Auth.Logout();

            AccountData account = new AccountData("admin", "secret");
            app.Auth.Login(account);

            Assert.That(app.Auth.IsLoggedIn(account), Is.True);
        }

        [Test]
        public void LoginWithInvalidCredentials()
        {
            app.Auth.Logout();

            AccountData account = new AccountData("admin", "123456");
            app.Auth.Login(account);

            Assert.That(app.Auth.IsLoggedIn(account), Is.False);
        }
    }
}
