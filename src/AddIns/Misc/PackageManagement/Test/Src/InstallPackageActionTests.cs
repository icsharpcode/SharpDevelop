// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class InstallPackageActionTests
	{
		FakePackageManagementService fakePackageManagementService;
		FakePackageManager fakePackageManager;
		InstallPackageAction action;
		InstallPackageHelper installPackageHelper;

		void CreateAction()
		{
			fakePackageManagementService = new FakePackageManagementService();
			fakePackageManager = fakePackageManagementService.FakePackageManagerToReturnFromCreatePackageManager;
			action = new InstallPackageAction(fakePackageManagementService);
			installPackageHelper = new InstallPackageHelper(action);
		}
		
		FakePackage AddOnePackageToPackageManagerSourceRepository(string packageId)
		{
			return fakePackageManager
				.FakeSourceRepository
				.AddFakePackage(packageId);
		}
		
		[Test]
		public void Execute_PackageIsSet_CallsPackageManagerInstallPackage()
		{
			CreateAction();
			installPackageHelper.InstallTestPackage();
			
			bool expectedIgnoreDependencies = false;
			var expectedInstallPackageParameters = new FakePackageManager.InstallPackageParameters() {
				IgnoreDependenciesPassedToInstallPackage = expectedIgnoreDependencies,
				PackagePassedToInstallPackage = installPackageHelper.TestPackage,
				PackageOperationsPassedToInstallPackage = installPackageHelper.PackageOperations
			};
			
			var actualInstallPackageParameters = fakePackageManager.ParametersPassedToInstallPackage;
			
			Assert.AreEqual(expectedInstallPackageParameters, actualInstallPackageParameters);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassed_RepositoryCreatedForPackageSource()
		{
			CreateAction();
			installPackageHelper.InstallPackageById("PackageId");
			
			var expectedPackageSource = installPackageHelper.PackageSource;
			var actualPackageSource = fakePackageManagementService.PackageSourcePassedToCreatePackageManager;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void IgnoreDependencies_DefaultValue_IsFalse()
		{
			CreateAction();
			Assert.IsFalse(action.IgnoreDependencies);
		}
		
		[Test]
		public void Execute_PackageAndPackageRepositoryPassed_CreatesPackageManagerWithPackageRepository()
		{
			CreateAction();
			installPackageHelper.InstallTestPackage();
			
			var expectedRepository = installPackageHelper.PackageRepository;
			var actualRepository = fakePackageManagementService.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void Execute_PackageAndPackageRepositoryPassed_PackageInstallNotificationRaisedWithInstalledPackage()
		{
			CreateAction();
			installPackageHelper.InstallTestPackage();
			
			var expectedPackage = installPackageHelper.TestPackage;
			var actualPackage = fakePackageManagementService.PackagePassedToOnParentPackageInstalled;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}

		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassed_InstallsPackageFromPackageSource()
		{
			CreateAction();
			
			var expectedPackage = AddOnePackageToPackageManagerSourceRepository("PackageId");
			
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualPackage = fakePackageManager.PackagePassedToInstallPackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}	
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassed_PackageOperationsRetrievedFromPackageManager()
		{
			CreateAction();
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualOperations = action.Operations;
			var expectedOperations = fakePackageManager.PackageOperationsToReturnFromGetInstallPackageOperations;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void Execute_PackageSpecifiedButNoPackageOperations_PackageUsedWhenPackageOperationsRetrievedFromPackageManager()
		{
			CreateAction();
			installPackageHelper.PackageOperations = null;
			installPackageHelper.InstallTestPackage();
			
			var expectedPackage = installPackageHelper.TestPackage;
			
			var actualPackage = fakePackageManager.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassed_ProjectIsUsedToCreatePackageManager()
		{
			CreateAction();
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualProject = fakePackageManagementService.ProjectPassedToCreatePackageManager;
			var expectedProject = installPackageHelper.TestableProject;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassedAndIgnoreDependenciesIsTrue_DependenciesIgnoredWhenInstallingPackage()
		{
			CreateAction();
			installPackageHelper.IgnoreDependencies = true;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakePackageManager.IgnoreDependenciesPassedToInstallPackage;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassedAndIgnoreDependenciesIsTrue_DependenciesIgnoredWhenGettingPackageOperations()
		{
			CreateAction();
			installPackageHelper.IgnoreDependencies = true;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void InstallPackage_PackageIdAndSourceAndProjectPassedAndIgnoreDependenciesIsFalse_DependenciesNotIgnoredWhenGettingPackageOperations()
		{
			CreateAction();
			installPackageHelper.IgnoreDependencies = false;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(result);
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
			
			installPackageHelper.Version = version;
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualPackage = 
				fakePackageManager
				.ParametersPassedToInstallPackage
				.PackagePassedToInstallPackage;
			
			Assert.AreEqual(package, actualPackage);
		}
	}
}
