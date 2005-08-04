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
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Creates a schema based on the xml in the currently active view.
	/// </summary>
	public class CreateSchemaCommand : AbstractMenuCommand
	{
		public CreateSchemaCommand()
		{
		}
		
		public override void Run()
		{
			// Find active XmlView.
			XmlView xmlView = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as XmlView;
			if (xmlView != null) {
				
				// Create a schema based on the xml.
				SharpDevelopTextEditorProperties properties = xmlView.TextEditorControl.TextEditorProperties as SharpDevelopTextEditorProperties;
				string schema = CreateSchema(xmlView.TextEditorControl.Document.TextContent, xmlView.TextEditorControl.Encoding, properties.ConvertTabsToSpaces, properties.TabIndent);
				
				// Create a new file and display the generated schema.
				string fileName = GenerateSchemaFileName(xmlView.TitleName);
				OpenNewXmlFile(fileName, schema);
				
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
					WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.SelectWindow();
				}
			}
		}
		
		/// <summary>
		/// Creates a schema based on the xml content.
		/// </summary>
		/// <returns>A generated schema.</returns>
		string CreateSchema(string xml, Encoding encoding, bool convertTabsToSpaces, int tabIndent)
		{
			string schema = String.Empty;
			
			using (DataSet dataSet = new DataSet()) {
				dataSet.ReadXml(new StringReader(xml), XmlReadMode.InferSchema);
				EncodedStringWriter writer = new EncodedStringWriter(encoding);
				XmlTextWriter xmlWriter = new XmlTextWriter(writer);
				
				xmlWriter.Formatting = Formatting.Indented;
				if (convertTabsToSpaces) {
					xmlWriter.Indentation = tabIndent;
					xmlWriter.IndentChar = ' ';
				} else {
					xmlWriter.Indentation = 1;
					xmlWriter.IndentChar = '\t';
				}
				
				dataSet.WriteXmlSchema(xmlWriter);
				schema = writer.ToString();
				writer.Close();
				xmlWriter.Close();
			}
			
			return schema;
		}
		
		/// <summary>
		/// Opens a new unsaved xml file in SharpDevelop.
		/// </summary>
		void OpenNewXmlFile(string fileName, string xml)
		{
			FileService.NewFile(fileName, XmlView.Language, xml);
		}
		
		/// <summary>
		/// Generates an xsd filename based on the name of the original xml
		/// file.  If a file with the same name is already open in SharpDevelop
		/// then a new name is generated (e.g. MyXml1.xsd).
		/// </summary>
		string GenerateSchemaFileName(string xmlFileName)
		{
			string baseFileName = Path.GetFileNameWithoutExtension(xmlFileName);
			string schemaFileName = String.Concat(baseFileName, ".xsd");
			
			int count = 1;
			while (FileService.IsOpen(schemaFileName)) {
				schemaFileName = String.Concat(baseFileName, count.ToString(), ".xsd");
				++count;
			}
			
			return schemaFileName;
		}
	}
}
