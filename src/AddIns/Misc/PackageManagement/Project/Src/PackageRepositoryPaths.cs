// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public class PackageRepositoryPaths
	{
		string packagesRelativeDirectory;
		string solutionPackagesPath;
		IProject project;
		
		public PackageRepositoryPaths(IProject project)
			: this(project, new PackageManagementOptions())
		{
		}
		
		public PackageRepositoryPaths(IProject project, PackageManagementOptions options)
		{
			packagesRelativeDirectory = options.PackagesDirectory;
			this.project = project;
			GetPaths();
		}
		
		void GetPaths()
		{
			GetSolutionPackagesPath();
		}
		
		void GetSolutionPackagesPath()
		{
			solutionPackagesPath = Path.Combine(project.ParentSolution.Directory, packagesRelativeDirectory);
		}
		
		public string SolutionPackagesPath {
			get { return solutionPackagesPath; }
		}
	}
}
