// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

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
		
		public event EventHandler<SolutionEventArgs> OpenSolutionChanged;
		public event EventHandler<ProjectEventArgs> CurrentProjectChanged;
		
		public ISolution OpenSolution {
			[DebuggerStepThrough]
			get { return openSolution; }
			private set {
				SD.MainThread.VerifyAccess();
				if (openSolution != value) {
					openSolution = value;
					if (OpenSolutionChanged != null)
						OpenSolutionChanged(this, new SolutionEventArgs(value));
					CommandManager.InvalidateRequerySuggested();
				}
			}
		}
		
		public IProject CurrentProject {
			[DebuggerStepThrough]
			get { return currentProject; }
			set {
				SD.MainThread.VerifyAccess();
				if (currentProject != value) {
					LoggingService.Info("CurrentProject changed to " + (value == null ? "null" : value.Name));
					currentProject = value;
					if (CurrentProjectChanged != null)
						CurrentProjectChanged(this, new ProjectEventArgs(value));
					CommandManager.InvalidateRequerySuggested();
				}
			}
		}
		
		ConcatModelCollection<IProject> allProjects = new ConcatModelCollection<IProject>();
		
		public IModelCollection<IProject> Projects {
			get { return allProjects; }
		}
		
		public IProject FindProjectContainingFile(FileName fileName)
		{
			throw new NotImplementedException();
		}
		
		public void OpenSolutionOrProject(FileName fileName)
		{
			throw new NotImplementedException();
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
		
		public ISolution LoadSolutionFile(FileName fileName)
		{
			throw new NotImplementedException();
		}
		
		public ISolution CreateEmptySolutionFile(FileName fileName)
		{
			throw new NotImplementedException();
		}
	}
}
