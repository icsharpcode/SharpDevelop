// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	public class SolutionDescriptor
	{
		SolutionFolderDescriptor mainFolder = new SolutionFolderDescriptor("");
		
		class SolutionFolderDescriptor
		{
			internal string name;
			internal List<ProjectDescriptor> projectDescriptors = new List<ProjectDescriptor>();
			internal List<SolutionFolderDescriptor> solutionFoldersDescriptors = new List<SolutionFolderDescriptor>();
			
			internal void Read(XmlElement element, string hintPath)
			{
				name = element.GetAttribute("name");
				foreach (XmlNode node in element.ChildNodes) {
					switch (node.Name) {
						case "Project":
							projectDescriptors.Add(new ProjectDescriptor((XmlElement)node, hintPath));
							break;
						case "SolutionFolder":
							solutionFoldersDescriptors.Add(new SolutionFolderDescriptor((XmlElement)node, hintPath));
							break;
					}
				}
			}
			
			internal bool AddContents(Solution solution, ProjectCreateInformation projectCreateInformation, string defaultLanguage, ISolutionFolderContainer parentFolder)
			{
				// Create sub projects
				foreach (SolutionFolderDescriptor folderDescriptor in solutionFoldersDescriptors) {
					SolutionFolder folder = solution.CreateFolder(folderDescriptor.name);
					parentFolder.AddFolder(folder);
					folderDescriptor.AddContents(solution, projectCreateInformation, defaultLanguage, folder);
				}
				foreach (ProjectDescriptor projectDescriptor in projectDescriptors) {
					IProject newProject = projectDescriptor.CreateProject(projectCreateInformation, defaultLanguage);
					if (newProject == null)
						return false;
					newProject.Location = FileUtility.GetRelativePath(projectCreateInformation.SolutionPath, newProject.FileName);
					parentFolder.AddFolder(newProject);
				}
				return true;
			}
			
			public SolutionFolderDescriptor(XmlElement element, string hintPath)
			{
				Read(element, hintPath);
			}
			
			public SolutionFolderDescriptor(string name)
			{
				this.name = name;
			}
		}
		
		string name;
		string startupProject    = null;
		string relativeDirectory = null;
		
		#region public properties
		public string StartupProject {
			get {
				return startupProject;
			}
		}

		public List<ProjectDescriptor> ProjectDescriptors {
			get {
				return mainFolder.projectDescriptors;
			}
		}
		#endregion

		protected SolutionDescriptor(string name)
		{
			this.name = name;
		}
		
		public string CreateSolution(ProjectCreateInformation projectCreateInformation, string defaultLanguage)
		{
			Solution newSolution = new Solution();
			projectCreateInformation.Solution = newSolution;
			
			string newSolutionName = StringParser.Parse(name, new string[,] {
			                                           	{"ProjectName", projectCreateInformation.SolutionName}
			                                           });
			
			newSolution.Name = newSolutionName;
			
			string oldSolutionPath = projectCreateInformation.SolutionPath;
			string oldProjectPath = projectCreateInformation.ProjectBasePath;
			if (relativeDirectory != null && relativeDirectory.Length > 0 && relativeDirectory != ".") {
				projectCreateInformation.SolutionPath     = Path.Combine(projectCreateInformation.SolutionPath, relativeDirectory);
				projectCreateInformation.ProjectBasePath = Path.Combine(projectCreateInformation.SolutionPath, relativeDirectory);
				if (!Directory.Exists(projectCreateInformation.SolutionPath)) {
					Directory.CreateDirectory(projectCreateInformation.SolutionPath);
				}
				if (!Directory.Exists(projectCreateInformation.ProjectBasePath)) {
					Directory.CreateDirectory(projectCreateInformation.ProjectBasePath);
				}
			}
			
			projectCreateInformation.SolutionPath = oldSolutionPath;
			projectCreateInformation.ProjectBasePath = oldProjectPath;
			
			if (!mainFolder.AddContents(newSolution, projectCreateInformation, defaultLanguage, newSolution)) {
				newSolution.Dispose();
				return null;
			}
			
			string solutionLocation = Path.Combine(projectCreateInformation.SolutionPath, newSolutionName + ".sln");
			// Save solution
			if (File.Exists(solutionLocation)) {
				
				StringParser.Properties["combineLocation"] = solutionLocation;
				if (MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.Internal.Templates.CombineDescriptor.OverwriteProjectQuestion}")) {
					newSolution.Save(solutionLocation);
				}
			} else {
				newSolution.Save(solutionLocation);
			}
			ProjectService.OnSolutionCreated(new SolutionEventArgs(newSolution));
			newSolution.Dispose();
			return solutionLocation;
		}
		
		public static SolutionDescriptor CreateSolutionDescriptor(XmlElement element, string hintPath)
		{
			SolutionDescriptor solutionDescriptor = new SolutionDescriptor(element.Attributes["name"].InnerText);
			
			if (element.Attributes["directory"] != null) {
				solutionDescriptor.relativeDirectory = element.Attributes["directory"].InnerText;
			}
			
			if (element["Options"] != null && element["Options"]["StartupProject"] != null) {
				solutionDescriptor.startupProject = element["Options"]["StartupProject"].InnerText;
			}
			
			solutionDescriptor.mainFolder.Read(element, hintPath);
			return solutionDescriptor;
		}
	}
}
