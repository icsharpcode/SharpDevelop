// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IUpdatePackageActionsFactory
	{
		IUpdatePackageActions CreateUpdateAllPackagesInProject(IPackageManagementProject project);
		
		IUpdatePackageActions CreateUpdateAllPackagesInSolution(
			IPackageManagementSolution solution,
			IPackageRepository sourceRepository);
		
		 IUpdatePackageActions CreateUpdatePackageInAllProjects(
			PackageReference packageReference,
			IPackageManagementSolution solution,
			IPackageRepository sourceRepository);
	}
}
