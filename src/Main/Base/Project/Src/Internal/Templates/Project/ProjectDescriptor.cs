// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class is used inside the combine templates for projects.
	/// </summary>
	public class ProjectDescriptor
	{
		string name;
		string relativePath;
		string languageName = null;
		
		ArrayList files      = new ArrayList(); // contains FileTemplate classes
		ArrayList references = new ArrayList();
		
		XmlElement projectOptions = null;
		
		#region public properties
		public string LanguageName {
			get {
				return languageName;
			}
		}
		
		public ArrayList Files {
			get {
				return files;
			}
		}
		
		public ArrayList References {
			get {
				return references;
			}
		}
		
		public XmlElement ProjectOptions {
			get {
				return projectOptions;
			}
		}
		#endregion
		
		protected ProjectDescriptor(string name, string relativePath)
		{
			this.name = name;
			this.relativePath = relativePath;
		}
		
		public IProject CreateProject(ProjectCreateInformation projectCreateInformation, string defaultLanguage)
		{
			// remember old outerProjectBasePath
			string outerProjectBasePath = projectCreateInformation.ProjectBasePath;
			try
			{
				projectCreateInformation.ProjectBasePath = Path.Combine(projectCreateInformation.ProjectBasePath, this.relativePath);
				if (!Directory.Exists(projectCreateInformation.ProjectBasePath)) {
					Directory.CreateDirectory(projectCreateInformation.ProjectBasePath);
				}
				
				string language = languageName != null && languageName.Length > 0 ? languageName : defaultLanguage;
				LanguageBindingDescriptor descriptor = LanguageBindingService.GetCodonPerLanguageName(language);
				ILanguageBinding languageinfo = descriptor.Binding;
				
				if (languageinfo == null) {
					StringParser.Properties["type"] = language;
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.CantCreateProjectWithTypeError}");
					return null;
				}
				
				
				string newProjectName = StringParser.Parse(name, new string[,] {
					{"ProjectName", projectCreateInformation.ProjectName}
				});
				string projectLocation = FileUtility.Combine(projectCreateInformation.ProjectBasePath, newProjectName + LanguageBindingService.GetProjectFileExtension(language));
				
				projectCreateInformation.OutputProjectFileName = projectLocation;
				IProject project = languageinfo.CreateProject(projectCreateInformation, projectOptions);
				
				StringBuilder standardNamespace  = new StringBuilder();
				
				// filter 'illegal' chars from standard namespace
				if (newProjectName != null && newProjectName.Length > 0) {
					char ch = newProjectName[0];
					// can only begin with a letter or '_'
					if (!Char.IsLetter(ch)) {
						standardNamespace.Append('_');
					} else {
						standardNamespace.Append(ch);
					}
					for (int i = 1; i < newProjectName.Length; ++i) {
						ch = newProjectName[i];
						// can only contain letters, digits or '_'
						if (!Char.IsLetterOrDigit(ch) && ch != '.') {
							standardNamespace.Append('_');
						} else {
							standardNamespace.Append(ch);
							
						}
					}
				}
				project.RootNamespace = standardNamespace.ToString();
				StringParser.Properties["StandardNamespace"] = project.RootNamespace;
				// Add References
				foreach (ReferenceProjectItem projectReference in references) {
					projectReference.Project = project;
					project.Items.Add(projectReference);
				}
				
				// Add Files
				foreach (FileDescriptionTemplate file in files) {
					string fileName = Path.Combine(projectCreateInformation.ProjectBasePath, StringParser.Parse(file.Name, new string[,] { {"ProjectName", projectCreateInformation.ProjectName} }));
					FileProjectItem projectFile = new FileProjectItem(project, ItemType.Compile);
					
					if (!project.CanCompile(fileName)) {
						projectFile.BuildAction = ItemType.None;
					}
					
					projectFile.Include = FileUtility.GetRelativePath(project.Directory, fileName);
					
					while (projectFile.Include.Length > 1 && projectFile.Include.StartsWith(".")) {
						projectFile.Include = projectFile.Include.Substring(2);
					}

					project.Items.Add(projectFile);
					
					if (File.Exists(fileName)) {
						StringParser.Properties["fileName"] = fileName;
						if (!MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion}", "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion.InfoName}")) {
							continue;
						}
					}
					
					try {
						if (!Directory.Exists(Path.GetDirectoryName(fileName))) {
							Directory.CreateDirectory(Path.GetDirectoryName(fileName));
						}
						Properties properties = ((Properties)PropertyService.Get("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new Properties()));
						
						StreamWriter sr = new StreamWriter(File.Create(fileName), Encoding.GetEncoding(properties.Get("Encoding", 1252)));
						sr.Write(StringParser.Parse(StringParser.Parse(file.Content, new string[,] { {"ProjectName", projectCreateInformation.ProjectName}, {"FileName", fileName}})));
						sr.Close();
					} catch (Exception ex) {
						StringParser.Properties["fileName"] = fileName;
						MessageService.ShowError(ex, "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.FileCouldntBeWrittenError}");
					}
				}
				
				// Save project
				if (File.Exists(projectLocation)) {
					StringParser.Properties["projectLocation"] = projectLocation;
					if (MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteProjectQuestion}", "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion.InfoName}")) {
						project.Save(projectLocation);
					}
				} else {
					project.Save(projectLocation);
				}
				
				return project;
			}
			finally
			{
				// set back outerProjectBasePath
				projectCreateInformation.ProjectBasePath = outerProjectBasePath;
			}
		}
		
		public static ProjectDescriptor CreateProjectDescriptor(XmlElement element)
		{
			ProjectDescriptor projectDescriptor = new ProjectDescriptor(element.Attributes["name"].InnerText, element.Attributes["directory"].InnerText);
			
			projectDescriptor.projectOptions = element["Options"];
			if (element.Attributes["language"] != null) {
				projectDescriptor.languageName = element.Attributes["language"].InnerText;
			}
			
			if (element["Files"] != null) {
				foreach (XmlNode node in element["Files"].ChildNodes) {
					if (node != null && node.Name == "File") {
						XmlElement filenode = (XmlElement)node;
						FileDescriptionTemplate template = new FileDescriptionTemplate(filenode.GetAttribute("name"),
						                                                               filenode.GetAttribute("language"),
						                                                               filenode.InnerText);
						projectDescriptor.files.Add(template);
					}
				}
			}
			if (element["References"] != null) {
				foreach (XmlNode node in element["References"].ChildNodes) {
					if (node != null && node.Name == "Reference") {
						ReferenceProjectItem referenceProjectItem = new ReferenceProjectItem(null);
						referenceProjectItem.Include = node.Attributes["refto"].InnerXml;
//						projectReference.ReferenceType = (ReferenceType)Enum.Parse(typeof(ReferenceType), node.Attributes["type"].InnerXml);
						projectDescriptor.references.Add(referenceProjectItem);
					}
				}
			}
			return projectDescriptor;
		}
	}
}
