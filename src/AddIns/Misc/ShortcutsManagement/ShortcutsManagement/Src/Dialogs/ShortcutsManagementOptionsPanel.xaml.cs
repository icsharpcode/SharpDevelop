using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// This panel is used in SharpDevelop options window to manage shortcuts
    /// </summary>
    public partial class ShortcutsManagementOptionsPanel : UserControl, IOptionPanel
    {
        public ShortcutsManagementOptionsPanel()
        {
            InitializeComponent();
            
        }

        private void shortcutEntry_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                MessageBox.Show("Changing shortcut");
            }
        }

        /// <summary>
        /// Filter shortcuts tree view. Display only matching shortcuts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var receiver = ((ShortcutsProvider) Resources["ShortcutsReceiver"]);
            receiver.Filter(searchTextBox.Text);

            if (!string.IsNullOrEmpty(searchTextBox.Text))
            {
            	// Select first visible shortcut
                var selectedAddIn = receiver.GetAddIns().FirstOrDefault(a => a.IsVisible);
                if (selectedAddIn != null)
                {
                    var selectedCategory = selectedAddIn.Categories.FirstOrDefault(c => c.IsVisible);
                    if (selectedCategory != null)
                    {
                        var selectedShortcut = selectedCategory.Shortcuts.FirstOrDefault(s => s.IsVisible);
                        if (selectedShortcut != null)
                        {
                            shortcutsTreeView.SelectItem(new List<object> { selectedAddIn, selectedCategory, selectedShortcut });
                        }
                    }
                }
            }
            else
            {
                shortcutsTreeView.SetExpandAll(false);
            }
        }

        public void LoadOptions()
        {
        }

        public bool SaveOptions()
        {
            return true;
        }

        public object Owner
        {
            get; set;
        }

        public object Control
        {
            get
            {
                return this;
            }
        }
    }
}
