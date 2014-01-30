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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageManagementProjectService : IPackageManagementProjectService
	{
		public bool IsRefreshProjectBrowserCalled;
		
		public IProject CurrentProject { get; set; }
		public ISolution OpenSolution { get; set; }
		
		public event EventHandler<SolutionEventArgs> SolutionClosed;
		public event EventHandler<SolutionEventArgs> SolutionOpened;
		
		public void RefreshProjectBrowser()
		{
			IsRefreshProjectBrowserCalled = true;
		}
		
		public void FireSolutionClosedEvent(ISolution solution)
		{
			if (SolutionClosed != null) {
				SolutionClosed(this, new SolutionEventArgs(solution));
			}
		}
		
		public void FireSolutionOpenedEvent(ISolution solution)
		{
			if (SolutionOpened != null) {
				SolutionOpened(this, new SolutionEventArgs(solution));
			}
		}
		
		public readonly IMutableModelCollection<IModelCollection<IProject>> ProjectCollections = new NullSafeSimpleModelCollection<IModelCollection<IProject>>();
		IModelCollection<IProject> allProjects;
		
		public IModelCollection<IProject> AllProjects {
			get { 
				if (allProjects == null)
					allProjects = ProjectCollections.SelectMany(c => c);
				return allProjects; 
			}
		}
		
		public void AddProject(IProject project)
		{
			ProjectCollections.Add(new ImmutableModelCollection<IProject>(new[] { project }));
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
		
		public ISolution SavedSolution;
		
		public void Save(ISolution solution)
		{
			SavedSolution = solution;
		}
		
//		public IProjectContent GetProjectContent(IProject project)
//		{
//			return new DefaultProjectContent();
//		}
		
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
