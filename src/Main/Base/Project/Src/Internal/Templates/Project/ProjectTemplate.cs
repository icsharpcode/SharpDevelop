// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	sealed class TargetFramework
	{
		public readonly static TargetFramework Net20 = new TargetFramework("v2.0", ".NET Framework 2.0");
		public readonly static TargetFramework Net30 = new TargetFramework("v3.0", ".NET Framework 3.0") { BasedOn = Net20 };
		public readonly static TargetFramework Net35 = new TargetFramework("v3.5", ".NET Framework 3.5") { BasedOn = Net30 };
		public readonly static TargetFramework CF = new TargetFramework("CF", null);
		public readonly static TargetFramework CF20 = new TargetFramework("CF 2.0", "Compact Framework 2.0") { BasedOn = CF };
		public readonly static TargetFramework CF35 = new TargetFramework("CF 3.5", "Compact Framework 3.5") { BasedOn = CF20 };
		
		public readonly static TargetFramework[] TargetFrameworks = {
			Net35, Net30, Net20,
			CF, CF35, CF20
		};
		
		public const string DefaultTargetFrameworkName = "v3.5";
		
		public static TargetFramework GetByName(string name)
		{
			foreach (TargetFramework tf in TargetFrameworks) {
				if (tf.Name == name)
					return tf;
			}
			throw new ArgumentException("No target framework '" + name + "' exists");
		}
		
		string name, displayName;
		
		public string Name {
			get { return name; }
		}
		
		public string DisplayName {
			get { return displayName; }
		}
		
		public TargetFramework BasedOn;
		
		public bool IsBasedOn(TargetFramework potentialBase)
		{
			TargetFramework tmp = this;
			while (tmp != null) {
				if (tmp == potentialBase)
					return true;
				tmp = tmp.BasedOn;
			}
			return false;
		}
		
		public TargetFramework(string name, string displayName)
		{
			this.name = name;
			this.displayName = displayName;
		}
		
		public override string ToString()
		{
			return DisplayName;
		}
	}
	
	/// <summary>
	/// This class defines and holds the new project templates.
	/// </summary>
	public class ProjectTemplate : IComparable
	{
		static List<ProjectTemplate> projectTemplates;
		
		/// <summary>
		/// Gets the list of project templates. Not thread-safe!
		/// </summary>
		public static ReadOnlyCollection<ProjectTemplate> ProjectTemplates {
			get {
				WorkbenchSingleton.AssertMainThread();
				
				#if DEBUG
				// Always reload project templates if debugging.
				// TODO: Make this a configurable option.
				UpdateTemplates();
				#else
				if (projectTemplates == null) {
					UpdateTemplates();
				}
				#endif
				return projectTemplates.AsReadOnly();
			}
		}
		
		string originator;
		string created;
		string lastmodified;
		string name;
		string category;
		string languagename;
		string description;
		string icon;
		string wizardpath;
		string subcategory;
		TargetFramework[] supportedTargetFrameworks;
		
		internal bool HasSupportedTargetFrameworks {
			get { return supportedTargetFrameworks != null; }
		}
		
		internal bool SupportsTargetFramework(TargetFramework framework)
		{
			if (supportedTargetFrameworks == null)
				return true;
			// return true if framework is based on any of the supported target frameworks
			return supportedTargetFrameworks.Any(framework.IsBasedOn);
		}
		
		int IComparable.CompareTo(object other)
		{
			ProjectTemplate pt = other as ProjectTemplate;
			if (pt == null) return -1;
			int res = category.CompareTo(pt.category);
			if (res != 0) return res;
			return name.CompareTo(pt.name);
		}
		
		bool newProjectDialogVisible = true;
		
		List<Action<ProjectCreateInformation>> openActions = new List<Action<ProjectCreateInformation>>();
		
		SolutionDescriptor solutionDescriptor = null;
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
		public SolutionDescriptor SolutionDescriptor {
			get {
				return solutionDescriptor;
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
			doc.Load(fileName);
			LoadFromXml(doc.DocumentElement, fileName);
		}
		
		void LoadFromXml(XmlElement templateElement, string xmlFileName)
		{
			// required for warning messages for unknown elements
			templateElement.SetAttribute("fileName", xmlFileName);
			
			originator   = templateElement.GetAttribute("originator");
			created      = templateElement.GetAttribute("created");
			lastmodified = templateElement.GetAttribute("lastModified");
			
			string newProjectDialogVisibleAttr  = templateElement.GetAttribute("newprojectdialogvisible");
			if (string.Equals(newProjectDialogVisibleAttr, "false", StringComparison.OrdinalIgnoreCase))
				newProjectDialogVisible = false;
			
			XmlElement config = templateElement["TemplateConfiguration"];
			
			if (config["Wizard"] != null) {
				wizardpath = config["Wizard"].InnerText;
			}
			
			name         = config["Name"].InnerText;
			category     = config["Category"].InnerText;
			
			if (config["LanguageName"] != null) {
				languagename = config["LanguageName"].InnerText;
				WarnObsoleteNode(config["LanguageName"], "use language attribute on the project node instead");
			}
			
			if (config["Subcategory"] != null) {
				subcategory = config["Subcategory"].InnerText;
			}
			
			if (config["Description"] != null) {
				description  = config["Description"].InnerText;
			}
			
			if (config["Icon"] != null) {
				icon = config["Icon"].InnerText;
			}
			
			if (config["SupportedTargetFrameworks"] != null) {
				supportedTargetFrameworks =
					config["SupportedTargetFrameworks"].InnerText.Split(';')
					.Select<string,TargetFramework>(TargetFramework.GetByName).ToArray();
			}
			
			string hintPath = Path.GetDirectoryName(xmlFileName);
			if (templateElement["Solution"] != null) {
				solutionDescriptor = SolutionDescriptor.CreateSolutionDescriptor(templateElement["Solution"], hintPath);
			} else if (templateElement["Combine"] != null) {
				solutionDescriptor = SolutionDescriptor.CreateSolutionDescriptor(templateElement["Combine"], hintPath);
				WarnObsoleteNode(templateElement["Combine"], "Use <Solution> instead!");
			}
			
			if (templateElement["Project"] != null) {
				projectDescriptor = new ProjectDescriptor(templateElement["Project"], hintPath);
			}
			
			if (solutionDescriptor == null && projectDescriptor == null
			    || solutionDescriptor != null && projectDescriptor != null)
			{
				throw new TemplateLoadException("Template must contain either Project or Solution node!");
			}
			
			// Read Actions;
			if (templateElement["Actions"] != null) {
				foreach (XmlElement el in templateElement["Actions"]) {
					Action<ProjectCreateInformation> action = ReadAction(el);
					if (action != null)
						openActions.Add(action);
				}
			}
		}
		
		static Action<ProjectCreateInformation> ReadAction(XmlElement el)
		{
			switch (el.Name) {
				case "Open":
					if (el.HasAttribute("filename")) {
						string fileName = el.GetAttribute("filename");
						return projectCreateInformation => {
							string parsedFileName = StringParser.Parse(fileName, new string[,] { {"ProjectName", projectCreateInformation.ProjectName} });
							string path = FileUtility.Combine(projectCreateInformation.ProjectBasePath, parsedFileName);
							FileService.OpenFile(path);
						};
					} else {
						WarnAttributeMissing(el, "filename");
						return null;
					}
				case "RunCommand":
					if (el.HasAttribute("path")) {
						ICommand command = (ICommand)AddInTree.BuildItem(el.GetAttribute("path"), null);
						return projectCreateInformation => {
							command.Owner = projectCreateInformation;
							command.Run();
						};
					} else {
						WarnAttributeMissing(el, "path");
						return null;
					}
				default:
					WarnObsoleteNode(el, "Unknown action element is interpreted as <Open>");
					goto case "Open";
			}
		}
		
		internal static void WarnObsoleteNode(XmlElement element, string message)
		{
			MessageService.ShowWarning("Obsolete node <" + element.Name +
			                           "> used in '" + element.OwnerDocument.DocumentElement.GetAttribute("fileName") +
			                           "':\n" + message);
		}
		
		internal static void WarnObsoleteAttribute(XmlElement element, string attribute, string message)
		{
			MessageService.ShowWarning("Obsolete attribute <" + element.Name +
			                           " " + attribute + "=...>" +
			                           "used in '" + element.OwnerDocument.DocumentElement.GetAttribute("fileName") +
			                           "':\n" + message);
		}
		
		internal static void WarnAttributeMissing(XmlElement element, string attribute)
		{
			MessageService.ShowWarning("Missing attribute <" + element.Name +
			                           " " + attribute + "=...>" +
			                           " in '" + element.OwnerDocument.DocumentElement.GetAttribute("fileName") +
			                           "'");
		}
		
//		string startupProject = null;

		public string CreateProject(ProjectCreateInformation projectCreateInformation)
		{
			LoggingService.Info("Creating project from template '" + this.Category + "/" + this.Subcategory + "/" + this.Name + "'");
			if (wizardpath != null) {
				Properties customizer = new Properties();
				customizer.Set("ProjectCreateInformation", projectCreateInformation);
				customizer.Set("ProjectTemplate", this);
				WizardDialog wizard = new WizardDialog("Project Wizard", customizer, wizardpath);
				if (wizard.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) != DialogResult.OK) {
					return null;
				}
			}
			if (solutionDescriptor != null) {
				return solutionDescriptor.CreateSolution(projectCreateInformation, this.languagename);
			} else if (projectDescriptor != null) {
				bool createNewSolution = projectCreateInformation.Solution == null;
				if (createNewSolution) {
					projectCreateInformation.Solution = new Solution();
					projectCreateInformation.Solution.Name = projectCreateInformation.SolutionName;
					projectCreateInformation.Solution.FileName = Path.Combine(projectCreateInformation.SolutionPath, projectCreateInformation.SolutionName + ".sln");
				}
				IProject project = projectDescriptor.CreateProject(projectCreateInformation, this.languagename);
				if (project != null) {
					string solutionLocation = projectCreateInformation.Solution.FileName;
					if (createNewSolution) {
						projectCreateInformation.Solution.AddFolder(project);
						projectCreateInformation.Solution.Save();
						ProjectService.OnSolutionCreated(new SolutionEventArgs(projectCreateInformation.Solution));
						projectCreateInformation.Solution.Dispose();
					} else {
						project.Dispose();
					}
					return solutionLocation;
				} else {
					return null;
				}
			} else {
				return null;
			}
		}
		
		public void RunOpenActions(ProjectCreateInformation projectCreateInformation)
		{
			foreach (Action<ProjectCreateInformation> action in openActions) {
				action(projectCreateInformation);
			}
		}
		
		public const string TemplatePath = "/SharpDevelop/BackendBindings/Templates";
		
		public static void UpdateTemplates()
		{
			projectTemplates = new List<ProjectTemplate>();
			string dataTemplateDir = FileUtility.Combine(PropertyService.DataDirectory, "templates", "project");
			List<string> files = FileUtility.SearchDirectory(dataTemplateDir, "*.xpt");
			foreach (string templateDirectory in AddInTree.BuildItems<string>(TemplatePath, null, false)) {
				files.AddRange(FileUtility.SearchDirectory(templateDirectory, "*.xpt"));
			}
			foreach (string fileName in files) {
				try {
					projectTemplates.Add(new ProjectTemplate(fileName));
				} catch (XmlException e) {
					MessageService.ShowError(ResourceService.GetString("Internal.Templates.ProjectTemplate.LoadingError") + "\n(" + fileName + ")\n" + e.Message);
				} catch (TemplateLoadException e) {
					MessageService.ShowError(ResourceService.GetString("Internal.Templates.ProjectTemplate.LoadingError") + "\n(" + fileName + ")\n" + e.ToString());
				} catch (Exception e) {
					MessageService.ShowError(e, ResourceService.GetString("Internal.Templates.ProjectTemplate.LoadingError") + "\n(" + fileName + ")\n");
				}
			}
			projectTemplates.Sort();
		}
	}
}
