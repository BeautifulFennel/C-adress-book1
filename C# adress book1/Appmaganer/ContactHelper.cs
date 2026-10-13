using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace addressbook_tests
{
    public class ContactHelper : HelperBase
    {
        // Пустой список — действительный кеш; null — необходимость загрузки.
        private List<ContactData>? contactCache = null;

        public ContactHelper(ApplicationManager manager) : base(manager)
        {
        }

        public ContactHelper Create(ContactData contact)
        {
            InitContactCreation();
            FillContactForm(contact);
            SubmitContactCreation();
            manager.Navigator.ReturnToHomePage();
            return this;
        }

        public ContactHelper Modify(int index, ContactData contact)
        {
            InitContactModification(index);
            FillContactForm(contact);
            SubmitContactModification();
            manager.Navigator.ReturnToHomePage();
            return this;
        }

        public ContactHelper Remove(int index)
        {
            SelectContact(index);
            RemoveSelectedContacts();

            if (IsAlertPresent())
            {
                CloseAlertAndGetItsText();
            }

            manager.Navigator.OpenHomePage();
            return this;
        }

        public bool IsThereAContact()
        {
            return IsElementPresent(By.Name("selected[]"));
        }

        public int GetContactCount()
        {
            return driver.FindElements(By.Name("selected[]")).Count;
        }

        public List<ContactData> GetContactList()
        {
            if (contactCache == null)
            {
                contactCache = new List<ContactData>();
                manager.Navigator.OpenHomePage();

                foreach (IWebElement row in driver.FindElements(By.Name("entry")))
                {
                    IReadOnlyCollection<IWebElement> cells = row.FindElements(By.TagName("td"));
                    IWebElement[] cellArray = cells.ToArray();

                    contactCache.Add(new ContactData(
                        cellArray[2].Text,
                        cellArray[1].Text));
                }

            }

            return new List<ContactData>(contactCache);
        }

        public ContactData GetContactInformationFromTable(int index)
        {
            manager.Navigator.OpenHomePage();
            IWebElement row = driver.FindElements(By.Name("entry"))[index];
            IWebElement[] cells = row.FindElements(By.TagName("td")).ToArray();

            return new ContactData(cells[2].Text, cells[1].Text)
            {
                Address = NormalizeLineEndings(cells[3].Text),
                AllEmails = NormalizeLineEndings(cells[4].Text),
                AllPhones = NormalizeLineEndings(cells[5].Text)
            };
        }

        public ContactData GetContactInformationFromEditForm(int index)
        {
            manager.Navigator.OpenHomePage();
            InitContactModification(index);

            return new ContactData(
                driver.FindElement(By.Name("firstname")).GetAttribute("value") ?? "",
                driver.FindElement(By.Name("lastname")).GetAttribute("value") ?? "")
            {
                Address = NormalizeLineEndings(
                    driver.FindElement(By.Name("address")).GetAttribute("value") ?? ""),
                HomePhone = driver.FindElement(By.Name("home")).GetAttribute("value") ?? "",
                MobilePhone = driver.FindElement(By.Name("mobile")).GetAttribute("value") ?? "",
                WorkPhone = driver.FindElement(By.Name("work")).GetAttribute("value") ?? "",
                Email = driver.FindElement(By.Name("email")).GetAttribute("value") ?? "",
                Email2 = driver.FindElement(By.Name("email2")).GetAttribute("value") ?? "",
                Email3 = driver.FindElement(By.Name("email3")).GetAttribute("value") ?? ""
            };
        }

        private static string NormalizeLineEndings(string text)
        {
            return text.Replace("\r\n", "\n");
        }

        public void InitContactCreation()
        {
            driver.FindElement(By.LinkText("add new")).Click();
        }

        public void FillContactForm(ContactData contact)
        {
            driver.FindElement(By.Name("firstname")).Click();
            driver.FindElement(By.Name("firstname")).Clear();
            driver.FindElement(By.Name("firstname")).SendKeys(contact.FirstName);
            driver.FindElement(By.Name("middlename")).Click();
            driver.FindElement(By.Name("middlename")).Clear();
            driver.FindElement(By.Name("middlename")).SendKeys(contact.MiddleName);
            driver.FindElement(By.Name("lastname")).Click();
            driver.FindElement(By.Name("lastname")).Clear();
            driver.FindElement(By.Name("lastname")).SendKeys(contact.LastName);
            driver.FindElement(By.Name("company")).Click();
            driver.FindElement(By.Name("company")).Clear();
            driver.FindElement(By.Name("company")).SendKeys(contact.Company);
            driver.FindElement(By.Name("address")).Click();
            driver.FindElement(By.Name("address")).Clear();
            driver.FindElement(By.Name("address")).SendKeys(contact.Address);
            driver.FindElement(By.Name("home")).Click();
            driver.FindElement(By.Name("home")).Clear();
            driver.FindElement(By.Name("home")).SendKeys(contact.HomePhone);
            driver.FindElement(By.Name("mobile")).Click();
            driver.FindElement(By.Name("mobile")).Clear();
            driver.FindElement(By.Name("mobile")).SendKeys(contact.MobilePhone);
            driver.FindElement(By.Name("work")).Click();
            driver.FindElement(By.Name("work")).Clear();
            driver.FindElement(By.Name("work")).SendKeys(contact.WorkPhone);
            driver.FindElement(By.Name("email")).Click();
            driver.FindElement(By.Name("email")).Clear();
            driver.FindElement(By.Name("email")).SendKeys(contact.Email);
            driver.FindElement(By.Name("email2")).Click();
            driver.FindElement(By.Name("email2")).Clear();
            driver.FindElement(By.Name("email2")).SendKeys(contact.Email2);
            driver.FindElement(By.Name("email3")).Click();
            driver.FindElement(By.Name("email3")).Clear();
            driver.FindElement(By.Name("email3")).SendKeys(contact.Email3);
            driver.FindElement(By.Name("bday")).Click();
            new SelectElement(driver.FindElement(By.Name("bday"))).SelectByText(contact.BirthDay);
            driver.FindElement(By.Name("bmonth")).Click();
            new SelectElement(driver.FindElement(By.Name("bmonth"))).SelectByText(contact.BirthMonth);
            driver.FindElement(By.Name("byear")).Click();
            driver.FindElement(By.Name("byear")).Clear();
            driver.FindElement(By.Name("byear")).SendKeys(contact.BirthYear);
        }

        public void SubmitContactCreation()
        {
            driver.FindElement(By.Name("submit")).Click();
            contactCache = null;
        }

        public void InitContactModification(int index)
        {
            driver.FindElements(By.Name("entry"))[index]
                .FindElement(By.XPath(".//img[@alt='Edit']")).Click();
        }

        public void SubmitContactModification()
        {
            driver.FindElement(By.Name("update")).Click();
            contactCache = null;
        }

        public void SelectContact(int index)
        {
            driver.FindElements(By.Name("selected[]"))[index].Click();
        }

        public void RemoveSelectedContacts()
        {
            driver.FindElement(By.Name("delete")).Click();
            contactCache = null;
        }
    }
}
