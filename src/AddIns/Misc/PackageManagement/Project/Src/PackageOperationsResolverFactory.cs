// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageOperationsResolverFactory : IPackageOperationResolverFactory
	{
		public IPackageOperationResolver CreateInstallPackageOperationResolver(
			IPackageRepository localRepository,
			IPackageRepository sourceRepository,
			ILogger logger,
			InstallPackageAction installAction)
		{
			return new InstallWalker(
				localRepository,
				sourceRepository,
				null,
				logger,
				installAction.IgnoreDependencies,
				installAction.AllowPrereleaseVersions);
		}
	}
}
