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
