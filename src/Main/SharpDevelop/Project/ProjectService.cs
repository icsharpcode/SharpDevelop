// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;
using Microsoft.Build.Exceptions;

namespace ICSharpCode.SharpDevelop.Project
{
	sealed class SDProjectService : IProjectService, IProjectServiceRaiseEvents
	{
		public SDProjectService()
		{
			allSolutions = new NullSafeSimpleModelCollection<ISolution>();
			allProjects = allSolutions.SelectMany(s => s.Projects);
			
			SD.GetFutureService<IWorkbench>().ContinueWith(t => t.Result.ActiveViewContentChanged += ActiveViewContentChanged);
			
			var applicationStateInfoService = SD.GetService<ApplicationStateInfoService>();
			if (applicationStateInfoService != null) {
				applicationStateInfoService.RegisterStateGetter("ProjectService.CurrentSolution", delegate { return CurrentSolution; });
				applicationStateInfoService.RegisterStateGetter("ProjectService.CurrentProject", delegate { return CurrentProject; });
			}
			
			SD.Services.AddService(typeof(IProjectServiceRaiseEvents), this);
		}
		
		#region CurrentSolution property + AllProjects collection
		volatile static ISolution currentSolution;
		readonly SimpleModelCollection<ISolution> allSolutions;
		readonly IModelCollection<IProject> allProjects;
		
		public event PropertyChangedEventHandler<ISolution> CurrentSolutionChanged = delegate { };
		
		public ISolution CurrentSolution {
			[DebuggerStepThrough]
			get { return currentSolution; }
			private set {
				SD.MainThread.VerifyAccess();
				var oldValue = currentSolution;
				if (oldValue != value) {
					currentSolution = value;
					if (oldValue != null)
						allSolutions.Remove(oldValue);
					CurrentSolutionChanged(this, new PropertyChangedEventArgs<ISolution>(oldValue, value));
					if (value != null)
						allSolutions.Add(value);
					CommandManager.InvalidateRequerySuggested();
					SD.ParserService.InvalidateCurrentSolutionSnapshot();
				}
			}
		}
		
		public IModelCollection<IProject> AllProjects {
			get { return allProjects; }
		}
		#endregion
		
		#region CurrentProject property
		volatile static IProject currentProject;
		public event PropertyChangedEventHandler<IProject> CurrentProjectChanged = delegate { };
		
		public IProject CurrentProject {
			[DebuggerStepThrough]
			get { return currentProject; }
			set {
				SD.MainThread.VerifyAccess();
				var oldValue = currentProject;
				if (oldValue != value) {
					LoggingService.Info("CurrentProject changed to " + (value == null ? "null" : value.Name));
					currentProject = value;
					CurrentProjectChanged(this, new PropertyChangedEventArgs<IProject>(oldValue, value));
					CommandManager.InvalidateRequerySuggested();
				}
			}
		}
		
		void ActiveViewContentChanged(object sender, EventArgs e)
		{
			IViewContent viewContent = SD.Workbench.ActiveViewContent;
			if (currentSolution == null || viewContent == null) {
				return;
			}
			FileName fileName = viewContent.PrimaryFileName;
			if (fileName == null) {
				return;
			}
			IProject project = FindProjectContainingFile(fileName);
			if (project != null)
				CurrentProject = project;
		}
		#endregion
		
		#region FindProjectContainingFile
		public IProject FindProjectContainingFile(FileName fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			IProject currentProject = this.CurrentProject;
			if (currentProject != null && currentProject.IsFileInProject(fileName))
				return currentProject;
			
			ISolution solution = this.CurrentSolution;
			if (solution == null)
				return null;
			// Try all project's in the solution.
			IProject linkedProject = null;
			foreach (IProject project in solution.Projects) {
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
		#endregion
		
		#region OpenSolutionOrProject
		public event EventHandler<SolutionEventArgs> SolutionOpened = delegate {};
		
		public bool OpenSolutionOrProject(FileName fileName)
		{
			if (!IsSolutionOrProjectFile(fileName)) {
				MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new StringTagPair("FileName", fileName)));
				return false;
			}
			if (!CloseSolution(allowCancel: true))
				return false;
			FileUtility.ObservedLoad(OpenSolutionInternal, fileName);
				
			return currentSolution != null;
		}
		
		void OpenSolutionInternal(FileName fileName)
		{
			ISolution solution;
			using (var progress = AsynchronousWaitDialog.ShowWaitDialog("Loading Solution...")) {
				
				solution = LoadSolutionFile(fileName, progress);
				//(openSolution.Preferences as IMementoCapable).SetMemento(solutionProperties);
				
//				try {
//					ApplyConfigurationAndReadProjectPreferences();
//				} catch (Exception ex) {
//					MessageService.ShowException(ex);
//				}
				
				this.CurrentSolution = solution;
			}
			OnSolutionOpened(solution);
		}
		
