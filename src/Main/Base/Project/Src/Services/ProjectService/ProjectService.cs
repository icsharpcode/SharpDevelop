// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
		static Solution openSolution   = null;
		static IProject currentProject = null;
		
		public static Solution OpenSolution {
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
					currentProject = value;
					OnCurrentProjectChanged(new ProjectEventArgs(currentProject));
				}
			}
		}
		
		public static IProject GetProject(string filename)
		{
			filename = Path.GetFullPath(filename).ToLower();
			foreach (IProject project in OpenSolution.Projects) {
				if (project.FileName.ToLower() == filename) {
					return project;
				}
			}
			return null;
		}
		
		static ProjectService()
		{
			if (WorkbenchSingleton.Workbench != null) {
				WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += new EventHandler(ActiveWindowChanged);
			}
			FileService.FileRenamed += FileServiceFileRenamed;
			FileService.FileRemoved += FileServiceFileRemoved;
		}
		
		public static bool IsSolutionExtension(string ext)
		{
			return ext.Equals(".SLN", StringComparison.OrdinalIgnoreCase)
				|| ext.Equals(".CMBX", StringComparison.OrdinalIgnoreCase)
				|| ext.Equals(".PRJX", StringComparison.OrdinalIgnoreCase);
			// prjx converter is called by Solution.Load, so treat prjx as solution
		}
		
		public static bool IsProjectExtension(string ext)
		{
			return ext.Equals(".CSPROJ", StringComparison.OrdinalIgnoreCase)
				|| ext.Equals(".VBPROJ", StringComparison.OrdinalIgnoreCase)
				|| ext.Equals(".ILPROJ", StringComparison.OrdinalIgnoreCase);
		}
		
		public static void LoadSolutionOrProject(string fileName)
		{
			string ext = Path.GetExtension(fileName);
			if (ProjectService.IsSolutionExtension(ext))
				ProjectService.LoadSolution(fileName);
			else if (ProjectService.IsProjectExtension(ext))
				ProjectService.LoadProject(fileName);
			else
				MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new string[,] {{"FileName", fileName}}));
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
							item.FileName = FileUtility.RenameBaseDirectory(item.FileName, oldName, newName);
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
						} else {
							++i;
						}
					}
				}
			}
		}
		
		static void ActiveWindowChanged(object sender, EventArgs e)
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveContent as IViewContent;
			if (OpenSolution == null || viewContent == null) {
				return;
			}
			string fileName = viewContent.FileName;
			if (fileName == null) {
				return;
			}
			foreach (IProject project in OpenSolution.Projects) {
				if (project.IsFileInProject(fileName)) {
					CurrentProject = project;
					break;
				}
			}
		}
		
		/// <summary>
		/// Adds a project item to the project, raising the ProjectItemAdded event.
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
				CloseSolution();
			}
		}
		
		public static void LoadSolution(string fileName)
		{
			BeforeLoadSolution();
			openSolution = Solution.Load(fileName);
			if (openSolution == null)
				return;
			OnSolutionLoaded(new SolutionEventArgs(openSolution));
			try {
				string file = GetPreferenceFileName(openSolution.FileName);
				if (FileUtility.IsValidFileName(file) && File.Exists(file)) {
					openSolution.Preferences.SetMemento(Properties.Load(file));
				}
				foreach (IProject project in openSolution.Projects) {
					file = GetPreferenceFileName(project.FileName);
					if (FileUtility.IsValidFileName(file) && File.Exists(file)) {
						project.SetMemento(Properties.Load(file));
					}
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
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
				project = binding.LoadProject(fileName, solution.Name);
			} else {
				MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new string[,] {{"FileName", fileName}}));
				return;
			}
			solution.AddFolder(project);
			solution.Save(solutionFile);
			openSolution = solution;
			OnSolutionLoaded(new SolutionEventArgs(openSolution));
			string file = GetPreferenceFileName(project.FileName);
			if (FileUtility.IsValidFileName(file) && File.Exists(file)) {
				project.SetMemento(Properties.Load(file));
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
		
		static string GetPreferenceFileName(string projectFileName)
		{
			string directory = PropertyService.ConfigDirectory + "preferences";
			string fileName = projectFileName.Substring(3).Replace('/', '.').Replace('\\', '.').Replace(Path.DirectorySeparatorChar, '.');
			string fullFileName = Path.Combine(directory, fileName + ".xml");
			return fullFileName;
		}
		
		public static void SaveSolutionPreferences()
		{
			if (openSolution == null)
				return;
			string directory = PropertyService.ConfigDirectory + "preferences";
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
			}
			
			string fullFileName;
			Properties memento = openSolution.Preferences.CreateMemento();
			if (memento != null) {
				fullFileName = GetPreferenceFileName(openSolution.FileName);
				
				if (FileUtility.IsValidFileName(fullFileName)) {
					FileUtility.ObservedSave(new NamedFileOperationDelegate(memento.Save), fullFileName, FileErrorPolicy.Inform);
				}
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
		
		public static event ProjectEventHandler CurrentProjectChanged;
		
		public static event EventHandler<ProjectItemEventArgs> ProjectItemAdded;
		public static event EventHandler<ProjectItemEventArgs> ProjectItemRemoved;
	}
}
