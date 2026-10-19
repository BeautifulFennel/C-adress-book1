using System;
using System.Threading;
using NUnit.Framework;

namespace addressbook_tests_white
{
    [Apartment(ApartmentState.STA)]
    [NonParallelizable]
    public abstract class TestBase
    {
        protected ApplicationManager app;

        [SetUp]
        public void SetUp()
        {
            app = new ApplicationManager();
            TestContext.Progress.WriteLine("Копия приложения: " + app.WorkingDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            app?.Stop();
            app = null;
        }

        protected static GroupData NewGroup(string name)
        {
            return new GroupData(name + " " + Guid.NewGuid().ToString("N").Substring(0, 8));
        }
    }
}
