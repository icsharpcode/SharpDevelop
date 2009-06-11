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
            var addins = new ObservableCollection<ShortcutManagement.AddIn>();
            addins.Add(new ShortcutManagement.AddIn("SharpDevelop"));
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

            addins.Add(new ShortcutManagement.AddIn("Search & replace"));
            addins[1].Categories.Add(new ShortcutCategory("Uncategorized"));
            addins[1].Categories[0].Shortcuts.Add(new Shortcut("Quick find", GetGestures("Ctrl + F")));
            addins[1].Categories[0].Shortcuts.Add(new Shortcut("Quick replace", GetGestures("Ctrl + H")));
            addins[1].Categories[0].Shortcuts.Add(new Shortcut("Find in files", GetGestures("Ctrl + Shift + F | Ctrl + Shift + H | Ctrl + I")));
            addins[1].Categories[0].Shortcuts.Add(new Shortcut("Replace in files", GetGestures("Ctrl + Shift + H")));
            addins[1].Categories[0].Shortcuts.Add(new Shortcut("Find symbol", null));

            addins.Add(new ShortcutManagement.AddIn("Unspecified"));
            addins[2].Categories.Add(new ShortcutCategory("Uncategorized"));
            addins[2].Categories[0].Shortcuts.Add(new Shortcut("Test regex expression", null));

            shortcutsManagementOptionsPanel.DataContext = addins;
        }

        public void LoadOptions()
        {
            // Load shortcuts for real
            var unspecifiedAddInSection = new ShortcutManagement.AddIn("Unspecified");
            unspecifiedAddInSection.Categories.Add(new ShortcutCategory("Uncategorized"));

            var addIns = new List<ShortcutManagement.AddIn>();
            addIns.Add(unspecifiedAddInSection);

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
                    addIns.Add(addinSection);
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

            addIns.Sort((a, b) => a.Name.CompareTo(b.Name));
            foreach (var addIn in addIns)
            {
                addIn.SortEntries();
            }

            new ShortcutsFinder(addIns).Filter("");
            shortcutsManagementOptionsPanel.DataContext = addIns;
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
