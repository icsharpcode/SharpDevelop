using System.Collections.Generic;
using System.ComponentModel;

namespace ICSharpCode.ShortcutsManagement.Data
{
	/// <summary>
	/// Add-in where shortcuts were registered
	/// </summary>
	public class AddIn : INotifyPropertyChanged, IShortcutTreeEntry
	{
		private string name;
		
		/// <summary>
		/// Gets or sets add-in name
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
		/// Gets or sets value specifying whether category is visible in shortcuts tree or not
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
		/// Gets list of add-in categories
		/// </summary>
		public List<ShortcutCategory> Categories
		{
			get; 
			private set;
		}
		
		/// <summary>
		/// Creates new instance of <see cref="AddIn" />
		/// </summary>
		/// <param name="addInName">Add-in name</param>
		public AddIn(string addInName)
		{
			IsVisible = true;
			Name = addInName;
			Categories = new List<ShortcutCategory>();
		}
		
		/// <summary>
		/// Finds shortcut by shortcut Id in add-in categories 
		/// </summary>
		/// <param name="shortcutId">Shortcut Id</param>
		/// <returns>Shortcut with matching Id value</returns>
		public Shortcut FindShortcut(string shortcutId)
		{
			foreach (var c in Categories) {
				Shortcut foundShortcut;
				if ((foundShortcut = c.FindShortcut(shortcutId)) != null) {
					return foundShortcut;
				}
			}
		
			return null;
		}
		
		/// <summary>
		/// Sorts add-in sub-categories
		/// </summary>
		public void SortSubEntries() 
		{
			Categories.Sort((a, b) => a.Name.CompareTo(b.Name));
			foreach (var category in Categories) {
				category.SortSubEntries();
			}
		}
		
		/// <summary>
		/// Occurs when <see cref="Name" /> or  or <see cref="IsVisible" /> property value changes
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		
		/// <summary>
		/// Compares add-in to other instances of type <see cref="IShortcutTreeEntry"/>
		/// </summary>
		/// <param name="obj">Compared object</param>
		/// <returns>Comparison result</returns>
		public int CompareTo(object obj) 
		{
			if (obj is ShortcutCategory) return -1;
			if (obj is Shortcut) return -1;
			
			var addInObj = (AddIn)obj;
			return Name.CompareTo(addInObj.Name);
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
