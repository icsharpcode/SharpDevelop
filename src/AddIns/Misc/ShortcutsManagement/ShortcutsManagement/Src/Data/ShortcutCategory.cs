using System.Collections.Generic;
using System.ComponentModel;

namespace ICSharpCode.ShortcutsManagement.Data
{
	/// <summary>
	/// Stores shortcut category data
	/// </summary>
	public class ShortcutCategory : INotifyPropertyChanged, IShortcutTreeEntry
	{
		private string name;
		
		/// <summary>
		/// Gets or sets category name
		/// </summary>
		public string Name
		{
			get {
				return name;
			}
			set {
				if (name != value) {
					name = value;
					InvokePropertyChanged("Name");
				}
			}
		}
		
		private bool isVisible;
		
		/// <summary>
		/// Gets or sets whether category is visible in shortcuts tree
		/// </summary>
		public bool IsVisible
		{
			get {
				return isVisible;
			}
			set {
				if (isVisible != value) {
					isVisible = value;
					InvokePropertyChanged("IsVisible");
				}
			}
		}
		
		/// <summary>
		/// Gets list of sub-cateories 
		/// </summary>
		public List<ShortcutCategory> SubCategories
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets shortcuts assigned to this category
		/// </summary>
		public List<Shortcut> Shortcuts
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Occurs when <see cref="Name" /> or <see cref="IsVisible" /> property changes
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		
		/// <summary>
		/// Creates new instance of <see cref="ShortcutCategory" />
		/// </summary>
		/// <param name="categoryName">Category name</param>
		public ShortcutCategory(string categoryName)
		{
			IsVisible = true;
			Shortcuts = new List<Shortcut>(); 
			SubCategories = new List<ShortcutCategory>();
			Name = categoryName;
		}
		
		/// <summary>
		/// Sorts category shortcuts
		/// </summary>
		public void SortSubEntries()
		{
			SubCategories.Sort((a, b) => a.Name.CompareTo(b.Name));
			Shortcuts.Sort((a, b) => a.Name.CompareTo(b.Name));
			
			foreach (var category in SubCategories) {
				category.SortSubEntries();
			}
		}
		
		/// <summary>
		/// In this category and sub-categories finds shortcut by shortcut ID 
		/// </summary>
		/// <param name="shortcutId">Shortcut ID</param>
		/// <returns>Shortcut with ID equal to provided</returns>
		public Shortcut FindShortcut(string shortcutId)
		{
			// Search for shortcut in shortcuts assigned to this category
			foreach (var s in Shortcuts) {
				if(s.Id == shortcutId) {
					return s;
				}
			}
			
			// Search for shortcut in sub categories
			foreach (var category in SubCategories) {
				Shortcut foundShortcut;
				if ((foundShortcut = category.FindShortcut(shortcutId)) != null) {
					return foundShortcut;
				}
			}
			
			return null;
		}
		
		/// <summary>
		/// Compares <see cref="ShortcutCategory" /> instance to another instance of <see cref="IShortcutTreeEntry"/> type
		/// </summary>
		/// <param name="obj">Compared object</param>
		/// <returns>Comparison result</returns>
		public int CompareTo(object obj)
		{
			if (obj is AddIn) return 1;
			if (obj is Shortcut) return -1;
			
			var categoryObj = (ShortcutCategory)obj;
			return Name.CompareTo(categoryObj.Name);
		}
		
		/// <summary>
		/// Invoke dependency property changed event
		/// </summary>
		/// <param name="propertyName">Name of dependency property from this classs</param>
		private void InvokePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
