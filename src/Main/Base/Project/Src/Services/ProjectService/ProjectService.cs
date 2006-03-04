// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public static class ProjectService
	{
		static Solution openSolution;
		static IProject currentProject;
		
		public static Solution OpenSolution {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return openSolution;
			}
		}
		
		public static IProject CurrentProject {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return currentProject;
			}
			set {
				if (currentProject != value) {
					LoggingService.Info("CurrentProject changed to " + (value == null ? "null" : value.Name));
					currentProject = value;
					OnCurrentProjectChanged(new ProjectEventArgs(currentProject));
				}
			}
		}
		
		/// <summary>
		/// Gets an open project by the name of the project file.
		/// </summary>
		public static IProject GetProject(string projectFilename)
		{
			if (openSolution == null) return null;
			foreach (IProject project in openSolution.Projects) {
				if (FileUtility.IsEqualFileName(project.FileName, projectFilename)) {
					return project;
				}
			}
			return null;
		}
		
		static bool initialized;
		
		public static void InitializeService()
		{
			if (initialized)
				throw new InvalidOperationException("ProjectService already is initialized");
			initialized = true;
			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += ActiveWindowChanged;
			FileService.FileRenamed += FileServiceFileRenamed;
			FileService.FileRemoved += FileServiceFileRemoved;
		}

		public static IProjectLoader GetProjectLoader(string fileName)
		{
			AddInTreeNode addinTreeNode = AddInTree.GetTreeNode("/SharpDevelop/Workbench/Combine/FileFilter");
			foreach (Codon codon in addinTreeNode.Codons) {
				string pattern = codon.Properties.Get("extensions", "");
				if (FileUtility.MatchesPattern(fileName, pattern) && codon.Properties.Contains("class")) {
					object binding = codon.AddIn.CreateObject(codon.Properties["class"]);
					return binding as IProjectLoader;
				}
			}
			return null;
		}

		public static void LoadSolutionOrProject(string fileName)
		{
			IProjectLoader loader = GetProjectLoader(fileName);
			if (loader != null)	{
				loader.Load(fileName);
			} else {
				MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new string[,] {{"FileName", fileName}}));
			}
		}
		
		static void FileServiceFileRenamed(object sender, FileRenameEventArgs e)
		{
			if (OpenSolution == null) {
				return;
			}
			string oldName = e.SourceFile;
			string newName = e.TargetFile;
			long x = 0;
			foreach (ISolutionFolderContainer container in OpenSolution.SolutionFolderContainers) {
				foreach (SolutionItem item in container.SolutionItems.Items) {
					string oldFullName  = Path.Combine(OpenSolution.Directory, item.Name);
					++x;
					if (FileUtility.IsBaseDirectory(oldName, oldFullName)) {
						string newFullName = FileUtility.RenameBaseDirectory(oldFullName, oldName, newName);
						item.Name = item.Location = FileUtility.GetRelativePath(OpenSolution.Directory, newFullName);
					}
				}
			}
			
			long y = 0;
			foreach (IProject project in OpenSolution.Projects) {
				if (FileUtility.IsBaseDirectory(project.Directory, oldName)) {
					foreach (ProjectItem item in project.Items) {
						++y;
						if (FileUtility.IsBaseDirectory(oldName, item.FileName)) {
							OnProjectItemRemoved(new ProjectItemEventArgs(project, item));
							item.FileName = FileUtility.RenameBaseDirectory(item.FileName, oldName, newName);
							OnProjectItemAdded(new ProjectItemEventArgs(project, item));
						}
					}
				}
			}
		}
		
		static void FileServiceFileRemoved(object sender, FileEventArgs e)
		{
			if (OpenSolution == null) {
				return;
			}
			string fileName = e.FileName;
			
			foreach (ISolutionFolderContainer container in OpenSolution.SolutionFolderContainers) {
				for (int i = 0; i < container.SolutionItems.Items.Count;) {
					SolutionItem item = container.SolutionItems.Items[i];
					if (FileUtility.IsBaseDirectory(fileName, Path.Combine(OpenSolution.Directory, item.Name))) {
						container.SolutionItems.Items.RemoveAt(i);
					} else {
						++i;
					}
				}
			}
			
			foreach (IProject project in OpenSolution.Projects) {
				if (FileUtility.IsBaseDirectory(project.Directory, fileName)) {
					for (int i = 0; i < project.Items.Count;) {
						ProjectItem item =project.Items[i];
						if (FileUtility.IsBaseDirectory(fileName, item.FileName)) {
							project.Items.RemoveAt(i);
							OnProjectItemRemoved(new ProjectItemEventArgs(project, item));
						} else {
							++i;
						}
					}
				}
			}
		}
		
		static void ActiveWindowChanged(object sender, EventArgs e)
		{
			object activeContent = WorkbenchSingleton.Workbench.ActiveContent;
			IViewContent viewContent = activeContent as IViewContent;
			if (viewContent == null && activeContent is ISecondaryViewContent) {
				// required if one creates a new winforms app and then immediately switches to design mode
				// without focussing the text editor
				IWorkbenchWindow window = ((ISecondaryViewContent)activeContent).WorkbenchWindow;
				if (window == null) // workbench window is being disposed
					return;
				viewContent = window.ViewContent;
			}
			if (OpenSolution == null || viewContent == null) {
				return;
			}
			string fileName = viewContent.FileName;
			if (fileName == null) {
				return;
			}
			CurrentProject = OpenSolution.FindProjectContainingFile(fileName) ?? CurrentProject;
		}
		
		/// <summary>
		/// Adds a project item to the project, raising the ProjectItemAdded event.
		/// Make sure you call project.Save() after adding new items!
		/// </summary>
		public static void AddProjectItem(IProject project, ProjectItem item)
		{
			if (project == null) throw new ArgumentNullException("project");
			if (item == null)    throw new ArgumentNullException("item");
			project.Items.Add(item);
			OnProjectItemAdded(new ProjectItemEventArgs(project, item));
		}
		
		/// <summary>
		/// Removes a project item from the project, raising the ProjectItemRemoved event.
		/// Make sure you call project.Save() after removing items!
		/// </summary>
		public static void RemoveProjectItem(IProject project, ProjectItem item)
		{
			if (project == null) throw new ArgumentNullException("project");
			if (item == null)    throw new ArgumentNullException("item");
			if (!project.Items.Remove(item)) {
				throw new ArgumentException("The item was not found in the project!");
			}
			OnProjectItemRemoved(new ProjectItemEventArgs(project, item));
		}
		
		static void BeforeLoadSolution()
		{
			if (openSolution != null) {
				SaveSolutionPreferences();
				WorkbenchSingleton.Workbench.CloseAllViews();
				CloseSolution();
			}
		}
		
		public static void LoadSolution(string fileName)
		{
			BeforeLoadSolution();
			openSolution = Solution.Load(fileName);
			if (openSolution == null)
				return;
			try {
				string file = GetPreferenceFileName(openSolution.FileName);
				if (FileUtility.IsValidFileName(file) && File.Exists(file)) {
					(openSolution.Preferences as IMementoCapable).SetMemento(Properties.Load(file));
				}
				ApplyConfigurationAndReadPreferences();
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
			// preferences must be read before OnSolutionLoad is called to enable
			// the event listeners to read e.Solution.Preferences.Properties
			OnSolutionLoaded(new SolutionEventArgs(openSolution));
		}
		
		static void ApplyConfigurationAndReadPreferences()
		{
			openSolution.ApplySolutionConfigurationToProjects();
			openSolution.ApplySolutionPlatformToProjects();
			foreach (IProject project in openSolution.Projects) {
				string file = GetPreferenceFileName(project.FileName);
				if (FileUtility.IsValidFileName(file) && File.Exists(file)) {
					project.SetMemento(Properties.Load(file));
				}
			}
		}
		
		/// <summary>
		/// Load a single project as solution.
		/// </summary>
		public static void LoadProject(string fileName)
		{
			BeforeLoadSolution();
			string solutionFile = Path.ChangeExtension(fileName, ".sln");
			if (File.Exists(solutionFile)) {
				LoadSolution(solutionFile);
				return;
			}
			Solution solution = new Solution();
			solution.Name = Path.GetFileNameWithoutExtension(fileName);
			ILanguageBinding binding = LanguageBindingService.GetBindingPerProjectFile(fileName);
			IProject project;
			if (binding != null) {
				project = LanguageBindingService.LoadProject(fileName, solution.Name);
			} else {
				MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new string[,] {{"FileName", fileName}}));
				return;
			}
			solution.AddFolder(project);
			ProjectSection configSection = solution.GetSolutionConfigurationsSection();
			foreach (string configuration in project.GetConfigurationNames()) {
				foreach (string platform in project.GetPlatformNames()) {
					string key = configuration + "|" + platform;
					configSection.Items.Add(new SolutionItem(key, key));
				}
			}
			solution.FixSolutionConfiguration(new IProject[] { project });
			solution.Save(solutionFile);
			
			openSolution = solution;
			ApplyConfigurationAndReadPreferences();
			// preferences must be read before OnSolutionLoad is called to enable
			// the event listeners to read e.Solution.Preferences.Properties
			OnSolutionLoaded(new SolutionEventArgs(openSolution));
		}
		
		public static void SaveSolution()
		{
			if (openSolution != null) {
				openSolution.Save();
				foreach (IProject project in openSolution.Projects) {
					project.Save();
				}
				OnSolutionSaved(new SolutionEventArgs(openSolution));
			}
		}
		
		static string GetPreferenceFileName(string projectFileName)
		{
			string directory = Path.Combine(PropertyService.ConfigDirectory, "preferences");
			return Path.Combine(directory,
			                    Path.GetFileName(projectFileName)
			                    + "." + projectFileName.ToLowerInvariant().GetHashCode().ToString("x")
			                    + ".xml");
		}
		
		public static void SaveSolutionPreferences()
		{
			if (openSolution == null)
				return;
			string directory = Path.Combine(PropertyService.ConfigDirectory, "preferences");
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
			}
			
			if (SolutionPreferencesSaving != null)
				SolutionPreferencesSaving(null, new SolutionEventArgs(openSolution));
			Properties memento = (openSolution.Preferences as IMementoCapable).CreateMemento();
			
			string fullFileName = GetPreferenceFileName(openSolution.FileName);
			if (FileUtility.IsValidFileName(fullFileName)) {
				FileUtility.ObservedSave(new NamedFileOperationDelegate(memento.Save), fullFileName, FileErrorPolicy.Inform);
			}
			
			foreach (IProject project in OpenSolution.Projects) {
				memento = project.CreateMemento();
				if (memento == null) continue;
				
				fullFileName = GetPreferenceFileName(project.FileName);
				if (FileUtility.IsValidFileName(fullFileName)) {
					FileUtility.ObservedSave(new NamedFileOperationDelegate(memento.Save), fullFileName, FileErrorPolicy.Inform);
				}
			}
		}
		
		public static void CloseSolution()
		{
			if (openSolution != null) {
				CurrentProject = null;
				OnSolutionClosing(new SolutionEventArgs(openSolution));
				
				openSolution.Dispose();
				openSolution = null;
				
				OnSolutionClosed(EventArgs.Empty);
			}
		}
		
		public static void MarkFileDirty(string fileName)
		{
			if (OpenSolution != null) {
				foreach (IProject project in OpenSolution.Projects) {
					if (project.IsFileInProject(fileName)) {
						MarkProjectDirty(project);
					}
				}
			}
		}
		
		public static void MarkProjectDirty(IProject project)
		{
			project.IsDirty = true;
		}
		
		static void OnCurrentProjectChanged(ProjectEventArgs e)
		{
			if (CurrentProjectChanged != null) {
				CurrentProjectChanged(null, e);
			}
		}
		
		static void OnSolutionClosed(EventArgs e)
		{
			if (SolutionClosed != null) {
				SolutionClosed(null, e);
			}
		}
		
		static void OnSolutionClosing(SolutionEventArgs e)
		{
			if (SolutionClosing != null) {
				SolutionClosing(null, e);
			}
		}
		
		static void OnSolutionLoaded(SolutionEventArgs e)
		{
			ParserService.OnSolutionLoaded();
			if (SolutionLoaded != null) {
				SolutionLoaded(null, e);
			}
		}
		
		static void OnSolutionSaved(SolutionEventArgs e)
		{
			if (SolutionSaved != null) {
				SolutionSaved(null, e);
			}
		}
		
		static void OnProjectConfigurationChanged(ProjectConfigurationEventArgs e)
		{
			if (ProjectConfigurationChanged != null) {
				ProjectConfigurationChanged(null, e);
			}
		}
		
		static void OnSolutionConfigurationChanged(SolutionConfigurationEventArgs e)
		{
			if (SolutionConfigurationChanged != null) {
				SolutionConfigurationChanged(null, e);
			}
		}
		
		static void OnStartBuild(EventArgs e)
		{
			if (StartBuild != null) {
				StartBuild(null, e);
			}
		}
		
		public static void OnEndBuild()
		{
			OnEndBuild(new EventArgs());
		}
		
		static void OnEndBuild(EventArgs e)
		{
			if (EndBuild != null) {
				EndBuild(null, e);
			}
		}
		
		public static void RemoveSolutionFolder(string guid)
		{
			if (OpenSolution == null) {
				return;
			}
			foreach (ISolutionFolder folder in OpenSolution.SolutionFolders) {
				if (folder.IdGuid == guid) {
					folder.Parent.RemoveFolder(folder);
					OnSolutionFolderRemoved(new SolutionFolderEventArgs(folder));
					break;
				}
			}
		}
		
		static void OnSolutionFolderRemoved(SolutionFolderEventArgs e)
		{
			if (SolutionFolderRemoved != null) {
				SolutionFolderRemoved(null, e);
			}
		}
		
		static void OnProjectItemAdded(ProjectItemEventArgs e)
		{
			if (ProjectItemAdded != null) {
				ProjectItemAdded(null, e);
			}
		}
		static void OnProjectItemRemoved(ProjectItemEventArgs e)
		{
			if (ProjectItemRemoved != null) {
				ProjectItemRemoved(null, e);
			}
		}
		
		public static event SolutionFolderEventHandler SolutionFolderRemoved;
		
		public static event EventHandler StartBuild;
		public static event EventHandler EndBuild;
		
		public static event ProjectConfigurationEventHandler ProjectConfigurationChanged;
		public static event SolutionConfigurationEventHandler SolutionConfigurationChanged;
		
		public static event EventHandler<SolutionEventArgs> SolutionLoaded;
		public static event EventHandler<SolutionEventArgs> SolutionSaved;
		
		public static event EventHandler<SolutionEventArgs> SolutionClosing;
		public static event EventHandler                    SolutionClosed;
		
		/// <summary>
		/// Raised before the solution preferences are being saved. Allows you to save
		/// your additional properties in the solution preferences.
		/// </summary>
		public static event EventHandler<SolutionEventArgs> SolutionPreferencesSaving;
		
		public static event ProjectEventHandler CurrentProjectChanged;
		
		public static event EventHandler<ProjectItemEventArgs> ProjectItemAdded;
		public static event EventHandler<ProjectItemEventArgs> ProjectItemRemoved;
	}
}
