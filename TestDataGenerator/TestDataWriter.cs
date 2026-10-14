using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace TestDataGenerator
{
    internal static class TestDataWriter
    {
        // T — тип элемента списка: GroupData или ContactData.
        public static void Write<T>(List<T> data, string fileName, string format)
        {
            string? directory = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // using закрывает файл, в том числе при исключении. false означает перезапись.
            using StreamWriter writer = new StreamWriter(fileName, false, new UTF8Encoding(false));

            if (format == "xml")
            {
                new XmlSerializer(typeof(List<T>)).Serialize(writer, data);
            }
            else
            {
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                writer.Write(json);
            }
        }
    }
}
