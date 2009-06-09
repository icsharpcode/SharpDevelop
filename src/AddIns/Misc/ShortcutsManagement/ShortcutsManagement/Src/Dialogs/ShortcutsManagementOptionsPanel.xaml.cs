using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;

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
            InitializeComponent();

            // Test data
            var addins = new ObservableCollection<AddIn>();
            addins.Add(new AddIn("SharpDevelop"));
            addins[0].Categories.Add(new ShortcutCategory("Editing"));
            addins[0].Categories[0].Shortcuts.Add(new Shortcut("Copy", GetGestures("Ctrl + C")));
            addins[0].Categories[0].Shortcuts.Add(new Shortcut("Paste", GetGestures("Ctrl + V | Ctrl+Insert")));
            addins[0].Categories[0].Shortcuts.Add(new Shortcut("Cut", GetGestures("Ctrl + X")));
            addins[0].Categories[0].Shortcuts.Add(new Shortcut("Undo", GetGestures("Ctrl + Z")));
            addins[0].Categories[0].Shortcuts.Add(new Shortcut("Redo", GetGestures("Ctrl + Y")));
            addins[0].Categories.Add(new ShortcutCategory("Building"));
            addins[0].Categories[1].Shortcuts.Add(new Shortcut("Build", GetGestures("Ctrl + Shift+B")));
            addins[0].Categories[1].Shortcuts.Add(new Shortcut("Run", GetGestures("F5")));
            addins[0].Categories[1].Shortcuts.Add(new Shortcut("Run without debuger", GetGestures("Ctrl + F5")));
            addins[0].Categories[1].Shortcuts.Add(new Shortcut("Attach debuger", GetGestures("Ctrl + F8")));
            addins[0].Categories.Add(new ShortcutCategory("Uncategorized"));
            addins[0].Categories[2].Shortcuts.Add(new Shortcut("Attach debuger", GetGestures("Ctrl + F8")));

            addins.Add(new AddIn("Search & replace"));
            addins[1].Categories.Add(new ShortcutCategory("Uncategorized"));
            addins[1].Categories[0].Shortcuts.Add(new Shortcut("Quick find", GetGestures("Ctrl + F")));
            addins[1].Categories[0].Shortcuts.Add(new Shortcut("Quick replace", GetGestures("Ctrl + H")));
            addins[1].Categories[0].Shortcuts.Add(new Shortcut("Find in files", GetGestures("Ctrl + Shift + F | Ctrl + Shift + H | Ctrl + I")));
            addins[1].Categories[0].Shortcuts.Add(new Shortcut("Replace in files", GetGestures("Ctrl + Shift + H")));
            addins[1].Categories[0].Shortcuts.Add(new Shortcut("Find symbol", null));

            addins.Add(new AddIn("Unspecified"));
            addins[2].Categories.Add(new ShortcutCategory("Uncategorized"));
            addins[2].Categories[0].Shortcuts.Add(new Shortcut("Test regex expression", null));

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
