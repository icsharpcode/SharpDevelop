// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementProjectService
	{
		IProject CurrentProject { get; }
		Solution OpenSolution { get; }
		
		event ProjectEventHandler ProjectAdded;
		event SolutionFolderEventHandler SolutionFolderRemoved;
		event EventHandler SolutionClosed;
		event EventHandler<SolutionEventArgs> SolutionLoaded;
		
		void RefreshProjectBrowser();
		void AddProjectItem(IProject project, ProjectItem item);
		void RemoveProjectItem(IProject project, ProjectItem item);
		void Save(IProject project);
		
		IEnumerable<IProject> GetOpenProjects();

		IProjectContent GetProjectContent(IProject project);

		IProjectBrowserUpdater CreateProjectBrowserUpdater();
	}
}
