// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Creates WPF toolbars from the AddIn Tree.
	/// </summary>
	public static class ToolBarService
	{
		/// <summary>
		/// Style key used for toolbar images.
		/// </summary>
		public static readonly object ImageStyleKey = new object();
		
		public static void UpdateStatus(IEnumerable toolBarItems)
		{
			MenuService.UpdateStatus(toolBarItems);
		}
		
		public static IList CreateToolBarItems(object owner, string addInTreePath)
		{
			return CreateToolBarItems(AddInTree.BuildItems<ToolbarItemDescriptor>(addInTreePath, owner, false));
		}
		
		static IList CreateToolBarItems(IEnumerable descriptors)
		{
			ArrayList result = new ArrayList();
			foreach (ToolbarItemDescriptor descriptor in descriptors) {
				object item = CreateToolBarItemFromDescriptor(descriptor);
				if (item is IMenuItemBuilder) {
					IMenuItemBuilder submenuBuilder = (IMenuItemBuilder)item;
					result.AddRange(submenuBuilder.BuildItems(descriptor.Codon, descriptor.Caller));
				} else {
					result.Add(item);
				}
			}
			return result;
		}
		
		static object CreateToolBarItemFromDescriptor(ToolbarItemDescriptor descriptor)
		{
			Codon codon = descriptor.Codon;
			object caller = descriptor.Caller;
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Item";
			
			bool createCommand = codon.Properties["loadclasslazy"] == "false";
			
			switch (type) {
				case "Separator":
					return new ConditionalSeparator(codon, caller, true);
				case "CheckBox":
					return "CheckBox";
					//return new ToolBarCheckBox(codon, caller);
				case "Item":
					return new ToolBarButton(codon, caller, createCommand);
				case "ComboBox":
					return new ToolBarComboBox(codon, caller);
				case "TextBox":
					return "TextBox";
					//return new ToolBarTextBox(codon, caller);
				case "Label":
					return "Label";
					//return new ToolBarLabel(codon, caller);
				case "DropDownButton":
					return "DropDownButton";
					//return new ToolBarDropDownButton(codon, caller, MenuService.CreateMenuItems(descriptor.SubItems));
				case "SplitButton":
					return new ToolBarSplitButton(codon, caller, MenuService.CreateMenuItems(null, descriptor.SubItems));
				case "Builder":
					return codon.AddIn.CreateObject(codon.Properties["class"]);
				default:
					throw new System.NotSupportedException("unsupported menu item type : " + type);
			}
		}
		
		static ToolBar CreateToolBar(object owner, AddInTreeNode treeNode)
		{
			ToolBar tb = new ToolBar();
			tb.ItemsSource = CreateToolBarItems(treeNode.BuildChildItems<ToolbarItemDescriptor>(owner));
			UpdateStatus(tb.ItemsSource); // setting Visible is only possible after the items have been added
			//new LanguageChangeWatcher(toolStrip);
			return tb;
		}
		
		public static ToolBar CreateToolBar(object owner, string addInTreePath)
		{
			return CreateToolBar(owner, AddInTree.GetTreeNode(addInTreePath));
		}
		
		public static ToolBar[] CreateToolBars(object owner, string addInTreePath)
		{
			AddInTreeNode treeNode;
			try {
				treeNode = AddInTree.GetTreeNode(addInTreePath);
			} catch (TreePathNotFoundException) {
				return null;
			}
			List<ToolBar> toolBars = new List<ToolBar>();
			foreach (AddInTreeNode childNode in treeNode.ChildNodes.Values) {
				toolBars.Add(CreateToolBar(owner, childNode));
			}
			return toolBars.ToArray();
		}
	}
}
