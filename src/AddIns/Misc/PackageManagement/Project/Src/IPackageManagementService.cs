// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementService
	{
		ISharpDevelopProjectManager CreateProjectManager(IPackageRepository repository, MSBuildBasedProject project);
		ISharpDevelopPackageManager CreatePackageManagerForActiveProject();
		ISharpDevelopPackageManager CreatePackageManagerForActiveProject(IPackageRepository packageRepository);
		ISharpDevelopPackageManager CreatePackageManager(PackageSource packageSource, MSBuildBasedProject project);
		
		IProjectManager ActiveProjectManager { get; }
		
		InstallPackageAction CreateInstallPackageAction();
		UninstallPackageAction CreateUninstallPackageAction();
		UpdatePackageAction CreateUpdatePackageAction();
		
		IPackageManagementOutputMessagesView OutputMessagesView { get; }
	}
}
