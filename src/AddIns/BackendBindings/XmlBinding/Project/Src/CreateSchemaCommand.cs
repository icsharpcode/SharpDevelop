// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: -1 $</version>
// </file>

using System;
using System.Globalization;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Creates a schema based on the xml in the currently active view.
	/// </summary>
	public class CreateSchemaCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			// Find active XmlView.
			XmlView properties = XmlView.ForViewContent(WorkbenchSingleton.Workbench.ActiveViewContent);
			if (properties != null) {
				// Create a schema based on the xml.
				string[] schemas = properties.InferSchema();
				if (schemas != null) {
					// Create a new file for each generated schema.
					for (int i = 0; i < schemas.Length; ++i) {
						string fileName = GenerateSchemaFileName(properties.File.FileName, i + 1);
						OpenNewXmlFile(fileName, schemas[i]);
					}
				}
			}
		}
		
		/// <summary>
		/// Opens a new unsaved xml file in SharpDevelop.
		/// </summary>
		static void OpenNewXmlFile(string fileName, string xml)
		{
			FileService.NewFile(fileName, xml);
		}
		
		/// <summary>
		/// Generates an xsd filename based on the name of the original xml file.
		/// </summary>
		static string GenerateSchemaFileName(string xmlFileName, int count)
		{
			string baseFileName = Path.GetFileNameWithoutExtension(xmlFileName);
			string schemaFileName = string.Concat(baseFileName, ".xsd");
			if (count == 1) {
				return schemaFileName;
			}
			return schemaFileName = string.Concat(baseFileName, count.ToString(CultureInfo.InvariantCulture), ".xsd");
		}
	}
}
