// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	public class TemplateProperty
	{
		string name;
		string localizedName;
		string type;
		string category;
		string description;
		string defaultValue;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string LocalizedName {
			get {
				return localizedName;
			}
		}
		
		public string Type {
			get {
				return type;
			}
		}
		
		public string Category {
			get {
				return category;
			}
		}
		
		public string Description {
			get {
				return description;
			}
		}
		
		public string DefaultValue {
			get {
				return defaultValue;
			}
		}
		
		public TemplateProperty(XmlElement propertyElement)
		{
			name         = propertyElement.GetAttribute("name");
			localizedName= propertyElement.GetAttribute("localizedName");
			type         = propertyElement.GetAttribute("type");
			category     = propertyElement.GetAttribute("category");
			description  = propertyElement.GetAttribute("description");
			defaultValue = propertyElement.GetAttribute("defaultValue");
		}
	}
	
	public class TemplateType
	{
		string    name;
		Hashtable pairs = new Hashtable();
		
		public string Name {
			get {
				return name;
			}
		}
		
		public Hashtable Pairs {
			get {
				return pairs;
			}
		}
		
		public TemplateType(XmlElement enumType)
		{
			name = enumType.GetAttribute("name");
			foreach (XmlElement node in enumType.ChildNodes) {
				pairs[node.GetAttribute("name")] = node.GetAttribute("value");
			}
		}
	}
	
	public class FileTemplateOptions
	{
		/// <summary>
		/// Gets/Sets whether the file being created will be untitled.
		/// </summary>
		public bool IsUntitled { get; set; }
		
		/// <summary>
		/// The parent project to which this file is added.
		/// Can be null when creating a file outside of a project.
		/// </summary>
		public IProject Project { get; set; }
		
		/// <summary>
		/// The name of the file
		/// </summary>
		public FileName FileName { get; set; }
		
		/// <summary>
		/// The default namespace to use for the newly created file.
		/// </summary>
		public string Namespace { get; set; }
		
		/// <summary>
		/// The class name (generated from the file name).
		/// </summary>
		public string ClassName { get; set; }
		
		//IDictionary<string, string> properties;
	}
	
	/// <summary>
	/// This class defines and holds the new file templates.
	/// </summary>
	public class FileTemplate : IComparable
	{
		public static List<FileTemplate> FileTemplates = new List<FileTemplate>();
		
		string author       = null;
		string name         = null;
		string category     = null;
		string languagename = null;
		string icon         = null;
		string description  = null;
		string wizardpath   = null;
		string defaultName  = null;
		string subcategory  = null;

		bool   newFileDialogVisible = true;
		
		List<FileDescriptionTemplate> files = new List<FileDescriptionTemplate>();
		List<TemplateProperty> properties  = new List<TemplateProperty>();
		List<TemplateType> customTypes = new List<TemplateType>();
		List<ReferenceProjectItem> requiredAssemblyReferences = new List<ReferenceProjectItem>();
		
		XmlElement fileoptions = null;
		Action<FileTemplateOptions> actions;
		
		int IComparable.CompareTo(object other)
		{
			FileTemplate pt = other as FileTemplate;
			if (pt == null) return -1;
			int res = category.CompareTo(pt.category);
			if (res != 0) return res;
			return name.CompareTo(pt.name);
		}
		
		public string Author {
			get {
				return author;
			}
		}
		public string Name {
			get {
				return name;
			}
		}
		public string Category {
			get {
				return category;
			}
		}
		public string Subcategory {
			get {
				return subcategory;
			}
		}
		public string LanguageName {
			get {
				return languagename;
			}
		}
		public string Icon {
			get {
				return icon;
			}
		}
		public string Description {
			get {
				return description;
			}
		}
		[Obsolete]
		public string WizardPath {
			get {
				return wizardpath;
			}
		}
		public string DefaultName {
			get {
				return defaultName;
			}
		}
		public XmlElement Fileoptions {
			get {
				return fileoptions;
			}
		}
		public bool NewFileDialogVisible {
			get {
				return newFileDialogVisible;
			}
		}
		
		public List<FileDescriptionTemplate> FileDescriptionTemplates {
			get {
				return files;
			}
		}
		
		public List<TemplateProperty> Properties {
			get {
				return properties;
			}
		}
		
		public List<TemplateType> CustomTypes {
			get {
				return customTypes;
			}
		}
		
		public List<ReferenceProjectItem> RequiredAssemblyReferences {
			get { return requiredAssemblyReferences; }
		}
		
		public bool HasProperties {
			get {
				return properties != null && properties.Count > 0;
			}
		}
		
		public FileTemplate(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			
			author = doc.DocumentElement.GetAttribute("author");
			
			XmlElement config = doc.DocumentElement["Config"];
			name         = config.GetAttribute("name");
			icon         = config.GetAttribute("icon");
			category     = config.GetAttribute("category");
			defaultName  = config.GetAttribute("defaultname");
			languagename = config.GetAttribute("language");
			
			if (config.HasAttribute("subcategory")) {
				subcategory = config.GetAttribute("subcategory");
			}

			string newFileDialogVisibleAttr  = config.GetAttribute("newfiledialogvisible");
			if (newFileDialogVisibleAttr != null && newFileDialogVisibleAttr.Length != 0) {
				if (newFileDialogVisibleAttr.Equals("false", StringComparison.OrdinalIgnoreCase))
					newFileDialogVisible = false;
			}

			if (doc.DocumentElement["Description"] != null) {
				description  = doc.DocumentElement["Description"].InnerText;
			}
			
			if (config["Wizard"] != null) {
				wizardpath = config["Wizard"].Attributes["path"].InnerText;
			}
			
			if (doc.DocumentElement["Properties"] != null) {
				XmlNodeList propertyList = doc.DocumentElement["Properties"].SelectNodes("Property");
				foreach (XmlElement propertyElement in propertyList) {
					properties.Add(new TemplateProperty(propertyElement));
				}
			}
			
			if (doc.DocumentElement["Types"] != null) {
				XmlNodeList typeList = doc.DocumentElement["Types"].SelectNodes("Type");
				foreach (XmlElement typeElement in typeList) {
					customTypes.Add(new TemplateType(typeElement));
				}
			}
			
			if (doc.DocumentElement["References"] != null) {
				XmlNodeList references = doc.DocumentElement["References"].SelectNodes("Reference");
				foreach (XmlElement reference in references) {
					if (!reference.HasAttribute("include"))
						throw new InvalidDataException("Reference without 'include' attribute!");
					ReferenceProjectItem item = new ReferenceProjectItem(null, reference.GetAttribute("include"));
					item.SetMetadata("HintPath", reference.GetAttribute("hintPath"));
					var requiredTargetFramework = reference.GetElementsByTagName("RequiredTargetFramework").OfType<XmlElement>().FirstOrDefault();
					if (requiredTargetFramework != null) {
						item.SetMetadata("RequiredTargetFramework", requiredTargetFramework.Value);
					}
					requiredAssemblyReferences.Add(item);
				}
			}
			
			if (doc.DocumentElement["Actions"] != null) {
				foreach (XmlElement el in doc.DocumentElement["Actions"]) {
					Action<FileTemplateOptions> action = ReadAction(el);
					if (action != null)
						actions += action;
				}
			}
			
			fileoptions = doc.DocumentElement["AdditionalOptions"];
			
			doc.DocumentElement.SetAttribute("fileName", filename); // used for template loading warnings
			
			// load the files
			XmlElement files  = doc.DocumentElement["Files"];
			XmlNodeList nodes = files.ChildNodes;
			foreach (XmlNode filenode in nodes) {
				if (filenode is XmlElement) {
					this.files.Add(new FileDescriptionTemplate((XmlElement)filenode, Path.GetDirectoryName(filename)));
				}
			}
		}
		
		static Action<FileTemplateOptions> ReadAction(XmlElement el)
		{
			switch (el.Name) {
				case "RunCommand":
					if (el.HasAttribute("path")) {
						try {
							ICommand command = (ICommand)AddInTree.BuildItem(el.GetAttribute("path"), null);
							return fileCreateInformation => {
								command.Owner = fileCreateInformation;
								command.Run();
							};
						} catch (TreePathNotFoundException ex) {
							MessageService.ShowWarning(ex.Message + " - in " + el.OwnerDocument.DocumentElement.GetAttribute("fileName"));
							return null;
						}
					} else {
						ProjectTemplate.WarnAttributeMissing(el, "path");
						return null;
					}
				default:
					ProjectTemplate.WarnObsoleteNode(el, "Unknown action element is ignored");
					return null;
			}
		}
		
		public void RunActions(FileTemplateOptions options)
		{
			if (actions != null)
				actions(options);
		}
		
		public static void UpdateTemplates()
		{
			string dataTemplateDir = Path.Combine(PropertyService.DataDirectory, "templates", "file");
			List<string> files = FileUtility.SearchDirectory(dataTemplateDir, "*.xft");
			foreach (string templateDirectory in AddInTree.BuildItems<string>(ProjectTemplate.TemplatePath, null, false)) {
				if (!Directory.Exists(templateDirectory))
					Directory.CreateDirectory(templateDirectory);
				files.AddRange(FileUtility.SearchDirectory(templateDirectory, "*.xft"));
			}
			FileTemplates.Clear();
			foreach (string file in files) {
				try {
					FileTemplates.Add(new FileTemplate(file));
				} catch (XmlException ex) {
					MessageService.ShowError("Error loading template file " + file + ":\n" + ex.Message);
				} catch (TemplateLoadException ex) {
					MessageService.ShowError("Error loading template file " + file + ":\n" + ex.ToString());
				} catch(Exception e) {
					MessageService.ShowException(e, "Error loading template file " + file + ".");
				}
			}
			FileTemplates.Sort();
		}
		
		static FileTemplate()
		{
			UpdateTemplates();
		}
	}
}
