// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

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
			xmlEditorProperties = PropertyService.NestedProperties(XmlEditorOptions.OptionsProperty);
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
				
			registeredXmlSchemas = new RegisteredXmlSchemas(readOnlySchemaFolders, userDefinedSchemaFolder, SD.FileSystem);
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
