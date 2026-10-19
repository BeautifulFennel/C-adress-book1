using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.WindowItems;

namespace addressbook_tests_white
{
    public sealed class ApplicationManager : IDisposable
    {
        public const string MainWindowTitle = "Free Address Book";
        private Application application;

        public Window MainWindow { get; private set; }
        public GroupHelper Groups { get; private set; }
        public string WorkingDirectory { get; private set; }

        public ApplicationManager()
        {
            string executable = Environment.GetEnvironmentVariable("ADDRESSBOOK_DESKTOP_EXE");
            if (string.IsNullOrWhiteSpace(executable))
            {
                executable = ConfigurationManager.AppSettings["AddressBookExecutable"];
            }

            if (string.IsNullOrWhiteSpace(executable) || !File.Exists(executable))
            {
                throw new FileNotFoundException(
                    "Укажите путь к AddressBook.exe в App.config или ADDRESSBOOK_DESKTOP_EXE.", executable);
            }

            // У каждого теста своя копия приложения и MDB: исходная адресная книга не изменяется.
            WorkingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestRuns", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(WorkingDirectory);
            foreach (string file in Directory.GetFiles(Path.GetDirectoryName(Path.GetFullPath(executable))))
            {
                File.Copy(file, Path.Combine(WorkingDirectory, Path.GetFileName(file)));
            }

            try
            {
                application = Application.Launch(new ProcessStartInfo
                {
                    FileName = Path.Combine(WorkingDirectory, Path.GetFileName(executable)),
                    WorkingDirectory = WorkingDirectory,
                    UseShellExecute = false
                });
                MainWindow = application.GetWindow(MainWindowTitle, InitializeOption.NoCache);
                Groups = new GroupHelper(this);
            }
            catch
            {
                Stop();
                throw;
            }
        }

        public void Stop()
        {
            if (application == null)
            {
                return;
            }

            try
            {
                if (!application.Process.HasExited && MainWindow != null)
                {
                    MainWindow.Get<Button>("uxExitAddressButton").Click();
                    application.Process.WaitForExit(5000);
                }
            }
            finally
            {
                if (!application.Process.HasExited)
                {
                    application.Kill();
                    application.Process.WaitForExit(5000);
                }
                application.Dispose();
                application = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
