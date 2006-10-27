// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

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
		}
		
		void Initialize()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.XmlEditor.Resources.SelectXmlSchema.xfrm"));
			
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
