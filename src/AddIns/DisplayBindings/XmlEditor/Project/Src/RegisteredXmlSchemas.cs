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
using System.Linq;
using System.Runtime.InteropServices;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Keeps track of all the schemas that the XML Editor is aware of.
	/// </summary>
	public class RegisteredXmlSchemas : IXmlSchemaCompletionDataFactory
	{
		List<DirectoryName> readOnlySchemaFolders = new List<DirectoryName>();
		DirectoryName userDefinedSchemaFolder;
		XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
		IFileSystem fileSystem;
		IXmlSchemaCompletionDataFactory factory;
		List<RegisteredXmlSchemaError> schemaErrors = new List<RegisteredXmlSchemaError>();
		
		public event EventHandler UserDefinedSchemaAdded;
		public event EventHandler UserDefinedSchemaRemoved;
			
		public RegisteredXmlSchemas(string[] readOnlySchemaFolders, 
			string userDefinedSchemaFolder, 
			IFileSystem fileSystem, 
			IXmlSchemaCompletionDataFactory factory)
		{
			this.readOnlySchemaFolders.AddRange(readOnlySchemaFolders.Select(DirectoryName.Create));
			this.userDefinedSchemaFolder = DirectoryName.Create(userDefinedSchemaFolder);
			this.fileSystem = fileSystem;
			this.factory = factory;
		}
		
		public RegisteredXmlSchemas(string[] readOnlySchemaFolders, string userDefinedSchemaFolder, IFileSystem fileSystem)
			: this(readOnlySchemaFolders, userDefinedSchemaFolder, fileSystem, null)
		{
			this.factory = this;
		}
		
		public XmlSchemaCompletionCollection Schemas {
			get { return schemas; }
		}
		
		public bool SchemaExists(string namespaceUri)
		{
			return schemas.Contains(namespaceUri);
		}
		
		public void RemoveUserDefinedSchema(string namespaceUri)
		{
			XmlSchemaCompletion schema = schemas[namespaceUri];
			if (schema != null) {
				if (fileSystem.FileExists(schema.FileName)) {
					fileSystem.Delete(schema.FileName);
				}
				schemas.Remove(schema);
				OnUserDefinedSchemaRemoved();
			}
		}
		
		void OnUserDefinedSchemaRemoved()
		{
			if (UserDefinedSchemaRemoved != null) {
				UserDefinedSchemaRemoved(this, new EventArgs());
			}
		}
				
		public void AddUserSchema(XmlSchemaCompletion schema)
		{
			fileSystem.CreateDirectory(userDefinedSchemaFolder);
			
			FileName newSchemaDestinationFileName = GetUserDefinedSchemaDestination(schema);
			fileSystem.CopyFile(schema.FileName, newSchemaDestinationFileName);			
			
			schema.FileName = newSchemaDestinationFileName;
			schemas.Add(schema);
			
			OnUserDefinedSchemaAdded();
		}		

		FileName GetUserDefinedSchemaDestination(XmlSchemaCompletion schema)
		{
			string fileName = Path.GetFileName(schema.FileName);
			return FileName.Create(Path.Combine(userDefinedSchemaFolder, fileName ?? string.Empty));
		}
				
		void OnUserDefinedSchemaAdded()
		{
			if (UserDefinedSchemaAdded != null) {
				UserDefinedSchemaAdded(this, new EventArgs());
			}
		}
		
		public RegisteredXmlSchemaError[] GetSchemaErrors()
		{
			return schemaErrors.ToArray();
		}
		
		public void ReadSchemas()
		{
			foreach (DirectoryName folder in readOnlySchemaFolders) {
				ReadSchemas(folder, "*.xsd", true);
			}
			ReadSchemas(userDefinedSchemaFolder, "*.*", false);
		}
		
		void ReadSchemas(DirectoryName directory, string searchPattern, bool readOnly)
		{
			if (fileSystem.DirectoryExists(directory)) {
				foreach (string fileName in fileSystem.GetFiles(directory, searchPattern)) {
					XmlSchemaCompletion schema = ReadSchema(fileName, readOnly);
					if (schema != null) {
						AddSchema(schema);
					}
				}
			}
		}		
		
		XmlSchemaCompletion ReadSchema(string fileName, bool readOnly)
		{
			try {
				string baseUri = XmlSchemaCompletion.GetUri(fileName);
				XmlSchemaCompletion schema = factory.CreateXmlSchemaCompletionData(baseUri, fileName);
				schema.FileName = FileName.Create(fileName);
				schema.IsReadOnly = readOnly;
				return schema;
			} catch (Exception ex) {
				schemaErrors.Add(new RegisteredXmlSchemaError("Unable to read schema '" + fileName + "'.", ex));
			}
			return null;
		}
		
		XmlSchemaCompletion IXmlSchemaCompletionDataFactory.CreateXmlSchemaCompletionData(string baseUri, string fileName)
		{
			return new XmlSchemaCompletion(baseUri, fileName);
		}
		
		void AddSchema(XmlSchemaCompletion schema)
		{
			if (schema.HasNamespaceUri) {
				schemas.Add(schema);
			} else {
				schemaErrors.Add(new RegisteredXmlSchemaError("Ignoring schema with no namespace '" + schema.FileName + "'."));
			}		
		}
	}
}
