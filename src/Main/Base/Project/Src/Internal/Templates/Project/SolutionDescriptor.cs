// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	internal class SolutionDescriptor
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
			
			internal bool AddContents(ISolutionFolder parentFolder, ProjectCreateOptions projectCreateOptions, string defaultLanguage)
			{
				// Create sub projects
				foreach (SolutionFolderDescriptor folderDescriptor in solutionFoldersDescriptors) {
					ISolutionFolder folder = parentFolder.CreateFolder(folderDescriptor.name);
					if (!folderDescriptor.AddContents(folder, projectCreateOptions, defaultLanguage))
						return false;
				}
				foreach (ProjectDescriptor projectDescriptor in projectDescriptors) {
					IProject newProject = projectDescriptor.CreateProject(parentFolder.ParentSolution, projectCreateOptions, defaultLanguage);
					if (newProject == null)
						return false;
					parentFolder.Items.Add(newProject);
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
		
		public ISolution CreateSolution(ProjectCreateOptions projectCreateInformation, string defaultLanguage)
		{
			string newSolutionName = StringParser.Parse(name, new StringTagPair("ProjectName", projectCreateInformation.SolutionName));
			
			string solutionLocation = Path.Combine(projectCreateInformation.SolutionPath, newSolutionName + ".sln");
			
			ISolution newSolution = SD.ProjectService.CreateEmptySolutionFile(FileName.Create(solutionLocation));
			
			if (!mainFolder.AddContents(newSolution, projectCreateInformation, defaultLanguage)) {
				newSolution.Dispose();
				return null;
			}
			
			// Save solution
			if (File.Exists(solutionLocation)) {
				
				string question = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Internal.Templates.CombineDescriptor.OverwriteProjectQuestion}",
				                                     new StringTagPair("combineLocation", solutionLocation));
				if (MessageService.AskQuestion(question)) {
					newSolution.Save();
				}
			} else {
				newSolution.Save();
			}
			return newSolution;
		}
		
		public static SolutionDescriptor CreateSolutionDescriptor(XmlElement element, string hintPath)
		{
			SolutionDescriptor solutionDescriptor = new SolutionDescriptor(element.Attributes["name"].InnerText);
			
			if (element["Options"] != null && element["Options"]["StartupProject"] != null) {
				solutionDescriptor.startupProject = element["Options"]["StartupProject"].InnerText;
			}
			
			solutionDescriptor.mainFolder.Read(element, hintPath);
			return solutionDescriptor;
		}
	}
}
