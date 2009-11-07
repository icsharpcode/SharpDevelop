using System;

namespace ICSharpCode.ShortcutsManagement.Data
{
    public interface IShortcutTreeEntry : IComparable
    {
		/// <summary>
		/// Gets or sets shortcut entry name displayed in shortcuts tree
		/// </summary>
		string Name
		{
		get; set;
		}
		
		/// <summary>
		/// Specifies whether shortcut tree entry is visible
		/// </summary>
		bool IsVisible
		{
			get; set;
		}
		
		/// <summary>
		/// Sort shortcut entry sub elements
		/// </summary>
		void SortSubEntries();
		
		/// <summary>
		/// Search for shortcut in this shortcut entry and sub-elements
		/// </summary>
		/// <param name="shortcutId"></param>
		/// <returns></returns>
		Shortcut FindShortcut(string shortcutId);
	}
}
