// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SolutionPackageRepositoryFactory : ISolutionPackageRepositoryFactory
	{
		ISharpDevelopPackageRepositoryFactory repositoryFactory;
		PackageManagementOptions options;
		
		public SolutionPackageRepositoryFactory()
			: this(PackageManagementServices.PackageRepositoryCache, PackageManagementServices.Options)
		{
		}
		
		public SolutionPackageRepositoryFactory(
			ISharpDevelopPackageRepositoryFactory repositoryFactory,
			PackageManagementOptions options)
		{
			this.repositoryFactory = repositoryFactory;
			this.options = options;
		}
		
		public ISolutionPackageRepository CreateSolutionPackageRepository(Solution solution)
		{
			return new SolutionPackageRepository(solution, repositoryFactory, options);
		}
	}
}
