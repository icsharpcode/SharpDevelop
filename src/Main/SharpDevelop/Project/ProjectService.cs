// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	sealed class SDProjectService : IProjectService
	{
		public SDProjectService()
		{
//			SD.Workbench.ActiveViewContentChanged += ActiveViewContentChanged;
//			FileService.FileRenamed += FileServiceFileRenamed;
//			FileService.FileRemoved += FileServiceFileRemoved;
			
			var applicationStateInfoService = SD.GetService<ApplicationStateInfoService>();
			if (applicationStateInfoService != null) {
				applicationStateInfoService.RegisterStateGetter("ProjectService.CurrentSolution", delegate { return CurrentSolution; });
				applicationStateInfoService.RegisterStateGetter("ProjectService.CurrentProject", delegate { return CurrentProject; });
			}
		}
		
		#region CurrentSolution property + AllProjects collection
		volatile static ISolution currentSolution;
		ConcatModelCollection<IProject> allProjects = new ConcatModelCollection<IProject>();
		
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
						allProjects.Inputs.Remove(oldValue.Projects);
					CurrentSolutionChanged(this, new PropertyChangedEventArgs<ISolution>(oldValue, value));
					if (value != null)
						allProjects.Inputs.Add(value.Projects);
					CommandManager.InvalidateRequerySuggested();
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
		public void OpenSolutionOrProject(FileName fileName)
		{
			if (!IsProjectOrSolutionFile(fileName)) {
				MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new StringTagPair("FileName", fileName)));
				return;
			}
			if (!CloseSolution(allowCancel: true))
				return;
			FileUtility.ObservedLoad(OpenSolutionOrProjectInternal, fileName);
		}
		
		void OpenSolutionOrProjectInternal(FileName fileName)
		{
			using (var progress = AsynchronousWaitDialog.ShowWaitDialog("Loading Solution")) {
				
				ISolution solution = LoadSolutionFile(fileName, progress);
				throw new NotImplementedException();
			}
		}
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
		
		#region IsProjectOrSolutionFile
		public bool IsProjectOrSolutionFile(FileName fileName)
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
			Solution solution = new Solution(fileName, new ProjectChangeWatcher(fileName), SD.FileService);
			solution.LoadPreferences();
			return solution;
		}
		#endregion
	}
}
