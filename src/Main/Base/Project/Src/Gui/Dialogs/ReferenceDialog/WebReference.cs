// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Xml.Schema;
using System.Xml.Serialization;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class WebReference
	{
		List<ProjectItem> items;
		string url = String.Empty;
		string relativePath = String.Empty;
		DiscoveryClientProtocol protocol;
		IProject project;
		string webReferencesDirectory = String.Empty;
		string proxyNamespace = String.Empty;
		string name = String.Empty;
		WebReferenceUrl webReferenceUrl;
		
		public WebReference(IProject project, string url, string name, string proxyNamespace, DiscoveryClientProtocol protocol)
		{
			this.project = project;
			this.url = url;
			this.protocol = protocol;
			this.proxyNamespace = proxyNamespace;
			this.name = name;
			GetRelativePath();
		}
		
		/// <summary>
		/// Checks that the string is valid to use for a web reference namespace.
		/// </summary>
		public static bool IsValidNamespace(string ns)
		{
			if (ns.Length > 0) {
				try {
					CodeNamespace codeNamespace = new CodeNamespace(ns);
					CodeGenerator.ValidateIdentifiers(codeNamespace);
					return true;
				} catch (ArgumentException) { }
			}
			return false;			
		}	
		
		/// <summary>
		/// Checks that the string is valid to use for a web reference name.
		/// </summary>
		public static bool IsValidReferenceName(string name)
		{
			if (name.Length > 0) {
				if (name.IndexOf('\\') == -1) {
					if (!ContainsInvalidDirectoryChar(name)) {
						return true;
					}
				}
			}
			return false;			
		}
		
		public static bool ProjectContainsWebReferencesFolder(IProject project)
		{
			return GetWebReferencesProjectItem(project) != null;
		}
		
		/// <summary>
		/// Checks that the project has the System.Web.Services assembly referenced.
		/// </summary>
		/// <param name="project"></param>
		/// <returns></returns>
		public static bool ProjectContainsWebServicesReference(IProject project)
		{
			foreach (ProjectItem item in project.Items) {
				if (item.ItemType == ItemType.Reference && item.Include != null) {
					if (item.Include.Trim().StartsWith("System.Web.Services", StringComparison.OrdinalIgnoreCase)) {
						return true;
					}
				}
			}
			return false;
		}
		
		public static WebReferencesProjectItem GetWebReferencesProjectItem(IProject project)
		{
			return GetWebReferencesProjectItem(project.Items);
		}
		
		/// <summary>
		/// Returns the reference name.  If the folder that will contain the
		/// web reference already exists this method looks for a new folder by
		/// adding a digit to the end of the reference name.
		/// </summary>
		public static string GetReferenceName(string webReferenceFolder, string name)
		{
			// If it is already in the project, or it does exists we get a new name.
			int count = 1;
			string referenceName = name;
			string folder = Path.Combine(webReferenceFolder, name);
			while (System.IO.Directory.Exists(folder)) {
				referenceName = String.Concat(name, count.ToString());
				folder = Path.Combine(webReferenceFolder, referenceName);
				++count;
			}
			return referenceName;
		}
		
		/// <summary>
		/// Gets all the file items that belong to the named web reference in
		/// the specified project.
		/// </summary>
		/// <param name="project">The specified project.</param>
		/// <param name="name">The name of the web reference to look for.  This is
		/// not the full path of the web reference, just the last folder's name.</param>
		/// <remarks>
		/// This method does not return the WebReferenceUrl project item only the
		/// files that are part of the web reference.
		/// </remarks>
		public static List<ProjectItem> GetFileItems(IProject project, string name)
		{
			List<ProjectItem> items = new List<ProjectItem>();
			
			// Find web references folder.
			WebReferencesProjectItem webReferencesProjectItem = GetWebReferencesProjectItem(project);
			if (webReferencesProjectItem != null) {
				
				// Look for files that are in the web reference folder.
				string webReferenceDirectory = Path.Combine(Path.Combine(project.Directory, webReferencesProjectItem.Include), name);
				foreach (ProjectItem item in project.Items) {
					FileProjectItem fileItem = item as FileProjectItem;
					if (fileItem != null) {
						if (FileUtility.IsBaseDirectory(webReferenceDirectory, fileItem.FileName)) {
							items.Add(fileItem);
						}
					}
				}
			}

			return items;
		}
		
		public WebReferencesProjectItem WebReferencesProjectItem {
			get { return GetWebReferencesProjectItem(Items); }
		}
		
		public WebReferenceUrl WebReferenceUrl {
			get {
				if (webReferenceUrl == null) {
					items = CreateProjectItems();
				}
				return webReferenceUrl;
			}
		}
		
		/// <summary>
		/// Gets the web references directory which is the parent folder for
		/// this web reference.
		/// </summary>
		public string WebReferencesDirectory {
			get { return webReferencesDirectory; }
		}
		
		/// <summary>
		/// Gets the directory where the web reference files will be saved.
		/// </summary>
		public string Directory {
			get { return Path.Combine(project.Directory, relativePath); }
		}
		
		/// <summary>
		/// Gets or sets the name of the web reference.
		/// </summary>
		/// <remarks>
		/// Changing the name will also change the directory where the
		/// web reference files are saved.
		/// </remarks>
		public string Name {
			get { return name; }
			set {
				name = value;
				OnNameChanged();
			}
		}
		
		public string ProxyNamespace {
			get { return proxyNamespace; }
			set { proxyNamespace = value; }
		}
		
		public List<ProjectItem> Items {
			get {
				if (items == null) {
					items = CreateProjectItems();
				}
				return items;
			}
		}
		
		public string WebProxyFileName {
			get { return GetFullProxyFileName(); }
		}
		
		/// <summary>
		/// Gets the changes that this web reference has undergone after being
		/// refreshed.
		/// </summary>
		public WebReferenceChanges GetChanges(IProject project)
		{
			WebReferenceChanges changes = new WebReferenceChanges();
			
			List<ProjectItem> existingItems = GetFileItems(project, name);
			
			// Check for new items.
			changes.NewItems.AddRange(GetNewItems(existingItems));
			
			// Check for removed items.
			changes.ItemsRemoved.AddRange(GetRemovedItems(existingItems));
			
			return changes;
		}
		
		public void Save()
		{
			System.IO.Directory.CreateDirectory(Directory);
			GenerateWebProxy();
			protocol.WriteAll(Directory, "Reference.map");
		}
		
		ServiceDescriptionCollection GetServiceDescriptionCollection(DiscoveryClientProtocol protocol)
		{
			ServiceDescriptionCollection services = new ServiceDescriptionCollection();
			foreach (DictionaryEntry entry in protocol.References) {
				ContractReference contractRef = entry.Value as ContractReference;
				DiscoveryDocumentReference discoveryRef = entry.Value as DiscoveryDocumentReference;
				if (contractRef != null) {
					services.Add(contractRef.Contract);
				}
			}
			return services;
		}
		
		XmlSchemas GetXmlSchemas(DiscoveryClientProtocol protocol)
		{
			XmlSchemas schemas = new XmlSchemas();
			foreach (DictionaryEntry entry in protocol.References) {
				SchemaReference schemaRef = entry.Value as SchemaReference;
				if (schemaRef != null) {
					schemas.Add(schemaRef.Schema);
				}
			}
			return schemas;
		}
		
		void GenerateWebProxy()
		{
			GenerateWebProxy(proxyNamespace, GetFullProxyFileName(), GetServiceDescriptionCollection(protocol), GetXmlSchemas(protocol));
		}
		
		static void GenerateWebProxy(string proxyNamespace, string fileName, ServiceDescriptionCollection serviceDescriptions, XmlSchemas schemas)
		{
			ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
			
			foreach (ServiceDescription description in serviceDescriptions) {
				importer.AddServiceDescription(description, null, null);
			}
			
			foreach (XmlSchema schema in schemas) {
				importer.Schemas.Add(schema);
			}
			
			CodeNamespace codeNamespace = new CodeNamespace(proxyNamespace);
			CodeCompileUnit codeUnit = new CodeCompileUnit();
			codeUnit.Namespaces.Add(codeNamespace);
			ServiceDescriptionImportWarnings warnings = importer.Import(codeNamespace, codeUnit);
			
			CodeDomProvider provider = null;
			
			IParser parser = ParserService.CreateParser(fileName);
			if (parser != null) {
				provider = parser.Language.CodeDomProvider;
			}
			
			if (provider != null) {
				StreamWriter sw = new StreamWriter(fileName);
				CodeGeneratorOptions options = new CodeGeneratorOptions();
				options.BracingStyle = "C";
				provider.GenerateCodeFromCompileUnit(codeUnit, sw, options);
				sw.Close();
			}
		}
		
		string GetFullProxyFileName()
		{
			return Path.Combine(project.Directory, GetProxyFileName());
		}
		
		string GetProxyFileName()
		{
			string fileName = String.Concat("Reference", GetProxyFileNameExtension(project.Language));
			return Path.Combine(relativePath, fileName);
		}
		
		string GetProxyFileNameExtension(string language)
		{
			ProjectBindingDescriptor binding = ProjectBindingService.GetCodonPerLanguageName(language);
			if (binding != null) {
				string[] extensions = binding.CodeFileExtensions;
				if (extensions.Length > 0) {
					return extensions[0];
				}
			}
			throw new NotSupportedException("Unsupported language: " + language);
		}
		
		static WebReferencesProjectItem GetWebReferencesProjectItem(IEnumerable<ProjectItem> items)
		{
			foreach (ProjectItem item in items) {
				if (item.ItemType == ItemType.WebReferences) {
					return (WebReferencesProjectItem)item;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Updates the various relative paths due to the change in the web
		/// reference name.
		/// </summary>
		void OnNameChanged()
		{
			GetRelativePath();
			
			if (items != null) {
				items = CreateProjectItems();
			}
		}
		
		/// <summary>
		/// Gets the web references relative path.
		/// </summary>
		void GetRelativePath()
		{
			ProjectItem webReferencesProjectItem = GetWebReferencesProjectItem(project);
			string webReferencesDirectoryName;
			if (webReferencesProjectItem != null) {
				webReferencesDirectoryName = webReferencesProjectItem.Include.Trim('\\', '/');
			} else {
				webReferencesDirectoryName = "Web References";
			}
			webReferencesDirectory = Path.Combine(project.Directory, webReferencesDirectoryName);
			relativePath = Path.Combine(webReferencesDirectoryName, name);
		}
		
		List<ProjectItem> CreateProjectItems()
		{
			List<ProjectItem> items = new List<ProjectItem>();
			
			// Web references item.
			if (!ProjectContainsWebReferencesFolder(project)) {
				WebReferencesProjectItem webReferencesItem = new WebReferencesProjectItem(project);
				webReferencesItem.Include = "Web References\\";
				items.Add(webReferencesItem);
			}
			
			// Web reference url.
			webReferenceUrl = new WebReferenceUrl(project);
			webReferenceUrl.Include = url;
			webReferenceUrl.UpdateFromURL = url;
			webReferenceUrl.RelPath = relativePath;
			webReferenceUrl.Namespace = proxyNamespace;
			items.Add(webReferenceUrl);
			
			// References.
			foreach (DictionaryEntry entry in protocol.References) {
				DiscoveryReference discoveryRef = entry.Value as DiscoveryReference;
				if (discoveryRef != null) {
					FileProjectItem item = new FileProjectItem(project, ItemType.None);
					item.Include = Path.Combine(relativePath, discoveryRef.DefaultFilename);
					items.Add(item);
				}
			}
			
			// Proxy
			FileProjectItem proxyItem = new FileProjectItem(project, ItemType.Compile);
			proxyItem.Include = GetProxyFileName();
			proxyItem.SetEvaluatedMetadata("AutoGen", "True");
			proxyItem.SetEvaluatedMetadata("DesignTime", "True");
			proxyItem.DependentUpon = "Reference.map";
			items.Add(proxyItem);
			
			// Reference map.
			FileProjectItem mapItem = new FileProjectItem(project, ItemType.None);
			mapItem.Include = Path.Combine(relativePath, "Reference.map");
			mapItem.SetEvaluatedMetadata("Generator", "MSDiscoCodeGenerator");
			mapItem.SetEvaluatedMetadata("LastGenOutput", "Reference.cs");
			items.Add(mapItem);
			
			// System.Web.Services reference.
			if (!ProjectContainsWebServicesReference(project)) {
				ReferenceProjectItem webServicesReferenceItem = new ReferenceProjectItem(project, "System.Web.Services");
				items.Add(webServicesReferenceItem);
			}
			return items;
		}
		
		/// <summary>
		/// Checks the file project items against the file items this web reference
		/// has and adds any items that do not exist in the project.
		/// </summary>
		List<ProjectItem> GetNewItems(List<ProjectItem> projectWebReferenceItems)
		{
			List<ProjectItem> newItems = new List<ProjectItem>();
			
			foreach (ProjectItem item in Items) {
				if (item is WebReferenceUrl) {
					// Ignore.
				} else if (!ContainsFileName(projectWebReferenceItems, item.FileName)) {
					newItems.Add(item);
				}
			}
			
			return newItems;
		}
		
		/// <summary>
		/// Checks the file project items against the file items this web reference
		/// has and adds any items that have been removed but still exist in the
		/// project.
		/// </summary>
		List<ProjectItem> GetRemovedItems(List<ProjectItem> projectWebReferenceItems)
		{
			List<ProjectItem> removedItems = new List<ProjectItem>();
			
			foreach (ProjectItem item in projectWebReferenceItems) {
				if (!ContainsFileName(Items, item.FileName)) {
					removedItems.Add(item);
				}
			}
			
			return removedItems;
		}
		
		static bool ContainsFileName(List<ProjectItem> items, string fileName)
		{
			foreach (ProjectItem item in items) {
				if (FileUtility.IsEqualFileName(item.FileName, fileName)) {
					return true;
				}
			}
			return false;
		}
		
		static bool ContainsInvalidDirectoryChar(string item)
		{
			foreach (char ch in Path.GetInvalidPathChars()) {
				if (item.IndexOf(ch) >= 0) {
					return true;
				}
			}
			return false;
		}		
	}
}
