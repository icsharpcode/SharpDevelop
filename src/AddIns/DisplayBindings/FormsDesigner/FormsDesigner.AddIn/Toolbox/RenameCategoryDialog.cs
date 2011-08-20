// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.FormsDesigner.Gui
{
	public partial class RenameCategoryDialog : Form
	{
		string categoryName = String.Empty;
		
		public string CategoryName {
			get {
				return categoryName;
			}
		}
		
		ToolboxProvider toolbox;
		
		public RenameCategoryDialog(ToolboxProvider toolbox, string categoryName, Form owner)
		{
			this.toolbox = toolbox;
			InitializeComponent();
			this.Owner = owner;
			
			if (categoryName == null) {
				categoryNameTextBox.Text = "New Category";
				Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.RenameCategoryDialog.NewCategoryDialogName}");
			} else {
				this.categoryName = categoryName;
				categoryNameTextBox.Text = categoryName;
				Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.RenameCategoryDialog.RenameCategoryDialogName}");
			}
		}
		
		void ShowDuplicateErrorMessage()
		{
			MessageService.ShowError("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.RenameCategoryDialog.DuplicateNameError}");
		}
		
		void OkButtonClick(object sender, System.EventArgs e)
		{
			if (categoryName != categoryNameTextBox.Text) {
				foreach (Category cat in toolbox.ComponentLibraryLoader.Categories) {
					if (cat.Name == categoryNameTextBox.Text) {
						ShowDuplicateErrorMessage();
						return;
					}
				}
				
				foreach (SideTab tab in toolbox.FormsDesignerSideBar.Tabs) {
					if (!(tab is DesignerSideTab) && !(tab is CustomComponentsSideTab)) {
						if (tab.Name == categoryNameTextBox.Text) {
							ShowDuplicateErrorMessage();
							return;
						}
					}
				}
				
				categoryName = categoryNameTextBox.Text;
			}
			DialogResult = System.Windows.Forms.DialogResult.OK;
		}
	}
}
