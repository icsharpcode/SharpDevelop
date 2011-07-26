// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdatePackageActionsFactory : IUpdatePackageActionsFactory
	{
		public IUpdatePackageActions CreateUpdateAllPackagesInProject(IPackageManagementProject project)
		{
			return new UpdateAllPackagesInProject(project);
		}
		
		public IUpdatePackageActions CreateUpdateAllPackagesInSolution(
			IPackageManagementSolution solution,
			IPackageRepository sourceRepository)
		{
			return new UpdateAllPackagesInSolution(solution, sourceRepository);
		}
		
		public IUpdatePackageActions CreateUpdatePackageInAllProjects(
			PackageReference packageReference,
			IPackageManagementSolution solution,
			IPackageRepository sourceRepository)
		{
			return new UpdatePackageInAllProjects(packageReference, solution, sourceRepository);
		}
	}
}
