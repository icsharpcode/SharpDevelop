// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;
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
		
		public ISolution OpenSolution {
			get { return ProjectService.OpenSolution; }
		}
		
		public IProjectBuilder ProjectBuilder { get; private set; }
		
		public void RefreshProjectBrowser()
		{
			SD.MainThread.InvokeAsyncAndForget(ProjectBrowserPad.RefreshViewAsync);
		}
		
		void InvokeIfRequired(Action action)
		{
			SD.MainThread.InvokeIfRequired(action);
		}
		
		T InvokeIfRequired<T>(Func<T> callback)
		{
			return SD.MainThread.InvokeIfRequired(callback);
		}
		
		public IModelCollection<IProject> AllProjects { 
			get { return SD.ProjectService.AllProjects; }
		}
		
		public void AddProjectItem(IProject project, ProjectItem item)
		{
			InvokeIfRequired(() => ProjectService.AddProjectItem(project, item));
		}
		
		public void RemoveProjectItem(IProject project, ProjectItem item)
		{
			InvokeIfRequired(() => ProjectService.RemoveProjectItem(project, item));
		}
		
		public void Save(IProject project)
		{
			InvokeIfRequired(() => project.Save());
		}
		
		public void Save(ISolution solution)
		{
			InvokeIfRequired(() => solution.Save());
		}
		
//		public IProjectContent GetProjectContent(IProject project)
//		{
//			return SD.ParserService.GetProjectContent(project);
//		}
		
		public event EventHandler<SolutionEventArgs> SolutionClosed {
			add { SD.ProjectService.SolutionClosed += value; }
			remove { SD.ProjectService.SolutionClosed -= value; }
		}
		
		public event EventHandler<SolutionEventArgs> SolutionOpened {
			add { SD.ProjectService.SolutionOpened += value; }
			remove { SD.ProjectService.SolutionOpened -= value; }
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
