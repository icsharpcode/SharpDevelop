// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class UninstallPackageHelper
	{
		PackageManagementService packageManagementService;
		
		public UninstallPackageHelper(PackageManagementService packageManagementService)
		{
			this.packageManagementService = packageManagementService;
		}
		
		public FakePackage TestPackage = new FakePackage() {
			Id = "Test"
		};
		
		public FakePackageRepository FakePackageRepository = new FakePackageRepository();
		
		public void UninstallTestPackage()
		{
			packageManagementService.UninstallPackage(FakePackageRepository, TestPackage);
		}
		
		public Version Version;
		public MSBuildBasedProject Project;
		public PackageSource PackageSource = new PackageSource("http://sharpdevelop.net");
		public bool ForceRemove;
		public bool RemoveDependencies;
		
		public void UninstallPackageById(string packageId)
		{
			packageManagementService.UninstallPackage(
				packageId,
				Version,
				Project,
				PackageSource,
				ForceRemove,
				RemoveDependencies);
		}
	}
}
