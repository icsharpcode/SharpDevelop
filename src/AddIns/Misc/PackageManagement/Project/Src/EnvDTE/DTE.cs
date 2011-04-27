// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class DTE
	{
		IPackageManagementProjectService projectService;
		IPackageManagementFileService fileService;
		
		public DTE()
			: this(new PackageManagementProjectService(), new PackageManagementFileService())
		{
		}
		
		public DTE(
			IPackageManagementProjectService projectService,
			IPackageManagementFileService fileService)
		{
			this.projectService = projectService;
			this.fileService = fileService;
			
			ItemOperations = new ItemOperations(fileService);
		}
		
		public Solution Solution {
			get {
				if (projectService.OpenSolution != null) {
					return new Solution(projectService.OpenSolution);
				}
				return null;
			}
		}
		
		public ItemOperations ItemOperations { get; private set; }
	}
}
