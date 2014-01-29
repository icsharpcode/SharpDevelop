// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface ISharpDevelopPackageManager : IPackageManager
	{
		ISharpDevelopProjectManager ProjectManager { get; }
		
		void InstallPackage(IPackage package, InstallPackageAction installAction);
		void UninstallPackage(IPackage package, UninstallPackageAction uninstallAction);
		void UpdatePackage(IPackage package, UpdatePackageAction updateAction);
		void UpdatePackages(UpdatePackagesAction updateAction);
		void UpdatePackageReference(IPackage package, IUpdatePackageSettings settings);
		
		IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, InstallPackageAction installAction);
		IEnumerable<PackageOperation> GetUpdatePackageOperations(IEnumerable<IPackage> packages, IUpdatePackageSettings settings);
		
		void RunPackageOperations(IEnumerable<PackageOperation> operations);
	}
}