		public bool OpenSolution(FileName fileName)
		{
			if (!CloseSolution(allowCancel: true))
				return false;
			FileUtility.ObservedLoad(OpenSolutionInternal, fileName);
			return currentSolution != null;
		}
		
		public bool OpenSolution(ISolution solution)
		{
			if (!CloseSolution(allowCancel: true))
				return false;
			this.CurrentSolution = solution;
			OnSolutionOpened(solution);
			return true;
		}
		
		void OnSolutionOpened(ISolution solution)
		{
			SolutionOpened(this, new SolutionEventArgs(solution));
			foreach (var project in solution.Projects)
				project.ProjectLoaded();
			SD.FileService.RecentOpen.AddRecentProject(solution.FileName);
			Project.Converter.UpgradeViewContent.ShowIfRequired(solution);
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
		#endregion
		
		#region CloseSolution
		public event EventHandler<SolutionClosingEventArgs> SolutionClosing = delegate { };
		public event EventHandler<SolutionEventArgs> SolutionClosed = delegate { };
		
		public bool CloseSolution(bool allowCancel)
		{
			SD.MainThread.VerifyAccess();
			var solution = this.CurrentSolution;
			if (solution == null)
				return true;
			
			var cancelEventArgs = new SolutionClosingEventArgs(solution, allowCancel);
			SolutionClosing(this, cancelEventArgs);
			if (allowCancel && cancelEventArgs.Cancel)
				return false;
			
			foreach (var project in solution.Projects)
				project.SavePreferences();
			solution.SavePreferences();
			
			if (!SD.Workbench.CloseAllSolutionViews(force: !allowCancel))
				return false;
			
			// If a build is running, cancel it.
			// If we would let a build run but unload the MSBuild projects, the next project.StartBuild call
			// could cause an exception.
			SD.BuildService.CancelBuild();
			
			CurrentProject = null;
			
			this.CurrentSolution = null; // this will fire the CurrentSolutionChanged event
			SolutionClosed(this, new SolutionEventArgs(solution));
			solution.Dispose();
			return true;
		}
		#endregion
		
		#region IsSolutionOrProjectFile
		public bool IsSolutionOrProjectFile(FileName fileName)
		{
			AddInTreeNode addinTreeNode = SD.AddInTree.GetTreeNode("/SharpDevelop/Workbench/Combine/FileFilter");
			foreach (Codon codon in addinTreeNode.Codons) {
				string pattern = codon.Properties.Get("extensions", "*.*");
				if (pattern != "*.*" && FileUtility.MatchesPattern(fileName, pattern)) {
					return true;
				}
			}
			return false;
		}
		#endregion
		
		#region LoadSolutionFile + CreateEmptySolutionFile
		public ISolution LoadSolutionFile(FileName fileName, IProgressMonitor progress)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (progress == null)
				throw new ArgumentNullException("progress");
			if (fileName.IsRelative)
				throw new ArgumentException("Path must be rooted!");
			Solution solution = new Solution(fileName, new ProjectChangeWatcher(fileName), SD.FileService);
			bool ok = false;
			try {
				using (var loader = new SolutionLoader(fileName)) {
					loader.ReadSolution(solution, progress);
				}
				ok = true;
			} finally {
				if (!ok)
					solution.Dispose();
			}
			return solution;
		}
		
		public ISolution CreateEmptySolutionFile(FileName fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (fileName.IsRelative)
				throw new ArgumentException("Path must be rooted!");
			Solution solution = new Solution(fileName, new ProjectChangeWatcher(fileName), SD.FileService);
			solution.LoadPreferences();
			return solution;
		}
		#endregion
		
		#region IProjectServiceRaiseEvents
		public event EventHandler<ProjectEventArgs> ProjectCreated = delegate { };
		public event EventHandler<SolutionEventArgs> SolutionCreated = delegate { };
		public event EventHandler<ProjectItemEventArgs> ProjectItemAdded = delegate { };
		public event EventHandler<ProjectItemEventArgs> ProjectItemRemoved = delegate { };
		
		void IProjectServiceRaiseEvents.RaiseProjectCreated(ProjectEventArgs e)
		{
			ProjectCreated(this, e);
		}

		void IProjectServiceRaiseEvents.RaiseSolutionCreated(SolutionEventArgs e)
		{
			SolutionCreated(this, e);
		}
		
		void IProjectServiceRaiseEvents.RaiseProjectItemAdded(ProjectItemEventArgs e)
		{
			ProjectItemAdded(this, e);
		}

		void IProjectServiceRaiseEvents.RaiseProjectItemRemoved(ProjectItemEventArgs e)
		{
			ProjectItemRemoved(this, e);
		}
		#endregion
	}
}
