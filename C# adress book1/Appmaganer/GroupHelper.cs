using OpenQA.Selenium;

namespace addressbook_tests
{
    public class GroupHelper : HelperBase
    {
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

        public GroupHelper RemoveSelectedGroup()
        {
            SelectGroup();
            RemoveSelectedGroups();
            manager.Navigator.ReturnToGroupsPage();
            return this;
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
        }

        public void SelectGroup()
        {
            driver.FindElement(By.Name("selected[]")).Click();
        }

        public void RemoveSelectedGroups()
        {
            driver.FindElement(By.Name("delete")).Click();
        }
    }
}
