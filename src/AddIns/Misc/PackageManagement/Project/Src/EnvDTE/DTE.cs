// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class DTE
	{
		IPackageManagementProjectService projectService;
		
		public DTE()
			: this(new PackageManagementProjectService())
		{
		}
		
		public DTE(IPackageManagementProjectService projectService)
		{
			this.projectService = projectService;
		}
		
		public Solution Solution {
			get {
				if (projectService.OpenSolution != null) {
					return new Solution(projectService.OpenSolution);
				}
				return null;
			}
		}
	}
}
