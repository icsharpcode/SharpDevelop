// created on 08.08.2003 at 13:02
using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.FormDesigner.Gui
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
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.RenameSidebarCategoryDialog.xfrm"));
			
			this.Owner = owner;
			
				
			if (categoryName == null) {
				ControlDictionary["categoryNameTextBox"].Text = "New Category";
				Text = StringParser.Parse("${res:ICSharpCode.FormDesigner.Gui.RenameCategoryDialog.NewCategoryDialogName}");
			} else {
				this.categoryName = categoryName;
				ControlDictionary["categoryNameTextBox"].Text = categoryName;
				Text = StringParser.Parse("${res:ICSharpCode.FormDesigner.Gui.RenameCategoryDialog.RenameCategoryDialogName}");
			}
			ControlDictionary["okButton"].Click += new EventHandler(okButtonClick);
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
			xmlLoader.ObjectCreator        = new SharpDevelopObjectCreator();
		}
		
		void ShowDuplicateErrorMessage()
		{
			
			MessageService.ShowError("${res:ICSharpCode.FormDesigner.Gui.RenameCategoryDialog.DuplicateNameError}");
		}
		
		// THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
		// DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
		void okButtonClick(object sender, System.EventArgs e)
		{
			if (categoryName != ControlDictionary["categoryNameTextBox"].Text) {
				foreach (Category cat in ToolboxProvider.ComponentLibraryLoader.Categories) {
					if (cat.Name == ControlDictionary["categoryNameTextBox"].Text) {
						ShowDuplicateErrorMessage();
						return;
					}
				}
					
				foreach (AxSideTab tab in SharpDevelopSideBar.SideBar.Tabs) {
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
