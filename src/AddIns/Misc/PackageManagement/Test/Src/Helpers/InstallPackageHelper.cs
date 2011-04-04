// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class InstallPackageHelper
	{
		PackageManagementService packageManagementService;
		
		public FakePackage TestPackage = new FakePackage() {
			Id = "Test"
		};
		
		public FakePackageRepository PackageRepository = new FakePackageRepository();
		public List<PackageOperation> PackageOperations = new List<PackageOperation>();
		
		public InstallPackageHelper(PackageManagementService packageManagementService)
		{
			this.packageManagementService = packageManagementService;
		}
		
		public void InstallTestPackage()
		{
			var task = packageManagementService.CreateInstallPackageAction();
			task.Package = TestPackage;
			task.PackageRepository = PackageRepository;
			task.Operations = PackageOperations;
			task.Execute();
		}
		
		public FakePackage AddPackageInstallOperation()
		{
			var package = new FakePackage("Package to install");
			var operation = new PackageOperation(package, PackageAction.Install);
			PackageOperations.Add(operation);
			return package;
		}
		
		public PackageSource PackageSource = new PackageSource("http://sharpdevelop/packages");
		public TestableProject TestableProject = ProjectHelper.CreateTestProject();
		public bool IgnoreDependencies;
		public Version Version;
		
		public void InstallPackageById(string packageId)
		{
			var task = packageManagementService.CreateInstallPackageAction();
			task.PackageId = packageId;
			task.PackageVersion = Version;
			task.Project = TestableProject;
			task.PackageSource = PackageSource;
			task.IgnoreDependencies = IgnoreDependencies;
			
			task.Execute();
		}
	}
}
