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
		
		public void RefreshProjectBrowser()
		{
			var refreshCommand = new RefreshProjectBrowser();
			refreshCommand.Run();
		}
	}
}
