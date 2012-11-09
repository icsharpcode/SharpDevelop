// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementProjectService : IPackageManagementProjectService
	{
		public PackageManagementProjectService()
		{
			ProjectBuilder = new ProjectBuilder();
		}
		
		public IProject CurrentProject {
			get { return ProjectService.CurrentProject; }
		}
		
		public Solution OpenSolution {
			get { return ProjectService.OpenSolution; }
		}
		
		public IProjectBuilder ProjectBuilder { get; private set; }
		
		public void RefreshProjectBrowser()
		{
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadAsyncCall(RefreshProjectBrowser);
			} else {
				var refreshCommand = new RefreshProjectBrowser();
				refreshCommand.Run();
			}
		}
		
		public IEnumerable<IProject> GetOpenProjects()
		{
			Solution solution = OpenSolution;
			if (solution != null) {
				return solution.Projects;
			}
			return new IProject[0];
		}
		
		public void AddProjectItem(IProject project, ProjectItem item)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				Action<IProject, ProjectItem> action = AddProjectItem;
				WorkbenchSingleton.SafeThreadCall<IProject, ProjectItem>(action, project, item);
			} else {
				ProjectService.AddProjectItem(project, item);
			}
		}
		
		public void RemoveProjectItem(IProject project, ProjectItem item)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				Action<IProject, ProjectItem> action = RemoveProjectItem;
				WorkbenchSingleton.SafeThreadCall<IProject, ProjectItem>(action, project, item);
			} else {
				ProjectService.RemoveProjectItem(project, item);
			}
		}
		
		public void Save(IProject project)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				Action<IProject> action = Save;
				WorkbenchSingleton.SafeThreadCall<IProject>(action, project);
			} else {
				project.Save();
			}
		}
		
		public void Save(Solution solution)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				Action<Solution> action = Save;
				WorkbenchSingleton.SafeThreadCall<Solution>(action, solution);
			} else {
				solution.Save();
			}
		}
		
		public IProjectContent GetProjectContent(IProject project)
		{
			return ParserService.GetProjectContent(project);
		}
		
		public event ProjectEventHandler ProjectAdded {
			add { ProjectService.ProjectAdded += value; }
			remove { ProjectService.ProjectAdded -= value; }
		}
	
		public event EventHandler SolutionClosed {
			add { ProjectService.SolutionClosed += value; }
			remove { ProjectService.SolutionClosed -= value; }
		}
		
		public event EventHandler<SolutionEventArgs> SolutionLoaded {
			add { ProjectService.SolutionLoaded += value; }
			remove { ProjectService.SolutionLoaded -= value; }
		}
		
		public event SolutionFolderEventHandler SolutionFolderRemoved {
			add { ProjectService.SolutionFolderRemoved += value; }
			remove { ProjectService.SolutionFolderRemoved -= value; }
		}
		
		public IProjectBrowserUpdater CreateProjectBrowserUpdater()
		{
			return new ThreadSafeProjectBrowserUpdater();
		}
		
		public string GetDefaultCustomToolForFileName(FileProjectItem projectItem)
		{
			return CustomToolsService.GetCompatibleCustomToolNames(projectItem).FirstOrDefault();
		}
	}
}
