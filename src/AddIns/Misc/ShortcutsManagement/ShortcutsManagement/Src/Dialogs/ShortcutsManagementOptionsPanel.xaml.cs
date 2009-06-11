using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.ShortcutsManagement.Data;
using AddIn=ICSharpCode.Core.AddIn;
using ShortcutManagement=ICSharpCode.ShortcutsManagement.Data;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// Interaction logic for ShortcutsManagementOptionsPanel.xaml
    /// </summary>
    public partial class ShortcutsManagementOptionsPanel : UserControl, IOptionPanel
    {
        private readonly Dictionary<Shortcut, InputBindingInfo> shortcutsMap = new Dictionary<Shortcut, InputBindingInfo>();

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
            var rootEntries = new ObservableCollection<IShortcutTreeEntry>();

            var addin1 = new ShortcutManagement.AddIn("SharpDevelop");
            rootEntries.Add(addin1);
            addin1.Categories.Add(new ShortcutCategory("Editing"));
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

            var addin2 = new ShortcutManagement.AddIn("Search & replace");
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

            var addin3 = new ShortcutManagement.AddIn("Unspecified");
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

            shortcutsManagementOptionsPanel.DataContext = rootEntries;
        }

        public void LoadOptions()
        {
            // Load shortcuts for real
            var unspecifiedAddInSection = new ShortcutManagement.AddIn("Unspecified");
            unspecifiedAddInSection.Categories.Add(new ShortcutCategory("Uncategorized"));

            var rootEntries = new List<IShortcutTreeEntry>();
            rootEntries.Add(unspecifiedAddInSection);

            var addInsMap = new Dictionary<AddIn, ShortcutManagement.AddIn>();
            var categoriesMap = new Dictionary<ShortcutManagement.AddIn, Dictionary<string, ShortcutCategory>>();

            var inputBindingInfos = CommandsRegistry.FindInputBindingInfos(null, null, null);
            foreach(var inputBindingInfo in inputBindingInfos) {
                ShortcutManagement.AddIn addinSection;
                if(inputBindingInfo.AddIn == null) {
                    addinSection = unspecifiedAddInSection;
                } else if (addInsMap.ContainsKey(inputBindingInfo.AddIn)) {
                    addinSection = addInsMap[inputBindingInfo.AddIn];
                } else {
                    addinSection = new ShortcutManagement.AddIn(inputBindingInfo.AddIn.Name);
                    addinSection.Categories.Add(new ShortcutCategory("Uncategorized"));
                    addInsMap.Add(inputBindingInfo.AddIn, addinSection);
                    categoriesMap.Add(addinSection, new Dictionary<string, ShortcutCategory>());
                    rootEntries.Add(addinSection);
                }

                ShortcutCategory categorySection;
                if(string.IsNullOrEmpty(inputBindingInfo.CategoryName)) {
                    categorySection = addinSection.Categories[0];
                } else if(!categoriesMap[addinSection].ContainsKey(inputBindingInfo.CategoryName)) {
                    categorySection = new ShortcutCategory(inputBindingInfo.CategoryName);
                    addinSection.Categories.Add(categorySection);
                    categoriesMap[addinSection].Add(inputBindingInfo.CategoryName, categorySection);
                } else {
                    categorySection = categoriesMap[addinSection][inputBindingInfo.CategoryName];
                }

                var shortcutText = !string.IsNullOrEmpty(inputBindingInfo.RoutedCommandText)
                                       ? inputBindingInfo.RoutedCommandText
                                       : inputBindingInfo.RoutedCommand.Text;
                shortcutText = StringParser.Parse(shortcutText);
                
                // Some commands have "&" sign to mark alternative key used to call this command from menu
                // Strip this sign
                shortcutText = Regex.Replace(shortcutText, @"&([^\s])", @"$1");

                var shortcut = new Shortcut(shortcutText, inputBindingInfo.Gestures);
                categorySection.Shortcuts.Add(shortcut);

                shortcutsMap.Add(shortcut, inputBindingInfo);
            }

            rootEntries.Sort();
            foreach (var entry in rootEntries)
            {
                entry.SortSubEntries();
            }

            new ShortcutsFinder(rootEntries).Filter("");
            shortcutsManagementOptionsPanel.DataContext = rootEntries;
        }

        public bool SaveOptions() {

            foreach (var pair in shortcutsMap)
            {
                var shortcut = pair.Key;
                var inputBindingInfo = pair.Value;

                inputBindingInfo.Gestures = new InputGestureCollection(shortcut.Gestures);
            }

            CommandsRegistry.InvokeInputBindingUpdateHandlers(null, null);

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
