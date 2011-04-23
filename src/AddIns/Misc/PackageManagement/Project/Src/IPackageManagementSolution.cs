// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementSolution
	{
		IPackageManagementProject GetActiveProject();
		IPackageManagementProject CreateProject(PackageSource source, MSBuildBasedProject project);
		IPackageManagementProject CreateProject(IPackageRepository sourceRepository, MSBuildBasedProject project);
		IPackageManagementProject GetActiveProject(IPackageRepository sourceRepository);
		IPackageManagementProject GetProject(PackageSource source, string projectName);
		IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName);
	}
}
