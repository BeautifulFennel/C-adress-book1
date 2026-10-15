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

        public ContactHelper Modify(string id, ContactData contact)
        {
            manager.Navigator.OpenHomePage();
            InitContactModification(id);
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

        public ContactHelper Remove(string id)
        {
            manager.Navigator.OpenHomePage();
            SelectContact(id);
            RemoveSelectedContacts();

            if (IsAlertPresent())
            {
                CloseAlertAndGetItsText();
            }

            manager.Navigator.OpenHomePage();
            return this;
        }

        public ContactHelper AddContactToGroup(ContactData contact, GroupData group)
        {
            ArgumentException.ThrowIfNullOrEmpty(group.Id);
            manager.Navigator.OpenHomePage();
            ClearGroupFilter();
            SelectContact(contact.Id!);
            // Названия могут совпадать или быть пустыми, поэтому выбираем по Id.
            IWebElement groupList = driver.FindElement(By.Name("to_group"));
            ScrollIntoView(groupList);
            new SelectElement(groupList).SelectByValue(group.Id);
            ClickInView(driver.FindElement(By.Name("add")));
            WaitForGroupOperation();
            return this;
        }

        public ContactHelper RemoveContactFromGroup(ContactData contact, GroupData group)
        {
            ArgumentException.ThrowIfNullOrEmpty(group.Id);
            manager.Navigator.OpenHomePage();
            SelectGroupFilter(group.Id);
            SelectContact(contact.Id!);
            ClickInView(driver.FindElement(By.Name("remove")));
            WaitForGroupOperation();
            return this;
        }

        public void ClearGroupFilter()
        {
            SelectGroupFilter("");
        }

        private void SelectGroupFilter(string groupId)
        {
            IWebElement groupList = driver.FindElement(By.CssSelector("select[name='group']"));
            ScrollIntoView(groupList);
            new SelectElement(groupList).SelectByValue(groupId);
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d =>
                d.FindElements(By.Id("maintable")).Count > 0
                // После перезагрузки выбранный option содержит имя вместо Id.
                // Применённый Id хранится в скрытом поле формы контактов.
                && d.FindElement(By.CssSelector("form[name='MainForm'] input[name='group']"))
                    .GetAttribute("value") == groupId);
        }

        private void WaitForGroupOperation()
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d =>
                d.FindElements(By.CssSelector("div.msgbox")).Any(message => message.Displayed));
            contactCache = null;
        }

        public bool IsThereAContact()
        {
            return IsElementPresent(By.Name("selected[]"));
        }

        public int GetContactCount()
        {
            return driver.FindElements(By.Name("selected[]")).Count;
        }

        public List<ContactData> GetContactList(bool forceReload = false)
        {
            if (forceReload)
            {
                contactCache = null;
                manager.Navigator.OpenHomePage();
                driver.Navigate().Refresh();
            }

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
                        cellArray[1].Text)
                    {
                        Id = row.FindElement(By.Name("selected[]")).GetAttribute("value"),
                        Address = NormalizeLineEndings(cellArray[3].Text),
                        AllEmails = NormalizeLineEndings(cellArray[4].Text),
                        AllPhones = NormalizeLineEndings(cellArray[5].Text)
                    });
                }

            }

            return new List<ContactData>(contactCache);
        }

        public ContactData GetContactInformationFromTable(string id)
        {
            manager.Navigator.OpenHomePage();
            IWebElement row = GetContactRow(id);
            IWebElement[] cells = row.FindElements(By.TagName("td")).ToArray();

            return new ContactData(cells[2].Text, cells[1].Text)
            {
                Id = id,
                Address = NormalizeLineEndings(cells[3].Text),
                AllEmails = NormalizeLineEndings(cells[4].Text),
                AllPhones = NormalizeLineEndings(cells[5].Text)
            };
        }

        public ContactData GetContactInformationFromEditForm(string id)
        {
            manager.Navigator.OpenHomePage();
            InitContactModification(id);

            return new ContactData(
                driver.FindElement(By.Name("firstname")).GetAttribute("value") ?? "",
                driver.FindElement(By.Name("lastname")).GetAttribute("value") ?? "")
            {
                Id = id,
                MiddleName = driver.FindElement(By.Name("middlename")).GetAttribute("value") ?? "",
                Nickname = driver.FindElement(By.Name("nickname")).GetAttribute("value") ?? "",
                Title = driver.FindElement(By.Name("title")).GetAttribute("value") ?? "",
                Company = driver.FindElement(By.Name("company")).GetAttribute("value") ?? "",
                Address = NormalizeLineEndings(
                    driver.FindElement(By.Name("address")).GetAttribute("value") ?? ""),
                HomePhone = driver.FindElement(By.Name("home")).GetAttribute("value") ?? "",
                MobilePhone = driver.FindElement(By.Name("mobile")).GetAttribute("value") ?? "",
                WorkPhone = driver.FindElement(By.Name("work")).GetAttribute("value") ?? "",
                Email = driver.FindElement(By.Name("email")).GetAttribute("value") ?? "",
                Email2 = driver.FindElement(By.Name("email2")).GetAttribute("value") ?? "",
                Email3 = driver.FindElement(By.Name("email3")).GetAttribute("value") ?? "",
                Homepage = driver.FindElement(By.Name("homepage")).GetAttribute("value") ?? "",
                BirthDay = new SelectElement(driver.FindElement(By.Name("bday"))).SelectedOption.Text,
                BirthMonth = new SelectElement(driver.FindElement(By.Name("bmonth"))).SelectedOption.Text,
                BirthYear = driver.FindElement(By.Name("byear")).GetAttribute("value") ?? "",
                AnniversaryDay = new SelectElement(driver.FindElement(By.Name("aday"))).SelectedOption.Text,
                AnniversaryMonth = new SelectElement(driver.FindElement(By.Name("amonth"))).SelectedOption.Text,
                AnniversaryYear = driver.FindElement(By.Name("ayear")).GetAttribute("value") ?? ""
            };
        }

        public string GetContactInformationFromDetails(string id)
        {
            manager.Navigator.OpenHomePage();
            IWebElement[] cells = GetContactRow(id)
                .FindElements(By.TagName("td")).ToArray();
            ClickInView(cells[6].FindElement(By.TagName("a")));

            return NormalizeLineEndings(driver.FindElement(By.Id("content")).Text);
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
            driver.FindElement(By.Name("nickname")).Clear();
            driver.FindElement(By.Name("nickname")).SendKeys(contact.Nickname);
            driver.FindElement(By.Name("title")).Clear();
            driver.FindElement(By.Name("title")).SendKeys(contact.Title);
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
            driver.FindElement(By.Name("homepage")).Clear();
            driver.FindElement(By.Name("homepage")).SendKeys(contact.Homepage);
            driver.FindElement(By.Name("bday")).Click();
            new SelectElement(driver.FindElement(By.Name("bday"))).SelectByText(contact.BirthDay);
            driver.FindElement(By.Name("bmonth")).Click();
            new SelectElement(driver.FindElement(By.Name("bmonth"))).SelectByText(contact.BirthMonth);
            driver.FindElement(By.Name("byear")).Click();
            driver.FindElement(By.Name("byear")).Clear();
            driver.FindElement(By.Name("byear")).SendKeys(contact.BirthYear);
            new SelectElement(driver.FindElement(By.Name("aday"))).SelectByText(contact.AnniversaryDay);
            new SelectElement(driver.FindElement(By.Name("amonth"))).SelectByText(contact.AnniversaryMonth);
            driver.FindElement(By.Name("ayear")).Clear();
            driver.FindElement(By.Name("ayear")).SendKeys(contact.AnniversaryYear);
        }

        public void SubmitContactCreation()
        {
            driver.FindElement(By.Name("submit")).Click();
            contactCache = null;
        }

        public void InitContactModification(int index)
        {
            IWebElement[] cells = driver.FindElements(By.Name("entry"))[index]
                .FindElements(By.TagName("td")).ToArray();
            cells[7].FindElement(By.TagName("a")).Click();
        }

        public void InitContactModification(string id)
        {
            ClickInView(GetContactRow(id).FindElements(By.TagName("td"))[7]
                .FindElement(By.TagName("a")));
        }

        private IWebElement GetContactRow(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);
            return driver.FindElement(By.XPath(
                $"//tr[@name='entry'][.//input[@name='selected[]' and @value='{id}']]"));
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

        public void SelectContact(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);
            IWebElement checkbox = driver.FindElement(
                By.CssSelector($"input[name='selected[]'][value='{id}']"));
            ClickInView(checkbox);
        }

        public void RemoveSelectedContacts()
        {
            ClickInView(driver.FindElement(By.Name("delete")));
            contactCache = null;
        }
    }
}
