// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	public class OpenFileAction
	{
		string fileName;
		
		public OpenFileAction(string fileName)
		{
			this.fileName = fileName;
		}
		
		public void Run(ProjectCreateInformation projectCreateInformation)
		{
			string parsedFileName = StringParser.Parse(fileName, new string[,] { {"ProjectName", projectCreateInformation.ProjectName} });
			string path = FileUtility.Combine(projectCreateInformation.ProjectBasePath, parsedFileName);
			FileService.OpenFile(path);
		}
	}
	
	/// <summary>
	/// This class defines and holds the new project templates.
	/// </summary>
	public class ProjectTemplate : IComparable
	{
		public static ArrayList ProjectTemplates = new ArrayList();
		
		string    originator    = null;
		string    created       = null;
		string    lastmodified  = null;
		string    name          = null;
		string    category      = null;
		string    languagename  = null;
		string    description   = null;
		string    icon          = null;
		string    wizardpath    = null;
		string    subcategory   = null;
		
		int IComparable.CompareTo(object other)
		{
			ProjectTemplate pt = other as ProjectTemplate;
			if (pt == null) return -1;
			int res = category.CompareTo(pt.category);
			if (res != 0) return res;
			return name.CompareTo(pt.name);
		}
		
		bool   newProjectDialogVisible = true;
		
		ArrayList actions      = new ArrayList();
		
		CombineDescriptor combineDescriptor = null;
		ProjectDescriptor projectDescriptor = null;
		
		#region Template Properties
		public string WizardPath {
			get {
				return wizardpath;
			}
		}
		
		public string Originator {
			get {
				return originator;
			}
		}
		
		public string Created {
			get {
				return created;
			}
		}
		
		public string LastModified {
			get {
				return lastmodified;
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
		
		public string Description {
			get {
				return description;
			}
		}
		
		public string Icon {
			get {
				return icon;
			}
		}
		
		public bool NewProjectDialogVisible {
			get {
				return newProjectDialogVisible;
			}
		}
		
		
		[Browsable(false)]
		public CombineDescriptor CombineDescriptor {
			get {
				return combineDescriptor;
			}
		}
		
		[Browsable(false)]
		public ProjectDescriptor ProjectDescriptor {
			get {
				return projectDescriptor;
			}
		}
		#endregion
		
		protected ProjectTemplate(string fileName)
		{
			XmlDocument doc = new XmlDocument();
			try {
				doc.Load(fileName);
			} catch (XmlException ex) {
				MessageService.ShowError("Invalid xml: " + fileName + "\n" + ex.Message);
				return;
			}
			
			doc.DocumentElement.SetAttribute("fileName", fileName); // needed for warning messages for unknown elements
			originator   = doc.DocumentElement.GetAttribute("originator");
			created      = doc.DocumentElement.GetAttribute("created");
			lastmodified = doc.DocumentElement.GetAttribute("lastModified");
			
			string newProjectDialogVisibleAttr  = doc.DocumentElement.GetAttribute("newprojectdialogvisible");
			if (newProjectDialogVisibleAttr != null && newProjectDialogVisibleAttr.Length != 0) {
				if (newProjectDialogVisibleAttr.Equals("false", StringComparison.OrdinalIgnoreCase))
					newProjectDialogVisible = false;
			}
			
			XmlElement config = doc.DocumentElement["TemplateConfiguration"];
			
			if (config["Wizard"] != null) {
				wizardpath = config["Wizard"].InnerText;
			}
			
			name         = config["Name"].InnerText;
			category     = config["Category"].InnerText;
			languagename = config["LanguageName"].InnerText;
			
			if (config["Subcategory"] != null) {
				subcategory = config["Subcategory"].InnerText;
			}
			
			if (config["Description"] != null) {
				description  = config["Description"].InnerText;
			}
			
			if (config["Icon"] != null) {
				icon = config["Icon"].InnerText;
			}
			
			if (doc.DocumentElement["Solution"] != null) {
				combineDescriptor = CombineDescriptor.CreateCombineDescriptor(doc.DocumentElement["Solution"]);
			} else if (doc.DocumentElement["Combine"] != null) {
				combineDescriptor = CombineDescriptor.CreateCombineDescriptor(doc.DocumentElement["Combine"]);
			}
			
			if (doc.DocumentElement["Project"] != null) {
				projectDescriptor = ProjectDescriptor.CreateProjectDescriptor(doc.DocumentElement["Project"]);
			}
			
			// Read Actions;
			if (doc.DocumentElement["Actions"] != null) {
				foreach (XmlElement el in doc.DocumentElement["Actions"]) {
					actions.Add(new OpenFileAction(el.Attributes["filename"].InnerText));
				}
			}
		}
		
		string lastCombine    = null;
//		string startupProject = null;
		ProjectCreateInformation projectCreateInformation;

		public ProjectCreateInformation ProjectCreateInformation
		{
			get {
				return projectCreateInformation;
			}
		}
		
		public string CreateProject(ProjectCreateInformation projectCreateInformation)
		{
			this.projectCreateInformation = projectCreateInformation;
			
			if (wizardpath != null) {
				//              TODO: WIZARD
				Properties customizer = new Properties();
				customizer.Set("ProjectCreateInformation", projectCreateInformation);
				customizer.Set("ProjectTemplate", this);
				WizardDialog wizard = new WizardDialog("Project Wizard", customizer, wizardpath);
				if (wizard.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					if (combineDescriptor != null)
						lastCombine = combineDescriptor.CreateSolution(projectCreateInformation, this.languagename);
					else if (projectDescriptor != null)
						lastCombine = projectDescriptor.CreateProject(projectCreateInformation, this.languagename).FileName;
				} else {
					return null;
				}
			} else {
				if (combineDescriptor != null)
					lastCombine = combineDescriptor.CreateSolution(projectCreateInformation, this.languagename);
				else if (projectDescriptor != null)
					lastCombine = projectDescriptor.CreateProject(projectCreateInformation, this.languagename).FileName;
			}
			
			return lastCombine;
		}
		
		public void OpenCreatedCombine()
		{
			ProjectService.LoadSolution(lastCombine);
			
			foreach (OpenFileAction action in actions) {
				action.Run(projectCreateInformation);
			}
		}
		
		public const string TemplatePath = "/SharpDevelop/BackendBindings/Templates";
		
		static ProjectTemplate()
		{
			string dataTemplateDir = FileUtility.Combine(PropertyService.DataDirectory, "templates", "project");
			List<string> files = FileUtility.SearchDirectory(dataTemplateDir, "*.xpt");
			foreach (string templateDirectory in AddInTree.BuildItems(TemplatePath, null, false)) {
				files.AddRange(FileUtility.SearchDirectory(templateDirectory, "*.xpt"));
			}
			foreach (string fileName in files) {
				try {
					ProjectTemplates.Add(new ProjectTemplate(fileName));
				} catch (Exception e) {
					MessageService.ShowError(e, ResourceService.GetString("Internal.Templates.ProjectTemplate.LoadingError") + "\n(" + fileName + ")\n");
				}
			}
			ProjectTemplates.Sort();
		}
	}
}
