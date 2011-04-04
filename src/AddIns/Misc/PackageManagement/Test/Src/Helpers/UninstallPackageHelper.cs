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
			var action = packageManagementService.CreateUninstallPackageAction();
			action.PackageRepository = FakePackageRepository;
			action.Package = TestPackage;
			action.Execute();
		}
		
		public Version Version;
		public MSBuildBasedProject Project;
		public PackageSource PackageSource = new PackageSource("http://sharpdevelop.net");
		public bool ForceRemove;
		public bool RemoveDependencies;
		
		public void UninstallPackageById(string packageId)
		{
			var action = packageManagementService.CreateUninstallPackageAction();
			action.PackageId = packageId;
			action.PackageVersion = Version;
			action.Project = Project;
			action.ForceRemove = ForceRemove;
			action.PackageSource = PackageSource;
			action.RemoveDependencies = RemoveDependencies;
			action.Execute();
		}
	}
}
