// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class UpdatePackageHelper
	{
		UpdatePackageAction action;
		
		public FakePackage TestPackage = new FakePackage() {
			Id = "Test"
		};
		
		public List<PackageOperation> PackageOperations = new List<PackageOperation>();
		
		public UpdatePackageHelper(UpdatePackageAction action)
		{
			this.action = action;
		}
		
		public void UpdateTestPackage()
		{
			action.UpdateDependencies = UpdateDependencies;
			action.AllowPrereleaseVersions = AllowPrereleaseVersions;
			action.Package = TestPackage;
			action.Operations = PackageOperations;
			action.Execute();
		}
		
		public FakePackage AddPackageInstallOperation()
		{
			var package = new FakePackage("Package to install");
			var operation = new PackageOperation(package, PackageAction.Install);
			PackageOperations.Add(operation);
			return package;
		}
		
		public PackageSource PackageSource = new PackageSource("http://sharpdevelop/packages");
		public bool UpdateDependencies;
		public bool AllowPrereleaseVersions;
		public SemanticVersion Version;
		
		public void UpdatePackageById(string packageId)
		{
			action.PackageId = packageId;
			action.PackageVersion = Version;
			action.UpdateDependencies = UpdateDependencies;
			action.AllowPrereleaseVersions = AllowPrereleaseVersions;
			action.Execute();
		}
	}
}
