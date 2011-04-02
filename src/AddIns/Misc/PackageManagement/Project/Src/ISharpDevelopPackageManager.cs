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
		void InstallPackage(IPackage package);
		void InstallPackage(IPackage package, IEnumerable<PackageOperation> operations);
		void UninstallPackage(IPackage package);
		void UpdatePackage(IPackage package, IEnumerable<PackageOperation> operations);
	}
}
