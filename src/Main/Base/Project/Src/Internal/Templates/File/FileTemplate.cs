// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using ICSharpCode.Core;

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
	
	public class TemplateScript
	{
		string languageName;
		string runAt;
		string scriptSourceCode;
		
		public string LanguageName {
			get {
				return languageName;
			}
		}
		
		public string RunAt {
			get {
				return runAt;
			}
		}
		string SourceText {
			get {
				return "public class ScriptObject : System.MarshalByRefObject { " + scriptSourceCode + "}";
			}
		}
		
		
		public TemplateScript(XmlElement scriptConfig)
		{
			languageName     = scriptConfig.GetAttribute("language");
			runAt            = scriptConfig.GetAttribute("runAt");
			scriptSourceCode = scriptConfig.InnerText;
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
	
	/// <summary>
	/// This class defines and holds the new file templates.
	/// </summary>
	public class FileTemplate : IComparable
	{
		public static ArrayList FileTemplates = new ArrayList();
		
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
		
		ArrayList files       = new ArrayList(); // contains FileDescriptionTemplate classes
		ArrayList properties  = new ArrayList();
		ArrayList scripts     = new ArrayList();
		ArrayList customTypes = new ArrayList();
		
		XmlElement fileoptions = null;
		
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
		
		public ArrayList FileDescriptionTemplates {
			get {
				return files;
			}
		}
		
		public ArrayList Properties {
			get {
				return properties;
			}
		}
		
		public ArrayList CustomTypes {
			get {
				return customTypes;
			}
		}
		
		public bool HasProperties {
			get {
				return properties != null && properties.Count > 0;
			}
		}
		
		public ArrayList Scripts {
			get {
				return scripts;
			}
		}
		
		public bool HasScripts {
			get {
				return scripts != null && scripts.Count > 0;
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
			
			fileoptions = doc.DocumentElement["AdditionalOptions"];
			
			// load the files
			XmlElement files  = doc.DocumentElement["Files"];
			XmlNodeList nodes = files.ChildNodes;
			foreach (XmlElement filenode in nodes) {
				this.files.Add(new FileDescriptionTemplate(filenode));
			}
			
			// load scripts (if any)
			XmlNodeList scriptList = doc.DocumentElement.SelectNodes("Script");
			foreach (XmlElement scriptElement in scriptList) {
				this.scripts.Add(new TemplateScript(scriptElement));
			}
			
		}
		
		static FileTemplate()
		{
			string dataTemplateDir = FileUtility.Combine(PropertyService.DataDirectory, "templates", "file");
			List<string> files = FileUtility.SearchDirectory(dataTemplateDir, "*.xft");
			foreach (string templateDirectory in AddInTree.BuildItems(ProjectTemplate.TemplatePath, null, false)) {
				files.AddRange(FileUtility.SearchDirectory(templateDirectory, "*.xft"));
			}
			foreach (string file in files) {
				try {
					FileTemplates.Add(new FileTemplate(file));
				} catch(Exception e) {
					MessageService.ShowError(e, "Error loading template file " + file + ".");
				}
			}
			FileTemplates.Sort();
		}
	}
}
