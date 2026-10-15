using System.Globalization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using LinqToDB.Mapping;

namespace addressbook_tests
{
    [Table(Name = "addressbook")]
    public class ContactData : IEquatable<ContactData>, IComparable<ContactData>
    {
        private string? allPhones;
        private string? allEmails;

        // XmlSerializer создаёт объект через конструктор без параметров.
        public ContactData() : this("", "")
        {
        }

        public ContactData(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        [Column(Name = "id"), PrimaryKey, Identity]
        public string? Id { get; set; }

        [Column(Name = "deprecated")]
        [JsonIgnore, XmlIgnore]
        public string? Deprecated { get; set; }

        [Column(Name = "firstname")]
        public string FirstName { get; set; }

        [Column(Name = "middlename")]
        public string MiddleName { get; set; } = "";

        [Column(Name = "lastname")]
        public string LastName { get; set; }

        [Column(Name = "nickname")]
        public string Nickname { get; set; } = "";

        [Column(Name = "title")]
        public string Title { get; set; } = "";

        [Column(Name = "company")]
        public string Company { get; set; } = "";

        [Column(Name = "address")]
        public string Address { get; set; } = "";

        [Column(Name = "home")]
        public string HomePhone { get; set; } = "";

        [Column(Name = "mobile")]
        public string MobilePhone { get; set; } = "";

        [Column(Name = "work")]
        public string WorkPhone { get; set; } = "";

        [Column(Name = "email")]
        public string Email { get; set; } = "";

        [Column(Name = "email2")]
        public string Email2 { get; set; } = "";

        [Column(Name = "email3")]
        public string Email3 { get; set; } = "";

        [Column(Name = "homepage")]
        public string Homepage { get; set; } = "";

        // Из таблицы получаем готовую строку, из формы — собираем её из отдельных полей.
        // В файле достаточно отдельных телефонов: сводное поле вычисляется из них.
        [JsonIgnore]
        [XmlIgnore]
        public string AllPhones
        {
            get
            {
                return allPhones ?? string.Join("\n",
                    new[] { HomePhone, MobilePhone, WorkPhone }
                        .Select(CleanUpPhone)
                        .Where(phone => !string.IsNullOrEmpty(phone)));
            }
            set { allPhones = value; }
        }

        [JsonIgnore]
        [XmlIgnore]
        public string AllEmails
        {
            get
            {
                return allEmails ?? string.Join("\n",
                    new[] { Email, Email2, Email3 }
                        .Where(email => !string.IsNullOrEmpty(email)));
            }
            set { allEmails = value; }
        }

        private static string CleanUpPhone(string? phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return "";
            }

            return phone.Replace(" ", "").Replace("-", "")
                .Replace("(", "").Replace(")", "")
                .Replace(".", "").Replace("/", "").Replace("'", "");
        }

        [Column(Name = "bday")]
        public string BirthDay { get; set; } = "-";

        [Column(Name = "bmonth")]
        public string BirthMonth { get; set; } = "-";

        [Column(Name = "byear")]
        public string BirthYear { get; set; } = "";

        [Column(Name = "aday")]
        public string AnniversaryDay { get; set; } = "-";

        [Column(Name = "amonth")]
        public string AnniversaryMonth { get; set; } = "-";

        [Column(Name = "ayear")]
        public string AnniversaryYear { get; set; } = "";

        public static List<ContactData> GetAll()
        {
            using AdressBookDB db = new AdressBookDB();
            // В старых версиях активная запись имеет нулевую дату, в новых — NULL.
            // Дата удаления также отмечает старую версию изменённого контакта.
            return (from contact in db.Contacts
                    where contact.Deprecated == null || contact.Deprecated == "0000-00-00 00:00:00"
                    select contact).ToList();
        }

        // Обратная проверка: собираем текст страницы свойств из отдельных полей формы.
        public string GetAllDetails(DateTime today)
        {
            string personalInformation = JoinLines(
                string.Join(" ", new[] { FirstName, MiddleName, LastName }
                    .Where(value => !string.IsNullOrEmpty(value))),
                Nickname,
                Title,
                Company,
                Address.Trim());

            string phones = JoinLines(
                FormatPhone(HomePhone, "H:"),
                FormatPhone(MobilePhone, "M:"),
                FormatPhone(WorkPhone, "W:"));

            string emails = JoinLines(
                Email,
                Email2,
                Email3,
                string.IsNullOrEmpty(Homepage) ? "" : "Homepage:\n"
                    + Homepage.Replace("https://", "").Replace("http://", ""));

            string dates = JoinLines(
                FormatDate("Birthday", BirthDay, BirthMonth, BirthYear, today),
                FormatDate("Anniversary", AnniversaryDay, AnniversaryMonth, AnniversaryYear, today));

            // В браузере непустые блоки разделены пустой строкой.
            return string.Join("\n\n", new[] { personalInformation, phones, emails, dates }
                .Where(value => !string.IsNullOrEmpty(value)));
        }

        private static string JoinLines(params string[] lines)
        {
            return string.Join("\n", lines.Where(line => !string.IsNullOrEmpty(line)));
        }

        private static string FormatPhone(string phone, string label)
        {
            return string.IsNullOrEmpty(phone) ? "" : label + " " + phone;
        }

        private static string FormatDate(string label, string day, string month, string year, DateTime today)
        {
            int.TryParse(day, out int dayNumber);
            string monthName = string.IsNullOrEmpty(month) || month == "-" ? "" : month;
            string date = (dayNumber > 0 ? dayNumber + ". " : "") + monthName
                + (string.IsNullOrEmpty(year) ? "" : " " + year);

            if (string.IsNullOrWhiteSpace(date))
            {
                return "";
            }

            if (int.TryParse(year, out int yearNumber) && yearNumber >= 1800 && yearNumber <= 2200)
            {
                int monthNumber = Array.IndexOf(CultureInfo.InvariantCulture.DateTimeFormat.MonthNames, month) + 1;
                int age = today.Year - yearNumber;
                if (today.Month < monthNumber || (today.Month == monthNumber && today.Day < dayNumber))
                {
                    age--;
                }

                if (age != -1 && age < 150)
                {
                    date += " (" + age + ")";
                }
            }

            return label + " " + date.Trim();
        }

        public bool Equals(ContactData? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return FirstName == other.FirstName
                && LastName == other.LastName;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ContactData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstName, LastName);
        }

        public int CompareTo(ContactData? other)
        {
            if (other is null)
            {
                return 1;
            }

            int lastNameComparison = string.Compare(
                LastName,
                other.LastName,
                StringComparison.Ordinal);

            if (lastNameComparison != 0)
            {
                return lastNameComparison;
            }

            return string.Compare(
                FirstName,
                other.FirstName,
                StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return $"FirstName={FirstName}, LastName={LastName}";
        }
    }
}
