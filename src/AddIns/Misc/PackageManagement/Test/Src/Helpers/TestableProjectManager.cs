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
		
		public IPackage PackagePassedToRemovePackageReference;
		public bool ForcePassedToRemovePackageReference;
		public bool RemoveDependenciesPassedToRemovePackageReference;
		
		public IPackage PackagePassedToUpdatePackageReference;
		public bool UpdateDependenciesPassedToUpdatePackageReference;
		
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
		
		public override void AddPackageReference(string packageId, Version version, bool ignoreDependencies)
		{
			var package = new FakePackage();
			package.Id = packageId;
			package.Version = version;
			PackagePassedToAddPackageReference = package;
			IgnoreDependenciesPassedToAddPackageReference = ignoreDependencies;
		}
		
		protected override void AddPackageReference(IPackage package, bool ignoreDependencies)
		{
			PackagePassedToAddPackageReference = package;
			IgnoreDependenciesPassedToAddPackageReference = ignoreDependencies;
		}
		
		protected override void RemovePackageReference(IPackage package, bool force, bool removeDependencies)
		{
			PackagePassedToRemovePackageReference = package;
			ForcePassedToRemovePackageReference = force;
			RemoveDependenciesPassedToRemovePackageReference = removeDependencies;
		}
		
		public override void UpdatePackageReference(string packageId, Version version, bool updateDependencies)
		{
			var package = new FakePackage();
			package.Id = packageId;
			package.Version = version;
			
			PackagePassedToUpdatePackageReference = package;
			UpdateDependenciesPassedToUpdatePackageReference = updateDependencies;
		}
	}
}
