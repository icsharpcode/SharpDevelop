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
using System.Diagnostics;
using System.IO;
using System.Linq;
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
			projectBindings = SD.AddInTree.BuildItems<ProjectBindingDescriptor>("/SharpDevelop/Workbench/ProjectBindings", null);
			targetFrameworks = SD.AddInTree.BuildItems<TargetFramework>("/SharpDevelop/TargetFrameworks", null);
			
			SD.GetFutureService<IWorkbench>().ContinueWith(t => t.Result.ActiveViewContentChanged += ActiveViewContentChanged).FireAndForget();
			
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
			FileUtility.ObservedLoad(OpenSolutionOrProjectInternal, fileName);
			
			return currentSolution != null;
		}
		
		void OpenSolutionOrProjectInternal(FileName fileName)
		{
			if (fileName.HasExtension(".sln")) {
				OpenSolutionInternal(fileName);
			} else {
				OpenProjectInternal(fileName);
			}
		}
		
		void OpenSolutionInternal(FileName fileName)
		{
			ISolution solution;
			using (var progress = AsynchronousWaitDialog.ShowWaitDialog("Loading Solution...")) {
				
				solution = LoadSolutionFile(fileName, progress);
				
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
			if (solution == null)
				throw new ArgumentNullException("solution");
			if (!CloseSolution(allowCancel: true))
				return false;
			this.CurrentSolution = solution;
			OnSolutionOpened(solution);
			return true;
		}
		
		void OnSolutionOpened(ISolution solution)
		{
			foreach (var project in solution.Projects)
				project.ProjectLoaded();
			SolutionOpened(this, new SolutionEventArgs(solution));
			SD.FileService.RecentOpen.AddRecentProject(solution.FileName);
			Project.Converter.UpgradeViewContent.ShowIfRequired(solution);
			foreach (var project in solution.Projects.OfType<ErrorProject>()) {
				var error = project.Exception as ProjectLoadException;
				if (error != null && error.CanShowDialog) {
					error.ShowDialog();
					break; // show at most 1 dialog
				}
			}
		}
		
		void OpenProjectInternal(FileName fileName)
		{
			if (!Path.IsPathRooted(fileName))
				throw new ArgumentException("Path must be rooted!");
			FileName solutionFile = new FileName(Path.ChangeExtension(fileName, ".sln"));
			ISolution solution = null;
			bool solutionOpened = false;
			// Use try-finally block to dispose the solution unless it is opened successfully. 
			try {
				if (SD.FileSystem.FileExists(solutionFile)) {
					using (var progress = AsynchronousWaitDialog.ShowWaitDialog("Loading Solution...")) {
						solution = LoadSolutionFile(solutionFile, progress);
					}
					// If LoadSolutionFile() throws ProjectLoadException, let that be handled by the ObservedLoad.
					
					if (solution.Projects.Any(p => p.FileName == fileName)) {
						// We can use the solution as-is
						OpenSolution(solution);
						solutionOpened = true;
						return;
					}
					// The solution does not contain the project, ask the user on how to proceed:
					var parseArgs = new[] { new StringTagPair("SolutionName", Path.GetFileName(solutionFile)), new StringTagPair("ProjectName", Path.GetFileName(fileName)) };
					int res = MessageService.ShowCustomDialog(MessageService.ProductName,
						StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.SolutionDoesNotContainProject}", parseArgs),
						0, 2,
						StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.SolutionDoesNotContainProject.AddProjectToSolution}", parseArgs),
						StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.SolutionDoesNotContainProject.CreateNewSolution}", parseArgs),
						"${res:Global.IgnoreButtonText}");
					if (res == 0) {
						// Fall-through: the code below will add the project to the solution
					} else if (res == 1) {
						// Create new solution: first make a backup of the old one
						try {
							File.Copy(solutionFile, Path.ChangeExtension(solutionFile, ".old.sln"), true);
						} catch (IOException) {
						}
						// Replace solution with an empty one:
						solution.Dispose();
						solution = CreateEmptySolutionFile(solutionFile);
						// Fall-through: the code below will add the project to the solution
					} else {
						// ignore, just open the solution as-is
						OpenSolution(solution);
						solutionOpened = true;
						return;
					}
				} else {
					// Solution does not already exist; create a new one
					solution = CreateEmptySolutionFile(solutionFile);
				}
				solution.AddExistingProject(fileName);
				solution.Save();
				OpenSolution(solution);
				solutionOpened = true;
			} finally {
				// Dispose the solution if it was not opened successfully
				if (solution != null && !solutionOpened)
					solution.Dispose();
			}
		}
		#endregion
		
		#region CloseSolution
		public event EventHandler<SolutionClosingEventArgs> SolutionClosing = delegate { };
		public event EventHandler<SolutionEventArgs> SolutionClosed = delegate { };
		
		public bool CloseSolution(bool allowCancel = true)
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
		
		#region Project Bindings
		readonly IReadOnlyList<ProjectBindingDescriptor> projectBindings;
		
		public IReadOnlyList<ProjectBindingDescriptor> ProjectBindings {
			get { return projectBindings; }
		}
		
		public IProject LoadProject(ProjectLoadInformation info)
		{
			if (info == null)
				throw new ArgumentNullException("info");
			info.ProgressMonitor.CancellationToken.ThrowIfCancellationRequested();
			ProjectBindingDescriptor descriptor = null;
			if (info.TypeGuid != Guid.Empty) {
				descriptor = projectBindings.FirstOrDefault(b => b.TypeGuid == info.TypeGuid);
			}
			if (descriptor == null) {
				string extension = info.FileName.GetExtension();
				if (extension.Equals(".proj", StringComparison.OrdinalIgnoreCase) || extension.Equals(".build", StringComparison.OrdinalIgnoreCase))
					return new MSBuildFileProject(info);
				descriptor = projectBindings.FirstOrDefault(b => extension.Equals(b.ProjectFileExtension, StringComparison.OrdinalIgnoreCase));
			}
			if (descriptor == null)
				throw new ProjectLoadException(SD.ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.ProjectBrowser.NoBackendForProjectType"));
			
			// Set type GUID based on file extension
			info.TypeGuid = descriptor.TypeGuid;
			IProjectBinding binding = descriptor.Binding;
			if (binding == null)
				throw new ProjectLoadException(SD.ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.ProjectBrowser.NoBackendForProjectType"));
			if (!binding.HandlingMissingProject && !SD.FileSystem.FileExists(info.FileName))
				throw new FileNotFoundException("Project file not found", info.FileName);
			var result = binding.LoadProject(info);
			if (result == null)
				throw new InvalidOperationException("IProjectBinding.LoadProject() must not return null");
			return result;
		}
		#endregion
		
		#region Target Frameworks
		readonly IReadOnlyList<TargetFramework> targetFrameworks;
		
		public IReadOnlyList<TargetFramework> TargetFrameworks {
			get { return targetFrameworks; }
		}
		#endregion
	}
}
