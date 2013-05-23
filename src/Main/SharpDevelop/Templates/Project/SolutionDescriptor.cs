// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Templates
{
	internal class SolutionDescriptor
	{
		SolutionFolderDescriptor mainFolder = new SolutionFolderDescriptor("");
		
		class SolutionFolderDescriptor
		{
			internal string name;
			internal List<ProjectDescriptor> projectDescriptors = new List<ProjectDescriptor>();
			internal List<SolutionFolderDescriptor> solutionFoldersDescriptors = new List<SolutionFolderDescriptor>();
			
			internal void Read(XmlElement element, IReadOnlyFileSystem fileSystem)
			{
				name = element.GetAttribute("name");
				foreach (XmlNode node in element.ChildNodes) {
					switch (node.Name) {
						case "Project":
							projectDescriptors.Add(new ProjectDescriptor((XmlElement)node, fileSystem));
							break;
						case "SolutionFolder":
							solutionFoldersDescriptors.Add(new SolutionFolderDescriptor((XmlElement)node, fileSystem));
							break;
					}
				}
			}
			
			internal bool AddContents(ISolutionFolder parentFolder, ProjectTemplateResult templateResult, string defaultLanguage)
			{
				// Create sub projects
				foreach (SolutionFolderDescriptor folderDescriptor in solutionFoldersDescriptors) {
					ISolutionFolder folder = parentFolder.CreateFolder(folderDescriptor.name);
					if (!folderDescriptor.AddContents(folder, templateResult, defaultLanguage))
						return false;
				}
				foreach (ProjectDescriptor projectDescriptor in projectDescriptors) {
					IProject newProject = projectDescriptor.CreateProject(templateResult, defaultLanguage);
					if (newProject == null)
						return false;
					parentFolder.Items.Add(newProject);
				}
				return true;
			}
			
			public SolutionFolderDescriptor(XmlElement element, IReadOnlyFileSystem fileSystem)
			{
				Read(element, fileSystem);
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
		
		internal bool AddContents(ISolutionFolder parentFolder, ProjectTemplateResult templateResult, string defaultLanguage)
		{
			return mainFolder.AddContents(parentFolder, templateResult, defaultLanguage);
		}
		
		public static SolutionDescriptor CreateSolutionDescriptor(XmlElement element, IReadOnlyFileSystem fileSystem)
		{
			SolutionDescriptor solutionDescriptor = new SolutionDescriptor(element.Attributes["name"].InnerText);
			
			if (element["Options"] != null && element["Options"]["StartupProject"] != null) {
				solutionDescriptor.startupProject = element["Options"]["StartupProject"].InnerText;
			}
			
			solutionDescriptor.mainFolder.Read(element, fileSystem);
			return solutionDescriptor;
		}
	}
}
