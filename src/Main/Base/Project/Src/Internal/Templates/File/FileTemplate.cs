// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
	public class FileTemplate
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

		bool   newFileDialogVisible = true;
		
		ArrayList files       = new ArrayList(); // contains FileDescriptionTemplate classes
		ArrayList properties  = new ArrayList();
		ArrayList scripts     = new ArrayList();
		ArrayList customTypes = new ArrayList();
		
		XmlElement fileoptions = null;
		
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

			string newFileDialogVisibleAttr  = config.GetAttribute("newfiledialogvisible");
			if (newFileDialogVisibleAttr != null && newFileDialogVisibleAttr.Length != 0) {
				if (newFileDialogVisibleAttr.ToLower() == "false")
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
				FileDescriptionTemplate template = new FileDescriptionTemplate(filenode.GetAttribute("name"),
				                                                               filenode.GetAttribute("language"),
				                                                               filenode.InnerText);
				this.files.Add(template);
			}
			
			// load scripts (if any)
			XmlNodeList scriptList = doc.DocumentElement.SelectNodes("Script");
			foreach (XmlElement scriptElement in scriptList) {
				this.scripts.Add(new TemplateScript(scriptElement));
			}
			
		}
		
		static void LoadFileTemplate(string filename)
		{
			FileTemplates.Add(new FileTemplate(filename));
		}
		
		static FileTemplate()
		{
			List<string> files = FileUtility.SearchDirectory(PropertyService.DataDirectory + 
			                            Path.DirectorySeparatorChar + "templates" + 
			                            Path.DirectorySeparatorChar + "file", "*.xft");
			foreach (string file in files) {
				try {
					if (Path.GetExtension(file) == ".xft") {
						LoadFileTemplate(file);
					}
				} catch(Exception e) {
					MessageService.ShowError(e, "Error loading template file " + file + ".");
				}
			}
		}
	}
}
