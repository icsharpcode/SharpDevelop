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
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	public class CombineDescriptor
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
					if (node != null) {
						switch (node.Name) {
							case "Project":
								projectDescriptors.Add(ProjectDescriptor.CreateProjectDescriptor((XmlElement)node, hintPath));
								break;
							case "SolutionFolder":
								solutionFoldersDescriptors.Add(new SolutionFolderDescriptor((XmlElement)node, hintPath));
								break;
						}
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
					newProject.Location = FileUtility.GetRelativePath(projectCreateInformation.CombinePath, newProject.FileName);
					parentFolder.AddFolder(newProject);
					projectCreateInformation.CreatedProjects.Add(newProject.FileName);
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

		protected CombineDescriptor(string name)
		{
			this.name = name;
		}
		
		public string CreateSolution(ProjectCreateInformation projectCreateInformation, string defaultLanguage)
		{
			Solution newSolution = new Solution();
			string newCombineName = StringParser.Parse(name, new string[,] {
			                                           	{"ProjectName", projectCreateInformation.ProjectName}
			                                           });
			
			newSolution.Name = newCombineName;
			
			string oldCombinePath = projectCreateInformation.CombinePath;
			string oldProjectPath = projectCreateInformation.ProjectBasePath;
			if (relativeDirectory != null && relativeDirectory.Length > 0 && relativeDirectory != ".") {
				projectCreateInformation.CombinePath     = Path.Combine(projectCreateInformation.CombinePath, relativeDirectory);
				projectCreateInformation.ProjectBasePath = Path.Combine(projectCreateInformation.CombinePath, relativeDirectory);
				if (!Directory.Exists(projectCreateInformation.CombinePath)) {
					Directory.CreateDirectory(projectCreateInformation.CombinePath);
				}
				if (!Directory.Exists(projectCreateInformation.ProjectBasePath)) {
					Directory.CreateDirectory(projectCreateInformation.ProjectBasePath);
				}
			}
			
			projectCreateInformation.CombinePath = oldCombinePath;
			projectCreateInformation.ProjectBasePath = oldProjectPath;
			
			if (!mainFolder.AddContents(newSolution, projectCreateInformation, defaultLanguage, newSolution)) {
				newSolution.Dispose();
				return null;
			}
			
			string combineLocation = Path.Combine(projectCreateInformation.CombinePath, newCombineName + ".sln");
			// Save combine
			if (File.Exists(combineLocation)) {
				
				StringParser.Properties["combineLocation"] = combineLocation;
				if (MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.Internal.Templates.CombineDescriptor.OverwriteProjectQuestion}")) {
					newSolution.Save(combineLocation);
				}
			} else {
				newSolution.Save(combineLocation);
			}
			newSolution.Dispose();
			return combineLocation;
		}
		
		public static CombineDescriptor CreateCombineDescriptor(XmlElement element, string hintPath)
		{
			CombineDescriptor combineDescriptor = new CombineDescriptor(element.Attributes["name"].InnerText);
			
			if (element.Attributes["directory"] != null) {
				combineDescriptor.relativeDirectory = element.Attributes["directory"].InnerText;
			}
			
			if (element["Options"] != null && element["Options"]["StartupProject"] != null) {
				combineDescriptor.startupProject = element["Options"]["StartupProject"].InnerText;
			}
			
			combineDescriptor.mainFolder.Read(element, hintPath);
			return combineDescriptor;
		}
	}
}
