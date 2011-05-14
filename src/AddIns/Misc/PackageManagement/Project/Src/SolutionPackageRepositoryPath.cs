// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SolutionPackageRepositoryPath
	{
		string packagesRelativeDirectory;
		Solution solution;
		DefaultPackagePathResolver pathResolver;
		
		public SolutionPackageRepositoryPath(IProject project)
			: this(project, new PackageManagementOptions())
		{
		}
		
		public SolutionPackageRepositoryPath(IProject project, PackageManagementOptions options)
			: this(project.ParentSolution, options)
		{
		}
		
		public SolutionPackageRepositoryPath(Solution solution, PackageManagementOptions options)
		{
			packagesRelativeDirectory = options.PackagesDirectory;
			this.solution = solution;
			GetSolutionPackageRepositoryPath();
		}
		
		void GetSolutionPackageRepositoryPath()
		{
			PackageRepositoryPath = Path.Combine(solution.Directory, packagesRelativeDirectory);
		}
		
		public string PackageRepositoryPath { get; private set; }
		
		public string GetInstallPath(IPackage package)
		{
			pathResolver = new DefaultPackagePathResolver(PackageRepositoryPath);
			return pathResolver.GetInstallPath(package);
		}
	}
}
