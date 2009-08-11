using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ICSharpCode.ShortcutsManagement.Data
{
	/// <summary>
	/// Creates deep copy of <see cref="Shortcut" />, <see cref="ShortcutTree" /> or <see cref="AddIn" />
	/// </summary>
	public class IShortcutTreeEntryCloner
	{
		/// <summary>
		/// Creates new instance of <see cref="IShortcutTreeEntryCloner" />
		/// </summary>
		public IShortcutTreeEntryCloner()
		{
		}
		
		/// <summary>
		/// Deep clone collection of <see cref="IShortcutTreeEntry" />
		/// 
		/// Re-uses <see cref="Shortcut" /> instances for similar shortcuts in different categories
		/// </summary>
		/// <param name="entries">Collection of <see cref="IShortcutTreeEntry" /></param>
		/// <returns>Cloned entries</returns>
		public ICollection<IShortcutTreeEntry> CloneShortcutTree(ICollection<IShortcutTreeEntry> entries)
		{
			Shortcut shortcutCopy = null;
			var clonedShortcuts = new Dictionary<string, Shortcut>();
			var clonedEntries = new List<IShortcutTreeEntry>();
			
			// Make a deep copy of all add-ins, categories and shortcuts
			foreach (var entry in entries) {
				var shortcut = entry as Shortcut;
				var category = entry as ShortcutCategory;
				var addin = entry as AddIn;
				
				IShortcutTreeEntry clonedEntry = null;
				if(shortcut != null) {
					clonedEntry = CloneShortcut(shortcut, clonedShortcuts);
				} else if(category != null) {
					clonedEntry = CloneShortutCategory(category, clonedShortcuts);
				} else if(addin != null) {
					clonedEntry = CloneAddin(addin, clonedShortcuts);
				}
				
				clonedEntries.Add(clonedEntry);
			}
			
			return clonedEntries;
		}
		
			
		/// <summary>
		/// Creates a deep copy of <see cref="ShortcutCategory" /> instance
		/// 
		/// Re-uses <see cref="Shortcut" /> instances for similar shortcuts in different categories
		/// </summary>
		/// <param name="category">Cloned shortcut category</param>
		/// <param name="clonedShortcuts">Dictionary of shortcuts which should be re-used. If cloned shortcut <see cref="Shortcut.Id" /> property value is already preset in this dictionary instance from dictionary is used instead</param>
		/// <returns>Deep copy of <see cref="ShortcutCategory" /></returns>
		public ShortcutCategory CloneShortutCategory(ShortcutCategory category, Dictionary<string, Shortcut> clonedShortcuts)
		{
			var clonedCategory = new ShortcutCategory(category.Name);
			
			foreach (var subCategory in category.SubCategories) {
				clonedCategory.SubCategories.Add(CloneShortutCategory(subCategory, clonedShortcuts));
			}
			
			foreach (var shortcut in category.Shortcuts) {
				clonedCategory.Shortcuts.Add(CloneShortcut(shortcut, clonedShortcuts));
			}
			
			return clonedCategory;
		}
		
		 /// <summary>
		/// Create a deep copy of <see cref="AddIn" /> instance
		/// 
		/// Re-uses <see cref="Shortcut" /> instances for similar shortcuts in different categories
		/// </summary>
		/// <param name="shortcut">Cloned shortcut</param>
		/// <param name="clonedShortcuts">Dictionary of shortcuts which should be re-used. If cloned shortcut <see cref="Shortcut.Id" /> property value is already preset in this dictionary instance from dictionary is used instead</param>
		/// <returns>Deep copy of <see cref="Shortcut" /></returns>
		public Shortcut CloneShortcut(Shortcut shortcut, Dictionary<string, Shortcut> clonedShortcuts)
        {
        	if(clonedShortcuts.ContainsKey(shortcut.Id)) {
        		return clonedShortcuts[shortcut.Id];
        	} else {
				var clonedShortcut = new Shortcut(
					shortcut.Id,
					shortcut.Name, 
					new InputGestureCollection(shortcut.Gestures), 
					new InputGestureCollection(shortcut.DefaultGestures));
				
				clonedShortcuts.Add(clonedShortcut.Id, clonedShortcut);
				
				return clonedShortcut;
        	}
        }

		/// <summary>
		/// Creates a deep copy of <see cref="AddIn" /> instance
		/// 
		/// Re-uses <see cref="Shortcut" /> instances for similar shortcuts in different categories
		/// </summary>
		/// <param name="category">Cloned add-in</param>
		/// <param name="clonedShortcuts">Dictionary of shortcuts which should be re-used. If cloned shortcut <see cref="Shortcut.Id" /> property value is already preset in this dictionary instance from dictionary is used instead</param>
		/// <returns>Deep copy of <see cref="AddIn" /> instance</returns>
		public AddIn CloneAddin(AddIn addin, Dictionary<string, Shortcut> clonedShortcuts)
		{
			var clonedAddIn = new AddIn(addin.Name);
			foreach (var category in addin.Categories) {
				clonedAddIn.Categories.Add(CloneShortutCategory(category, clonedShortcuts));
			}
			
			return clonedAddIn;
		}
	}
}
