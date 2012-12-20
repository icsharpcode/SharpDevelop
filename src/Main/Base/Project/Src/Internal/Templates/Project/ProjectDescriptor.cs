// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Exceptions;
using Import = System.Collections.Generic.KeyValuePair<string, string>;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class is used inside the solution templates for projects.
	/// </summary>
	public sealed class ProjectDescriptor
	{
		private class ProjectProperty
		{
			public string Name, Value;
			public string Configuration, Platform;
			public PropertyStorageLocations Location;
			/// <summary>specifies whether to MSBuild-escape the value</summary>
			public bool ValueIsLiteral;
			
			public ProjectProperty(string name, string value, string configuration, string platform, PropertyStorageLocations location)
			{
				this.Name = name;
				this.Value = value;
				this.Configuration = configuration;
				this.Platform = platform;
				this.Location = location;
			}
		}
		
		string name;
		string relativePath;
		string defaultPlatform;
		
		/// <summary>
		/// The language of the project.
		/// </summary>
		string languageName;
		
		bool clearExistingImports;
		List<Import> projectImports = new List<Import>();
		string importsFailureMessage;
		
		List<FileDescriptionTemplate> files = new List<FileDescriptionTemplate>();
		List<ProjectItem> projectItems = new List<ProjectItem>();
		List<ProjectProperty> projectProperties = new List<ProjectProperty>();
		List<Action<IProject>> createActions = new List<Action<IProject>>();
		List<Action<ProjectCreateInformation>> preCreateActions = new List<Action<ProjectCreateInformation>>();
		
		/// <summary>
		/// Creates a project descriptor for the project node specified by the xml element.
		/// </summary>
		/// <param name="element">The &lt;Project&gt; node of the xml template file.</param>
		/// <param name="hintPath">The directory on which relative paths (e.g. for referenced files) are based.</param>
		public ProjectDescriptor(XmlElement element, string hintPath)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			if (hintPath == null)
				throw new ArgumentNullException("hintPath");
			
			if (element.HasAttribute("name")) {
				name = element.GetAttribute("name");
			} else {
				name = "${ProjectName}";
			}
			if (element.HasAttribute("directory")) {
				relativePath = element.GetAttribute("directory");
			} else {
				relativePath = ".";
			}
			languageName = element.GetAttribute("language");
			if (string.IsNullOrEmpty(languageName)) {
				ProjectTemplate.WarnAttributeMissing(element, "language");
			}
			defaultPlatform = element.GetAttribute("defaultPlatform");
			
			LoadElementChildren(element, hintPath);
		}
		
		#region Loading XML template
		void LoadElementChildren(XmlElement parentElement, string hintPath)
		{
			foreach (XmlElement node in ChildElements(parentElement)) {
				LoadElement(node, hintPath);
			}
		}
		
		static IEnumerable<XmlElement> ChildElements(XmlElement parentElement)
		{
			return parentElement.ChildNodes.OfType<XmlElement>();
		}
		
		void LoadElement(XmlElement node, string hintPath)
		{
			switch (node.Name) {
				case "Options":
					ProjectTemplate.WarnObsoleteNode(node, "Options are no longer supported, use properties instead.");
					break;
				case "CreateActions":
					LoadCreateActions(node);
					break;
				case "PreCreateActions":
					LoadPreCreateActions(node);
					break;
				case "ProjectItems":
					LoadProjectItems(node);
					break;
				case "Files":
					LoadFiles(node, hintPath);
					break;
				case "Imports":
					LoadImports(node);
					break;
				case "PropertyGroup":
					LoadPropertyGroup(node);
					break;
				case "Include":
					TemplateLoadException.AssertAttributeExists(node, "src");
					string includeFileName = Path.Combine(hintPath, node.GetAttribute("src"));
					try {
						XmlDocument doc = new XmlDocument();
						doc.Load(includeFileName);
						doc.DocumentElement.SetAttribute("fileName", includeFileName);
						if (doc.DocumentElement.Name == "Include") {
							LoadElementChildren(doc.DocumentElement, Path.GetDirectoryName(includeFileName));
						} else {
							LoadElement(doc.DocumentElement, Path.GetDirectoryName(includeFileName));
						}
					} catch (XmlException ex) {
						throw new TemplateLoadException("Error loading include file " + includeFileName, ex);
					}
					break;
				default:
					throw new TemplateLoadException("Unknown node in <Project>: " + node.Name);
			}
		}
		
		void LoadCreateActions(XmlElement createActionsElement)
		{
			foreach (XmlElement el in createActionsElement) {
				Action<IProject> action = ReadAction(el);
				if (action != null)
					createActions.Add(action);
			}
		}
		
		void LoadPreCreateActions(XmlElement preCreateActionsElement)
		{
			foreach (XmlElement el in preCreateActionsElement) {
				Action<ProjectCreateInformation> action = ReadAction(el);
				if (action != null)
					preCreateActions.Add(action);
			}
		}
		
		static Action<object> ReadAction(XmlElement el)
		{
			switch (el.Name) {
				case "RunCommand":
					if (el.HasAttribute("path")) {
						string path = el.GetAttribute("path");
						#if DEBUG
						ICommand command = (ICommand)AddInTree.BuildItem(path, null);
						if (command == null)
							throw new TemplateLoadException("Unknown create action " + path);
						#endif
						return project => {
							#if !DEBUG
							ICommand command = (ICommand)AddInTree.BuildItem(path, null);
							#endif
							if (command != null) {
								command.Owner = project;
								command.Run();
							}
						};
					} else {
						ProjectTemplate.WarnAttributeMissing(el, "path");
						return null;
					}
				default:
					throw new TemplateLoadException("Unknown node in <CreateActions>: " + el.Name);
			}
		}
		
		void LoadProjectItems(XmlElement projectItemsElement)
		{
			bool escapeIncludeValue = String.Equals(projectItemsElement.GetAttribute("escapeValue"), "false", StringComparison.OrdinalIgnoreCase);
			foreach (XmlElement projectItemElement in ChildElements(projectItemsElement)) {
				ProjectItem item = new UnknownProjectItem(null,
				                                          projectItemElement.Name,
				                                          projectItemElement.GetAttribute("Include"),
				                                          escapeIncludeValue);
				foreach (XmlElement metadataElement in ChildElements(projectItemElement)) {
					item.SetMetadata(metadataElement.Name, metadataElement.InnerText);
				}
				projectItems.Add(item);
			}
		}
		
		void LoadPropertyGroup(XmlElement propertyGroupElement)
		{
			string configuration = propertyGroupElement.GetAttribute("configuration");
			string platform = propertyGroupElement.GetAttribute("platform");
			PropertyStorageLocations storageLocation;
			if (string.IsNullOrEmpty(configuration) && string.IsNullOrEmpty(platform)) {
				storageLocation = PropertyStorageLocations.Base;
			} else {
				storageLocation = 0;
				if (!string.IsNullOrEmpty(configuration))
					storageLocation |= PropertyStorageLocations.ConfigurationSpecific;
				if (!string.IsNullOrEmpty(platform))
					storageLocation |= PropertyStorageLocations.PlatformSpecific;
			}
			if (string.Equals(propertyGroupElement.GetAttribute("userFile"), "true", StringComparison.OrdinalIgnoreCase)) {
				storageLocation |= PropertyStorageLocations.UserFile;
			}
			
			foreach (XmlElement propertyElement in ChildElements(propertyGroupElement)) {
				ProjectProperty p = new ProjectProperty(propertyElement.Name,
				                                        propertyElement.InnerText,
				                                        configuration, platform, storageLocation);
				if (string.Equals(propertyGroupElement.GetAttribute("escapeValue"), "false", StringComparison.OrdinalIgnoreCase)) {
					p.ValueIsLiteral = false;
				} else {
					p.ValueIsLiteral = true;
				}
				projectProperties.Add(p);
			}
		}
		
		void LoadImports(XmlElement importsElement)
		{
			if (string.Equals(importsElement.GetAttribute("clear"), "true", StringComparison.OrdinalIgnoreCase)) {
				clearExistingImports = true;
			}
			if (importsElement.HasAttribute("failureMessage")) {
				importsFailureMessage = importsElement.GetAttribute("failureMessage");
			}
			foreach (XmlElement importElement in ChildElements(importsElement)) {
				TemplateLoadException.AssertAttributeExists(importElement, "Project");
				projectImports.Add(new Import(
					importElement.GetAttribute("Project"),
					importElement.HasAttribute("Condition") ? importElement.GetAttribute("Condition") : null
				));
			}
		}
		
		void LoadFiles(XmlElement filesElement, string hintPath)
		{
			foreach (XmlElement fileElement in ChildElements(filesElement)) {
				files.Add(new FileDescriptionTemplate(fileElement, hintPath));
			}
		}
		#endregion
		
		
		#region Create new project from template
		//Show prompt, create files from template, create project, execute command, save project
		public IProject CreateProject(ProjectCreateInformation projectCreateInformation, string defaultLanguage)
		{
			// remember old outerProjectBasePath
			string outerProjectBasePath = projectCreateInformation.ProjectBasePath;
			string outerProjectName = projectCreateInformation.ProjectName;
			try
			{
				projectCreateInformation.ProjectBasePath = Path.Combine(projectCreateInformation.ProjectBasePath, GetRelativePath(projectCreateInformation));
				if (!Directory.Exists(projectCreateInformation.ProjectBasePath)) {
					Directory.CreateDirectory(projectCreateInformation.ProjectBasePath);
				}
				
				string language = string.IsNullOrEmpty(languageName) ? defaultLanguage : languageName;
				ProjectBindingDescriptor descriptor = ProjectBindingService.GetCodonPerLanguageName(language);
				IProjectBinding languageinfo = (descriptor != null) ? descriptor.Binding : null;
				
				if (languageinfo == null) {
					MessageService.ShowError(
						StringParser.Parse("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.CantCreateProjectWithTypeError}",
						                   new StringTagPair("type", language)));
					return null;
				}
				
				string newProjectName = StringParser.Parse(name, new StringTagPair("ProjectName", projectCreateInformation.ProjectName));
				string projectLocation = Path.GetFullPath(Path.Combine(projectCreateInformation.ProjectBasePath,
				                                                       newProjectName + ProjectBindingService.GetProjectFileExtension(language)));
				
				
				StringBuilder standardNamespace = new StringBuilder();
				// filter 'illegal' chars from standard namespace
				if (newProjectName != null && newProjectName.Length > 0) {
					char ch = '.';
					for (int i = 0; i < newProjectName.Length; ++i) {
						if (ch == '.') {
							// at beginning or after '.', only a letter or '_' is allowed
							ch = newProjectName[i];
							if (!Char.IsLetter(ch)) {
								standardNamespace.Append('_');
							} else {
								standardNamespace.Append(ch);
							}
						} else {
							ch = newProjectName[i];
							// can only contain letters, digits or '_'
							if (!Char.IsLetterOrDigit(ch) && ch != '.') {
								standardNamespace.Append('_');
							} else {
								standardNamespace.Append(ch);
							}
						}
					}
				}
				
				projectCreateInformation.OutputProjectFileName = projectLocation;
				projectCreateInformation.RootNamespace = standardNamespace.ToString();
				projectCreateInformation.ProjectName = newProjectName;
				if (!string.IsNullOrEmpty(defaultPlatform))
					projectCreateInformation.Platform = defaultPlatform;
				
				RunPreCreateActions(projectCreateInformation);
				
				StringParserPropertyContainer.FileCreation["StandardNamespace"] = projectCreateInformation.RootNamespace;
				
				if (File.Exists(projectLocation))
				{
					
					if (!MessageService.AskQuestion(
						StringParser.Parse("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteProjectQuestion}",
						                   new StringTagPair("projectLocation", projectLocation)),
						"${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion.InfoName}"))
					{
						return null; //The user doesnt want to overwrite the project...
					}
				}
				
				//Show prompt if any of the files exist
				StringBuilder existingFileNames = new StringBuilder();
				foreach (FileDescriptionTemplate file in files)
				{
					string fileName = Path.Combine(projectCreateInformation.ProjectBasePath, StringParser.Parse(file.Name, new StringTagPair("ProjectName", projectCreateInformation.ProjectName)));
					
					if (File.Exists(fileName))
					{
						if (existingFileNames.Length > 0)
							existingFileNames.Append(", ");
						existingFileNames.Append(Path.GetFileName(fileName));
					}
				}
				
				bool overwriteFiles = true;
				if (existingFileNames.Length > 0)
				{
					if (!MessageService.AskQuestion(
						StringParser.Parse("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion}",
						                   new StringTagPair("fileNames", existingFileNames.ToString())),
						"${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion.InfoName}"))
					{
						overwriteFiles = false;
					}
				}
				
				
				
				#region Copy files to target directory
				foreach (FileDescriptionTemplate file in files)
				{
					string fileName = Path.Combine(projectCreateInformation.ProjectBasePath, StringParser.Parse(file.Name, new StringTagPair("ProjectName", projectCreateInformation.ProjectName)));
					if (File.Exists(fileName) && !overwriteFiles)
					{
						continue;
					}
					
					try
					{
						if (!Directory.Exists(Path.GetDirectoryName(fileName))) {
							Directory.CreateDirectory(Path.GetDirectoryName(fileName));
						}
						if (!String.IsNullOrEmpty(file.BinaryFileName)) {
							// Binary content
							File.Copy(file.BinaryFileName, fileName, true);
						} else {
							// Textual content
							StreamWriter sr = new StreamWriter(File.Create(fileName), ParserService.DefaultFileEncoding);
							string fileContent = StringParser.Parse(file.Content, 
								new StringTagPair("ProjectName", projectCreateInformation.ProjectName),
								new StringTagPair("SolutionName", projectCreateInformation.SolutionName),
								new StringTagPair("FileName", fileName));
							fileContent = StringParser.Parse(fileContent);
							if (EditorControlService.GlobalOptions.IndentationString != "\t") {
								fileContent = fileContent.Replace("\t", EditorControlService.GlobalOptions.IndentationString);
							}
							sr.Write(fileContent);
							sr.Close();
						}
					}
					catch (Exception ex)
					{
						MessageService.ShowException(ex, "Exception writing " + fileName);
					}
				}
				#endregion
				
				#region Create Project
				IProject project;
				try {
					project = languageinfo.CreateProject(projectCreateInformation);
				} catch (ProjectLoadException ex) {
					MessageService.ShowError(ex.Message);
					return null;
				}
				#endregion
				
				#region Create Project Items, Imports and Files
				// Add Project items
				if (project is IProjectItemListProvider)
				{
					foreach (ProjectItem projectItem in projectItems) {
						ProjectItem newProjectItem = new UnknownProjectItem(
							project,
							StringParser.Parse(projectItem.ItemType.ItemName),
							StringParser.Parse(projectItem.Include,
								new StringTagPair("ProjectName", projectCreateInformation.ProjectName),
								new StringTagPair("SolutionName", projectCreateInformation.SolutionName))
						);
						foreach (string metadataName in projectItem.MetadataNames) {
							string metadataValue = projectItem.GetMetadata(metadataName);
							// if the input contains any special MSBuild sequences, don't escape the value
							// we want to escape only when the special characters are introduced by the StringParser.Parse replacement
							if (metadataValue.Contains("$(") || metadataValue.Contains("%"))
								newProjectItem.SetMetadata(StringParser.Parse(metadataName), StringParser.Parse(metadataValue));
							else
								newProjectItem.SetEvaluatedMetadata(StringParser.Parse(metadataName), StringParser.Parse(metadataValue));
						}
						((IProjectItemListProvider)project).AddProjectItem(newProjectItem);
					}
				}
				
				// Add properties from <PropertyGroup>
				// This must be done before adding <Imports>, because the import path can refer to properties.
				if (projectProperties.Count > 0) {
					if (!(project is MSBuildBasedProject))
						throw new Exception("<PropertyGroup> may be only used in project templates for MSBuildBasedProjects");
					
					foreach (ProjectProperty p in projectProperties) {
						((MSBuildBasedProject)project).SetProperty(
							StringParser.Parse(p.Configuration),
							StringParser.Parse(p.Platform),
							StringParser.Parse(p.Name),
							StringParser.Parse(p.Value),
							p.Location,
							p.ValueIsLiteral
						);
					}
				}
				
				// Add Imports
				if (clearExistingImports || projectImports.Count > 0) {
					MSBuildBasedProject msbuildProject = project as MSBuildBasedProject;
					if (msbuildProject == null)
						throw new Exception("<Imports> may be only used in project templates for MSBuildBasedProjects");
					try {
						msbuildProject.PerformUpdateOnProjectFile(
							delegate {
								var projectFile = msbuildProject.MSBuildProjectFile;
								if (clearExistingImports) {
									foreach (var import in projectFile.Imports.ToArray())
										projectFile.RemoveChild(import);
								}
								foreach (Import projectImport in projectImports) {
									projectFile.AddImport(projectImport.Key).Condition = projectImport.Value;
								}
								
							});
					} catch (InvalidProjectFileException ex) {
						if (string.IsNullOrEmpty(importsFailureMessage)) {
							MessageService.ShowError("Error creating project:\n" + ex.Message);
						} else {
							MessageService.ShowError(importsFailureMessage + "\n\n" + ex.Message);
						}
						return null;
					}
				}
				
				// Add Files
				if (project is IProjectItemListProvider) {
					
					foreach (FileDescriptionTemplate file in files) {
						string fileName = Path.Combine(projectCreateInformation.ProjectBasePath, StringParser.Parse(file.Name, new StringTagPair("ProjectName", projectCreateInformation.ProjectName)));
						FileProjectItem projectFile = new FileProjectItem(project, project.GetDefaultItemType(fileName));
						
						projectFile.Include = FileUtility.GetRelativePath(project.Directory, fileName);
						
						file.SetProjectItemProperties(projectFile);
						
						((IProjectItemListProvider)project).AddProjectItem(projectFile);
					}
				}
				
				#endregion
				
				RunCreateActions(project);
				
				project.ProjectCreationComplete();
				
				// Save project
				project.Save();
				
				
				projectCreateInformation.createdProjects.Add(project);
				ProjectService.OnProjectCreated(new ProjectEventArgs(project));
				return project;
			}
			finally
			{
				// set back outerProjectBasePath
				projectCreateInformation.ProjectBasePath = outerProjectBasePath;
				projectCreateInformation.ProjectName = outerProjectName;
			}
		}
		
		string GetRelativePath(ProjectCreateInformation projectCreateInformation)
		{
			return StringParser.Parse(this.relativePath, new StringTagPair("ProjectName", projectCreateInformation.ProjectName));
		}
		
		void RunPreCreateActions(ProjectCreateInformation projectCreateInformation)
		{
			foreach (var action in preCreateActions) {
				action(projectCreateInformation);
			}
		}
		
		void RunCreateActions(IProject project)
		{
			foreach (Action<IProject> action in createActions) {
				action(project);
			}
		}
		#endregion
	}
}
