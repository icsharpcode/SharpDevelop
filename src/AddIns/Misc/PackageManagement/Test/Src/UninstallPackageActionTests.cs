// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class UninstallPackageActionTests
	{
		UninstallPackageAction action;
		FakePackageManagementService fakePackageManagementService;
		FakePackageManagementEvents fakePackageManagementEvents;
		FakePackageManager fakePackageManager;
		UninstallPackageHelper uninstallPackageHelper;
		
		void CreateAction()
		{
			fakePackageManagementService = new FakePackageManagementService();
			fakePackageManagementEvents = new FakePackageManagementEvents();
			fakePackageManager = fakePackageManagementService.FakePackageManagerToReturnFromCreatePackageManager;
			action = new UninstallPackageAction(fakePackageManagementService, fakePackageManagementEvents);
			uninstallPackageHelper = new UninstallPackageHelper(action);
		}
		
		FakePackage AddOnePackageToPackageManagerSourceRepository(string packageId)
		{
			return fakePackageManager
				.FakeSourceRepository
				.AddFakePackage(packageId);
		}
		
		[Test]
		public void Execute_PackageObjectPassed_CallsPackageManagerUninstallPackageWithPackage()
		{
			CreateAction();
			
			uninstallPackageHelper.UninstallTestPackage();
			
			var actualPackage = fakePackageManager.PackagePassedToUninstallPackage;
			var expectedPackage = uninstallPackageHelper.TestPackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageObjectPassed_PackageRepositoryUsedToCreatePackageManager()
		{
			CreateAction();
			
			uninstallPackageHelper.UninstallTestPackage();
			
			var actualRepository = fakePackageManagementService.PackageRepositoryPassedToCreatePackageManager;
	
			var expectedRepository = uninstallPackageHelper.FakePackageRepository;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void Execute_PackageAndPackageRepositoryPassed_OnPackageUninstalledCalledWithPackage()
		{
			CreateAction();

			uninstallPackageHelper.UninstallTestPackage();
			
			var actualPackage = fakePackageManagementEvents.PackagePassedToOnParentPackageUninstalled;
			var expectedPackage = uninstallPackageHelper.TestPackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageObjectPassed_PackageIsNotForcefullyRemoved()
		{
			CreateAction();
			
			uninstallPackageHelper.ForceRemove = false;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool forceRemove = fakePackageManager.ForceRemovePassedToUninstallPackage;
			
			Assert.IsFalse(forceRemove);
		}
		
		[Test]
		public void Execute_PackageObjectPassed_PackageDependenciesAreNotRemoved()
		{
			CreateAction();
			
			uninstallPackageHelper.RemoveDependencies = false;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool removeDependencies = fakePackageManager.RemoveDependenciesPassedToUninstallPackage;
			
			Assert.IsFalse(removeDependencies);
		}
		
		[Test]
		public void Execute_PackageIdSpecified_PackageSourcePassedIsUsedToCreateRepository()
		{
			CreateAction();
			
			var expectedPackageSource = new PackageSource("http://test");
			uninstallPackageHelper.PackageSource = expectedPackageSource;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			var actualPackageSource = fakePackageManagementService.PackageSourcePassedToCreatePackageManager;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void Execute_PackageIdSpecified_ProjectPassedIsUsedToCreateRepository()
		{
			CreateAction();
			var expectedProject = ProjectHelper.CreateTestProject();
			uninstallPackageHelper.Project = expectedProject;
			
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			var actualProject = fakePackageManagementService.ProjectPassedToCreatePackageManager;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void Execute_PackageIdSpecifiedAndForceRemoveIsTrue_PackageIsForcefullyRemoved()
		{
			CreateAction();
			
			uninstallPackageHelper.ForceRemove = true;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool forceRemove = fakePackageManager.ForceRemovePassedToUninstallPackage;
			
			Assert.IsTrue(forceRemove);
		}
		
		[Test]
		public void Execute_PackageIdSpecifiedAndRemoveDependenciesIsTrue_PackageDependenciesAreRemoved()
		{
			CreateAction();
			
			uninstallPackageHelper.RemoveDependencies = true;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool removeDependencies = fakePackageManager.RemoveDependenciesPassedToUninstallPackage;
			
			Assert.IsTrue(removeDependencies);
		}
		
		[Test]
		public void Execute_VersionSpecified_VersionUsedWhenSearchingForPackage()
		{
			CreateAction();
			
			var recentPackage = AddOnePackageToPackageManagerSourceRepository("PackageId");
			recentPackage.Version = new Version("1.2.0");
			
			var oldPackage = AddOnePackageToPackageManagerSourceRepository("PackageId");
			oldPackage.Version = new Version("1.0.0");
			
			var package = AddOnePackageToPackageManagerSourceRepository("PackageId");
			var version = new Version("1.1.0");
			package.Version = version;
			
			uninstallPackageHelper.Version = version;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			var actualPackage = fakePackageManager.PackagePassedToUninstallPackage;
			
			Assert.AreEqual(package, actualPackage);
		}
		
		[Test]
		public void ForceRemove_DefaultValue_ReturnsFalse()
		{
			CreateAction();
			Assert.IsFalse(action.ForceRemove);
		}
		
		[Test]
		public void RemoveDependencies_DefaultValue_ReturnsFalse()
		{
			CreateAction();
			Assert.IsFalse(action.RemoveDependencies);
		}
	}
}
