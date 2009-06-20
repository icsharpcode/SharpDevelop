using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;
using ICSharpCode.ShortcutsManagement.Data;
using AddInSection = ICSharpCode.ShortcutsManagement.Data.AddIn;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            // Test data
            var rootEntries = new ObservableCollection<IShortcutTreeEntry>();
            
            var addin1 = new AddInSection("SharpDevelop");
            rootEntries.Add(addin1);
            addin1.Categories.Add(new ShortcutCategory("Editing"));
            addin1.Categories[0].Shortcuts.Add(new Shortcut("MultiKey", GetGestures("Ctrl+C, Ctrl+K")));
            addin1.Categories[0].Shortcuts.Add(new Shortcut("Copy", GetGestures("Ctrl + C")));
            addin1.Categories[0].Shortcuts.Add(new Shortcut("Paste", GetGestures("Ctrl + V | Ctrl+Insert")));
            addin1.Categories[0].Shortcuts.Add(new Shortcut("Cut", GetGestures("Ctrl + X")));
            addin1.Categories[0].Shortcuts.Add(new Shortcut("Undo", GetGestures("Ctrl + Z")));
            addin1.Categories[0].Shortcuts.Add(new Shortcut("Redo", GetGestures("Ctrl + Y")));
            addin1.Categories.Add(new ShortcutCategory("Building"));
            addin1.Categories[1].Shortcuts.Add(new Shortcut("Build", GetGestures("Ctrl + Shift+B")));
            addin1.Categories[1].Shortcuts.Add(new Shortcut("Run", GetGestures("F5")));
            addin1.Categories[1].Shortcuts.Add(new Shortcut("Run without debuger", GetGestures("Ctrl + F5")));
            addin1.Categories[1].Shortcuts.Add(new Shortcut("Attach debuger", GetGestures("Ctrl + F8")));
            addin1.Categories.Add(new ShortcutCategory("Uncategorized"));
            addin1.Categories[2].Shortcuts.Add(new Shortcut("Attach debuger", GetGestures("Ctrl + F8")));

            var addin2 = new AddInSection("Search & replace");
            rootEntries.Add(addin2);
            addin2.Categories.Add(new ShortcutCategory("Uncategorized"));
            addin2.Categories[0].Shortcuts.Add(new Shortcut("Quick find", GetGestures("Ctrl + F")));
            addin2.Categories[0].Shortcuts.Add(new Shortcut("Quick replace", GetGestures("Ctrl + H")));
            addin2.Categories[0].SubCategories.Add(new ShortcutCategory("Subcategory 3"));
            addin2.Categories[0].SubCategories[0].SubCategories.Add(new ShortcutCategory("Subcategory 4"));
            addin2.Categories[0].SubCategories[0].SubCategories[0].Shortcuts.Add(new Shortcut("Shortcut N", GetGestures("Ctrl + N")));
            addin2.Categories[0].SubCategories[0].SubCategories[0].Shortcuts.Add(new Shortcut("Shortcut O", GetGestures("Ctrl + O")));
            addin2.Categories[0].SubCategories[0].Shortcuts.Add(new Shortcut("Shortcut L", GetGestures("Ctrl + L")));
            addin2.Categories[0].SubCategories[0].Shortcuts.Add(new Shortcut("Shortcut M", GetGestures("Ctrl + M")));
            addin2.Categories[0].Shortcuts.Add(new Shortcut("Find in files", GetGestures("Ctrl + Shift + F | Ctrl + Shift + H | Ctrl + I")));
            addin2.Categories[0].Shortcuts.Add(new Shortcut("Replace in files", GetGestures("Ctrl + Shift + H")));
            addin2.Categories[0].Shortcuts.Add(new Shortcut("Find symbol", null));

            var addin3 = new AddInSection("Unspecified");
            rootEntries.Add(addin3);
            addin3.Categories.Add(new ShortcutCategory("Uncategorized"));
            addin3.Categories[0].Shortcuts.Add(new Shortcut("Test regex expression", null));

            var rootCategory = new ShortcutCategory("Without addin");
            rootEntries.Add(rootCategory);
            rootCategory.SubCategories.Add(new ShortcutCategory("Subcategory 1"));
            rootCategory.SubCategories[0].Shortcuts.Add(new Shortcut("Shortcut H", GetGestures("Ctrl + H")));
            rootCategory.SubCategories[0].Shortcuts.Add(new Shortcut("Shortcut I", GetGestures("Ctrl + I")));
            rootCategory.SubCategories.Add(new ShortcutCategory("Subcategory 2"));
            rootCategory.SubCategories[0].Shortcuts.Add(new Shortcut("Shortcut J", GetGestures("Ctrl + J")));
            rootCategory.SubCategories[0].Shortcuts.Add(new Shortcut("Shortcut K", GetGestures("Ctrl + K")));
            rootCategory.Shortcuts.Add(new Shortcut("Shortcut A", GetGestures("Ctrl + A")));
            rootCategory.Shortcuts.Add(new Shortcut("Shortcut B", GetGestures("Ctrl + B")));
            rootCategory.Shortcuts.Add(new Shortcut("Shortcut C", GetGestures("Ctrl + C")));
            rootCategory.Shortcuts.Add(new Shortcut("Shortcut D", GetGestures("Ctrl + D")));
            rootCategory.Shortcuts.Add(new Shortcut("Shortcut E", null));

            rootEntries.Add(new Shortcut("Shortcut F", GetGestures("Ctrl + F")));
            rootEntries.Add(new Shortcut("Shortcut G", GetGestures("Ctrl + G")));

            optionsPanel.DataContext = rootEntries;
        }

        private static InputGestureCollection GetGestures(string gesturesString)
        {
            var converter = new InputGestureCollectionConverter();
            return (InputGestureCollection)converter.ConvertFromInvariantString(gesturesString);
        }
    }
}
