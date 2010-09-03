// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		static RegisteredXmlSchemas registeredXmlSchemas;
		static XmlSchemaFileAssociations schemaFileAssociations;
		static Properties xmlEditorProperties;
		
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
			CreateXmlEditorProperties();
			options = new XmlEditorOptions(xmlEditorProperties);
		}
		
		static void CreateXmlEditorProperties()
		{
			xmlEditorProperties = PropertyService.Get(XmlEditorOptions.OptionsProperty, new Properties());
		}
		
		public static bool ShowAttributesWhenFolded {
			get { return XmlEditorOptions.ShowAttributesWhenFolded; }
			set { XmlEditorOptions.ShowAttributesWhenFolded = value; }
		}
		
		public static bool ShowSchemaAnnotation {
			get { return XmlEditorOptions.ShowSchemaAnnotation; }
			set { XmlEditorOptions.ShowSchemaAnnotation = value; }
		}
		
		public static XmlSchemaFileAssociations XmlSchemaFileAssociations {
			get {
				if (schemaFileAssociations == null) {
					CreateXmlSchemaFileAssociations();
				}
				return schemaFileAssociations;
			}
		}
		
		static void CreateXmlSchemaFileAssociations()
		{
			CreateXmlEditorProperties();
			schemaFileAssociations = new XmlSchemaFileAssociations(xmlEditorProperties, new DefaultXmlSchemaFileAssociations(), RegisteredXmlSchemas.Schemas);
		}
		
		
		public static RegisteredXmlSchemas RegisteredXmlSchemas {
			get {
				if (registeredXmlSchemas == null) {
					CreateRegisteredXmlSchemas();
					registeredXmlSchemas.ReadSchemas();
					LogRegisteredSchemaErrorsAsWarnings();
				}
				return registeredXmlSchemas;
			}
		}
		
		static void CreateRegisteredXmlSchemas()
		{
			string[] readOnlySchemaFolders = GetReadOnlySchemaFolders();
			string userDefinedSchemaFolder = GetUserDefinedSchemaFolder();
			FileSystem fileSystem = new FileSystem();
				
			registeredXmlSchemas = new RegisteredXmlSchemas(readOnlySchemaFolders, userDefinedSchemaFolder, fileSystem);
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
		
		static void LogRegisteredSchemaErrorsAsWarnings()
		{
			foreach (RegisteredXmlSchemaError error in registeredXmlSchemas.GetSchemaErrors()) {
				if (error.HasException) {
					LoggingService.Warn(error.Message, error.Exception);
				} else {
					LoggingService.Warn(error.Message);
				}
			}
		}
	}
}
