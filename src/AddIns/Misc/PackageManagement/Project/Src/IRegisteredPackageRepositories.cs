// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IRegisteredPackageRepositories
	{
		IPackageRepository ActiveRepository { get; }
		IRecentPackageRepository RecentPackageRepository { get; }
		
		IPackageRepository CreateRepository(PackageSource source);
		IPackageRepository CreateAggregateRepository();
		
		bool HasMultiplePackageSources { get; }
		PackageSource ActivePackageSource { get; set; }
		RegisteredPackageSources PackageSources { get; }
	}
}
