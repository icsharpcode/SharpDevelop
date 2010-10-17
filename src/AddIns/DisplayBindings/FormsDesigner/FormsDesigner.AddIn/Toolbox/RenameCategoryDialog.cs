// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

// created on 08.08.2003 at 13:02
using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.FormsDesigner.Gui
{
	public class RenameCategoryDialog : BaseSharpDevelopForm
	{
		string categoryName = String.Empty;
		
		public string CategoryName {
			get {
				return categoryName;
			}
		}
		
		public RenameCategoryDialog(string categoryName, Form owner)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.FormsDesigner.Resources.RenameSidebarCategoryDialog.xfrm"));
			
			this.Owner = owner;
			
			
			if (categoryName == null) {
				ControlDictionary["categoryNameTextBox"].Text = "New Category";
				Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.RenameCategoryDialog.NewCategoryDialogName}");
			} else {
				this.categoryName = categoryName;
				ControlDictionary["categoryNameTextBox"].Text = categoryName;
				Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.RenameCategoryDialog.RenameCategoryDialogName}");
			}
			ControlDictionary["okButton"].Click += new EventHandler(okButtonClick);
		}
		
		void ShowDuplicateErrorMessage()
		{
			
			MessageService.ShowError("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.RenameCategoryDialog.DuplicateNameError}");
		}
		
		void okButtonClick(object sender, System.EventArgs e)
		{
			if (categoryName != ControlDictionary["categoryNameTextBox"].Text) {
				foreach (Category cat in ToolboxProvider.ComponentLibraryLoader.Categories) {
					if (cat.Name == ControlDictionary["categoryNameTextBox"].Text) {
						ShowDuplicateErrorMessage();
						return;
					}
				}
				
				foreach (SideTab tab in ToolboxProvider.FormsDesignerSideBar.Tabs) {
					if (!(tab is SideTabDesigner) && !(tab is CustomComponentsSideTab)) {
						if (tab.Name == ControlDictionary["categoryNameTextBox"].Text) {
							ShowDuplicateErrorMessage();
							return;
						}
					}
				}
				
				categoryName = ControlDictionary["categoryNameTextBox"].Text;
			}
			DialogResult = System.Windows.Forms.DialogResult.OK;
		}
	}
}
