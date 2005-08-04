//
// SharpDevelop Xml Editor
//
// Copyright (C) 2005 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using System;
using System.Windows.Forms;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Allows the use to choose a schema from the schemas that SharpDevelop 
	/// knows about.
	/// </summary>
	public class SelectXmlSchemaForm : XmlForm
	{
		ListBox schemaListBox;
		string NoSchemaSelectedText = String.Empty;
		
		public SelectXmlSchemaForm(string[] namespaces)
		{
			Initialize();
			PopulateListBox(namespaces);
		}
		
		/// <summary>
		/// Gets or sets the selected schema namesace.
		/// </summary>
		public string SelectedNamespaceUri {
			get {
				string namespaceUri = schemaListBox.Text;
				if (namespaceUri == NoSchemaSelectedText) {
					namespaceUri = String.Empty;
				}
				return namespaceUri;
			}
			
			set {
				int index = 0;
				if (value.Length > 0) {
					index = schemaListBox.Items.IndexOf(value);
				} 
				
				// Select the option representing "no schema" if 
				// the value does not exist in the list box.
				if (index == -1) {
					index = 0;
				}
				
				schemaListBox.SelectedIndex = index;
			}
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
			xmlLoader.ObjectCreator        = new SharpDevelopObjectCreator();
		}
		
		void Initialize()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.XmlEditor.SelectXmlSchema.xfrm"));
			
			NoSchemaSelectedText = StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.None}");
			
			schemaListBox = (ListBox)ControlDictionary["schemaListBox"];
			
			AcceptButton = (Button)ControlDictionary["okButton"];
			CancelButton = (Button)ControlDictionary["cancelButton"];
			AcceptButton.DialogResult = DialogResult.OK;
		}
		
		void PopulateListBox(string[] namespaces)
		{
			foreach (string schemaNamespace in namespaces) {
				schemaListBox.Items.Add(schemaNamespace);
			}
			
			// Add the "None" string at the top of the list.
			schemaListBox.Sorted = true;
			schemaListBox.Sorted = false;
			
			schemaListBox.Items.Insert(0, NoSchemaSelectedText);
		}
	}
}
