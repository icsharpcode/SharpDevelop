// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.FormsDesigner.Gui
{
	#pragma warning disable 618
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
