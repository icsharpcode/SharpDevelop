// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementProjectService
	{
		IProject CurrentProject { get; }
		ISolution OpenSolution { get; }
		IProjectBuilder ProjectBuilder { get; }
		
		event EventHandler<SolutionEventArgs> SolutionClosed;
		event EventHandler<SolutionEventArgs> SolutionOpened;
		
		void RefreshProjectBrowser();
		void AddProjectItem(IProject project, ProjectItem item);
		void RemoveProjectItem(IProject project, ProjectItem item);
		void Save(IProject project);
		void Save(ISolution solution);
		
		IModelCollection<IProject> AllProjects { get; }
		
		//IProjectContent GetProjectContent(IProject project);
		
		IProjectBrowserUpdater CreateProjectBrowserUpdater();
		
		string GetDefaultCustomToolForFileName(FileProjectItem projectItem);
	}
}
