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
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Project
{
	public static class ProjectService
	{
		public static ISolution OpenSolution {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return SD.ProjectService.OpenSolution;
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
				return SD.ProjectService.CurrentProject;
			}
			set {
				SD.ProjectService.CurrentProject = value;
			}
		}
		
		/// <summary>
		/// Gets an open project by the name of the project file.
		/// </summary>
		public static IProject GetProject(FileName projectFilename)
		{
			ISolution sln = SD.ProjectService.OpenSolution;
			if (sln == null)
				return null;
			foreach (IProject project in sln.Projects) {
				if (project.FileName == projectFilename) {
					return project;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Returns if a project loader exists for the given file. This method works even in early
		/// startup (before service initialization)
		/// </summary>
		[Obsolete("Use SD.ProjectService.IsProjectOrSolutionFile instead")]
		public static bool HasProjectLoader(string fileName)
		{
			return SD.ProjectService.IsProjectOrSolutionFile(FileName.Create(fileName));
		}
		
		/*
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
		}*/
		
		public static void LoadSolutionOrProject(string fileName)
		{
			SD.ProjectService.OpenSolutionOrProject(FileName.Create(fileName));
			/*IProjectLoader loader = GetProjectLoader(fileName);
			if (loader != null)	{
				loader.Load(fileName);
			} else {
				MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new StringTagPair("FileName", fileName)));
			}*/
		}
		
		static void FileServiceFileRenamed(object sender, FileRenameEventArgs e)
		{
			if (OpenSolution == null) {
				return;
			}
			string oldName = e.SourceFile;
			string newName = e.TargetFile;
			foreach (ISolutionFileItem fileItem in OpenSolution.AllItems.OfType<ISolutionFileItem>().ToArray()) {
				if (FileUtility.IsBaseDirectory(oldName, fileItem.FileName)) {
					string newFullName = FileUtility.RenameBaseDirectory(fileItem.FileName, oldName, newName);
					fileItem.FileName = FileName.Create(newFullName);
				}
			}
			
			foreach (IProject project in OpenSolution.Projects) {
				if (FileUtility.IsBaseDirectory(project.Directory, oldName)) {
					foreach (ProjectItem item in project.Items) {
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
			
			foreach (ISolutionFileItem fileItem in OpenSolution.AllItems.OfType<ISolutionFileItem>().ToArray()) {
				if (FileUtility.IsBaseDirectory(fileName, fileItem.FileName)) {
					fileItem.ParentFolder.Items.Remove(fileItem);
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
			IViewContent viewContent = SD.Workbench.ActiveViewContent;
			if (OpenSolution == null || viewContent == null) {
				return;
			}
			FileName fileName = viewContent.PrimaryFileName;
			if (fileName == null) {
				return;
			}
			CurrentProject = SD.ProjectService.FindProjectContainingFile(fileName) ?? CurrentProject;
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
		
		/*
		static void BeforeLoadSolution()
		{
			if (openSolution != null && !IsClosingCanceled()) {
				SaveSolutionPreferences();
				SD.Workbench.CloseAllViews();
				CloseSolution();
			}
		}
		*/
		
		[Obsolete("Use SD.ProjectService.OpenSolutionOrProject() instead")]
		public static void LoadSolution(string fileName)
		{
			SD.ProjectService.OpenSolutionOrProject(FileName.Create(fileName));
			//FileUtility.ObservedLoad(LoadSolutionInternal, fileName);
		}
		/*
		static void LoadSolutionInternal(string fileName)
		{
			if (!Path.IsPathRooted(fileName))
				throw new ArgumentException("Path must be rooted!");
			
			if (IsClosingCanceled())
				return;
			
			BeforeLoadSolution();
			OnSolutionLoading(fileName);
			var solutionProperties = LoadSolutionPreferences(fileName);
			try {
				openSolution = ISolution.Load(fileName, solutionProperties["ActiveConfiguration"], solutionProperties["ActivePlatform"]);
				CommandManager.InvalidateRequerySuggested();
				SD.ParserService.InvalidateCurrentSolutionSnapshot();
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
			(openSolution.Preferences as IMementoCapable).SetMemento(solutionProperties);
			
			try {
				ApplyConfigurationAndReadProjectPreferences();
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
			SD.ParserService.InvalidateCurrentSolutionSnapshot();
			SD.FileService.RecentOpen.AddRecentProject(openSolution.FileName);
			
			Project.Converter.UpgradeViewContent.ShowIfRequired(openSolution);
			
			// preferences must be read before OnSolutionLoad is called to enable
			// the event listeners to read e.Solution.Preferences.Properties
			OnSolutionLoaded(new SolutionEventArgs(openSolution));
		}
		
		static Properties LoadSolutionPreferences(string solutionFileName)
		{
			try {
				string file = GetPreferenceFileName(solutionFileName);
				if (FileUtility.IsValidPath(file) && File.Exists(file)) {
					try {
						return Properties.Load(file);
					} catch (IOException) {
					} catch (UnauthorizedAccessException) {
					} catch (XmlException) {
						// ignore errors about inaccessible or malformed files
					}
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
			return new Properties();
		}
		
		static void ApplyConfigurationAndReadProjectPreferences()
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
		 */
		
		/// <summary>
		/// Load a single project as solution.
		/// </summary>
		[Obsolete("Use SD.ProjectService.OpenSolutionOrProject() instead")]
		public static void LoadProject(string fileName)
		{
			SD.ProjectService.OpenSolutionOrProject(FileName.Create(fileName));
			//FileUtility.ObservedLoad(LoadProjectInternal, fileName);
		}
		/*
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
							Commands.AddExistingProjectToSolution.AddProject((ISolutionItemNode)ProjectBrowserPad.Instance.SolutionNode, FileName.Create(fileName));
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
			ISolution solution = new Solution(new ProjectChangeWatcher(solutionFile));
			solution.Name = Path.GetFileNameWithoutExtension(fileName);
			IProjectBinding binding = ProjectBindingService.GetBindingPerProjectFile(fileName);
			IProject project;
			if (binding != null) {
				project = ProjectBindingService.LoadProject(new ProjectLoadInformation(solution, FileName.Create(fileName), solution.Name));
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
		}*/
		
		public static void SaveSolution()
		{
			if (SD.ProjectService.OpenSolution != null) {
				SD.ProjectService.OpenSolution.Save();
				/*	foreach (IProject project in openSolution.Projects) {
					project.Save();
				}
				OnSolutionSaved(new SolutionEventArgs(openSolution));*/
			}
		}
		
		/// <summary>
		/// Gets the list of file filters.
		/// </summary>
		public static IReadOnlyList<FileFilterDescriptor> GetFileFilters()
		{
			return AddInTree.BuildItems<FileFilterDescriptor>("/SharpDevelop/Workbench/FileFilter", null);
		}
		
		/// <summary>
		/// Returns a File Dialog filter that can be used to filter on all registered project formats
		/// </summary>
		public static string GetAllProjectsFilter(object caller, bool includeSolutions)
		{
			IEnumerable<FileFilterDescriptor> filters = AddInTree.BuildItems<FileFilterDescriptor>("/SharpDevelop/Workbench/Combine/FileFilter", null);
			if (!includeSolutions)
				filters = filters.Where(f => !f.ContainsExtension(".sln"));
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
		
		internal static string GetPreferenceFileName(string projectFileName)
		{
			string directory = Path.Combine(PropertyService.ConfigDirectory, "preferences");
			return Path.Combine(directory,
			                    Path.GetFileName(projectFileName)
			                    + "." + projectFileName.ToLowerInvariant().GetHashCode().ToString("x")
			                    + ".xml");
		}
		
		public static void SaveSolutionPreferences()
		{
			throw new NotImplementedException();
			/*if (openSolution == null)
				return;
			string directory = Path.Combine(PropertyService.ConfigDirectory, "preferences");
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
			}
			
			if (SolutionPreferencesSaving != null)
				SolutionPreferencesSaving(null, new SolutionEventArgs(openSolution));
			Properties memento = openSolution.Preferences.Clone();
			
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
			}*/
		}
		
		/// <summary>
		/// Executes the OnBeforeSolutionClosing event.
		/// </summary>
		/// <remarks>This method must be used after CloseSolution is called.</remarks>
		/// <returns><c>true</c>, if closing solution was canceled; <c>false</c>, otherwise.</returns>
		internal static bool IsClosingCanceled()
		{
			// run onbefore closing
			var beforeClosingArgs = new SolutionCancelEventArgs(OpenSolution);
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
			throw new NotImplementedException();
			/*// If a build is running, cancel it.
			// If we would let a build run but unload the MSBuild projects, the next project.StartBuild call
			// could cause an exception.
			SD.BuildService.CancelBuild();
			
			if (openSolution != null) {
				CurrentProject = null;
				OnSolutionClosing(new SolutionEventArgs(openSolution));
				
				openSolution.Dispose();
				openSolution = null;
				
				OnSolutionClosed(EventArgs.Empty);
				CommandManager.InvalidateRequerySuggested();
			}*/
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
		
		[Obsolete]
		public static bool IsBuilding {
			get {
				return SD.BuildService.IsBuilding;
			}
		}
		
		/*
		public static void RemoveSolutionFolder(string guid)
		{
			if (OpenSolution == null) {
				return;
			}
			foreach (ISolutionItem folder in OpenSolution.SolutionFolders) {
				if (folder.IdGuid == guid) {
					folder.Parent.RemoveFolder(folder);
					OnSolutionFolderRemoved(new SolutionFolderEventArgs(folder));
					HandleRemovedSolutionFolder(folder);
					break;
				}
			}
		}
		
		static void HandleRemovedSolutionFolder(ISolutionItem folder)
		{
			IProject project = folder as IProject;
			if (project != null) {
				OpenSolution.RemoveProjectConfigurations(project.IdGuid);
				OnProjectRemoved(new ProjectEventArgs(project));
				project.Dispose();
			}
			if (folder is ISolutionFolder) {
				// recurse into child folders that were also removed
				((ISolutionFolder)folder).Folders.ForEach(HandleRemovedSolutionFolder);
			}
		}
		
		static void OnSolutionFolderRemoved(SolutionFolderEventArgs e)
		{
			if (SolutionFolderRemoved != null) {
				SolutionFolderRemoved(null, e);
			}
		}
		 */
		
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
		public static event EventHandler<ProjectEventArgs> ProjectCreated;
		/// <summary>
		/// Is raised when a new or existing project is added to the solution.
		/// </summary>
		public static event EventHandler<ProjectEventArgs> ProjectAdded;
		/// <summary>
		/// Is raised when a project is removed from the solution.
		/// </summary>
		public static event EventHandler<ProjectEventArgs> ProjectRemoved;
		
		/// <summary>
		/// Is raised when a solution folder is removed from the solution.
		/// This might remove multiple projects from the solution.
		/// </summary>
		public static event SolutionFolderEventHandler SolutionFolderRemoved;
		
		[Obsolete("Use SD.BuildService.BuildStarted instead")]
		public static event EventHandler<BuildEventArgs> BuildStarted {
			add { SD.BuildService.BuildStarted += value; }
			remove { SD.BuildService.BuildStarted -= value; }
		}
		[Obsolete("Use SD.BuildService.BuildFinished instead")]
		public static event EventHandler<BuildEventArgs> BuildFinished {
			add { SD.BuildService.BuildFinished += value; }
			remove { SD.BuildService.BuildFinished -= value; }
		}
		
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
		
		public static event EventHandler<ProjectEventArgs> CurrentProjectChanged {
			add { SD.ProjectService.CurrentProjectChanged += value; }
			remove { SD.ProjectService.CurrentProjectChanged -= value; }
		}
		
		public static event EventHandler<ProjectItemEventArgs> ProjectItemAdded;
		public static event EventHandler<ProjectItemEventArgs> ProjectItemRemoved;
	}
}
