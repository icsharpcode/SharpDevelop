// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementProjectService : IPackageManagementProjectService
	{
		public IProject CurrentProject {
			get { return ProjectService.CurrentProject; }
		}
		
		public Solution OpenSolution {
			get { return ProjectService.OpenSolution; }
		}
		
		public void RefreshProjectBrowser()
		{
			var refreshCommand = new RefreshProjectBrowser();
			refreshCommand.Run();
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
	}
}
