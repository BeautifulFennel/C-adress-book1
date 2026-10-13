using NUnit.Framework;
using System.Text;

namespace addressbook_tests
{
    public class TestBase
    {
        protected ApplicationManager app = null!;
        protected static readonly Random rnd = new Random();

        public static string GenerateRandomString(int max)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(max);

            const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int length = rnd.Next(max + 1);
            StringBuilder builder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                builder.Append(characters[rnd.Next(characters.Length)]);
            }

            return builder.ToString();
        }

        [SetUp]
        public void SetupTest()
        {
            app = ApplicationManager.GetInstance();
            app.Navigator.OpenHomePage();
        }
    }
}
