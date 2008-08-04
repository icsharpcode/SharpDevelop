// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public static class ToolbarService
	{
		public static ToolStripItem[] CreateToolStripItems(string path, object owner, bool throwOnNotFound)
		{
			return CreateToolStripItems(owner, AddInTree.GetTreeNode(path, throwOnNotFound));
		}
		
		public static ToolStripItem[] CreateToolStripItems(object owner, AddInTreeNode treeNode)
		{
			if (treeNode == null)
				return new ToolStripItem[0];
			List<ToolStripItem> collection = new List<ToolStripItem>();
			foreach (ToolbarItemDescriptor descriptor in treeNode.BuildChildItems<ToolbarItemDescriptor>(owner)) {
				object item = CreateToolbarItemFromDescriptor(descriptor);
				if (item is ToolStripItem) {
					collection.Add((ToolStripItem)item);
				} else {
					ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
					collection.AddRange(submenuBuilder.BuildSubmenu(null, owner));
				}
			}
			
			return collection.ToArray();
		}
		
		static object CreateToolbarItemFromDescriptor(ToolbarItemDescriptor descriptor)
		{
			Codon codon = descriptor.Codon;
			object caller = descriptor.Caller;
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Item";
			
			bool createCommand = codon.Properties["loadclasslazy"] == "false";
			
			switch (type) {
				case "Separator":
					return new ToolBarSeparator(codon, caller);
				case "CheckBox":
					return new ToolBarCheckBox(codon, caller);
				case "Item":
					return new ToolBarCommand(codon, caller, createCommand);
				case "ComboBox":
					return new ToolBarComboBox(codon, caller);
				case "TextBox":
					return new ToolBarTextBox(codon, caller);
				case "Label":
					return new ToolBarLabel(codon, caller);
				case "DropDownButton":
					return new ToolBarDropDownButton(codon, caller, MenuService.ConvertSubItems(descriptor.SubItems));
				case "SplitButton":
					return new ToolBarSplitButton(codon, caller, MenuService.ConvertSubItems(descriptor.SubItems));
				case "Builder":
					return codon.AddIn.CreateObject(codon.Properties["class"]);
				default:
					throw new System.NotSupportedException("unsupported menu item type : " + type);
			}
		}
		
		public static ToolStrip CreateToolStrip(object owner, AddInTreeNode treeNode)
		{
			ToolStrip toolStrip = new ToolStrip();
			toolStrip.Items.AddRange(CreateToolStripItems(owner, treeNode));
			UpdateToolbar(toolStrip); // setting Visible is only possible after the items have been added
			new LanguageChangeWatcher(toolStrip);
			return toolStrip;
		}
		
		class LanguageChangeWatcher {
			ToolStrip toolStrip;
			public LanguageChangeWatcher(ToolStrip toolStrip) {
				this.toolStrip = toolStrip;
				toolStrip.Disposed += Disposed;
				ResourceService.LanguageChanged += OnLanguageChanged;
			}
			void OnLanguageChanged(object sender, EventArgs e) {
				ToolbarService.UpdateToolbarText(toolStrip);
			}
			void Disposed(object sender, EventArgs e) {
				ResourceService.LanguageChanged -= OnLanguageChanged;
			}
		}
		
		public static ToolStrip CreateToolStrip(object owner, string addInTreePath)
		{
			return CreateToolStrip(owner, AddInTree.GetTreeNode(addInTreePath));
		}
		
		public static ToolStrip[] CreateToolbars(object owner, string addInTreePath)
		{
			AddInTreeNode treeNode;
			try {
				treeNode = AddInTree.GetTreeNode(addInTreePath);
			} catch (TreePathNotFoundException) {
				return null;
				
			}
			List<ToolStrip> toolBars = new List<ToolStrip>();
			foreach (AddInTreeNode childNode in treeNode.ChildNodes.Values) {
				toolBars.Add(CreateToolStrip(owner, childNode));
			}
			return toolBars.ToArray();
		}
		
		public static void UpdateToolbar(ToolStrip toolStrip)
		{
			toolStrip.SuspendLayout();
			foreach (ToolStripItem item in toolStrip.Items) {
				if (item is IStatusUpdate) {
					((IStatusUpdate)item).UpdateStatus();
				}
			}
			toolStrip.ResumeLayout();
			//toolStrip.Refresh();
		}
		
		public static void UpdateToolbarText(ToolStrip toolStrip)
		{
			toolStrip.SuspendLayout();
			foreach (ToolStripItem item in toolStrip.Items) {
				if (item is IStatusUpdate) {
					((IStatusUpdate)item).UpdateText();
				}
			}
			toolStrip.ResumeLayout();
		}
	}
}
