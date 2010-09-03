// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Keeps track of all the schemas that the XML Editor is aware of.
	/// </summary>
	public class RegisteredXmlSchemas : IXmlSchemaCompletionDataFactory
	{
		List<string> readOnlySchemaFolders = new List<string>();
		string userDefinedSchemaFolder;
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
			this.readOnlySchemaFolders.AddRange(readOnlySchemaFolders);
			this.userDefinedSchemaFolder = userDefinedSchemaFolder;
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
					fileSystem.DeleteFile(schema.FileName);
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
			if (!fileSystem.DirectoryExists(userDefinedSchemaFolder)) {
				fileSystem.CreateDirectory(userDefinedSchemaFolder);
			}
			
			string newSchemaDestinationFileName = GetUserDefinedSchemaDestination(schema);
			fileSystem.CopyFile(schema.FileName, newSchemaDestinationFileName);			
			
			schema.FileName = newSchemaDestinationFileName;
			schemas.Add(schema);
			
			OnUserDefinedSchemaAdded();
		}		

		string GetUserDefinedSchemaDestination(XmlSchemaCompletion schema)
		{
			string fileName = Path.GetFileName(schema.FileName);
			return Path.Combine(userDefinedSchemaFolder, fileName);
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
			foreach (string folder in readOnlySchemaFolders) {
				ReadSchemas(folder, "*.xsd", true);
			}
			ReadSchemas(userDefinedSchemaFolder, "*.*", false);
		}
		
		void ReadSchemas(string directory, string searchPattern, bool readOnly)
		{
			if (fileSystem.DirectoryExists(directory)) {
				foreach (string fileName in fileSystem.GetFilesInDirectory(directory, searchPattern)) {
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
				schema.FileName = fileName;
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
