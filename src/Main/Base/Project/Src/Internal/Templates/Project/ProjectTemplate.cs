// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
			string path = FileUtility.Combine(projectCreateInformation.ProjectBasePath, fileName);
			FileService.OpenFile(path);
		}
	}
	
	/// <summary>
	/// This class defines and holds the new project templates.
	/// </summary>
	public class ProjectTemplate
	{
		public static ArrayList ProjectTemplates = new ArrayList();
		
		string    originator   = null;
		string    created      = null;
		string    lastmodified = null;
		string    name         = null;
		string    category     = null;
		string    languagename = null;
		string    description  = null;
		string    icon         = null;
		string    wizardpath   = null;
		ArrayList actions      = new ArrayList();

		
		CombineDescriptor combineDescriptor = null;
		
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

		[Browsable(false)]
		public CombineDescriptor CombineDescriptor
		{
			get
			{
				return combineDescriptor;
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
			
			XmlElement config = doc.DocumentElement["TemplateConfiguration"];
			
			if (config["Wizard"] != null) {
				wizardpath = config["Wizard"].InnerText;
			}
			
			name         = config["Name"].InnerText;
			category     = config["Category"].InnerText;
			languagename = config["LanguageName"].InnerText;
			
			if (config["Description"] != null) {
				description  = config["Description"].InnerText;
			}
			
			if (config["Icon"] != null) {
				icon = config["Icon"].InnerText;
			}
			
			if (doc.DocumentElement["Combine"] != null) {
				combineDescriptor = CombineDescriptor.CreateCombineDescriptor(doc.DocumentElement["Combine"]);
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
					lastCombine = combineDescriptor.CreateCombine(projectCreateInformation, this.languagename);
				} else {
					return null;
				}
			} else {
				lastCombine = combineDescriptor.CreateCombine(projectCreateInformation, this.languagename);
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

		static void LoadProjectTemplate(string fileName)
		{
			try {
				
			} catch (Exception e) {
				throw new ApplicationException("error while loading " + fileName + " original exception was : " + e.ToString());
			}
		}
		
		static ProjectTemplate()
		{
			List<string> files = FileUtility.SearchDirectory(FileUtility.Combine(PropertyService.DataDirectory, "templates", "project"), "*.xpt");
			foreach (string fileName in files) {
				try {
					ProjectTemplates.Add(new ProjectTemplate(fileName));
				} catch (Exception e) {
					MessageService.ShowError(e, ResourceService.GetString("Internal.Templates.ProjectTemplate.LoadingError") + "\n(" + fileName + ")\n");
				}
			}
		}
	}
}
