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
	/// <summary>
	/// Keeps track of all the schemas that the XML Editor is aware of.
	/// </summary>
	public class XmlSchemaManager : IXmlSchemaCompletionDataFactory
	{
		public const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";
		
		List<string> readOnlySchemaFolders = new List<string>();
		string userDefinedSchemaFolder;
		XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
		IFileSystem fileSystem;
		IXmlSchemaCompletionDataFactory factory;
		
		public event EventHandler UserSchemaAdded;
		public event EventHandler UserSchemaRemoved;
			
		public XmlSchemaManager(string[] readOnlySchemaFolders, 
			string userDefinedSchemaFolder, 
			IFileSystem fileSystem, 
			IXmlSchemaCompletionDataFactory factory)
		{
			this.readOnlySchemaFolders.AddRange(readOnlySchemaFolders);
			this.userDefinedSchemaFolder = userDefinedSchemaFolder;
			this.fileSystem = fileSystem;
			this.factory = factory;
		}
		
		public XmlSchemaManager(string[] readOnlySchemaFolders, string userDefinedSchemaFolder, IFileSystem fileSystem)
			: this(readOnlySchemaFolders, userDefinedSchemaFolder, fileSystem, null)
		{
			this.factory = this;
		}
				
		/// <summary>
		/// Determines whether the specified namespace is actually the W3C namespace for
		/// XSD files.
		/// </summary>
		public static bool IsXmlSchemaNamespace(string schemaNamespace)
		{
			return schemaNamespace == XmlSchemaNamespace;
		}
		
		public XmlSchemaCompletionDataCollection Schemas {
			get { return schemas; }
		}
		
		public bool SchemaExists(string namespaceUri)
		{
			return schemas[namespaceUri] != null;
		}
		
		public void RemoveUserSchema(string namespaceUri)
		{
			XmlSchemaCompletionData schema = schemas[namespaceUri];
			if (schema != null) {
				if (fileSystem.FileExists(schema.FileName)) {
					fileSystem.DeleteFile(schema.FileName);
				}
				schemas.Remove(schema);
			}
				OnUserSchemaRemoved();
		}
		
		void OnUserSchemaRemoved()
		{
			if (UserSchemaRemoved != null) {
				UserSchemaRemoved(this, new EventArgs());
			}
		}
				
		public void AddUserSchema(XmlSchemaCompletionData schema)
		{
			if (!fileSystem.DirectoryExists(userDefinedSchemaFolder)) {
				fileSystem.CreateDirectory(userDefinedSchemaFolder);
			}
			
			string newSchemaDestinationFileName = GetUserDefinedSchemaDestination(schema);
			fileSystem.CopyFile(schema.FileName, newSchemaDestinationFileName);			
			
			schema.FileName = newSchemaDestinationFileName;
			schemas.Add(schema);
			
			OnUserSchemaAdded();
		}		

		string GetUserDefinedSchemaDestination(XmlSchemaCompletionData schema)
		{
			string fileName = Path.GetFileName(schema.FileName);
			return Path.Combine(userDefinedSchemaFolder, fileName);
		}
				
		void OnUserSchemaAdded()
		{
			if (UserSchemaAdded != null) {
				UserSchemaAdded(this, new EventArgs());
			}
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
					ReadSchema(fileName, readOnly);
				}
			}
		}		
				
		/// <summary>
		/// Reads an individual schema and adds it to the collection.
		/// </summary>
		/// <remarks>
		/// If the schema namespace exists in the collection it is not added.
		/// </remarks>
//		static void ReadSchema(string fileName, bool readOnly)
//		{
//			try {
//				string baseUri = XmlSchemaCompletionData.GetUri(fileName);
//				XmlSchemaCompletionData data = new XmlSchemaCompletionData(baseUri, fileName);
//				if (data.NamespaceUri != null) {
//					if (schemas[data.NamespaceUri] == null) {
//						data.ReadOnly = readOnly;
//						schemas.Add(data);
//					} else {
//						// Namespace already exists.
//						LoggingService.Warn("Ignoring duplicate schema namespace " + data.NamespaceUri);
//					} 
//				} else {
//					// Namespace is null.
//					LoggingService.Warn("Ignoring schema with no namespace " + data.FileName);
//				}
//			} catch (Exception ex) {
//				LoggingService.Warn("Unable to read schema '" + fileName + "'. ", ex);
//			}
//		}
//		
		void ReadSchema(string fileName, bool readOnly)
		{
			string baseUri = XmlSchemaCompletionData.GetUri(fileName);
			XmlSchemaCompletionData schema = factory.CreateXmlSchemaCompletionData(baseUri, fileName);
			schema.FileName = fileName;
			schema.ReadOnly = readOnly;
			schemas.Add(schema);
		}
		
		XmlSchemaCompletionData IXmlSchemaCompletionDataFactory.CreateXmlSchemaCompletionData(string baseUri, string fileName)
		{
			return new XmlSchemaCompletionData(baseUri, fileName);
		}
	}
}
