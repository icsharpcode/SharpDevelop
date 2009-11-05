// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public class XmlEditorService
	{
		static XmlEditorOptions options;
		static XmlSchemaManager schemaManager;
		
		XmlEditorService()
		{
		}
		
		public static XmlEditorOptions XmlEditorOptions {
			get {
				if (options == null) {
					CreateXmlEditorOptions();
				}
				return options;
			}
		}
		
		static void CreateXmlEditorOptions()
		{
			Properties properties = PropertyService.Get(XmlEditorOptions.OptionsProperty, new Properties());
			options = new XmlEditorOptions(properties, new DefaultXmlSchemaFileAssociations(), XmlSchemaManager.Schemas);
		}
		
		public static bool ShowAttributesWhenFolded {
			get { return XmlEditorOptions.ShowAttributesWhenFolded; }
			set { XmlEditorOptions.ShowAttributesWhenFolded = value; }
		}
		
		public static bool ShowSchemaAnnotation {
			get { return XmlEditorOptions.ShowSchemaAnnotation; }
			set { XmlEditorOptions.ShowSchemaAnnotation = value; }
		}
		
		public static XmlSchemaManager XmlSchemaManager {
			get {
				if (schemaManager == null) {
					CreateXmlSchemaManager();
					schemaManager.ReadSchemas();
				}
				return schemaManager;
			}
		}
		
		static void CreateXmlSchemaManager()
		{
			string[] readOnlySchemaFolders = GetReadOnlySchemaFolders();
			string userDefinedSchemaFolder = GetUserDefinedSchemaFolder();
			FileSystem fileSystem = new FileSystem();
				
			schemaManager = new XmlSchemaManager(readOnlySchemaFolders, userDefinedSchemaFolder, fileSystem);
		}
		
		static string[] GetReadOnlySchemaFolders()
		{
			List<string> folders = new List<string>();
			folders.Add(RuntimeEnvironment.GetRuntimeDirectory());
			folders.Add(GetSharpDevelopSchemaFolder());
			return folders.ToArray();
		}
		
		static string GetUserDefinedSchemaFolder()
		{
			return Path.Combine(PropertyService.ConfigDirectory, "schemas");
		}
		
		static string GetSharpDevelopSchemaFolder()
		{
			return Path.Combine(PropertyService.DataDirectory, "schemas");
		}
	}
}
