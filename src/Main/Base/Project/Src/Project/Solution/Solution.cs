// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class Solution : SolutionFolder, IDisposable
	{
		// contains <guid>, (IProject/ISolutionFolder) pairs.
		Dictionary<string, ISolutionFolder> guidDictionary = new Dictionary<string, ISolutionFolder>();
		
		string fileName = String.Empty;
		
		public IProject FindProjectContainingFile(string fileName)
		{
			IProject currentProject = ProjectService.CurrentProject;
			if (currentProject != null && currentProject.IsFileInProject(fileName))
				return currentProject;
			
			// Try all project's in the solution.
			foreach (IProject project in Projects) {
				if (project.IsFileInProject(fileName)) {
					return project;
				}
			}
			return null;
		}
		
		[Browsable(false)]
		public IEnumerable<IProject> Projects {
			get {
				Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>();
				
				foreach (ISolutionFolder solutionFolder in Folders) {
					stack.Push(solutionFolder);
				}
				
				while (stack.Count > 0) {
					ISolutionFolder currentFolder = stack.Pop();
					
					if (currentFolder is IProject) {
						yield return ((IProject)currentFolder);
					}
					
					if (currentFolder is ISolutionFolderContainer) {
						ISolutionFolderContainer currentContainer = (ISolutionFolderContainer)currentFolder;
						foreach (ISolutionFolder subFolder in currentContainer.Folders) {
							stack.Push(subFolder);
						}
					}
				}
			}
		}
		
		[Browsable(false)]
		public IEnumerable<ISolutionFolderContainer> SolutionFolderContainers {
			get {
				Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>();
				
				foreach (ISolutionFolder solutionFolder in Folders) {
					stack.Push(solutionFolder);
				}
				
				while (stack.Count > 0) {
					ISolutionFolder currentFolder = stack.Pop();
					
					if (currentFolder is ISolutionFolderContainer) {
						ISolutionFolderContainer currentContainer = (ISolutionFolderContainer)currentFolder;
						yield return currentContainer;
						foreach (ISolutionFolder subFolder in currentContainer.Folders) {
							stack.Push(subFolder);
						}
					}
				}
			}
		}

		[Browsable(false)]
		public IEnumerable<ISolutionFolder> SolutionFolders {
			get {
				Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>();
				
				foreach (ISolutionFolder solutionFolder in Folders) {
					stack.Push(solutionFolder);
				}
				
				while (stack.Count > 0) {
					ISolutionFolder currentFolder = stack.Pop();
					
					yield return currentFolder;
					
					if (currentFolder is ISolutionFolderContainer) {
						ISolutionFolderContainer currentContainer = (ISolutionFolderContainer)currentFolder;
						foreach (ISolutionFolder subFolder in currentContainer.Folders) {
							stack.Push(subFolder);
						}
					}
				}
			}
		}
		
		[Browsable(false)]
		public IProject StartupProject {
			get {
				if (!HasProjects) {
					return null;
				}
				IProject startupProject = preferences.StartupProject;
				if (startupProject != null)
					return startupProject;
				foreach (IProject project in Projects) {
					if (project.IsStartable) {
						return project;
					}
				}
				return null;
			}
		}
		
		[Browsable(false)]
		public bool HasProjects {
			get {
				return Projects.GetEnumerator().MoveNext();
			}
		}
		
		[Browsable(false)]
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		[Browsable(false)]
		public string Directory {
			get {
				return Path.GetDirectoryName(fileName);
			}
		}
		
		[Browsable(false)]
		public bool IsDirty {
			get {
				foreach (IProject project in Projects) {
					if (project.IsDirty) {
						return true;
					}
				}
				return false;
			}
		}
		
		#region ISolutionFolderContainer implementations
		public override ProjectSection SolutionItems {
			get {
				foreach (SolutionFolder folder in Folders) {
					if (folder.Name == "Solution Items") {
						return folder.SolutionItems;
					}
				}
				
				SolutionFolder newFolder = CreateFolder("Solution Items");
				return newFolder.SolutionItems;
			}
		}
		
		public override void AddFolder(ISolutionFolder folder)
		{
			base.AddFolder(folder);
			guidDictionary[folder.IdGuid] = folder;
		}
		
		#endregion
		
		SolutionPreferences preferences;
		
		[Browsable(false)]
		public SolutionPreferences Preferences {
			get {
				return preferences;
			}
		}
		
		public Solution()
		{
			preferences = new SolutionPreferences(this);
		}
		
		public ISolutionFolder GetSolutionFolder(string guid)
		{
			foreach (ISolutionFolder solutionFolder in SolutionFolders) {
				if (solutionFolder.IdGuid == guid) {
					return solutionFolder;
				}
			}
			return null;
		}
		
		public void Save()
		{
			try {
				Save(fileName);
			} catch (IOException ex) {
				MessageService.ShowError("Could not save " + fileName + ":\n" + ex.Message);
			} catch (UnauthorizedAccessException ex) {
				MessageService.ShowError("Could not save " + fileName + ":\n" + ex.Message + "\n\nEnsure the file is writable.");
			}
		}
		
		public SolutionFolder CreateFolder(string folderName)
		{
			return new SolutionFolder(folderName, folderName, "{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}");
		}
		
		
		public void Save(string fileName)
		{
			this.fileName = fileName;
			string outputDirectory = Path.GetDirectoryName(fileName);
			if (!System.IO.Directory.Exists(outputDirectory)) {
				System.IO.Directory.CreateDirectory(outputDirectory);
			}
			
			StringBuilder projectSection        = new StringBuilder();
			StringBuilder nestedProjectsSection = new StringBuilder();
			
			List<ISolutionFolder> folderList = Folders;
			Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>(folderList.Count);
			// push folders in reverse order because it's a stack
			for (int i = folderList.Count - 1; i >= 0; i--) {
				stack.Push(folderList[i]);
			}
			
			while (stack.Count > 0) {
				ISolutionFolder currentFolder = stack.Pop();
				
				projectSection.Append("Project(\"");
				projectSection.Append(currentFolder.TypeGuid);
				projectSection.Append("\")");
				projectSection.Append(" = ");
				projectSection.Append('"');
				projectSection.Append(currentFolder.Name);
				projectSection.Append("\", ");
				string relativeLocation;
				if (currentFolder is IProject) {
					currentFolder.Location = ((IProject)currentFolder).FileName;
				}
				if (Path.IsPathRooted(currentFolder.Location)) {
					relativeLocation = FileUtility.GetRelativePath(Path.GetDirectoryName(FileName), currentFolder.Location);
				} else {
					relativeLocation = currentFolder.Location;
				}
				projectSection.Append('"');
				projectSection.Append(relativeLocation);
				projectSection.Append("\", ");
				projectSection.Append('"');
				projectSection.Append(currentFolder.IdGuid);
				projectSection.Append("\"");
				projectSection.AppendLine();
				
				if (currentFolder is IProject) {
					IProject project = (IProject)currentFolder;
					// Web projects can have sections
					SaveProjectSections(project.ProjectSections, projectSection);
					
				} else if (currentFolder is SolutionFolder) {
					SolutionFolder folder = (SolutionFolder)currentFolder;
					
					SaveProjectSections(folder.Sections, projectSection);
					
					foreach (ISolutionFolder subFolder in folder.Folders) {
						stack.Push(subFolder);
						nestedProjectsSection.Append("\t\t");
						nestedProjectsSection.Append(subFolder.IdGuid);
						nestedProjectsSection.Append(" = ");
						nestedProjectsSection.Append(folder.IdGuid);
						nestedProjectsSection.Append(Environment.NewLine);
					}
				} else {
					LoggingService.Warn("Solution.Load(): unknown folder : " + currentFolder);
				}
				projectSection.Append("EndProject");
				projectSection.Append(Environment.NewLine);
			}
			
			StringBuilder globalSection = new StringBuilder();
			globalSection.Append("Global");
			globalSection.Append(Environment.NewLine);
			foreach (ProjectSection section in Sections) {
				globalSection.Append("\tGlobalSection(");
				globalSection.Append(section.Name);
				globalSection.Append(") = ");
				globalSection.Append(section.SectionType);
				globalSection.Append(Environment.NewLine);
				
				section.AppendSection(globalSection, "\t\t");
				
				globalSection.Append("\tEndGlobalSection");
				globalSection.Append(Environment.NewLine);
			}
			
			// we need to specify UTF8 because MsBuild needs the BOM
			using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8)) {
				sw.WriteLine("Microsoft Visual Studio Solution File, Format Version 9.00");
				Version v = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
				sw.WriteLine("# SharpDevelop " + v.Major + "." + v.Minor + "." + v.Build + "." + v.Revision);
				sw.Write(projectSection.ToString());
				
				sw.Write(globalSection.ToString());
				
				if (nestedProjectsSection.Length > 0) {
					sw.WriteLine("\tGlobalSection(NestedProjects) = preSolution");
					sw.Write(nestedProjectsSection.ToString());
					sw.WriteLine("\tEndGlobalSection");
				}
				
				sw.WriteLine("EndGlobal");
			}
		}
		
		static void SaveProjectSections(IEnumerable<ProjectSection> sections, StringBuilder projectSection)
		{
			foreach (ProjectSection section in sections) {
				projectSection.Append("\tProjectSection(");
				projectSection.Append(section.Name);
				projectSection.Append(") = ");
				projectSection.Append(section.SectionType);
				projectSection.Append(Environment.NewLine);
				
				section.AppendSection(projectSection, "\t\t");
				
				projectSection.Append("\tEndProjectSection");
				projectSection.Append(Environment.NewLine);
			}
		}
		
		static Regex versionPattern       = new Regex("Microsoft Visual Studio Solution File, Format Version\\s+(?<Version>.*)", RegexOptions.Compiled);
		
		static Regex projectLinePattern   = new Regex("Project\\(\"(?<ProjectGuid>.*)\"\\)\\s+=\\s+\"(?<Title>.*)\",\\s*\"(?<Location>.*)\",\\s*\"(?<Guid>.*)\"", RegexOptions.Compiled);
		static Regex globalSectionPattern = new Regex("\\s*GlobalSection\\((?<Name>.*)\\)\\s*=\\s*(?<Type>.*)", RegexOptions.Compiled);
		
		static string GetFirstNonCommentLine(TextReader sr)
		{
			string line = "";
			while ((line = sr.ReadLine()) != null) {
				line = line.Trim();
				if (line.Length > 0 && line[0] != '#')
					return line;
			}
			return "";
		}
		
		/// <summary>
		/// Reads the specified solution file. The project-location-guid information is written into the conversion class.
		/// </summary>
		/// <returns>The version number of the solution.</returns>
		public static string ReadSolutionInformation(string solutionFileName, Converter.PrjxToSolutionProject.Conversion conversion)
		{
			LoggingService.Debug("ReadSolutionInformation: " + solutionFileName);
			string solutionDirectory = Path.GetDirectoryName(solutionFileName);
			using (StreamReader sr = File.OpenText(solutionFileName)) {
				string line = GetFirstNonCommentLine(sr);
				Match match = versionPattern.Match(line);
				if (!match.Success) {
					return null;
				}
				string version = match.Result("${Version}");
				while ((line = sr.ReadLine()) != null) {
					match = projectLinePattern.Match(line);
					if (match.Success) {
						string projectGuid  = match.Result("${ProjectGuid}");
						string title        = match.Result("${Title}");
						string location     = Path.Combine(solutionDirectory, match.Result("${Location}"));
						string guid         = match.Result("${Guid}");
						LoggingService.Debug(guid + ": " + title);
						conversion.NameToGuid[title] = new Guid(guid);
						conversion.NameToPath[title] = location;
						conversion.GuidToPath[new Guid(guid)] = location;
					}
				}
				return version;
			}
		}
		
		static bool SetupSolution(Solution newSolution, string fileName)
		{
			string         solutionDirectory     = Path.GetDirectoryName(fileName);
			ProjectSection nestedProjectsSection = null;
			
			bool needsConversion = false;
			
			using (StreamReader sr = File.OpenText(fileName)) {
				string line = GetFirstNonCommentLine(sr);
				Match match = versionPattern.Match(line);
				if (!match.Success) {
					MessageService.ShowErrorFormatted("${res:SharpDevelop.Solution.InvalidSolutionFile}", fileName);
					return false;
				}
				
				switch (match.Result("${Version}")) {
					case "7.00":
						needsConversion = true;
						if (!MessageService.AskQuestion("${res:SharpDevelop.Solution.ConvertSolutionVersion7}")) {
							return false;
						}
						break;
					case "8.00":
						needsConversion = true;
						if (!MessageService.AskQuestion("${res:SharpDevelop.Solution.ConvertSolutionVersion8}")) {
							return false;
						}
						break;
					case "9.00":
						break;
					default:
						MessageService.ShowErrorFormatted("${res:SharpDevelop.Solution.UnknownSolutionVersion}", match.Result("${Version}"));
						return false;
				}
				
				while (true) {
					line = sr.ReadLine();
					
					if (line == null) {
						break;
					}
					match = projectLinePattern.Match(line);
					if (match.Success) {
						string projectGuid  = match.Result("${ProjectGuid}");
						string title        = match.Result("${Title}");
						string location     = match.Result("${Location}");
						string guid         = match.Result("${Guid}");
						
						if (!FileUtility.IsUrl(location)) {
							location = Path.Combine(solutionDirectory, location);
						}
						
						if (projectGuid == FolderGuid) {
							SolutionFolder newFolder = SolutionFolder.ReadFolder(sr, title, location, guid);
							newSolution.AddFolder(newFolder);
						} else {
							IProject newProject = LanguageBindingService.LoadProject(location, title, projectGuid);
							ReadProjectSections(sr, newProject.ProjectSections);
							newProject.IdGuid = guid;
							newSolution.AddFolder(newProject);
						}
						match = match.NextMatch();
					} else {
						match = globalSectionPattern.Match(line);
						if (match.Success) {
							ProjectSection newSection = ProjectSection.ReadGlobalSection(sr, match.Result("${Name}"), match.Result("${Type}"));
							// Don't put the NestedProjects section into the global sections list
							// because it's transformed to a tree representation and the tree representation
							// is transformed back to the NestedProjects section during save.
							if (newSection.Name == "NestedProjects") {
								nestedProjectsSection = newSection;
							} else {
								newSolution.Sections.Add(newSection);
							}
						}
					}
				}
			}
			// Create solution folder 'tree'.
			if (nestedProjectsSection != null) {
				foreach (SolutionItem item in nestedProjectsSection.Items) {
					string from = item.Name;
					string to   = item.Location;
					ISolutionFolderContainer folder = newSolution.guidDictionary[to] as ISolutionFolderContainer;
					folder.AddFolder(newSolution.guidDictionary[from]);
				}
			}
			
			if (newSolution.FixSolutionConfiguration(newSolution.Projects) || needsConversion) {
				// save in new format
				newSolution.Save();
			}
			return true;
		}
		
		public ProjectSection GetSolutionConfigurationsSection()
		{
			foreach (ProjectSection sec in Sections) {
				if (sec.Name == "SolutionConfigurationPlatforms")
					return sec;
			}
			ProjectSection newSec = new ProjectSection("SolutionConfigurationPlatforms", "preSolution");
			Sections.Insert(0, newSec);
			return newSec;
		}
		
		public ProjectSection GetProjectConfigurationsSection()
		{
			foreach (ProjectSection sec in Sections) {
				if (sec.Name == "ProjectConfigurationPlatforms")
					return sec;
			}
			ProjectSection newSec = new ProjectSection("ProjectConfigurationPlatforms", "postSolution");
			Sections.Add(newSec);
			return newSec;
		}
		
		public bool FixSolutionConfiguration(IEnumerable<IProject> projects)
		{
			ProjectSection solSec = GetSolutionConfigurationsSection();
			ProjectSection prjSec = GetProjectConfigurationsSection();
			bool changed = false;
			if (solSec.Items.Count == 0) {
				solSec.Items.Add(new SolutionItem("Debug|Any CPU", "Debug|Any CPU"));
				solSec.Items.Add(new SolutionItem("Release|Any CPU", "Release|Any CPU"));
				LoggingService.Warn("!! Inserted default SolutionConfigurationPlatforms !!");
				changed = true;
			}
			foreach (IProject project in projects) {
				string guid = project.IdGuid.ToUpperInvariant();
				foreach (SolutionItem configuration in solSec.Items) {
					string searchKey = guid + "." + configuration.Name + ".Build.0";
					if (!prjSec.Items.Exists(delegate (SolutionItem item) {
					                         	return item.Name == searchKey;
					                         }))
					{
						prjSec.Items.Add(new SolutionItem(searchKey, configuration.Location));
						changed = true;
					}
					searchKey = guid + "." + configuration.Name + ".ActiveCfg";
					if (!prjSec.Items.Exists(delegate (SolutionItem item) {
					                         	return item.Name == searchKey;
					                         }))
					{
						prjSec.Items.Add(new SolutionItem(searchKey, configuration.Location));
						changed = true;
					}
				}
			}
			return changed;
		}
		
		public IList<string> GetConfigurationNames()
		{
			List<string> configurationNames = new List<string>();
			foreach (SolutionItem item in GetSolutionConfigurationsSection().Items) {
				string name = AbstractProject.GetConfigurationNameFromKey(item.Name);
				if (!configurationNames.Contains(name))
					configurationNames.Add(name);
			}
			return configurationNames;
		}
		
		public IList<string> GetPlatformNames()
		{
			List<string> platformNames = new List<string>();
			foreach (SolutionItem item in GetSolutionConfigurationsSection().Items) {
				string name = AbstractProject.GetPlatformNameFromKey(item.Name);
				if (!platformNames.Contains(name))
					platformNames.Add(name);
			}
			return platformNames;
		}
		
		public void ApplySolutionConfigurationToProjects()
		{
			// TODO: Use assignments from project configuration section
			foreach (IProject p in Projects) {
				p.Configuration = preferences.ActiveConfiguration;
			}
		}
		
		public void ApplySolutionPlatformToProjects()
		{
			// TODO: Use assignments from project configuration section
			foreach (IProject p in Projects) {
				p.Platform = preferences.ActivePlatform;
			}
		}
		
		static Solution solutionBeingLoaded;
		
		public static Solution SolutionBeingLoaded {
			get {
				return solutionBeingLoaded;
			}
		}
		
		public static Solution Load(string fileName)
		{
			Solution newSolution = new Solution();
			solutionBeingLoaded = newSolution;
			newSolution.Name     = Path.GetFileNameWithoutExtension(fileName);
			
			string extension = Path.GetExtension(fileName).ToUpperInvariant();
			if (extension == ".CMBX") {
				if (!MessageService.AskQuestion("${res:SharpDevelop.Solution.ImportCmbx}")) {
					return null;
				}
				newSolution.fileName = Path.ChangeExtension(fileName, ".sln");
				ICSharpCode.SharpDevelop.Project.Converter.CombineToSolution.ConvertSolution(newSolution, fileName);
				if (newSolution.FixSolutionConfiguration(newSolution.Projects)) {
					newSolution.Save();
				}
			} else if (extension == ".PRJX") {
				if (!MessageService.AskQuestion("${res:SharpDevelop.Solution.ImportPrjx}")) {
					return null;
				}
				newSolution.fileName = Path.ChangeExtension(fileName, ".sln");
				ICSharpCode.SharpDevelop.Project.Converter.CombineToSolution.ConvertProject(newSolution, fileName);
				if (newSolution.FixSolutionConfiguration(newSolution.Projects)) {
					newSolution.Save();
				}
			} else {
				newSolution.fileName = fileName;
				if (!SetupSolution(newSolution, fileName)) {
					return null;
				}
			}
			
			solutionBeingLoaded = null;
			return newSolution;
		}
		
		#region System.IDisposable interface implementation
		public void Dispose()
		{
			foreach (IProject project in Projects) {
				project.Dispose();
			}
		}
		#endregion
		
		public void RunMSBuild(string target, MSBuildEngineCallback callback, IDictionary<string, string> additionalProperties)
		{
			MSBuildProject.RunMSBuild(FileName, target, preferences.ActiveConfiguration, preferences.ActivePlatform, false, callback, additionalProperties);
		}
		
		public void Build(MSBuildEngineCallback callback)
		{
			RunMSBuild(null, callback, null);
		}
		
		public void Rebuild(MSBuildEngineCallback callback)
		{
			RunMSBuild("Rebuild", callback, null);
		}
		
		public void Clean(MSBuildEngineCallback callback)
		{
			RunMSBuild("Clean", callback, null);
		}
		
		public void Publish(MSBuildEngineCallback callback)
		{
			RunMSBuild("Publish", callback, null);
		}
	}
}
