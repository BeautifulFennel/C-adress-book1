using System.Globalization;
using System.Text;

namespace TestDataGenerator
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (args.Length == 1 && args[0] == "--help")
            {
                PrintUsage(Console.Out);
                return 0;
            }

            // Сначала проверяем все параметры, чтобы ошибочный запуск не затёр файл.
            if (args.Length != 4)
            {
                return ReportArgumentError("Нужно передать ровно четыре параметра.");
            }

            string dataType = args[0].Trim().ToLowerInvariant();
            if (dataType != "group" && dataType != "contacts")
            {
                return ReportArgumentError("Тип данных должен быть group или contacts.");
            }

            if (!int.TryParse(args[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int count)
                || count < 0)
            {
                return ReportArgumentError("Количество должно быть целым числом от 0 до 2147483647.");
            }

            if (string.IsNullOrWhiteSpace(args[2]))
            {
                return ReportArgumentError("Имя файла не должно быть пустым.");
            }

            string format = args[3].Trim().ToLowerInvariant();
            if (format != "xml" && format != "json")
            {
                return ReportArgumentError("Формат должен быть xml или json.");
            }

            try
            {
                string fileName = Path.GetFullPath(args[2]);

                if (dataType == "group")
                {
                    var groups = TestDataFactory.CreateGroups(count);
                    TestDataWriter.Write(groups, fileName, format);
                }
                else
                {
                    var contacts = TestDataFactory.CreateContacts(count);
                    TestDataWriter.Write(contacts, fileName, format);
                }

                Console.WriteLine($"Создано объектов: {count}. Тип: {dataType}. Формат: {format}.");
                Console.WriteLine($"Файл: {fileName}");
                return 0;
            }
            catch (Exception exception) when (exception is IOException
                or UnauthorizedAccessException or ArgumentException or NotSupportedException)
            {
                Console.Error.WriteLine($"Не удалось записать файл: {exception.Message}");
                return 1;
            }
        }

        private static int ReportArgumentError(string message)
        {
            Console.Error.WriteLine(message);
            PrintUsage(Console.Error);
            return 1;
        }

        private static void PrintUsage(TextWriter writer)
        {
            writer.WriteLine("Использование: TestDataGenerator <group|contacts> <количество> <файл> <xml|json>");
            writer.WriteLine("Пример: TestDataGenerator group 5 groups.json json");
            writer.WriteLine("Пример: TestDataGenerator contacts 5 contacts.xml xml");
            writer.WriteLine("Если путь содержит пробелы, заключите его в кавычки.");
            writer.WriteLine("При количестве 0 записывается пустой список. Существующий файл перезаписывается.");
        }
    }
}
