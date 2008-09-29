// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using NoGoop.ObjBrowser;

namespace ICSharpCode.ComponentInspector.AddIn
{
	public class ObjectTreeOptionsPanel : XmlFormsOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.ComponentInspector.AddIn.Resources.ObjectTreeOptionsPanel.xfrm"));
			
			ShowPropertyAccessorsMethodsCheckBox.Checked = ComponentInspectorProperties.ShowPropertyAccessorMethods;
			ShowFieldsCheckBox.Checked = ComponentInspectorProperties.ShowFields;
			ShowPropertiesCheckBox.Checked = ComponentInspectorProperties.ShowProperties;
			ShowMethodsCheckBox.Checked = ComponentInspectorProperties.ShowMethods;
			ShowEventsCheckBox.Checked = ComponentInspectorProperties.ShowEvents;

			ShowBaseClassMembersCheckBox.Checked = ComponentInspectorProperties.ShowBaseClasses;
			ShowPublicMembersOnlyCheckBox.Checked = ComponentInspectorProperties.ShowPublicMembersOnly;

			ShowMemberCategoriesCheckBox.Checked = ComponentInspectorProperties.ShowMemberCategories;
			ShowObjectAsBaseClassCheckBox.Checked = ComponentInspectorProperties.ShowObjectAsBaseClass;
			ShowBaseClassCategoryCheckBox.Checked = ComponentInspectorProperties.ShowBaseCategories;
			CategoryCountTextBox.Text = Convert.ToString(ComponentInspectorProperties.CategoryThreshold);
			
			EnableCategoryCount();

			ShowBaseClassNamesCheckBox.Checked = ComponentInspectorProperties.ShowBaseClassNames;
			DisplayIntegersInHexCheckBox.Checked = ComponentInspectorProperties.DisplayHex;
			
			ShowMemberCategoriesCheckBox.Click += ShowCategoryClicked;
			ShowBaseClassCategoryCheckBox.Click += ShowCategoryClicked;
		}

		public override bool StorePanelContents()
		{
			int categoriesCount = Convert.ToInt32(CategoryCountTextBox.Text);

			ComponentInspectorProperties.ShowPropertyAccessorMethods =	ShowPropertyAccessorsMethodsCheckBox.Checked;
			ComponentInspectorProperties.ShowFields = ShowFieldsCheckBox.Checked;
			ComponentInspectorProperties.ShowProperties = ShowPropertiesCheckBox.Checked;
			ComponentInspectorProperties.ShowMethods = ShowMethodsCheckBox.Checked;
			ComponentInspectorProperties.ShowEvents = ShowEventsCheckBox.Checked;

			ComponentInspectorProperties.ShowBaseClasses = ShowBaseClassMembersCheckBox.Checked;
			ComponentInspectorProperties.ShowPublicMembersOnly = ShowPublicMembersOnlyCheckBox.Checked;

			ComponentInspectorProperties.ShowMemberCategories = ShowMemberCategoriesCheckBox.Checked;
			ComponentInspectorProperties.ShowBaseCategories = ShowBaseClassCategoryCheckBox.Checked;
			ComponentInspectorProperties.ShowObjectAsBaseClass = ShowObjectAsBaseClassCheckBox.Checked;
			ComponentInspectorProperties.CategoryThreshold = categoriesCount;

			ComponentInspectorProperties.ShowBaseClassNames = ShowBaseClassNamesCheckBox.Checked;
			ComponentInspectorProperties.DisplayHex = DisplayIntegersInHexCheckBox.Checked;

			return true;
		}
		
		CheckBox ShowObjectAsBaseClassCheckBox {
			get {
				return Get<CheckBox>("showObjectAsBaseClass");
			}
		}
		
		TextBox CategoryCountTextBox {
			get {
				return Get<TextBox>("categoryCount");
			}
		}		
		
		CheckBox ShowBaseClassCategoryCheckBox {
			get {
				return Get<CheckBox>("showBaseClassCategory");
			}
		}
		
		CheckBox ShowMemberCategoriesCheckBox {
			get {
				return Get<CheckBox>("showMemberCategories");
			}
		}
		
		CheckBox DisplayIntegersInHexCheckBox {
			get {
				return Get<CheckBox>("displayIntegersInHex");
			}
		}
		
		CheckBox ShowBaseClassNamesCheckBox {
			get {
				return Get<CheckBox>("showBaseClassNames");
			}
		}
		
		CheckBox ShowPropertyAccessorsMethodsCheckBox {
			get {
				return Get<CheckBox>("showPropertyAccessorsMethods");
			}
		}
		
		CheckBox ShowPublicMembersOnlyCheckBox {
			get {
				return Get<CheckBox>("showPublicMembersOnly");
			}
		}
		
		CheckBox ShowBaseClassMembersCheckBox {
			get {
				return Get<CheckBox>("showBaseClassMembers");
			}
		}
		
		CheckBox ShowEventsCheckBox {
			get {
				return Get<CheckBox>("showEvents");
			}
		}
		
		CheckBox ShowMethodsCheckBox {
			get {
				return Get<CheckBox>("showMethods");
			}
		}
		
		CheckBox ShowPropertiesCheckBox {
			get {
				return Get<CheckBox>("showProperties");
			}
		}
		
		CheckBox ShowFieldsCheckBox {
			get {
				return Get<CheckBox>("showFields");
			}
		}
		
		void ShowCategoryClicked(object source, EventArgs e)
		{
			EnableCategoryCount();
		}
		
		/// <summary>
		/// Enables/disables the category count 
		/// </summary>
		void EnableCategoryCount()
		{
			CategoryCountTextBox.Enabled = ShowMemberCategoriesCheckBox.Checked || ShowBaseClassCategoryCheckBox.Checked;
		}
	}
}
