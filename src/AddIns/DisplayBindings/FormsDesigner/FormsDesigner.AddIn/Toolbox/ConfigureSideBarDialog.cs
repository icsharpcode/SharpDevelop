// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.FormsDesigner.Gui
{
	public partial class ConfigureSideBarDialog : Form
	{
		List<Category> oldComponents;
		ToolboxProvider toolbox;
		
		public ConfigureSideBarDialog(ToolboxProvider toolbox)
		{
			if (toolbox == null)
				throw new ArgumentNullException("toolbox");
			InitializeComponent();
			
			this.toolbox = toolbox;
			oldComponents = toolbox.ComponentLibraryLoader.CopyCategories();
			Translate(this);
			FillCategories();
			CategoryListViewSelectedIndexChanged(this, EventArgs.Empty);
			ComponentListViewSelectedIndexChanged(this, EventArgs.Empty);
		}
		
		void Translate(Control control)
		{
			control.Text = StringParser.Parse(control.Text);
			foreach (Control child in control.Controls) {
				Translate(child);
			}
			if (control is ListView) {
				var lv = control as ListView;
				foreach (ColumnHeader col in lv.Columns) {
					col.Text = StringParser.Parse(col.Text);
				}
			}
		}
		
		void FillCategories()
		{
			categoryListView.BeginUpdate();
			categoryListView.Items.Clear();
			foreach (Category category in toolbox.ComponentLibraryLoader.Categories) {
				ListViewItem newItem = new ListViewItem(category.Name);
				newItem.Checked = category.IsEnabled;
				newItem.Tag     = category;
				categoryListView.Items.Add(newItem);
			}
			categoryListView.EndUpdate();
		}
		
		void FillComponents()
		{
			componentListView.ItemCheck -= this.ComponentListViewItemCheck;
			componentListView.BeginUpdate();
			componentListView.Items.Clear();
			componentListView.SmallImageList = new ImageList();
			
			if (categoryListView.SelectedItems != null && categoryListView.SelectedItems.Count == 1) {
				Category category = (Category)categoryListView.SelectedItems[0].Tag;
				foreach (ToolComponent component in category.ToolComponents) {
					Bitmap icon = null;
					string loadError = null;
					
					try {
						icon = toolbox.ComponentLibraryLoader.GetIcon(component);
					} catch (Exception e) {
						
						icon = IconService.GetBitmap("Icons.16x16.Warning");
						loadError = e.Message;
					}
					if (icon != null) {
						componentListView.SmallImageList.Images.Add(icon);
					}
					
					ListViewItem newItem = new ListViewItem(component.Name);
					newItem.SubItems.Add(component.Namespace);
					newItem.SubItems.Add(loadError != null ? loadError :component.AssemblyName);
					
					newItem.Checked = component.IsEnabled;
					newItem.Tag     = component;
					newItem.ImageIndex = componentListView.SmallImageList.Images.Count - 1;
					componentListView.Items.Add(newItem);
				}
			}
			componentListView.EndUpdate();
			componentListView.ItemCheck += this.ComponentListViewItemCheck;
		}
		
		void CategoryListViewSelectedIndexChanged(object sender, System.EventArgs e)
		{
			removeCategoryButton.Enabled = renameCategoryButton.Enabled = addComponentsButton.Enabled = CurrentCategory != null;
			FillComponents();
		}
		
		Category CurrentCategory {
			get {
				if (categoryListView.SelectedItems != null && categoryListView.SelectedItems.Count == 1) {
					return (Category)categoryListView.SelectedItems[0].Tag;
				}
				return null;
			}
		}
		
		void AddComponentsButtonClick(object sender, System.EventArgs e)
		{
			AddComponentsDialog addComponentsDialog = new AddComponentsDialog();
			if (addComponentsDialog.ShowDialog(this) == DialogResult.OK) {
				foreach (ToolComponent component in addComponentsDialog.SelectedComponents) {
					CurrentCategory.ToolComponents.Add(component);
				}
				FillComponents();
				CategoryListViewSelectedIndexChanged(this, EventArgs.Empty);
				ComponentListViewSelectedIndexChanged(this, EventArgs.Empty);
			}
		}
		
		void NewCategoryButtonClick(object sender, System.EventArgs e)
		{
			RenameCategoryDialog renameCategoryDialog = new RenameCategoryDialog(toolbox, null, this);
			if (renameCategoryDialog.ShowDialog(this) == DialogResult.OK) {
				toolbox.ComponentLibraryLoader.Categories.Add(new Category(renameCategoryDialog.CategoryName));
				FillCategories();
			}
		}
		
		void RemoveCategoryButtonClick(object sender, System.EventArgs e)
		{
			if (MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSideBarDialog.RemoveCategoryQuestion}")) {
				CategoryListViewSelectedIndexChanged(this, EventArgs.Empty);
				toolbox.ComponentLibraryLoader.Categories.Remove(CurrentCategory);
				FillCategories();
				FillComponents();
				CategoryListViewSelectedIndexChanged(this, EventArgs.Empty);
				ComponentListViewSelectedIndexChanged(this, EventArgs.Empty);
			}
		}
		
		void ComponentListViewSelectedIndexChanged(object sender, System.EventArgs e)
		{
			removeComponentsButton.Enabled = componentListView.SelectedItems != null && componentListView.SelectedItems.Count > 0;
		}
		
		void RemoveComponentsButtonClick(object sender, System.EventArgs e)
		{
			
			if (MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSideBarDialog.RemoveComponentsQuestion}")) {
				foreach (ListViewItem item in componentListView.SelectedItems) {
					CurrentCategory.ToolComponents.Remove((ToolComponent)item.Tag);
				}
				FillComponents();
				ComponentListViewSelectedIndexChanged(this, EventArgs.Empty);
			}
		}
		
		protected override void OnClosed(System.EventArgs e)
		{
			base.OnClosed(e);
			if (oldComponents != null) {
				toolbox.ComponentLibraryLoader.Categories = oldComponents;
			}
		}
		
		void RenameCategoryButtonClick(object sender, System.EventArgs e)
		{
			RenameCategoryDialog renameCategoryDialog = new RenameCategoryDialog(toolbox, this.CurrentCategory.Name, this);
			if (renameCategoryDialog.ShowDialog(this) == DialogResult.OK) {
				this.CurrentCategory.Name = renameCategoryDialog.CategoryName;
				FillCategories();
				CategoryListViewSelectedIndexChanged(sender, e);
			}
		}
		
		void OkButtonClick(object sender, System.EventArgs e)
		{
			oldComponents = null;
			
			foreach (ListViewItem item in categoryListView.Items) {
				Category category = (Category)item.Tag;
				category.IsEnabled = item.Checked;
			}
					
			toolbox.SaveToolbox();
		}
		
		void ComponentListViewItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			ToolComponent tc = (ToolComponent)componentListView.Items[e.Index].Tag;
			tc.IsEnabled = !tc.IsEnabled;
		}
	}
}
