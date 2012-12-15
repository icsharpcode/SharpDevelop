// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public static class ProjectService
	{
		static Solution openSolution;
		volatile static IProject currentProject;
		
		public static Solution OpenSolution {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return openSolution;
			}
		}
		
		/// <summary>
		/// Gets/Sets the active project.
		/// Returns null if no project is active.
		/// The getter is thread-safe; the setter may only be called on the main thread.
		/// </summary>
		public static IProject CurrentProject {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return currentProject;
			}
			set {
				WorkbenchSingleton.AssertMainThread();
				if (currentProject != value) {
					LoggingService.Info("CurrentProject changed to " + (value == null ? "null" : value.Name));
					currentProject = value;
					OnCurrentProjectChanged(new ProjectEventArgs(currentProject));
					CommandManager.InvalidateRequerySuggested();
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
			if (!initialized) {
				initialized = true;
				WorkbenchSingleton.Workbench.ActiveViewContentChanged += ActiveViewContentChanged;
				FileService.FileRenamed += FileServiceFileRenamed;
				FileService.FileRemoved += FileServiceFileRemoved;
				ApplicationStateInfoService.RegisterStateGetter("ProjectService.OpenSolution", delegate { return OpenSolution; });
				ApplicationStateInfoService.RegisterStateGetter("ProjectService.CurrentProject", delegate { return CurrentProject; });
			}
		}

		/// <summary>
		/// Returns if a project loader exists for the given file. This method works even in early
		/// startup (before service initialization)
		/// </summary>
		public static bool HasProjectLoader(string fileName)
		{
			AddInTreeNode addinTreeNode = AddInTree.GetTreeNode("/SharpDevelop/Workbench/Combine/FileFilter");
			foreach (Codon codon in addinTreeNode.Codons) {
				string pattern = codon.Properties.Get("extensions", "");
				if (FileUtility.MatchesPattern(fileName, pattern) && codon.Properties.Contains("class")) {
					return true;
				}
			}
			return false;
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
				MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new StringTagPair("FileName", fileName)));
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
					IProjectItemListProvider provider = project as IProjectItemListProvider;
					if (provider != null) {
						foreach (ProjectItem item in provider.Items.ToArray()) {
							if (FileUtility.IsBaseDirectory(fileName, item.FileName)) {
								provider.RemoveProjectItem(item);
								OnProjectItemRemoved(new ProjectItemEventArgs(project, item));
							}
						}
					}
				}
			}
		}
		
		static void ActiveViewContentChanged(object sender, EventArgs e)
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (OpenSolution == null || viewContent == null) {
				return;
			}
			string fileName = viewContent.PrimaryFileName;
			if (fileName == null) {
				return;
			}
			CurrentProject = OpenSolution.FindProjectContainingFile(fileName) ?? CurrentProject;
		}
		
		public static void AddProject(ISolutionFolderNode solutionFolderNode, IProject newProject)
		{
			if (solutionFolderNode.Solution.SolutionFolders.Any(
				folder => string.Equals(folder.IdGuid, newProject.IdGuid, StringComparison.OrdinalIgnoreCase)))
			{
				LoggingService.Warn("ProjectService.AddProject: Duplicate IdGuid detected");
				newProject.IdGuid = Guid.NewGuid().ToString().ToUpperInvariant();
			}
			solutionFolderNode.Container.AddFolder(newProject);
			ParserService.CreateProjectContentForAddedProject(newProject);
			OnProjectAdded(new ProjectEventArgs(newProject));
		}
		
		/// <summary>
		/// Adds a project item to the project, raising the ProjectItemAdded event.
		/// Make sure you call project.Save() after adding new items!
		/// </summary>
		public static void AddProjectItem(IProject project, ProjectItem item)
		{
			if (project == null) throw new ArgumentNullException("project");
			if (item == null)    throw new ArgumentNullException("item");
			IProjectItemListProvider provider = project as IProjectItemListProvider;
			if (provider != null) {
				provider.AddProjectItem(item);
				OnProjectItemAdded(new ProjectItemEventArgs(project, item));
			}
		}
		
		/// <summary>
		/// Removes a project item from the project, raising the ProjectItemRemoved event.
		/// Make sure you call project.Save() after removing items!
		/// No action (not even raising the event) is taken when the item was already removed form the project.
		/// </summary>
		public static void RemoveProjectItem(IProject project, ProjectItem item)
		{
			if (project == null) throw new ArgumentNullException("project");
			if (item == null)    throw new ArgumentNullException("item");
			IProjectItemListProvider provider = project as IProjectItemListProvider;
			if (provider != null) {
				if (provider.RemoveProjectItem(item)) {
					OnProjectItemRemoved(new ProjectItemEventArgs(project, item));
				}
			}
		}
		
		static void BeforeLoadSolution()
		{
			if (openSolution != null && !IsClosingCanceled()) {
				SaveSolutionPreferences();
				WorkbenchSingleton.Workbench.CloseAllViews();
				CloseSolution();
			}
		}
		
		public static void LoadSolution(string fileName)
		{
			FileUtility.ObservedLoad(LoadSolutionInternal, fileName);
		}
		
		static void LoadSolutionInternal(string fileName)
		{
			if (!Path.IsPathRooted(fileName))
				throw new ArgumentException("Path must be rooted!");
			
			if (IsClosingCanceled())
				return;
			
			BeforeLoadSolution();
			OnSolutionLoading(fileName);
			try {
				openSolution = Solution.Load(fileName);
				CommandManager.InvalidateRequerySuggested();
				if (openSolution == null)
					return;
			} catch (IOException ex) {
				LoggingService.Warn(ex);
				MessageService.ShowError(ex.Message);
				return;
			} catch (UnauthorizedAccessException ex) {
				LoggingService.Warn(ex);
				MessageService.ShowError(ex.Message);
				return;
			}
			AbstractProject.filesToOpenAfterSolutionLoad.Clear();
			try {
				string file = GetPreferenceFileName(openSolution.FileName);
				Properties properties = null;
				if (FileUtility.IsValidPath(file) && File.Exists(file)) {
					try {
						properties = Properties.Load(file);
					} catch (IOException) {
					} catch (UnauthorizedAccessException) {
					} catch (XmlException) {
						// ignore errors about inaccessible or malformed files
					}
				}
				(openSolution.Preferences as IMementoCapable).SetMemento(properties ?? new Properties());
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
			try {
				ApplyConfigurationAndReadPreferences();
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
			// Create project contents for solution
			ParserService.OnSolutionLoaded();
			
			Project.Converter.UpgradeViewContent.ShowIfRequired(openSolution);
			
			// preferences must be read before OnSolutionLoad is called to enable
			// the event listeners to read e.Solution.Preferences.Properties
			OnSolutionLoaded(new SolutionEventArgs(openSolution));
		}
		
		internal static void ParserServiceCreatedProjectContents()
		{
			foreach (string file in AbstractProject.filesToOpenAfterSolutionLoad) {
				if (File.Exists(file)) {
					FileService.OpenFile(file);
				}
			}
			AbstractProject.filesToOpenAfterSolutionLoad.Clear();
		}
		
		static void ApplyConfigurationAndReadPreferences()
		{
			openSolution.ApplySolutionConfigurationAndPlatformToProjects();
			foreach (IProject project in openSolution.Projects) {
				string file = GetPreferenceFileName(project.FileName);
				if (FileUtility.IsValidPath(file) && File.Exists(file)) {
					Properties properties = null;
					try {
						properties = Properties.Load(file);
					} catch (IOException) {
					} catch (UnauthorizedAccessException) {
					} catch (XmlException) {
						// ignore errors about inaccessible or malformed files
					}
					if (properties != null)
						project.SetMemento(properties);
				}
			}
		}
		
		/// <summary>
		/// Load a single project as solution.
		/// </summary>
		public static void LoadProject(string fileName)
		{
			FileUtility.ObservedLoad(LoadProjectInternal, fileName);
		}
		
		static void LoadProjectInternal(string fileName)
		{
			if (!Path.IsPathRooted(fileName))
				throw new ArgumentException("Path must be rooted!");
			string solutionFile = Path.ChangeExtension(fileName, ".sln");
			if (File.Exists(solutionFile)) {
				LoadSolutionInternal(solutionFile);
				
				if (openSolution != null) {
					bool found = false;
					foreach (IProject p in openSolution.Projects) {
						if (FileUtility.IsEqualFileName(fileName, p.FileName)) {
							found = true;
							break;
						}
					}
					if (found == false) {
						var parseArgs = new[] { new StringTagPair("SolutionName", Path.GetFileName(solutionFile)), new StringTagPair("ProjectName", Path.GetFileName(fileName))};
						int res = MessageService.ShowCustomDialog(MessageService.ProductName,
						                                          StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.SolutionDoesNotContainProject}", parseArgs),
						                                          0, 2,
						                                          StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.SolutionDoesNotContainProject.AddProjectToSolution}", parseArgs),
						                                          StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.SolutionDoesNotContainProject.CreateNewSolution}", parseArgs),
						                                          "${res:Global.IgnoreButtonText}");
						if (res == 0) {
							// Add project to solution
							Commands.AddExistingProjectToSolution.AddProject((ISolutionFolderNode)ProjectBrowserPad.Instance.SolutionNode, fileName);
							SaveSolution();
							return;
						} else if (res == 1) {
							CloseSolution();
							try {
								File.Copy(solutionFile, Path.ChangeExtension(solutionFile, ".old.sln"), true);
							} catch (IOException){}
						} else {
							// ignore, just open the solution
							return;
						}
					} else {
						// opened solution instead and correctly found the project
						return;
					}
				} else {
					// some problem during opening, abort
					return;
				}
			}
			Solution solution = new Solution(new ProjectChangeWatcher(solutionFile));
			solution.Name = Path.GetFileNameWithoutExtension(fileName);
			IProjectBinding binding = ProjectBindingService.GetBindingPerProjectFile(fileName);
			IProject project;
			if (binding != null) {
				project = ProjectBindingService.LoadProject(new ProjectLoadInformation(solution, fileName, solution.Name));
				if (project is UnknownProject) {
					if (((UnknownProject)project).WarningDisplayedToUser == false) {
						((UnknownProject)project).ShowWarningMessageBox();
					}
					return;
				}
			} else {
				MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new StringTagPair("FileName", fileName)));
				return;
			}
			solution.AddFolder(project);
			
			if (FileUtility.ObservedSave((NamedFileOperationDelegate)solution.Save, solutionFile) == FileOperationResult.OK) {
				// only load when saved succesfully
				LoadSolution(solutionFile);
			}
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
		
		/// <summary>
		/// Gets the list of file filters.
		/// </summary>
		public static IList<FileFilterDescriptor> GetFileFilters()
		{
			return AddInTree.BuildItems<FileFilterDescriptor>("/SharpDevelop/Workbench/FileFilter", null);
		}
		
		/// <summary>
		/// Returns a File Dialog filter that can be used to filter on all registered project formats
		/// </summary>
		public static string GetAllProjectsFilter(object caller, bool includeSolutions)
		{
			var filters = AddInTree.BuildItems<FileFilterDescriptor>("/SharpDevelop/Workbench/Combine/FileFilter", null);
			if (!includeSolutions)
				filters.RemoveAll(f => f.ContainsExtension(".sln"));
			StringBuilder b = new StringBuilder(StringParser.Parse("${res:SharpDevelop.Solution.AllKnownProjectFormats}|"));
			bool first = true;
			foreach (var filter in filters) {
				string ext = filter.Extensions;
				if (ext != "*.*" && ext.Length > 0) {
					if (!first) {
						b.Append(';');
					} else {
						first = false;
					}
					b.Append(ext);
				}
			}
			foreach (var filter in filters) {
				b.Append('|');
				b.Append(filter.ToString());
			}
			return b.ToString();
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
			if (FileUtility.IsValidPath(fullFileName)) {
				#if DEBUG
				memento.Save(fullFileName);
				#else
				FileUtility.ObservedSave(new NamedFileOperationDelegate(memento.Save), fullFileName, FileErrorPolicy.Inform);
				#endif
			}
			
			foreach (IProject project in OpenSolution.Projects) {
				memento = project.CreateMemento();
				if (memento == null) continue;
				
				fullFileName = GetPreferenceFileName(project.FileName);
				if (FileUtility.IsValidPath(fullFileName)) {
					#if DEBUG
					memento.Save(fullFileName);
					#else
					FileUtility.ObservedSave(new NamedFileOperationDelegate(memento.Save), fullFileName, FileErrorPolicy.Inform);
					#endif
				}
			}
		}
		
		/// <summary>
		/// Executes the OnBeforeSolutionClosing event.
		/// </summary>
		/// <remarks>This method must be used after CloseSolution is called.</remarks>
		/// <returns><c>true</c>, if closing solution was canceled; <c>false</c>, otherwise.</returns>
		internal static bool IsClosingCanceled()
		{
			// run onbefore closing
			var beforeClosingArgs = new SolutionCancelEventArgs(openSolution);
			OnBeforeSolutionClosing(beforeClosingArgs);
			
			return beforeClosingArgs.Cancel;
		}
		
		/// <summary>
		/// Closes the solution: cancels build, clears solution data, fires the SolutionClosing and SolutionClosed events.
		/// <remarks>
		/// 	Before invoking this method, one should check if the closing was canceled (<see cref="IsClosingCanceled"/>),
		/// 	save solution and project data (e.g. files, bookmarks), then invoke CloseSolution().
		/// </remarks>
		/// </summary>
		internal static void CloseSolution()
		{
			// If a build is running, cancel it.
			// If we would let a build run but unload the MSBuild projects, the next project.StartBuild call
			// could cause an exception.
			BuildEngine.CancelGuiBuild();
			
			if (openSolution != null) {
				CurrentProject = null;
				OnSolutionClosing(new SolutionEventArgs(openSolution));
				
				openSolution.Dispose();
				openSolution = null;
				ParserService.OnSolutionClosed();
				
				OnSolutionClosed(EventArgs.Empty);
				CommandManager.InvalidateRequerySuggested();
			}
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
		
		static void OnBeforeSolutionClosing(SolutionCancelEventArgs e)
		{
			if (BeforeSolutionClosing != null) {
				BeforeSolutionClosing(null, e);
			}
		}
		
		static void OnSolutionLoading(string fileName)
		{
			if (SolutionLoading != null) {
				SolutionLoading(fileName, EventArgs.Empty);
			}
		}
		
		static void OnSolutionLoaded(SolutionEventArgs e)
		{
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
		
		internal static void OnSolutionConfigurationChanged(SolutionConfigurationEventArgs e)
		{
			if (SolutionConfigurationChanged != null) {
				SolutionConfigurationChanged(null, e);
			}
		}
		
		static bool building;
		
		public static bool IsBuilding {
			get {
				return building;
			}
		}
		
		/// <summary>
		/// Raises the <see cref="BuildStarted"/> event.
		/// 
		/// You do not need to call this method if you use BuildEngine.BuildInGui - the build
		/// engine will call these events itself.
		/// </summary>
		public static void RaiseEventBuildStarted(BuildEventArgs e)
		{
			if (e == null)
				throw new ArgumentNullException("e");
			WorkbenchSingleton.AssertMainThread();
			building = true;
			BuildStarted.RaiseEvent(null, e);
		}
		
		/// <summary>
		/// Raises the <see cref="BuildFinished"/> event.
		/// 
		/// You do not need to call this method if you use BuildEngine.BuildInGui - the build
		/// engine will call these events itself.
		/// </summary>
		public static void RaiseEventBuildFinished(BuildEventArgs e)
		{
			if (e == null)
				throw new ArgumentNullException("e");
			WorkbenchSingleton.AssertMainThread();
			building = false;
			BuildFinished.RaiseEvent(null, e);
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
					HandleRemovedSolutionFolder(folder);
					break;
				}
			}
		}
		
		static void HandleRemovedSolutionFolder(ISolutionFolder folder)
		{
			IProject project = folder as IProject;
			if (project != null) {
				OpenSolution.RemoveProjectConfigurations(project.IdGuid);
				ParserService.RemoveProjectContentForRemovedProject(project);
				OnProjectRemoved(new ProjectEventArgs(project));
				project.Dispose();
			}
			if (folder is ISolutionFolderContainer) {
				// recurse into child folders that were also removed
				((ISolutionFolderContainer)folder).Folders.ForEach(HandleRemovedSolutionFolder);
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
		static void OnProjectAdded(ProjectEventArgs e)
		{
			if (ProjectAdded != null) {
				ProjectAdded(null, e);
			}
		}
		static void OnProjectRemoved(ProjectEventArgs e)
		{
			if (ProjectRemoved != null) {
				ProjectRemoved(null, e);
			}
		}
		internal static void OnProjectCreated(ProjectEventArgs e)
		{
			if (ProjectCreated != null) {
				ProjectCreated(null, e);
			}
		}
		internal static void OnSolutionCreated(SolutionEventArgs e)
		{
			if (SolutionCreated != null) {
				SolutionCreated(null, e);
			}
		}
		
		/// <summary>
		/// Is raised when a new project is created.
		/// </summary>
		public static event ProjectEventHandler ProjectCreated;
		/// <summary>
		/// Is raised when a new or existing project is added to the solution.
		/// </summary>
		public static event ProjectEventHandler ProjectAdded;
		/// <summary>
		/// Is raised when a project is removed from the solution.
		/// </summary>
		public static event ProjectEventHandler ProjectRemoved;
		
		/// <summary>
		/// Is raised when a solution folder is removed from the solution.
		/// This might remove multiple projects from the solution.
		/// </summary>
		public static event SolutionFolderEventHandler SolutionFolderRemoved;
		
		public static event EventHandler<BuildEventArgs> BuildStarted;
		public static event EventHandler<BuildEventArgs> BuildFinished;
		
		public static event SolutionConfigurationEventHandler SolutionConfigurationChanged;
		
		public static event EventHandler<SolutionEventArgs> SolutionCreated;
		
		public static event EventHandler                    SolutionLoading;
		public static event EventHandler<SolutionEventArgs> SolutionLoaded;
		public static event EventHandler<SolutionEventArgs> SolutionSaved;
		
		public static event EventHandler<SolutionEventArgs> SolutionClosing;
		public static event EventHandler                    SolutionClosed;
		
		/// <summary>
		/// Raised before SolutionClosing.
		/// <remarks>
		/// When one modifies the e.Cancel property, should have in mind that other consumers might want to cancel the closing.<br/>
		/// Setting e.Cancel = false might override other consumers (if they exist) e.Cancel = true, and might break other functionalities.
		/// </remarks>
		/// </summary>
		public static event EventHandler<SolutionCancelEventArgs> BeforeSolutionClosing;
		
		/// <summary>
		/// Raised before the solution preferences are being saved. Allows you to save
		/// your additional properties in the solution preferences.
		/// </summary>
		public static event EventHandler<SolutionEventArgs> SolutionPreferencesSaving;
		
		public static event ProjectEventHandler CurrentProjectChanged;
		
		public static event EventHandler<ProjectItemEventArgs> ProjectItemAdded;
		public static event EventHandler<ProjectItemEventArgs> ProjectItemRemoved;
	}
	
	public class BuildEventArgs : EventArgs
	{
		/// <summary>
		/// The project/solution to be built.
		/// </summary>
		public readonly IBuildable Buildable;
		
		/// <summary>
		/// The build options.
		/// </summary>
		public readonly BuildOptions Options;
		
		/// <summary>
		/// Gets the build results.
		/// This property is null for build started events.
		/// </summary>
		public readonly BuildResults Results;
		
		public BuildEventArgs(IBuildable buildable, BuildOptions options)
			: this(buildable, options, null)
		{
		}
		
		public BuildEventArgs(IBuildable buildable, BuildOptions options, BuildResults results)
		{
			if (buildable == null)
				throw new ArgumentNullException("buildable");
			if (options == null)
				throw new ArgumentNullException("options");
			this.Buildable = buildable;
			this.Options = options;
			this.Results = results;
		}
	}
}
