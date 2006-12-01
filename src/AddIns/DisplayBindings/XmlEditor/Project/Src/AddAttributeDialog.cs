// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.XmlEditor
{
	public class AddAttributeDialog : BaseSharpDevelopForm, IAddAttributeDialog
	{
		ListBox attributesListBox;
		Button okButton;
		TextBox attributeTextBox;
		
		public AddAttributeDialog(string[] attributeNames)
		{
			SetupFromXmlStream(GetType().Assembly.GetManifestResourceStream("ICSharpCode.XmlEditor.Resources.AddAttributeDialog.xfrm"));
			
			okButton = (Button)ControlDictionary["okButton"];
			okButton.Enabled = false;
			AcceptButton = okButton;
			CancelButton = (Button)ControlDictionary["cancelButton"];
			
			attributeTextBox = (TextBox)ControlDictionary["attributeTextBox"];
			attributeTextBox.TextChanged += AttributeTextBoxTextChanged;
			
			attributesListBox = (ListBox)ControlDictionary["attributesListBox"];
			attributesListBox.SelectedIndexChanged += AttributesListBoxSelectedIndexChanged;
			foreach (string name in attributeNames) {
				attributesListBox.Items.Add(name);
			}
		}
		
		/// <summary>
		/// Gets the attribute names selected.
		/// </summary>
		public string[] AttributeNames {
			get {
				List<string> attributeNames = new List<string>();
				if (IsAttributeSelected) {
					foreach (string attributeName in attributesListBox.SelectedItems) {
						attributeNames.Add(attributeName);
					}
				}
				string customAttributeName = attributeTextBox.Text.Trim();
				if (customAttributeName.Length > 0) {
					attributeNames.Add(customAttributeName);
				}
				return attributeNames.ToArray();
			}
		}
		
		void AttributesListBoxSelectedIndexChanged(object source, EventArgs e)
		{
			okButton.Enabled = IsOkButtonEnabled;
		}
		
		void AttributeTextBoxTextChanged(object source, EventArgs e)
		{
			okButton.Enabled = IsOkButtonEnabled;
		}
		
		bool IsAttributeSelected {
			get {
				return attributesListBox.SelectedIndex >= 0;
			}
		}
		
		bool IsOkButtonEnabled {
			get {
				return IsAttributeSelected || IsAttributeNameEntered;
			}
		}
		
		bool IsAttributeNameEntered {
			get {
				return attributeTextBox.Text.Trim().Length > 0;
			}
		}
	}
}
