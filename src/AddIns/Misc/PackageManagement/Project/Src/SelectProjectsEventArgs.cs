// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.PackageManagement
{
	public class SelectProjectsEventArgs : EventArgs
	{
		public SelectProjectsEventArgs(IEnumerable<IPackageManagementSelectedProject> selectedProjects)
		{
			this.SelectedProjects = selectedProjects;
		}
		
		public IEnumerable<IPackageManagementSelectedProject> SelectedProjects { get; private set; }
		
		public bool IsAccepted { get; set; }
	}
}
