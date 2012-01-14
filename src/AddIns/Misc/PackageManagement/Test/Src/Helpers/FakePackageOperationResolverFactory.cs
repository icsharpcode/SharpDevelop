// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageOperationResolverFactory : IPackageOperationResolverFactory
	{
		public FakePackageOperationResolver FakeInstallPackageOperationResolver = new FakePackageOperationResolver();
		public IPackageRepository LocalRepositoryPassedToCreateInstallPackageOperationsResolver;
		public IPackageRepository SourceRepositoryPassedToCreateInstallPackageOperationsResolver;
		public ILogger LoggerPassedToCreateInstallPackageOperationResolver;
		public bool IgnoreDependenciesPassedToCreateInstallPackageOperationResolver;
		public bool AllowPrereleaseVersionsPassedToCreateInstallPackageOperationResolver;
		
		public IPackageOperationResolver CreateInstallPackageOperationResolver(
			IPackageRepository localRepository,
			IPackageRepository sourceRepository,
			ILogger logger,
			InstallPackageAction installAction)
		{
			LocalRepositoryPassedToCreateInstallPackageOperationsResolver = localRepository;
			SourceRepositoryPassedToCreateInstallPackageOperationsResolver = sourceRepository;
			LoggerPassedToCreateInstallPackageOperationResolver = logger;
			IgnoreDependenciesPassedToCreateInstallPackageOperationResolver = installAction.IgnoreDependencies;
			AllowPrereleaseVersionsPassedToCreateInstallPackageOperationResolver = installAction.AllowPrereleaseVersions;
			
			return FakeInstallPackageOperationResolver;
		}
	}
}
