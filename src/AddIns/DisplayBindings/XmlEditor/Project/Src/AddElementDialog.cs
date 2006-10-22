// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui.XmlForms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.XmlEditor
{
	public class AddElementDialog : BaseSharpDevelopForm
	{
		ListBox elementsListBox;
		Button okButton;
		TextBox elementTextBox;
		
		public AddElementDialog(string[] elementNames)
		{
			SetupFromXmlStream(GetType().Assembly.GetManifestResourceStream("ICSharpCode.XmlEditor.Resources.AddElementDialog.xfrm"));
			
			okButton = (Button)ControlDictionary["okButton"];
			okButton.Enabled = false;
			AcceptButton = okButton;
			CancelButton = (Button)ControlDictionary["cancelButton"];
			
			elementTextBox = (TextBox)ControlDictionary["elementTextBox"];
			elementTextBox.TextChanged += ElementTextBoxTextChanged;
			
			elementsListBox = (ListBox)ControlDictionary["elementsListBox"];
			elementsListBox.SelectedIndexChanged += ElementsListBoxSelectedIndexChanged;
			foreach (string name in elementNames) {
				elementsListBox.Items.Add(name);
			}
		}
		
		/// <summary>
		/// Gets the element names selected.
		/// </summary>
		public string[] ElementNames {
			get {
				List<string> elementNames = new List<string>();
				if (IsElementSelected) {
					foreach (string elementName in elementsListBox.SelectedItems) {
						elementNames.Add(elementName);
					}
				}
				string customElementName = elementTextBox.Text.Trim();
				if (customElementName.Length > 0) {
					elementNames.Add(customElementName);
				}
				return elementNames.ToArray();
			}
		}
		
		void ElementsListBoxSelectedIndexChanged(object source, EventArgs e)
		{
			okButton.Enabled = IsOkButtonEnabled;
		}
		
		void ElementTextBoxTextChanged(object source, EventArgs e)
		{
			okButton.Enabled = IsOkButtonEnabled;
		}
		
		bool IsElementSelected {
			get {
				return elementsListBox.SelectedIndex >= 0;
			}
		}
		
		bool IsOkButtonEnabled {
			get {
				return IsElementSelected || IsElementNameEntered;
			}
		}
		
		bool IsElementNameEntered {
			get {
				return elementTextBox.Text.Trim().Length > 0;
			}
		}
	}
}
