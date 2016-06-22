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

using ICSharpCode.XmlEditor;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace XmlEditor.Tests.Utils
{
	/// <summary>
	/// Helper class when testing a schema which includes 
	/// another schema.
	/// </summary>
	public class SchemaIncludeTestFixtureHelper
	{
		static string mainSchemaFileName = "main.xsd";
		static string includedSchemaFileName = "include.xsd";
		static readonly string schemaPath;

		SchemaIncludeTestFixtureHelper()
		{
		}
		
		static SchemaIncludeTestFixtureHelper()
		{
			schemaPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "XmlEditorTests");
		}
		
		/// <summary>
		/// Creates a schema with the given filename
		/// </summary>
		/// <param name="fileName">Filename of the schema that will be 
		/// generated.</param>
		/// <param name="xml">The schema xml</param>
		public static void CreateSchema(string fileName, string xml)
		{
			XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8);
			writer.WriteRaw(xml);
			writer.Close();
		}
		
		/// <summary>
		/// Creates two schemas, one which references the other via an
		/// xs:include.  Both schemas will exist in the same folder.
		/// </summary>
		/// <param name="mainSchema">The main schema's xml.</param>
		/// <param name="includedSchema">The included schema's xml.</param>
		public static XmlSchemaCompletion CreateSchemaCompletionDataObject(string mainSchema, string includedSchema)
		{	
			if (!Directory.Exists(schemaPath)) {
				Directory.CreateDirectory(schemaPath);
			}
			
			CreateSchema(Path.Combine(schemaPath, mainSchemaFileName), mainSchema);
			CreateSchema(Path.Combine(schemaPath, includedSchemaFileName), includedSchema);
			
			// Parse schema.
			string schemaFileName = Path.Combine(schemaPath, mainSchemaFileName);
			string baseUri = XmlSchemaCompletion.GetUri(schemaFileName);
			return new XmlSchemaCompletion(baseUri, schemaFileName);
		}
		
		/// <summary>
		/// Removes any files generated for the test fixture.
		/// </summary>
		public static void FixtureTearDown()
		{
			// Delete the created schemas.
			string fileName = Path.Combine(schemaPath, mainSchemaFileName);
			if (File.Exists(fileName)) {
				File.Delete(fileName);
			}
			
			fileName = Path.Combine(schemaPath, includedSchemaFileName);
			if (File.Exists(fileName)) {
				File.Delete(fileName);
			}
			
			if (Directory.Exists(schemaPath)) {
				Directory.Delete(schemaPath);
			}
		}
	}
}
