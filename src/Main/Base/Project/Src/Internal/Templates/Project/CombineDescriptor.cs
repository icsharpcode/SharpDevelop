// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
		ArrayList projectDescriptors = new ArrayList();
		ArrayList combineDescriptors = new ArrayList();
		
		string name;
		string startupProject    = null;
		string relativeDirectory = null;
		
		#region public properties
		public string StartupProject {
			get {
				return startupProject;
			}
		}

		public ArrayList ProjectDescriptors {
			get {
				return projectDescriptors;
			}
		}

		public ArrayList CombineDescriptors {
			get {
				return projectDescriptors;
			}
		}
		#endregion

		protected CombineDescriptor(string name)
		{
			this.name = name;
		}
		
		public string CreateCombine(ProjectCreateInformation projectCreateInformation, string defaultLanguage)
		{
			Solution newCombine     = new Solution();
			string  newCombineName = StringParser.Parse(name, new string[,] { 
				{"ProjectName", projectCreateInformation.ProjectName}
			});
			
			newCombine.Name = newCombineName;
			
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
			
			// Create sub projects
			foreach (ProjectDescriptor projectDescriptor in projectDescriptors) {
				IProject newProject = projectDescriptor.CreateProject(projectCreateInformation, defaultLanguage);
				if (newProject == null)
					return null;
				newProject.Location = FileUtility.GetRelativePath(oldCombinePath, newProject.FileName);
				newCombine.AddFolder(newProject);
				projectCreateInformation.CreatedProjects.Add(newProject.FileName);
			}
			
//			// Create sub combines
//			foreach (CombineDescriptor combineDescriptor in combineDescriptors) {
//				newCombine.AddEntry(combineDescriptor.CreateCombine(projectCreateInformation, defaultLanguage));
//			}
			
			projectCreateInformation.CombinePath = oldCombinePath;
			projectCreateInformation.ProjectBasePath = oldProjectPath;
			
			string combineLocation = Path.Combine(projectCreateInformation.CombinePath, newCombineName + ".sln");
			// Save combine
			if (File.Exists(combineLocation)) {
				
				StringParser.Properties["combineLocation"] = combineLocation;
				if (MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.Internal.Templates.CombineDescriptor.OverwriteProjectQuestion}")) {
					newCombine.Save(combineLocation);
				}
			} else {
				newCombine.Save(combineLocation);
			}
			newCombine.Dispose();
			return combineLocation;
		}
		
		public static CombineDescriptor CreateCombineDescriptor(XmlElement element)
		{
			CombineDescriptor combineDescriptor = new CombineDescriptor(element.Attributes["name"].InnerText);
			
			if (element.Attributes["directory"] != null) {
				combineDescriptor.relativeDirectory = element.Attributes["directory"].InnerText;
			}
			
			if (element["Options"] != null && element["Options"]["StartupProject"] != null) {
				combineDescriptor.startupProject = element["Options"]["StartupProject"].InnerText;
			}
			
			foreach (XmlNode node in element.ChildNodes) {
				if (node != null) {
					switch (node.Name) {
						case "Project":
							combineDescriptor.projectDescriptors.Add(ProjectDescriptor.CreateProjectDescriptor((XmlElement)node));
							break;
						case "Combine":
							combineDescriptor.combineDescriptors.Add(CreateCombineDescriptor((XmlElement)node));
							break;
					}
				}
			}
			return combineDescriptor;
		}
	}
}
