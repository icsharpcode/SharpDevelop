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
			AppendComponentProperties(codeBuilder);
			AppendChildComponentProperties(codeBuilder);
		}
		
		/// <summary>
		/// Gets the parameters to the ListViewItem constructor.
		/// </summary>
		/// <remarks>
		/// The constructors that are used:
		/// ListViewItem()
		/// ListViewItem(String text)
		/// ListViewItem(string[] subItems)
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
			if (String.IsNullOrEmpty(item.Text)) {
				return new object[0];
			}
			return new object[] {item.Text};
		}
		
		static ListView.ListViewItemCollection GetListViewItems(IComponent component)
		{
			ListView listView = (ListView)component;
			return listView.Items;
		}
		
		void AppendListViewItemProperties(PythonCodeBuilder codeBuilder)
		{
			int count = 1;
			foreach (ListViewItem item in GetListViewItems(Component)) {
				AppendObjectProperties(codeBuilder, item, count);
				++count;
			}
		}
	}
}
