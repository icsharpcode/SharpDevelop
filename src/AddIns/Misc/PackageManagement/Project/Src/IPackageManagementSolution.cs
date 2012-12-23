// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementSolution
	{
		IPackageManagementProject GetActiveProject();
		IPackageManagementProject GetActiveProject(IPackageRepository sourceRepository);
		IPackageManagementProject GetProject(PackageSource source, string projectName);
		IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName);
		IPackageManagementProject GetProject(IPackageRepository sourceRepository, IProject project);
		IEnumerable<IPackageManagementProject> GetProjects(IPackageRepository sourceRepository);
		
		IProject GetActiveMSBuildProject();
		IEnumerable<IProject> GetMSBuildProjects();
		bool HasMultipleProjects();
		
		bool IsPackageInstalled(IPackage package);
		IQueryable<IPackage> GetPackages();
		IEnumerable<IPackage> GetPackagesInReverseDependencyOrder();
		string GetInstallPath(IPackage package);
		
		bool IsOpen { get; }
		string FileName { get; }
	}
}
