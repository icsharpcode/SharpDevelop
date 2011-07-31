// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface ISolutionPackageRepository
	{
		string GetInstallPath(IPackage package);
		IEnumerable<IPackage> GetPackagesByDependencyOrder();
		IEnumerable<IPackage> GetPackagesByReverseDependencyOrder();
		IQueryable<IPackage> GetPackages();
		bool IsInstalled(IPackage package);
		
		ISharedPackageRepository Repository { get; }
		IFileSystem FileSystem { get; }
		IPackagePathResolver PackagePathResolver { get; }
	}
}
