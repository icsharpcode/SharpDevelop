// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class Solution : SolutionFolder, IDisposable, IBuildable
	{
		public const int SolutionVersionVS2005 = 9;
		public const int SolutionVersionVS2008 = 10;
		public const int SolutionVersionVS2010 = 11;
		public const int SolutionVersionVS11 = 12;
		
		/// <summary>contains &lt;GUID, (IProject/ISolutionFolder)&gt; pairs.</summary>
		Dictionary<string, ISolutionFolder> guidDictionary = new Dictionary<string, ISolutionFolder>();
		
		bool isLoading;
		string fileName = String.Empty;
		IProjectChangeWatcher changeWatcher;
		
		public Solution(IProjectChangeWatcher changeWatcher)
		{
			preferences = new SolutionPreferences(this);
			this.MSBuildProjectCollection = new Microsoft.Build.Evaluation.ProjectCollection();
			this.changeWatcher = changeWatcher;
		}
		
		[BrowsableAttribute(false)]
		public Microsoft.Build.Evaluation.ProjectCollection MSBuildProjectCollection { get; private set; }
		
		#region Enumerate projects/folders
		public IProject FindProjectContainingFile(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			IProject currentProject = ProjectService.CurrentProject;
			if (currentProject != null && currentProject.IsFileInProject(fileName))
				return currentProject;
			
			// Try all project's in the solution.
			IProject linkedProject = null;
			foreach (IProject project in Projects) {
				FileProjectItem file = project.FindFile(fileName);
				if (file != null) {
					if (file.IsLink)
						linkedProject = project;
					else
						return project; // prefer projects with non-links over projects with links
				}
			}
			return linkedProject;
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
		
		/// <summary>
		/// Returns the startup project. If no startup project is set in the solution preferences,
		/// returns any project that is startable.
		/// </summary>
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
		
		public ISolutionFolder GetSolutionFolder(string guid)
		{
			foreach (ISolutionFolder solutionFolder in SolutionFolders) {
				if (solutionFolder.IdGuid == guid) {
					return solutionFolder;
				}
			}
			return null;
		}
		
		public SolutionFolder CreateFolder(string folderName)
		{
			return new SolutionFolder(folderName, folderName, "{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}");
		}
		#endregion
		
		#region Properties
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
				changeWatcher.Disable();
				fileName = value;
				changeWatcher.Rename(fileName);
				changeWatcher.Enable();
			}
		}
		
		[Browsable(false)]
		public string Directory {
			get {
				return Path.GetDirectoryName(fileName);
			}
		}
		
		SolutionPreferences preferences;
		
		[Browsable(false)]
		public SolutionPreferences Preferences {
			get {
				return preferences;
			}
		}
		
		/// <summary>Returns true if the solution is readonly.</summary>
		[Browsable(false)]
		public bool ReadOnly {
			get
			{
				try {
					FileAttributes attributes = File.GetAttributes(fileName);
					return ((FileAttributes.ReadOnly & attributes) == FileAttributes.ReadOnly);
				} catch (FileNotFoundException) {
					return false;
				} catch (DirectoryNotFoundException) {
					return true;
				}
			}
		}
		#endregion
		
		#region ISolutionFolderContainer implementations
		[Browsable(false)]
		public override Solution ParentSolution {
			get { return this; }
		}
		
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
		
		internal void BeforeAddFolderToSolution(ISolutionFolder folder)
		{
			IProject project = folder as IProject;
			if (project != null && !isLoading) {
				// HACK: don't deal with configurations during loading
				if (this.GetConfigurationNames().Count == 0) {
					foreach (string config in project.ConfigurationNames) {
						foreach (string platform in project.PlatformNames)
							AddSolutionConfigurationPlatform(config, FixPlatformNameForSolution(platform), null, false, false);
					}
				}
			}
		}
		
		public override void AddFolder(ISolutionFolder folder)
		{
			base.AddFolder(folder);
			guidDictionary[folder.IdGuid] = folder;
		}
		
		internal void AfterAddFolderToSolution(ISolutionFolder folder)
		{
			IProject project = folder as IProject;
			if (project != null && !isLoading) {
				FixSolutionConfiguration(new[] { project });
			}
		}
		
		#endregion
		
		#region Save
		public void Save()
		{
			try {
				changeWatcher.Disable();
				Save(fileName);
				changeWatcher.Enable();
			} catch (IOException ex) {
				MessageService.ShowErrorFormatted("${res:SharpDevelop.Solution.CannotSave.IOException}", fileName, ex.Message);
			} catch (UnauthorizedAccessException ex) {
				FileAttributes attributes = File.GetAttributes(fileName);
				if ((FileAttributes.ReadOnly & attributes) == FileAttributes.ReadOnly) {
					MessageService.ShowErrorFormatted("${res:SharpDevelop.Solution.CannotSave.ReadOnly}", fileName);
				}
				else
				{
					MessageService.ShowErrorFormatted
						("${res:SharpDevelop.Solution.CannotSave.UnauthorizedAccessException}", fileName, ex.Message);
				}
			}
		}
		
		public void Save(string fileName)
		{
			changeWatcher.Disable();
			changeWatcher.Rename(fileName);
			this.fileName = fileName;
			UpdateMSBuildProperties();
			string outputDirectory = Path.GetDirectoryName(fileName);
			if (!System.IO.Directory.Exists(outputDirectory)) {
				System.IO.Directory.CreateDirectory(outputDirectory);
			}
			
			StringBuilder projectSection        = new StringBuilder();
			StringBuilder nestedProjectsSection = new StringBuilder();
			
			Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>(Folders.Count);
			// push folders in reverse order because it's a stack
			for (int i = Folders.Count - 1; i >= 0; i--) {
				stack.Push(Folders[i]);
			}
			
			while (stack.Count > 0) {
				ISolutionFolder currentFolder = stack.Pop();
				string relativeLocation;
				
				// The project file relative to the solution file.
				if (currentFolder is IProject) {
					currentFolder.Location = ((IProject)currentFolder).FileName;
				}
				if (Path.IsPathRooted(currentFolder.Location)) {
					relativeLocation = FileUtility.GetRelativePath(Path.GetDirectoryName(FileName), currentFolder.Location);
				} else {
					relativeLocation = currentFolder.Location;
				}
				
				projectSection.AppendFormat
					("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
					 new object [] {currentFolder.TypeGuid, currentFolder.Name, relativeLocation, currentFolder.IdGuid});
				projectSection.AppendLine();
				
				if (currentFolder is IProject) {
					IProject project = (IProject)currentFolder;
					// Web projects can have sections
					SaveProjectSections(project.ProjectSections, projectSection);
					
				} else if (currentFolder is SolutionFolder) {
					SolutionFolder folder = (SolutionFolder)currentFolder;
					
					SaveProjectSections(folder.Sections, projectSection);
					
					// Push the sub folders in reverse order so that we pop them
					// in the correct order.
					for (int i = folder.Folders.Count - 1; i >= 0; i--) {
						stack.Push(folder.Folders[i]);
					}
					// But use normal order for printing the nested projects section
					for (int i = 0; i < folder.Folders.Count; i++) {
						ISolutionFolder subFolder = folder.Folders[i];
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
			
			// we need to specify UTF8 because MSBuild needs the BOM
			using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8)) {
				sw.WriteLine();
				int versionNumber = SolutionVersionVS2005;
				foreach (IProject p in this.Projects) {
					if (p.MinimumSolutionVersion > versionNumber)
						versionNumber = p.MinimumSolutionVersion;
				}
				
				sw.WriteLine("Microsoft Visual Studio Solution File, Format Version " + versionNumber + ".00");
				if (versionNumber == SolutionVersionVS2005) {
					sw.WriteLine("# Visual Studio 2005");
				} else if (versionNumber == SolutionVersionVS2008) {
					sw.WriteLine("# Visual Studio 2008");
				} else if (versionNumber == SolutionVersionVS2010) {
					sw.WriteLine("# Visual Studio 2010");
				} else if (versionNumber == SolutionVersionVS11) {
					sw.WriteLine("# Visual Studio 11");
				}
				sw.WriteLine("# SharpDevelop " + RevisionClass.Major + "." + RevisionClass.Minor);
				sw.Write(projectSection.ToString());
				
				sw.Write(globalSection.ToString());
				
				if (nestedProjectsSection.Length > 0) {
					sw.WriteLine("\tGlobalSection(NestedProjects) = preSolution");
					sw.Write(nestedProjectsSection.ToString());
					sw.WriteLine("\tEndGlobalSection");
				}
				
				sw.WriteLine("EndGlobal");
			}
			changeWatcher.Enable();
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
		#endregion
		
		#region Read/SetupSolution
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
		
		static bool SetupSolution(Solution newSolution)
		{
			ProjectSection nestedProjectsSection = null;
			
			// read solution files using system encoding, but detect UTF8 if BOM is present
			using (StreamReader sr = new StreamReader(newSolution.FileName, Encoding.Default, true)) {
				string line = GetFirstNonCommentLine(sr);
				Match match = versionPattern.Match(line);
				if (!match.Success) {
					MessageService.ShowErrorFormatted("${res:SharpDevelop.Solution.InvalidSolutionFile}", newSolution.FileName);
					return false;
				}
				
				switch (match.Result("${Version}")) {
					case "7.00":
					case "8.00":
						MessageService.ShowError("${res:SharpDevelop.Solution.CannotLoadOldSolution}");
						return false;
					case "9.00":
					case "10.00":
					case "11.00":
					case "12.00":
						break;
					default:
						MessageService.ShowErrorFormatted("${res:SharpDevelop.Solution.UnknownSolutionVersion}", match.Result("${Version}"));
						return false;
				}
				
				using (AsynchronousWaitDialog waitDialog = AsynchronousWaitDialog.ShowWaitDialog("Loading solution")) {
					nestedProjectsSection = SetupSolutionLoadSolutionProjects(newSolution, sr, waitDialog);
				}
			}
			// Create solution folder 'tree'.
			if (nestedProjectsSection != null) {
				foreach (SolutionItem item in nestedProjectsSection.Items) {
					string from = item.Name;
					string to   = item.Location;
					if (newSolution.guidDictionary.ContainsKey(to) && newSolution.guidDictionary.ContainsKey(from)) {
						// ignore invalid entries
						
						ISolutionFolderContainer folder = newSolution.guidDictionary[to] as ISolutionFolderContainer;
						folder.AddFolder(newSolution.guidDictionary[from]);
					}
				}
			}
			
			if (!newSolution.ReadOnly && (newSolution.FixSolutionConfiguration(newSolution.Projects))) {
				// save in new format
				newSolution.Save();
			}
			return true;
		}
		
		static ProjectSection SetupSolutionLoadSolutionProjects(Solution newSolution, StreamReader sr, IProgressMonitor progressMonitor)
		{
			if (progressMonitor == null)
				throw new ArgumentNullException("progressMonitor");
			
			string solutionDirectory = Path.GetDirectoryName(newSolution.FileName);
			
			ProjectSection nestedProjectsSection = null;
			IList<ProjectLoadInformation> projectsToLoad = new List<ProjectLoadInformation>();
			IList<IList<ProjectSection>> readProjectSections = new List<IList<ProjectSection>>();
			
			// process the solution file contents
			while (true) {
				string line = sr.ReadLine();
				
				if (line == null) {
					break;
				}
				Match match = projectLinePattern.Match(line);
				if (match.Success) {
					string projectGuid  = match.Result("${ProjectGuid}");
					string title        = match.Result("${Title}");
					string location     = match.Result("${Location}");
					string guid         = match.Result("${Guid}");
					
					if (!FileUtility.IsUrl(location)) {
						location = FileUtility.NormalizePath(Path.Combine(solutionDirectory, location));
					}
					
					if (projectGuid == FolderGuid) {
						SolutionFolder newFolder = SolutionFolder.ReadFolder(sr, title, location, guid);
						newSolution.AddFolder(newFolder);
					} else {
						ProjectLoadInformation loadInfo = new ProjectLoadInformation(newSolution, location, title);
						loadInfo.TypeGuid = projectGuid;
						loadInfo.Guid = guid;
						projectsToLoad.Add(loadInfo);
						IList<ProjectSection> currentProjectSections = new List<ProjectSection>();
						ReadProjectSections(sr, currentProjectSections);
						readProjectSections.Add(currentProjectSections);
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
			// load projects
			for(int i=0; i<projectsToLoad.Count; i++) {
				ProjectLoadInformation loadInfo = projectsToLoad[i];
				IList<ProjectSection> projectSections = readProjectSections[i];
				loadInfo.ProjectSections = projectSections;
				
				// set the target platform
				SolutionItem projectConfig = newSolution.GetProjectConfiguration(loadInfo.Guid);
				loadInfo.Platform = AbstractProject.GetPlatformNameFromKey(projectConfig.Location);
				
				loadInfo.ProgressMonitor = progressMonitor;
				progressMonitor.Progress = (double)i / projectsToLoad.Count;
				progressMonitor.TaskName = "Loading " + loadInfo.ProjectName;
				
				using (IProgressMonitor nestedProgressMonitor = progressMonitor.CreateSubTask(1.0 / projectsToLoad.Count)) {
					loadInfo.ProgressMonitor = nestedProgressMonitor;
					IProject newProject = ProjectBindingService.LoadProject(loadInfo);
					newProject.IdGuid = loadInfo.Guid;
					newProject.ProjectSections.AddRange(projectSections);
					newSolution.AddFolder(newProject);
				}
			}
			return nestedProjectsSection;
		}
		#endregion
		
		#region Configuration/Platform management
		#region Section management
		public ProjectSection GetSolutionConfigurationsSection()
		{
			foreach (ProjectSection sec in this.Sections) {
				if (sec.Name == "SolutionConfigurationPlatforms")
					return sec;
			}
			ProjectSection newSec = new ProjectSection("SolutionConfigurationPlatforms", "preSolution");
			this.Sections.Insert(0, newSec);
			
			// convert VS 2003 solution to VS 2005 (or later)
			foreach (ProjectSection sec in this.Sections) {
				if (sec.Name == "SolutionConfiguration") {
					this.Sections.Remove(sec);
					foreach (SolutionItem item in sec.Items) {
						// item.Name = item.Location
						// might be  ConfigName.0 = Debug   (VS.NET)
						// or        Debug = Debug          (VS.NET 03)
						newSec.Items.Add(new SolutionItem(item.Location + "|x86", item.Location + "|x86"));
					}
					break;
				}
			}
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
			foreach (ProjectSection sec in this.Sections) {
				if (sec.Name == "ProjectConfiguration") {
					this.Sections.Remove(sec);
					foreach (SolutionItem item in sec.Items) {
						string name = item.Name;
						string location = item.Location;
						if (!name.Contains("|")) {
							int pos = name.LastIndexOf('.');
							if (pos > 0) {
								string firstpart = name.Substring(0, pos);
								string lastpart = name.Substring(pos);
								if (lastpart == ".0") {
									pos = firstpart.LastIndexOf('.');
									if (pos > 0) {
										lastpart = name.Substring(pos);
										firstpart = name.Substring(0, pos);
									}
								}
								name = firstpart + "|Any CPU" + lastpart;
							}
							
							pos = location.LastIndexOf('|');
							if (pos < 0) {
								location += "|Any CPU";
							} else {
								string platform = location.Substring(pos+1);
								bool found = false;
								foreach (IProject p in this.Projects) {
									if (p.PlatformNames.Contains(platform)) {
										found = true;
										break;
									}
								}
								if (!found) {
									location = location.Substring(0, pos) + "|Any CPU";
								}
							}
						}
						newSec.Items.Add(new SolutionItem(name, location));
					}
					break;
				}
			}
			return newSec;
		}
		
		public SolutionItem GetProjectConfiguration(string guid) {
			ProjectSection projectConfigSection = GetProjectConfigurationsSection();
			SolutionItem foundItem = projectConfigSection.Items.Find(item => item.Name.StartsWith(guid));
			if (foundItem != null)
				return foundItem;
			LoggingService.Warn("No configuration for project "+guid + "using default.");
			return new SolutionItem("Debug|Any CPU", "Debug|Any CPU");
		}
		
		/// <summary>
		/// Repairs the solution configuration to project configuration mapping for the specified projects.
		/// </summary>
		public bool FixSolutionConfiguration(IEnumerable<IProject> projects)
		{
			ProjectSection solSec = GetSolutionConfigurationsSection();
			ProjectSection prjSec = GetProjectConfigurationsSection();
			bool changed = false;
			var solutionConfigurations = this.GetConfigurationNames();
			var solutionPlatforms = this.GetPlatformNames();
			
			// Create configurations/platforms if none exist
			if (solutionConfigurations.Count == 0) {
				solutionConfigurations.Add("Debug");
				solutionConfigurations.Add("Release");
			}
			if (solutionPlatforms.Count == 0) {
				solutionPlatforms.Add("Any CPU");
			}
			
			// Ensure all solution configurations/platforms exist in the SolutionConfigurationPlatforms section:
			foreach (string config in solutionConfigurations) {
				foreach (string platform in solutionPlatforms) {
					string key = config + "|" + platform;
					if (!solSec.Items.Exists(item => key.Equals(item.Location, StringComparison.OrdinalIgnoreCase) && key.Equals(item.Name, StringComparison.OrdinalIgnoreCase))) {
						solSec.Items.Add(new SolutionItem(key, key));
						changed = true;
					}
				}
			}
			
			// Ensure all solution configurations/platforms are mapped to a project configuration:
			foreach (var project in projects) {
				string guid = project.IdGuid.ToUpperInvariant();
				var projectConfigurations = project.ConfigurationNames;
				var projectPlatforms = project.PlatformNames;
				foreach (string config in solutionConfigurations) {
					string projectConfig = config;
					if (!projectConfigurations.Contains(projectConfig))
						projectConfig = projectConfigurations.FirstOrDefault() ?? "Debug";
					foreach (string platform in solutionPlatforms) {
						string activeCfgKey = guid + "." + config + "|" + platform + ".ActiveCfg";
						// Only add the mapping if the ActiveCfg is not specified.
						// If ActiveCfg is specific but Build.0 isn't, we don't add the Build.0.
						if (!prjSec.Items.Exists(item => activeCfgKey.Equals(item.Name, StringComparison.OrdinalIgnoreCase))) {
							string projectPlatform = FixPlatformNameForProject(platform);
							if (!projectPlatforms.Contains(projectPlatform))
								projectPlatform = projectPlatforms.FirstOrDefault() ?? "AnyCPU";
							
							changed = true;
							CreateMatchingItem(config, platform, project, projectConfig + "|" + FixPlatformNameForSolution(projectPlatform));
						}
					}
				}
			}
			
			// remove all configuration entries belonging to removed projects
			prjSec.Items.RemoveAll(
				item => {
					if (item.Name.Contains(".")) {
						string guid = item.Name.Substring(0, item.Name.IndexOf('.'));
						if (!this.Projects.Any(p => string.Equals(p.IdGuid, guid, StringComparison.OrdinalIgnoreCase))) {
							changed = true;
							return true;
						}
					}
					return false;
				});
			return changed;
		}
		#endregion
		
		/// <summary>
		/// Removes all configurations belonging to the specified project.
		/// Is used to remove a project from the solution.
		/// </summary>
		internal void RemoveProjectConfigurations(string projectGuid)
		{
			ProjectSection prjSec = GetProjectConfigurationsSection();
			prjSec.Items.RemoveAll(
				item => {
					if (item.Name.Contains(".")) {
						string guid = item.Name.Substring(0, item.Name.IndexOf('.'));
						if (string.Equals(projectGuid, guid, StringComparison.OrdinalIgnoreCase))
							return true; // remove configuration
					}
					return false;
				});
		}
		
		#region GetProjectConfigurationsSection/GetPlatformNames
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
		#endregion
		
		#region Solution - project configuration matching
		public void ApplySolutionConfigurationAndPlatformToProjects()
		{
			foreach (ProjectConfigurationPlatformMatching l in
			         GetActiveConfigurationsAndPlatformsForProjects(preferences.ActiveConfiguration,
			                                                        preferences.ActivePlatform))
			{
				l.Project.ActiveConfiguration = l.Configuration;
				l.Project.ActivePlatform = FixPlatformNameForProject(l.Platform);
			}
			ProjectService.OnSolutionConfigurationChanged(new SolutionConfigurationEventArgs(this, preferences.ActiveConfiguration));
		}
		
		/// <summary>
		/// This is a special case in MSBuild we need to take care of.
		/// </summary>
		static string FixPlatformNameForProject(string platformName)
		{
			return MSBuildInternals.FixPlatformNameForProject(platformName);
		}
		
		/// <summary>
		/// This is a special case in MSBuild we need to take care of.
		/// Opposite of FixPlatformNameForProject
		/// </summary>
		static string FixPlatformNameForSolution(string platformName)
		{
			return MSBuildInternals.FixPlatformNameForSolution(platformName);
		}
		
		internal sealed class ProjectConfigurationPlatformMatching
		{
			public readonly IProject Project;
			public string Configuration;
			public string Platform;
			public SolutionItem SolutionItem;
			
			public ProjectConfigurationPlatformMatching(IProject project, string configuration, string platform, SolutionItem solutionItem)
			{
				this.Project = project;
				this.Configuration = configuration;
				this.Platform = platform;
				this.SolutionItem = solutionItem;
			}
			
			public void SetSolutionConfigurationPlatform(ProjectSection section, string newConfiguration, string newPlatform)
			{
				if (this.SolutionItem == null)
					return;
				string oldName = this.SolutionItem.Name;
				this.SolutionItem.Name = this.Project.IdGuid + "." + newConfiguration + "|" + newPlatform + ".Build.0";
				string newName = this.SolutionItem.Name;
				if (StripBuild0(ref oldName) && StripBuild0(ref newName)) {
					oldName += ".ActiveCfg";
					newName += ".ActiveCfg";
					foreach (SolutionItem item in section.Items) {
						if (item.Name == oldName)
							item.Name = newName;
					}
				}
			}
			
			public void SetProjectConfigurationPlatform(ProjectSection section, string newConfiguration, string newPlatform)
			{
				this.Configuration = newConfiguration;
				this.Platform = newPlatform;
				if (this.SolutionItem == null)
					return;
				this.SolutionItem.Location = newConfiguration + "|" + newPlatform;
				string thisName = this.SolutionItem.Name;
				if (StripBuild0(ref thisName)) {
					thisName += ".ActiveCfg";
					foreach (SolutionItem item in section.Items) {
						if (item.Name == thisName)
							item.Location = this.SolutionItem.Location;
					}
				}
			}
			
			internal static bool StripBuild0(ref string s)
			{
				if (s.EndsWith(".Build.0")) {
					s = s.Substring(0, s.Length - ".Build.0".Length);
					return true;
				} else {
					return false;
				}
			}
		}
		
		internal List<ProjectConfigurationPlatformMatching>
			GetActiveConfigurationsAndPlatformsForProjects(string solutionConfiguration, string solutionPlatform)
		{
			List<ProjectConfigurationPlatformMatching> results = new List<ProjectConfigurationPlatformMatching>();
			ProjectSection prjSec = GetProjectConfigurationsSection();
			Dictionary<string, SolutionItem> dict = new Dictionary<string, SolutionItem>(StringComparer.OrdinalIgnoreCase);
			foreach (SolutionItem item in prjSec.Items) {
				dict[item.Name] = item;
			}
			string searchKeyPostFix = "." + solutionConfiguration + "|" + solutionPlatform + ".ActiveCfg";
			foreach (IProject p in Projects) {
				string searchKey = p.IdGuid + searchKeyPostFix;
				SolutionItem solutionItem;
				if (dict.TryGetValue(searchKey, out solutionItem)) {
					string targetConfPlat = solutionItem.Location;
					if (targetConfPlat.IndexOf('|') > 0) {
						string conf = AbstractProject.GetConfigurationNameFromKey(targetConfPlat);
						string plat = AbstractProject.GetPlatformNameFromKey(targetConfPlat);
						results.Add(new ProjectConfigurationPlatformMatching(p, conf, plat, solutionItem));
					} else {
						results.Add(new ProjectConfigurationPlatformMatching(p, targetConfPlat, solutionPlatform, solutionItem));
					}
				} else {
					results.Add(new ProjectConfigurationPlatformMatching(p, solutionConfiguration, solutionPlatform, null));
				}
			}
			return results;
		}
		
		internal SolutionItem CreateMatchingItem(string solutionConfiguration, string solutionPlatform, IProject project, string initialLocation)
		{
			SolutionItem item = new SolutionItem(project.IdGuid + "." + solutionConfiguration + "|"
			                                     + solutionPlatform + ".Build.0", initialLocation);
			GetProjectConfigurationsSection().Items.Add(item);
			GetProjectConfigurationsSection().Items.Add(new SolutionItem(project.IdGuid + "." + solutionConfiguration + "|"
			                                                             + solutionPlatform + ".ActiveCfg", initialLocation));
			return item;
		}
		#endregion
		
		#region Rename Solution Configuration/Platform
		public void RenameSolutionConfiguration(string oldName, string newName)
		{
			foreach (string platform in GetPlatformNames()) {
				foreach (ProjectConfigurationPlatformMatching m
				         in GetActiveConfigurationsAndPlatformsForProjects(oldName, platform))
				{
					m.SetSolutionConfigurationPlatform(GetProjectConfigurationsSection(), newName, platform);
				}
			}
			foreach (SolutionItem item in GetSolutionConfigurationsSection().Items) {
				if (AbstractProject.GetConfigurationNameFromKey(item.Name) == oldName) {
					item.Name = newName + "|" + AbstractProject.GetPlatformNameFromKey(item.Name);
					item.Location = item.Name;
				}
			}
		}
		
		public void RenameSolutionPlatform(string oldName, string newName)
		{
			foreach (string configuration in GetConfigurationNames()) {
				foreach (ProjectConfigurationPlatformMatching m
				         in GetActiveConfigurationsAndPlatformsForProjects(configuration, oldName))
				{
					m.SetSolutionConfigurationPlatform(GetProjectConfigurationsSection(), configuration, newName);
				}
			}
			foreach (SolutionItem item in GetSolutionConfigurationsSection().Items) {
				if (AbstractProject.GetPlatformNameFromKey(item.Name) == oldName) {
					item.Name = AbstractProject.GetConfigurationNameFromKey(item.Name) + "|" + newName;
					item.Location = item.Name;
				}
			}
		}
		#endregion
		
		#region Rename Project Configuration/Platform
		public bool RenameProjectConfiguration(IProject project, string oldName, string newName)
		{
			IProjectAllowChangeConfigurations pacc = project as IProjectAllowChangeConfigurations;
			if (pacc == null)
				return false;
			if (!pacc.RenameProjectConfiguration(oldName, newName))
				return false;
			// project configuration was renamed successfully, adjust solution:
			foreach (SolutionItem item in GetProjectConfigurationsSection().Items) {
				if (item.Name.ToLowerInvariant().StartsWith(project.IdGuid.ToLowerInvariant())) {
					if (AbstractProject.GetConfigurationNameFromKey(item.Location) == oldName) {
						item.Location = newName + "|" + AbstractProject.GetPlatformNameFromKey(item.Location);
					}
				}
			}
			return true;
		}
		
		public bool RenameProjectPlatform(IProject project, string oldName, string newName)
		{
			IProjectAllowChangeConfigurations pacc = project as IProjectAllowChangeConfigurations;
			if (pacc == null)
				return false;
			if (!pacc.RenameProjectPlatform(FixPlatformNameForProject(oldName), FixPlatformNameForProject(newName)))
				return false;
			// project configuration was renamed successfully, adjust solution:
			foreach (SolutionItem item in GetProjectConfigurationsSection().Items) {
				if (item.Name.ToLowerInvariant().StartsWith(project.IdGuid.ToLowerInvariant())) {
					if (AbstractProject.GetPlatformNameFromKey(item.Location) == oldName) {
						item.Location = AbstractProject.GetConfigurationNameFromKey(item.Location) + "|" + newName;
					}
				}
			}
			return true;
		}
		#endregion
		
		#region Add Solution Configuration/Platform
		/// <summary>
		/// Creates a new solution configuration.
		/// </summary>
		/// <param name="newName">Name of the new configuration</param>
		/// <param name="copyFrom">Name of existing configuration to copy values from, or null</param>
		/// <param name="createInProjects">true to also create the new configuration in all projects</param>
		public void AddSolutionConfiguration(string newName, string copyFrom, bool createInProjects)
		{
			foreach (string platform in this.GetPlatformNames()) {
				AddSolutionConfigurationPlatform(newName, platform, copyFrom, createInProjects, false);
			}
		}
		
		public void AddSolutionPlatform(string newName, string copyFrom, bool createInProjects)
		{
			foreach (string configuration in this.GetConfigurationNames()) {
				AddSolutionConfigurationPlatform(configuration, newName, copyFrom, createInProjects, true);
			}
		}
		
		void AddSolutionConfigurationPlatform(string newConfiguration, string newPlatform,
		                                      string copyFrom, bool createInProjects, bool addPlatform)
		{
			List<ProjectConfigurationPlatformMatching> matchings;
			if (string.IsNullOrEmpty(copyFrom)) {
				matchings = new List<ProjectConfigurationPlatformMatching>();
			} else {
				if (addPlatform) {
					matchings = GetActiveConfigurationsAndPlatformsForProjects(newConfiguration, copyFrom);
				} else {
					matchings = GetActiveConfigurationsAndPlatformsForProjects(copyFrom, newPlatform);
				}
			}
			GetSolutionConfigurationsSection().Items.Add(new SolutionItem(newConfiguration + "|" + newPlatform,
			                                                              newConfiguration + "|" + newPlatform));
			foreach (IProject project in this.Projects) {
				// get old project configuration and platform
				string projectConfiguration, projectPlatform;
				
				ProjectConfigurationPlatformMatching matching = matchings.Find(
					delegate(ProjectConfigurationPlatformMatching m) { return m.Project == project; });
				if (matching != null) {
					projectConfiguration = matching.Configuration;
					projectPlatform = matching.Platform;
				} else {
					projectConfiguration = project.ConfigurationNames.First();
					projectPlatform = FixPlatformNameForSolution(project.PlatformNames.First());
				}
				if (createInProjects) {
					ICollection<string> existingInProject = addPlatform ? project.PlatformNames : project.ConfigurationNames;
					if (existingInProject.Contains(addPlatform ? newPlatform : newConfiguration)) {
						// target platform/configuration already exists, so reference it
						if (addPlatform) {
							projectPlatform = newPlatform;
						} else {
							projectConfiguration = newConfiguration;
						}
					} else {
						// create in project
						IProjectAllowChangeConfigurations pacc = project as IProjectAllowChangeConfigurations;
						if (pacc != null) {
							if (addPlatform) {
								if (pacc.AddProjectPlatform(FixPlatformNameForProject(newPlatform),
								                            FixPlatformNameForProject(projectPlatform))) {
									projectPlatform = newPlatform;
								}
							} else {
								if (pacc.AddProjectConfiguration(newConfiguration, projectConfiguration)) {
									projectConfiguration = newConfiguration;
								}
							}
						}
					}
				}
				
				// create item combining solution configuration with project configuration
				CreateMatchingItem(newConfiguration, newPlatform, project, projectConfiguration + "|" + projectPlatform);
			}
		}
		#endregion
		
		#region Remove Solution Configuration/Platform
		/// <summary>
		/// Gets the configuration|platform name from a conf item, e.g.
		/// "Release|Any CPU" from
		/// "{7115F3A9-781C-4A95-90AE-B5AB53C4C588}.Release|Any CPU.Build.0"
		/// </summary>
		static string GetKeyFromProjectConfItem(string name)
		{
			int pos = name.IndexOf('.');
			if (pos < 0) return null;
			name = name.Substring(pos + 1);
			if (!ProjectConfigurationPlatformMatching.StripBuild0(ref name)) {
				pos = name.LastIndexOf('.');
				if (pos < 0) return null;
				name = name.Substring(0, pos);
			}
			return name;
		}
		
		public void RemoveSolutionConfiguration(string name)
		{
			GetSolutionConfigurationsSection().Items.RemoveAll(
				delegate(SolutionItem item) {
					return AbstractProject.GetConfigurationNameFromKey(item.Name) == name;
				});
			GetProjectConfigurationsSection().Items.RemoveAll(
				delegate(SolutionItem item) {
					return AbstractProject.GetConfigurationNameFromKey(GetKeyFromProjectConfItem(item.Name)) == name;
				});
		}
		
		public void RemoveSolutionPlatform(string name)
		{
			GetSolutionConfigurationsSection().Items.RemoveAll(
				delegate(SolutionItem item) {
					return AbstractProject.GetPlatformNameFromKey(item.Name) == name;
				});
			GetProjectConfigurationsSection().Items.RemoveAll(
				delegate(SolutionItem item) {
					return AbstractProject.GetPlatformNameFromKey(GetKeyFromProjectConfItem(item.Name)) == name;
				});
		}
		#endregion
		
		#region Remove Project Configuration/Platform
		public bool RemoveProjectConfiguration(IProject project, string name)
		{
			IProjectAllowChangeConfigurations pacc = project as IProjectAllowChangeConfigurations;
			if (pacc == null)
				return false;
			if (!pacc.RemoveProjectConfiguration(name))
				return false;
			string otherConfigurationName = "other";
			foreach (string configName in project.ConfigurationNames) {
				otherConfigurationName = configName;
			}
			// project configuration was removed successfully, adjust solution:
			foreach (SolutionItem item in GetProjectConfigurationsSection().Items) {
				if (item.Name.ToLowerInvariant().StartsWith(project.IdGuid.ToLowerInvariant())) {
					if (AbstractProject.GetConfigurationNameFromKey(item.Location) == name) {
						// removed configuration was in use here, replace with other configuration
						item.Location = otherConfigurationName + "|" + AbstractProject.GetPlatformNameFromKey(item.Location);
					}
				}
			}
			return true;
		}
		
		public bool RemoveProjectPlatform(IProject project, string name)
		{
			IProjectAllowChangeConfigurations pacc = project as IProjectAllowChangeConfigurations;
			if (pacc == null)
				return false;
			if (!pacc.RemoveProjectPlatform(name))
				return false;
			string otherPlatformName = "other";
			foreach (string platformName in project.PlatformNames) {
				otherPlatformName = platformName;
			}
			// project configuration was removed successfully, adjust solution:
			foreach (SolutionItem item in GetProjectConfigurationsSection().Items) {
				if (item.Name.ToLowerInvariant().StartsWith(project.IdGuid.ToLowerInvariant())) {
					if (AbstractProject.GetPlatformNameFromKey(item.Location) == name) {
						// removed configuration was in use here, replace with other configuration
						item.Location = AbstractProject.GetConfigurationNameFromKey(item.Location) + "|" + otherPlatformName;
					}
				}
			}
			return true;
		}
		#endregion
		#endregion
		
		#region Load
		static Solution solutionBeingLoaded;
		
		public static Solution SolutionBeingLoaded {
			get {
				return solutionBeingLoaded;
			}
		}
		
		public static Solution Load(string fileName)
		{
			Solution newSolution = new Solution(new ProjectChangeWatcher(fileName));
			solutionBeingLoaded  = newSolution;
			newSolution.Name     = Path.GetFileNameWithoutExtension(fileName);
			
			string extension = Path.GetExtension(fileName).ToUpperInvariant();
			newSolution.fileName = fileName;
			newSolution.UpdateMSBuildProperties();
			newSolution.isLoading = true;
			try {
				if (!SetupSolution(newSolution)) {
					return null;
				}
			} finally {
				newSolution.isLoading = false;
			}
			
			solutionBeingLoaded = null;
			return newSolution;
		}
		
		public void UpdateMSBuildProperties()
		{
			var dict = new Dictionary<string, string>();
			AddMSBuildSolutionProperties(dict);
			foreach (var pair in dict) {
				MSBuildProjectCollection.SetGlobalProperty(pair.Key, pair.Value);
			}
		}
		
		public void AddMSBuildSolutionProperties(IDictionary<string, string> propertyDict)
		{
			propertyDict["SolutionDir"] = EnsureBackslash(this.Directory);
			propertyDict["SolutionExt"] = ".sln";
			propertyDict["SolutionFileName"] = Path.GetFileName(this.FileName);
			propertyDict["SolutionName"] = this.Name ?? string.Empty;
			propertyDict["SolutionPath"] = this.FileName;
		}
		
		static string EnsureBackslash(string path)
		{
			if (path.EndsWith("\\", StringComparison.Ordinal))
				return path;
			else
				return path + "\\";
		}
		
		#endregion
		
		#region System.IDisposable interface implementation
		public void Dispose()
		{
			changeWatcher.Dispose();
			foreach (IProject project in Projects) {
				project.Dispose();
			}
		}
		#endregion
		
		#region Building
		ICollection<IBuildable> IBuildable.GetBuildDependencies(ProjectBuildOptions buildOptions)
		{
			List<IBuildable> result = new List<IBuildable>();
			foreach (IProject p in this.Projects)
				result.Add(p);
			return result;
		}
		
		void IBuildable.StartBuild(ProjectBuildOptions buildOptions, IBuildFeedbackSink feedbackSink)
		{
			// building a solution finishes immediately: we only care for the dependencies
			feedbackSink.Done(true);
		}
		
		ProjectBuildOptions IBuildable.CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
		{
			return null;
		}
		#endregion
		
		public override string ToString()
		{
			return "[Solution: FileName=" + (this.FileName ?? "<null>") +
				", HasProjects=" + this.HasProjects.ToString(System.Globalization.CultureInfo.InvariantCulture) +
				", ReadOnly=" + this.ReadOnly.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
		}
	}
}
