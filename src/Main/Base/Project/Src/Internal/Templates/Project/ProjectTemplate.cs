// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
							string parsedFileName = StringParser.Parse(fileName, new StringTagPair("ProjectName", projectCreateInformation.ProjectName));
							string path = Path.Combine(projectCreateInformation.ProjectBasePath, parsedFileName);
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
			if (solutionDescriptor != null) {
				return solutionDescriptor.CreateSolution(projectCreateInformation, this.languagename);
			} else if (projectDescriptor != null) {
				bool createNewSolution = projectCreateInformation.Solution == null;
				if (createNewSolution) {
					string fileName = Path.Combine(projectCreateInformation.SolutionPath, projectCreateInformation.SolutionName + ".sln");
					projectCreateInformation.Solution = new Solution(new ProjectChangeWatcher(fileName));
					projectCreateInformation.Solution.Name = projectCreateInformation.SolutionName;
					projectCreateInformation.Solution.FileName = fileName;
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
					if (createNewSolution)
						projectCreateInformation.Solution.Dispose();
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
			string dataTemplateDir = Path.Combine(PropertyService.DataDirectory, "templates", "project");
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
					MessageService.ShowException(e, ResourceService.GetString("Internal.Templates.ProjectTemplate.LoadingError") + "\n(" + fileName + ")\n");
				}
			}
			projectTemplates.Sort();
		}
	}
}
