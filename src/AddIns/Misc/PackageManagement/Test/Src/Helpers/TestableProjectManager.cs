// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class TestableProjectManager : SharpDevelopProjectManager
	{
		public IPackage PackagePassedToAddPackageReference;
		public bool IgnoreDependenciesPassedToAddPackageReference;
		public bool AllowPrereleaseVersionsPassedToAddPackageReference;
		
		public IPackage PackagePassedToRemovePackageReference;
		public bool ForcePassedToRemovePackageReference;
		public bool RemoveDependenciesPassedToRemovePackageReference;
		
		public IPackage PackagePassedToUpdatePackageReference;
		public bool UpdateDependenciesPassedToUpdatePackageReference;
		public bool AllowPrereleaseVersionsPassedToUpdatePackageReference;
		
		public FakePackageRepository FakeLocalRepository {
			get { return LocalRepository as FakePackageRepository; }
		}
		
		public TestableProjectManager()
			: base(new FakePackageRepository(),
				new FakePackagePathResolver(),
				new FakeProjectSystem(),
				new FakePackageRepository())
		{
		}
		
		public override void AddPackageReference(string packageId, SemanticVersion version, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			var package = new FakePackage();
			package.Id = packageId;
			package.Version = version;
			PackagePassedToAddPackageReference = package;
			IgnoreDependenciesPassedToAddPackageReference = ignoreDependencies;
			AllowPrereleaseVersionsPassedToAddPackageReference = allowPrereleaseVersions;
		}
		
		public override void AddPackageReference(IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			PackagePassedToAddPackageReference = package;
			IgnoreDependenciesPassedToAddPackageReference = ignoreDependencies;
			AllowPrereleaseVersionsPassedToAddPackageReference = allowPrereleaseVersions;
		}
		
		public override void RemovePackageReference(IPackage package, bool force, bool removeDependencies)
		{
			PackagePassedToRemovePackageReference = package;
			ForcePassedToRemovePackageReference = force;
			RemoveDependenciesPassedToRemovePackageReference = removeDependencies;
		}
		
		public override void UpdatePackageReference(string packageId, SemanticVersion version, bool updateDependencies, bool allowPrereleaseVersions)
		{
			var package = new FakePackage();
			package.Id = packageId;
			package.Version = version;
			
			PackagePassedToUpdatePackageReference = package;
			UpdateDependenciesPassedToUpdatePackageReference = updateDependencies;
			AllowPrereleaseVersionsPassedToUpdatePackageReference = allowPrereleaseVersions;
		}
		
		public FakePackage AddFakePackageToProjectLocalRepository(string packageId, string version)
		{
			return FakeLocalRepository.AddFakePackageWithVersion(packageId, version);
		}
	}
}
