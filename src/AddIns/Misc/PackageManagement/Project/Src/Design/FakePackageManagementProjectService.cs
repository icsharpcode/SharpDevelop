// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageManagementProjectService : IPackageManagementProjectService
	{
		public bool IsRefreshProjectBrowserCalled;
		
		public IProject CurrentProject { get; set; }
		public Solution OpenSolution { get; set; }
		
		public event ProjectEventHandler ProjectAdded;
		public event SolutionFolderEventHandler SolutionFolderRemoved;
		public event EventHandler SolutionClosed;
		public event EventHandler<SolutionEventArgs> SolutionLoaded;
		
		public void RefreshProjectBrowser()
		{
			IsRefreshProjectBrowserCalled = true;
		}		
		
		public void FireProjectAddedEvent(IProject project)
		{
			if (ProjectAdded != null) {
				ProjectAdded(this, new ProjectEventArgs(project));
			}
		}
		
		public void FireSolutionClosedEvent()
		{
			if (SolutionClosed != null) {
				SolutionClosed(this, new EventArgs());
			}
		}
		
		public void FireSolutionLoadedEvent(Solution solution)
		{
			if (SolutionLoaded != null) {
				SolutionLoaded(this, new SolutionEventArgs(solution));
			}
		}
		
		public void FireSolutionFolderRemoved(ISolutionFolder solutionFolder)
		{
			if (SolutionFolderRemoved != null) {
				SolutionFolderRemoved(this, new SolutionFolderEventArgs(solutionFolder));
			}
		}
		
		public List<IProject> FakeOpenProjects = new List<IProject>();
		
		public void AddFakeProject(IProject project)
		{
			FakeOpenProjects.Add(project);
		}
		
		public IEnumerable<IProject> GetOpenProjects()
		{
			return FakeOpenProjects;
		}
		
		public void AddProjectItem(IProject project, ProjectItem item)
		{
			ProjectService.AddProjectItem(project, item);
		}
		
		public void RemoveProjectItem(IProject project, ProjectItem item)
		{
			ProjectService.RemoveProjectItem(project, item);
		}
		
		public void Save(IProject project)
		{
			project.Save();
		}
		
		public Solution SavedSolution;
		
		public void Save(Solution solution)
		{
			SavedSolution = solution;
		}
		
		public IProjectContent GetProjectContent(IProject project)
		{
			return new DefaultProjectContent();
		}
		
		public IProjectBrowserUpdater ProjectBrowserUpdater;
		
		public IProjectBrowserUpdater CreateProjectBrowserUpdater()
		{
			return ProjectBrowserUpdater;
		}
		
		Dictionary<string, string> defaultCustomTools = new Dictionary<string, string>();
		
		public void AddDefaultCustomToolForFileName(string fileName, string customTool)
		{
			defaultCustomTools.Add(fileName, customTool);
		}
		
		public string GetDefaultCustomToolForFileName(FileProjectItem projectItem)
		{
			if (defaultCustomTools.ContainsKey(projectItem.FileName)) {
				return defaultCustomTools[projectItem.FileName];
			}
			return String.Empty;
		}
		
		public FakeProjectBuilder FakeProjectBuilder = new FakeProjectBuilder();
		
		public IProjectBuilder ProjectBuilder {
			get { return FakeProjectBuilder; }
		}
	}
}
