// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Used to generate code for a ListView component currently being designed.
	/// </summary>
	public class PythonListViewComponent : PythonDesignerComponent
	{
		public PythonListViewComponent(IComponent component) : this(null, component)
		{
		}
		
		public PythonListViewComponent(PythonDesignerComponent parent, IComponent component) 
			: base(parent, component)
		{
		}
		
		/// <summary>
		/// Appends code that creates an instance of the list view.
		/// </summary>
		public override void AppendCreateInstance(PythonCodeBuilder codeBuilder)
		{
			// Append list view item creation first.
			int count = 1;
			foreach (ListViewItem item in GetListViewItems(Component)) {
				AppendCreateInstance(codeBuilder, item, count, GetConstructorParameters(item));
				++count;
			}
			
			// Append list view group creation.
			count = 1;
			foreach (ListViewGroup group in GetListViewGroups(Component)) {
				AppendCreateInstance(codeBuilder, group, count, new object[0]);
				++count;
			}
			
			// Append list view creation.
			base.AppendCreateInstance(codeBuilder);
		}
		
		/// <summary>
		/// Appends the component's properties.
		/// </summary>
		public override void AppendComponent(PythonCodeBuilder codeBuilder)
		{
			AppendComment(codeBuilder);
			AppendListViewItemProperties(codeBuilder);
			AppendListViewGroupProperties(codeBuilder);
			AppendComponentProperties(codeBuilder, true, false);
		}

		protected override bool ShouldAppendCollectionContent {
			get { return false; }
		}
		
		/// <summary>
		/// Gets the parameters to the ListViewItem constructor.
		/// </summary>
		/// <remarks>
		/// The constructors that are used:
		/// ListViewItem()
		/// ListViewItem(string text)
		/// ListViewItem(string[] subItems)
		/// ListViewItem(string text, int imageIndex)
		/// ListViewItem(string text, string imageKey)
		/// </remarks>
		object[] GetConstructorParameters(ListViewItem item)
		{
			if (item.SubItems.Count > 1) {
				string[] subItems = new string[item.SubItems.Count];
				for (int i = 0; i < item.SubItems.Count; ++i) {
					subItems[i] = item.SubItems[i].Text;
				}
				return new object[] {subItems};
			}
			
			if (item.ImageIndex != -1) {
				return GetConstructorParameters(item.Text, item.ImageIndex);
			}
			
			if (!String.IsNullOrEmpty(item.ImageKey)) {
				return GetConstructorParameters(item.Text, item.ImageKey);
			}
			
			if (String.IsNullOrEmpty(item.Text)) {
				return new object[0];
			}
			return new object[] {item.Text};
		}
		
		/// <summary>
		/// Creates an object array:
		/// [0] = text.
		/// [1] = obj.
		/// </summary>
		object[] GetConstructorParameters(string text, object obj)
		{
			if (String.IsNullOrEmpty(text)) {
				text = String.Empty;
			}
			return new object[] {text, obj};
		}
		
		static ListView.ListViewItemCollection GetListViewItems(IComponent component)
		{
			ListView listView = (ListView)component;
			return listView.Items;
		}

		static ListViewGroupCollection GetListViewGroups(IComponent component)
		{
			ListView listView = (ListView)component;
			return listView.Groups;
		}
		
		void AppendListViewItemProperties(PythonCodeBuilder codeBuilder)
		{
			int count = 1;
			foreach (ListViewItem item in GetListViewItems(Component)) {
				AppendObjectProperties(codeBuilder, item, count);
				++count;
			}
		}
		
		void AppendListViewGroupProperties(PythonCodeBuilder codeBuilder)
		{
			int count = 1;
			foreach (ListViewGroup item in GetListViewGroups(Component)) {
				AppendObjectProperties(codeBuilder, item, count);
				++count;
			}
		}		
	}
}
