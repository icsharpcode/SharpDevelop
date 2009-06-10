using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ShortcutManagement=ICSharpCode.ShortcutsManagement.Data;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// Interaction logic for ShortcutsManagementOptionsPanel.xaml
    /// </summary>
    public partial class ShortcutsManagementOptionsPanel : UserControl, IOptionPanel
    {
        private static InputGestureCollection GetGestures(string gesturesString)
        {
            var converter = new InputGestureCollectionConverter();
            return (InputGestureCollection)converter.ConvertFromInvariantString(gesturesString);
        }


        public ShortcutsManagementOptionsPanel()
        {
            ResourceService.RegisterStrings("ICSharpCode.ShortcutsManagement.Resources.StringResources", GetType().Assembly);

            InitializeComponent();

            // Test data
            var addins = new ObservableCollection<ShortcutManagement.AddIn>();
            addins.Add(new ShortcutManagement.AddIn("SharpDevelop"));
            addins[0].Categories.Add(new ShortcutManagement.ShortcutCategory("Editing"));
            addins[0].Categories[0].Shortcuts.Add(new ShortcutManagement.Shortcut("Copy", GetGestures("Ctrl + C")));
            addins[0].Categories[0].Shortcuts.Add(new ShortcutManagement.Shortcut("Paste", GetGestures("Ctrl + V | Ctrl+Insert")));
            addins[0].Categories[0].Shortcuts.Add(new ShortcutManagement.Shortcut("Cut", GetGestures("Ctrl + X")));
            addins[0].Categories[0].Shortcuts.Add(new ShortcutManagement.Shortcut("Undo", GetGestures("Ctrl + Z")));
            addins[0].Categories[0].Shortcuts.Add(new ShortcutManagement.Shortcut("Redo", GetGestures("Ctrl + Y")));
            addins[0].Categories.Add(new ShortcutManagement.ShortcutCategory("Building"));
            addins[0].Categories[1].Shortcuts.Add(new ShortcutManagement.Shortcut("Build", GetGestures("Ctrl + Shift+B")));
            addins[0].Categories[1].Shortcuts.Add(new ShortcutManagement.Shortcut("Run", GetGestures("F5")));
            addins[0].Categories[1].Shortcuts.Add(new ShortcutManagement.Shortcut("Run without debuger", GetGestures("Ctrl + F5")));
            addins[0].Categories[1].Shortcuts.Add(new ShortcutManagement.Shortcut("Attach debuger", GetGestures("Ctrl + F8")));
            addins[0].Categories.Add(new ShortcutManagement.ShortcutCategory("Uncategorized"));
            addins[0].Categories[2].Shortcuts.Add(new ShortcutManagement.Shortcut("Attach debuger", GetGestures("Ctrl + F8")));

            addins.Add(new ShortcutManagement.AddIn("Search & replace"));
            addins[1].Categories.Add(new ShortcutManagement.ShortcutCategory("Uncategorized"));
            addins[1].Categories[0].Shortcuts.Add(new ShortcutManagement.Shortcut("Quick find", GetGestures("Ctrl + F")));
            addins[1].Categories[0].Shortcuts.Add(new ShortcutManagement.Shortcut("Quick replace", GetGestures("Ctrl + H")));
            addins[1].Categories[0].Shortcuts.Add(new ShortcutManagement.Shortcut("Find in files", GetGestures("Ctrl + Shift + F | Ctrl + Shift + H | Ctrl + I")));
            addins[1].Categories[0].Shortcuts.Add(new ShortcutManagement.Shortcut("Replace in files", GetGestures("Ctrl + Shift + H")));
            addins[1].Categories[0].Shortcuts.Add(new ShortcutManagement.Shortcut("Find symbol", null));

            addins.Add(new ShortcutManagement.AddIn("Unspecified"));
            addins[2].Categories.Add(new ShortcutManagement.ShortcutCategory("Uncategorized"));
            addins[2].Categories[0].Shortcuts.Add(new ShortcutManagement.Shortcut("Test regex expression", null));

            shortcutsManagementOptionsPanel.DataContext = addins;
        }

        public void LoadOptions() {
            
        }

        public bool SaveOptions() {
            return true;
        }

        public object Owner {
            get; set;
        }

        public object Control {
            get { return this; }
        }
    }
}
