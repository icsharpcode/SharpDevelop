// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementService
	{
		event EventHandler PackageInstalled;
		event EventHandler PackageUninstalled;
		
		IPackageRepository ActivePackageRepository { get; }
		IProjectManager ActiveProjectManager { get; }
		
		void InstallPackage(IPackageRepository repository, IPackage package);
		void UninstallPackage(IPackageRepository repository, IPackage package);

		PackageManagementOptions Options { get; }
		
		bool HasMultiplePackageSources { get; }
		PackageSource ActivePackageSource { get; set; }
	}
}
