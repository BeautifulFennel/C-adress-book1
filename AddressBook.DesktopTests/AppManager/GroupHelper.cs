using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TreeItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.WindowsAPI;

namespace addressbook_tests_white
{
    public class GroupHelper : HelperBase
    {
        public const string GroupWindowTitle = "Group editor";

        public GroupHelper(ApplicationManager manager) : base(manager) { }

        public List<GroupData> GetGroupList()
        {
            Window dialog = OpenGroupsDialog();
            try
            {
                return GetRoot(dialog).Nodes.Select(node => new GroupData(node.Text)).ToList();
            }
            finally
            {
                CloseGroupsDialog(dialog);
            }
        }

        public int GetGroupCount()
        {
            return GetGroupList().Count;
        }

        public void Add(GroupData group)
        {
            if (group == null || string.IsNullOrWhiteSpace(group.Name))
            {
                throw new ArgumentException("Название группы не должно быть пустым.", nameof(group));
            }

            Window dialog = OpenGroupsDialog();
            try
            {
                // Новая группа создаётся в корне, даже если до этого был выбран другой узел.
                GetRoot(dialog).Select();
                dialog.Get<Button>("uxNewAddressButton").Click();
                TextBox editor = dialog.Get<TextBox>(SearchCriteria.ByControlType(ControlType.Edit));
                // BulkText передаёт строку через UI Automation и не зависит от раскладки.
                editor.BulkText = group.Name;
                editor.Focus();
                dialog.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);
            }
            finally
            {
                CloseGroupsDialog(dialog);
            }
        }

        public void Remove(GroupData group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            Window dialog = OpenGroupsDialog();
            try
            {
                TreeNode node = GetRoot(dialog).Nodes.Single(item => item.Text == group.Name);
                node.Select();
                dialog.Get<Button>("uxDeleteAddressButton").Click();
                Window confirmation = dialog.ModalWindows().Single();
                confirmation.Get<RadioButton>("uxDeleteGroupsOnlyRadioButton").Select();
                confirmation.Get<Button>("uxOKAddressButton").Click();
            }
            finally
            {
                CloseGroupsDialog(dialog);
            }
        }

        private Window OpenGroupsDialog()
        {
            manager.MainWindow.Get<Button>("groupButton").Click();
            return manager.MainWindow.ModalWindow(GroupWindowTitle);
        }

        private static TreeNode GetRoot(Window dialog)
        {
            Tree tree = dialog.Get<Tree>("uxAddressTreeView");
            TreeNode root = tree.Nodes[0];
            root.Expand();
            return root;
        }

        private static void CloseGroupsDialog(Window dialog)
        {
            dialog.Get<Button>("uxCloseAddressButton").Click();
        }
    }
}
