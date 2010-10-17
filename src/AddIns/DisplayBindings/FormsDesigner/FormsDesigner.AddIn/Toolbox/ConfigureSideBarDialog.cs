// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

// created on 07.08.2003 at 13:36
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.FormsDesigner.Gui
{
	public class ConfigureSideBarDialog : BaseSharpDevelopForm
	{
		ArrayList oldComponents;
		
		public ConfigureSideBarDialog()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.FormsDesigner.Resources.ConfigureSidebarDialog.xfrm"));
			
			oldComponents = ToolboxProvider.ComponentLibraryLoader.CopyCategories();
			
			FillCategories();
			categoryListViewSelectedIndexChanged(this, EventArgs.Empty);
			componentListViewSelectedIndexChanged(this, EventArgs.Empty);
			ControlDictionary["okButton"].Click               += new System.EventHandler(this.okButtonClick);
			ControlDictionary["newCategoryButton"].Click      += new System.EventHandler(this.newCategoryButtonClick);
			ControlDictionary["renameCategoryButton"].Click   += new System.EventHandler(this.renameCategoryButtonClick);
			ControlDictionary["removeCategoryButton"].Click   += new System.EventHandler(this.removeCategoryButtonClick);
			ControlDictionary["addComponentsButton"].Click    += new System.EventHandler(this.button3Click);
			ControlDictionary["removeComponentsButton"].Click += new System.EventHandler(this.removeComponentsButtonClick);
			
			((ListView)ControlDictionary["categoryListView"]).SelectedIndexChanged  += new System.EventHandler(this.categoryListViewSelectedIndexChanged);
			((ListView)ControlDictionary["componentListView"]).SelectedIndexChanged += new System.EventHandler(this.componentListViewSelectedIndexChanged);
		}
		
		void FillCategories()
		{
			((ListView)ControlDictionary["categoryListView"]).BeginUpdate();
			((ListView)ControlDictionary["categoryListView"]).Items.Clear();
			foreach (Category category in ToolboxProvider.ComponentLibraryLoader.Categories) {
				ListViewItem newItem = new ListViewItem(category.Name);
				newItem.Checked = category.IsEnabled;
				newItem.Tag     = category;
				((ListView)ControlDictionary["categoryListView"]).Items.Add(newItem);
			}
			((ListView)ControlDictionary["categoryListView"]).EndUpdate();
		}
		
		void FillComponents()
		{
			((ListView)ControlDictionary["componentListView"]).ItemCheck -= new System.Windows.Forms.ItemCheckEventHandler(this.componentListViewItemCheck);
			((ListView)ControlDictionary["componentListView"]).BeginUpdate();
			((ListView)ControlDictionary["componentListView"]).Items.Clear();
			((ListView)ControlDictionary["componentListView"]).SmallImageList = new ImageList();
			
			if (((ListView)ControlDictionary["categoryListView"]).SelectedItems != null && ((ListView)ControlDictionary["categoryListView"]).SelectedItems.Count == 1) {
				Category category = (Category)((ListView)ControlDictionary["categoryListView"]).SelectedItems[0].Tag;
				foreach (ToolComponent component in category.ToolComponents) {
					Bitmap icon = null;
					string loadError = null;
					try {
						icon = ToolboxProvider.ComponentLibraryLoader.GetIcon(component);
					} catch (Exception e) {
						
						icon = IconService.GetBitmap("Icons.16x16.Warning");
						loadError = e.Message;
					}
					if (icon != null) {
						((ListView)ControlDictionary["componentListView"]).SmallImageList.Images.Add(icon);
					}
					
					ListViewItem newItem = new ListViewItem(component.Name);
					newItem.SubItems.Add(component.Namespace);
					newItem.SubItems.Add(loadError != null ? loadError :component.AssemblyName);
					
					newItem.Checked = component.IsEnabled;
					newItem.Tag     = component;
					newItem.ImageIndex = ((ListView)ControlDictionary["componentListView"]).SmallImageList.Images.Count - 1;
					((ListView)ControlDictionary["componentListView"]).Items.Add(newItem);
				}
			}
			((ListView)ControlDictionary["componentListView"]).EndUpdate();
			((ListView)ControlDictionary["componentListView"]).ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.componentListViewItemCheck);
		}
		
		// THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
		// DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
		void categoryListViewSelectedIndexChanged(object sender, System.EventArgs e)
		{
			ControlDictionary["renameCategoryButton"].Enabled = ControlDictionary["addComponentsButton"].Enabled = CurrentCategory != null;
			FillComponents();
		}
		
		Category CurrentCategory {
			get {
				if (((ListView)ControlDictionary["categoryListView"]).SelectedItems != null && ((ListView)ControlDictionary["categoryListView"]).SelectedItems.Count == 1) {
					return (Category)((ListView)ControlDictionary["categoryListView"]).SelectedItems[0].Tag;
				}
				return null;
			}
		}
		
		void button3Click(object sender, System.EventArgs e)
		{
			AddComponentsDialog addComponentsDialog = new AddComponentsDialog();
			if (addComponentsDialog.ShowDialog(this) == DialogResult.OK) {
				foreach (ToolComponent component in addComponentsDialog.SelectedComponents) {
					CurrentCategory.ToolComponents.Add(component);
				}
				FillComponents();
				categoryListViewSelectedIndexChanged(this, EventArgs.Empty);
				componentListViewSelectedIndexChanged(this, EventArgs.Empty);
			}
		}
		
		void newCategoryButtonClick(object sender, System.EventArgs e)
		{
			RenameCategoryDialog renameCategoryDialog = new RenameCategoryDialog(null, this);
			if (renameCategoryDialog.ShowDialog(this) == DialogResult.OK) {
				ToolboxProvider.ComponentLibraryLoader.Categories.Add(new Category(renameCategoryDialog.CategoryName));
				FillCategories();
			}
		}
		
		void removeCategoryButtonClick(object sender, System.EventArgs e)
		{
			
			if (MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSideBarDialog.RemoveCategoryQuestion}")) {
				categoryListViewSelectedIndexChanged(this, EventArgs.Empty);
				ToolboxProvider.ComponentLibraryLoader.Categories.Remove(CurrentCategory);
				FillCategories();
				FillComponents();
				categoryListViewSelectedIndexChanged(this, EventArgs.Empty);
				componentListViewSelectedIndexChanged(this, EventArgs.Empty);
			}
		}
		
		void componentListViewSelectedIndexChanged(object sender, System.EventArgs e)
		{
			ControlDictionary["removeComponentsButton"].Enabled = ((ListView)ControlDictionary["componentListView"]).SelectedItems != null && ((ListView)ControlDictionary["componentListView"]).SelectedItems.Count > 0;
		}
		
		void removeComponentsButtonClick(object sender, System.EventArgs e)
		{
			
			if (MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSideBarDialog.RemoveComponentsQuestion}")) {
				foreach (ListViewItem item in ((ListView)ControlDictionary["componentListView"]).SelectedItems) {
					CurrentCategory.ToolComponents.Remove((ToolComponent)item.Tag);
				}
				FillComponents();
				componentListViewSelectedIndexChanged(this, EventArgs.Empty);
			}
		}
		
		protected override void OnClosed(System.EventArgs e)
		{
			base.OnClosed(e);
			if (oldComponents != null) {
				ToolboxProvider.ComponentLibraryLoader.Categories = oldComponents;
			}
		}
		
		void renameCategoryButtonClick(object sender, System.EventArgs e)
		{
			RenameCategoryDialog renameCategoryDialog = new RenameCategoryDialog(this.CurrentCategory.Name, this);
			if (renameCategoryDialog.ShowDialog(this) == DialogResult.OK) {
				this.CurrentCategory.Name = renameCategoryDialog.CategoryName;
				FillCategories();
			}
		}
		
		void okButtonClick(object sender, System.EventArgs e)
		{
			oldComponents = null;
			
			foreach (ListViewItem item in ((ListView)ControlDictionary["categoryListView"]).Items) {
				Category category = (Category)item.Tag;
				category.IsEnabled = item.Checked;
			}
					
			ToolboxProvider.SaveToolbox();
		}
		
		void componentListViewItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			ToolComponent tc = (ToolComponent)((ListView)ControlDictionary["componentListView"]).Items[e.Index].Tag;
			tc.IsEnabled = !tc.IsEnabled;
		}
	}
}
