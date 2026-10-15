using OpenQA.Selenium;

namespace addressbook_tests
{
    public class GroupHelper : HelperBase
    {
        // null означает, что список ещё не загружен или кеш сброшен.
        private List<GroupData>? groupCache = null;

        public GroupHelper(ApplicationManager manager) : base(manager)
        {
        }

        public GroupHelper Create(GroupData group)
        {
            InitGroupCreation();
            FillGroupForm(group);
            SubmitGroupCreation();
            manager.Navigator.ReturnToGroupsPage();
            return this;
        }

        public GroupHelper Modify(int index, GroupData group)
        {
            SelectGroup(index);
            InitGroupModification();
            FillGroupForm(group);
            SubmitGroupModification();
            manager.Navigator.ReturnToGroupsPage();
            return this;
        }

        public GroupHelper Modify(string id, GroupData group)
        {
            manager.Navigator.OpenGroupsPage();
            SelectGroup(id);
            InitGroupModification();
            FillGroupForm(group);
            SubmitGroupModification();
            manager.Navigator.ReturnToGroupsPage();
            return this;
        }

        public GroupHelper Remove(int index)
        {
            SelectGroup(index);
            RemoveSelectedGroups();
            manager.Navigator.ReturnToGroupsPage();
            return this;
        }

        public GroupHelper Remove(string id)
        {
            manager.Navigator.OpenGroupsPage();
            SelectGroup(id);
            RemoveSelectedGroups();
            manager.Navigator.ReturnToGroupsPage();
            return this;
        }

        public bool IsThereAGroup()
        {
            return IsElementPresent(By.Name("selected[]"));
        }

        public int GetGroupCount()
        {
            return driver.FindElements(By.Name("selected[]")).Count;
        }

        public List<GroupData> GetGroupList(bool forceReload = false)
        {
            if (forceReload)
            {
                groupCache = null;
                manager.Navigator.OpenGroupsPage();
                driver.Navigate().Refresh();
            }

            if (groupCache == null)
            {
                groupCache = new List<GroupData>();
                manager.Navigator.OpenGroupsPage();

                foreach (IWebElement element in driver.FindElements(By.CssSelector("span.group")))
                {
                    groupCache.Add(new GroupData(element.Text)
                    {
                        Id = element.FindElement(By.TagName("input")).GetAttribute("value")
                    });
                }
            }

            // Копия списка позволяет тесту выполнять Add, RemoveAt и Sort.
            return new List<GroupData>(groupCache);
        }

        public void InitGroupCreation()
        {
            driver.FindElement(By.Name("new")).Click();
        }

        public void FillGroupForm(GroupData group)
        {
            driver.FindElement(By.Name("group_name")).Click();
            driver.FindElement(By.Name("group_name")).Clear();
            driver.FindElement(By.Name("group_name")).SendKeys(group.Name);
            driver.FindElement(By.Name("group_header")).Click();
            driver.FindElement(By.Name("group_header")).Clear();
            driver.FindElement(By.Name("group_header")).SendKeys(group.Header);
            driver.FindElement(By.Name("group_footer")).Click();
            driver.FindElement(By.Name("group_footer")).Clear();
            driver.FindElement(By.Name("group_footer")).SendKeys(group.Footer);
        }

        public void SubmitGroupCreation()
        {
            driver.FindElement(By.Name("submit")).Click();
            groupCache = null;
        }

        public void SelectGroup(int index)
        {
            driver.FindElements(By.Name("selected[]"))[index].Click();
        }

        public void SelectGroup(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);
            ClickInView(driver.FindElement(By.CssSelector($"input[name='selected[]'][value='{id}']")));
        }

        public void InitGroupModification()
        {
            ClickInView(driver.FindElement(By.Name("edit")));
        }

        public void SubmitGroupModification()
        {
            driver.FindElement(By.Name("update")).Click();
            groupCache = null;
        }

        public void RemoveSelectedGroups()
        {
            ClickInView(driver.FindElement(By.Name("delete")));
            groupCache = null;
        }
    }
}
