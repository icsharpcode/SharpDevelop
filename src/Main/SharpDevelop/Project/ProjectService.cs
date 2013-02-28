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
				applicationStateInfoService.RegisterStateGetter("ProjectService.OpenSolution", delegate { return OpenSolution; });
				applicationStateInfoService.RegisterStateGetter("ProjectService.CurrentProject", delegate { return CurrentProject; });
			}
		}
		
		volatile static ISolution openSolution;
		volatile static IProject currentProject;
		
		public event PropertyChangedEventHandler<ISolution> OpenSolutionChanged = delegate { };
		public event PropertyChangedEventHandler<IProject> CurrentProjectChanged = delegate { };
		
		public ISolution OpenSolution {
			[DebuggerStepThrough]
			get { return openSolution; }
			private set {
				SD.MainThread.VerifyAccess();
				var oldValue = openSolution;
				if (oldValue != value) {
					openSolution = value;
					OpenSolutionChanged(this, new PropertyChangedEventArgs<ISolution>("OpenSolution", oldValue, value));
					CommandManager.InvalidateRequerySuggested();
				}
			}
		}
		
		public IProject CurrentProject {
			[DebuggerStepThrough]
			get { return currentProject; }
			set {
				SD.MainThread.VerifyAccess();
				var oldValue = currentProject;
				if (oldValue != value) {
					LoggingService.Info("CurrentProject changed to " + (value == null ? "null" : value.Name));
					currentProject = value;
					CurrentProjectChanged(this, new PropertyChangedEventArgs<IProject>("CurrentProject", oldValue, value));
					CommandManager.InvalidateRequerySuggested();
				}
			}
		}
		
		public IProject FindProjectContainingFile(FileName fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			IProject currentProject = this.CurrentProject;
			if (currentProject != null && currentProject.IsFileInProject(fileName))
				return currentProject;
			
			ISolution openSolution = this.OpenSolution;
			if (openSolution == null)
				return null;
			// Try all project's in the solution.
			IProject linkedProject = null;
			foreach (IProject project in openSolution.Projects) {
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
		
		public void OpenSolutionOrProject(FileName fileName)
		{
			CloseSolution();
			FileUtility.ObservedLoad(OpenSolutionOrProjectInternal, fileName);
		}
		
		void OpenSolutionOrProjectInternal(string fileName)
		{
			using (var progress = AsynchronousWaitDialog.ShowWaitDialog("Loading Solution")) {
				ISolution solution = LoadSolutionFile(FileName.Create(fileName), progress);
				throw new NotImplementedException();
			}
		}
		
		public void CloseSolution()
		{
			throw new NotImplementedException();
		}
		
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
		
		public ISolution LoadSolutionFile(FileName fileName, IProgressMonitor progress)
		{
			Solution solution = new Solution(fileName, new ProjectChangeWatcher(fileName));
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
			Solution solution = new Solution(fileName, new ProjectChangeWatcher(fileName));
			solution.LoadPreferences();
			return solution;
		}
	}
}
