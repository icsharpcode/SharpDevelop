// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Exceptions;
using Import = System.Collections.Generic.KeyValuePair<string, string>;

namespace ICSharpCode.SharpDevelop.Templates
{
	/// <summary>
	/// This class is used inside the solution templates for projects.
	/// </summary>
	internal sealed class ProjectDescriptor
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
		string defaultPlatform;
		string relativePath;
		
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
		/// <param name="fileSystem">The file system from which referenced files are loaded. Use a ChrootFileSystem to specify the base directory for relative paths.</param>
		public ProjectDescriptor(XmlElement element, IReadOnlyFileSystem fileSystem)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			if (fileSystem == null)
				throw new ArgumentNullException("fileSystem");
			
			if (element.HasAttribute("name")) {
				name = element.GetAttribute("name");
			} else {
				name = "${ProjectName}";
			}
			if (element.HasAttribute("directory")) {
				relativePath = element.GetAttribute("directory");
			}
			languageName = element.GetAttribute("language");
			if (string.IsNullOrEmpty(languageName)) {
				ProjectTemplateImpl.WarnAttributeMissing(element, "language");
			}
			defaultPlatform = element.GetAttribute("defaultPlatform");
			
			LoadElementChildren(element, fileSystem);
		}
		
		#region Loading XML template
		void LoadElementChildren(XmlElement parentElement, IReadOnlyFileSystem fileSystem)
		{
			foreach (XmlElement node in ChildElements(parentElement)) {
				LoadElement(node, fileSystem);
			}
		}
		
		static IEnumerable<XmlElement> ChildElements(XmlElement parentElement)
		{
			return parentElement.ChildNodes.OfType<XmlElement>();
		}
		
		void LoadElement(XmlElement node, IReadOnlyFileSystem fileSystem)
		{
			switch (node.Name) {
				case "Options":
					ProjectTemplateImpl.WarnObsoleteNode(node, "Options are no longer supported, use properties instead.");
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
					LoadFiles(node, fileSystem);
					break;
				case "Imports":
					LoadImports(node);
					break;
				case "PropertyGroup":
					LoadPropertyGroup(node);
					break;
				case "Include":
					TemplateLoadException.AssertAttributeExists(node, "src");
					FileName includeFileName = FileName.Create(node.GetAttribute("src"));
					try {
						XmlDocument doc = new XmlDocument();
						using (var stream = fileSystem.OpenRead(includeFileName)) {
							doc.Load(stream);
						}
						doc.DocumentElement.SetAttribute("fileName", includeFileName);
						var fileSystemForInclude = new ReadOnlyChrootFileSystem(fileSystem, includeFileName.GetParentDirectory());
						if (doc.DocumentElement.Name == "Include") {
							LoadElementChildren(doc.DocumentElement, fileSystemForInclude);
						} else {
							LoadElement(doc.DocumentElement, fileSystemForInclude);
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
						ICommand command = (ICommand)SD.AddInTree.BuildItem(path, null);
						if (command == null)
							throw new TemplateLoadException("Unknown create action " + path);
						#endif
						return project => {
							#if !DEBUG
							ICommand command = (ICommand)SD.AddInTree.BuildItem(path, null);
							#endif
							if (command != null) {
								command.Execute(project);
							}
						};
					} else {
						ProjectTemplateImpl.WarnAttributeMissing(el, "path");
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
		
		void LoadFiles(XmlElement filesElement, IReadOnlyFileSystem fileSystem)
		{
			foreach (XmlElement fileElement in ChildElements(filesElement)) {
				files.Add(new FileDescriptionTemplate(fileElement, fileSystem));
			}
		}
		#endregion
		
		
		#region Create new project from template
		//Show prompt, create files from template, create project, execute command, save project
		public bool CreateProject(ProjectTemplateResult templateResults, string defaultLanguage, ISolutionFolder target)
		{
			var projectCreateOptions = templateResults.Options;
			var parentSolution = templateResults.Options.Solution;
			IProject project = null;
			bool success = false;
			try
			{
				string language = string.IsNullOrEmpty(languageName) ? defaultLanguage : languageName;
				ProjectBindingDescriptor descriptor = SD.ProjectService.ProjectBindings.FirstOrDefault(b => b.Language == language);
				IProjectBinding languageinfo = (descriptor != null) ? descriptor.Binding : null;
				
				if (languageinfo == null) {
					MessageService.ShowError(
						StringParser.Parse("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.CantCreateProjectWithTypeError}",
						                   new StringTagPair("type", language)));
					return false;
				}
				
				DirectoryName projectBasePath = GetProjectBasePath(projectCreateOptions);
				string newProjectName = StringParser.Parse(name, new StringTagPair("ProjectName", projectCreateOptions.ProjectName));
				Directory.CreateDirectory(projectBasePath);
				FileName projectLocation = projectBasePath.CombineFile(newProjectName + descriptor.ProjectFileExtension);
				ProjectCreateInformation info = new ProjectCreateInformation(parentSolution, projectLocation);
				info.TargetFramework = projectCreateOptions.TargetFramework;
				
				StringBuilder standardNamespace = new StringBuilder();
				// filter 'illegal' chars from standard namespace
				if (!string.IsNullOrEmpty(newProjectName)) {
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
				
				info.TypeGuid = descriptor.TypeGuid;
				info.RootNamespace = standardNamespace.ToString();
				info.ProjectName = newProjectName;
				if (!string.IsNullOrEmpty(defaultPlatform))
					info.ActiveProjectConfiguration = new ConfigurationAndPlatform("Debug", defaultPlatform);
				
				RunPreCreateActions(info);
				
				StringParserPropertyContainer.FileCreation["StandardNamespace"] = info.RootNamespace;
				
				if (File.Exists(projectLocation))
				{
					
					if (!MessageService.AskQuestion(
						StringParser.Parse("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteProjectQuestion}",
						                   new StringTagPair("projectLocation", projectLocation)),
						"${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion.InfoName}"))
					{
						return false; //The user doesnt want to overwrite the project...
					}
				}
				
				//Show prompt if any of the files exist
				StringBuilder existingFileNames = new StringBuilder();
				foreach (FileDescriptionTemplate file in files)
				{
					string fileName = Path.Combine(projectBasePath, StringParser.Parse(file.Name, new StringTagPair("ProjectName", info.ProjectName)));
					
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
					string fileName = Path.Combine(projectBasePath, StringParser.Parse(file.Name, new StringTagPair("ProjectName", info.ProjectName)));
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
							StreamWriter sr = new StreamWriter(File.Create(fileName), SD.FileService.DefaultFileEncoding);
							string fileContent = StringParser.Parse(file.Content,
							                                        new StringTagPair("ProjectName", projectCreateOptions.ProjectName),
							                                        new StringTagPair("SolutionName", projectCreateOptions.SolutionName),
							                                        new StringTagPair("FileName", fileName));
							fileContent = StringParser.Parse(fileContent);
							if (SD.EditorControlService.GlobalOptions.IndentationString != "\t") {
								fileContent = fileContent.Replace("\t", SD.EditorControlService.GlobalOptions.IndentationString);
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
				
				// Create Project
				info.InitializeTypeSystem = false;
				project = languageinfo.CreateProject(info);
				
				#region Create Project Items, Imports and Files
				// Add Project items
				if (!project.Items.IsReadOnly)
				{
					foreach (ProjectItem projectItem in projectItems) {
						ProjectItem newProjectItem = new UnknownProjectItem(
							project,
							StringParser.Parse(projectItem.ItemType.ItemName),
							StringParser.Parse(projectItem.Include,
							                   new StringTagPair("ProjectName", projectCreateOptions.ProjectName),
							                   new StringTagPair("SolutionName", projectCreateOptions.SolutionName))
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
						project.Items.Add(newProjectItem);
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
						string message;
						if (string.IsNullOrEmpty(importsFailureMessage)) {
							message = "Error creating project:\n" + ex.Message;
						} else {
							message = importsFailureMessage + "\n\n" + ex.Message;
						}
						throw new ProjectLoadException(message, ex);
					}
				}
				
				// Add Files
				if (!project.Items.IsReadOnly) {
					
					foreach (FileDescriptionTemplate file in files) {
						string fileName = Path.Combine(projectBasePath, StringParser.Parse(file.Name, new StringTagPair("ProjectName", projectCreateOptions.ProjectName)));
						FileProjectItem projectFile = new FileProjectItem(project, project.GetDefaultItemType(fileName));
						
						projectFile.Include = FileUtility.GetRelativePath(project.Directory, fileName);
						
						file.SetProjectItemProperties(projectFile);
						
						project.Items.Add(projectFile);
					}
				}
				
				#endregion
				
				RunCreateActions(project);
				
				project.ProjectCreationComplete();
				
				// Save project
				project.Save();
				
				// HACK : close and reload
				var fn = project.FileName;
				project.Dispose();
				ProjectLoadInformation loadInfo = new ProjectLoadInformation(parentSolution, fn, fn.GetFileNameWithoutExtension());
				project = SD.ProjectService.LoadProject(loadInfo);
				target.Items.Add(project);
				project.ProjectLoaded();
				
				SD.GetRequiredService<IProjectServiceRaiseEvents>().RaiseProjectCreated(new ProjectEventArgs(project));
				templateResults.NewProjects.Add(project);
				success = true;
				return true;
			} finally {
				if (project != null && !success)
					project.Dispose();
			}
		}
		
		DirectoryName GetProjectBasePath(ProjectTemplateOptions projectCreateOptions)
		{
			if (String.IsNullOrEmpty(relativePath)) {
				return projectCreateOptions.ProjectBasePath;
			}
			string relativeBasePath = StringParser.Parse(relativePath, new StringTagPair("ProjectName", projectCreateOptions.ProjectName));
			return projectCreateOptions.ProjectBasePath.CombineDirectory(relativeBasePath);
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
