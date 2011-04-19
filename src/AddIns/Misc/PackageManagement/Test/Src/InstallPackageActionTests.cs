// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class InstallPackageActionTests
	{
		FakePackageManagementSolution fakeSolution;
		FakePackageManagementEvents fakePackageManagementEvents;
		FakePackageManagementProject fakeProject;
		InstallPackageAction action;
		InstallPackageHelper installPackageHelper;

		void CreateAction()
		{
			fakeSolution = new FakePackageManagementSolution();
			fakePackageManagementEvents = new FakePackageManagementEvents();
			fakeProject = fakeSolution.FakeProject;
			action = new InstallPackageAction(fakeSolution, fakePackageManagementEvents);
			installPackageHelper = new InstallPackageHelper(action);
		}
		
		FakePackage AddOnePackageToProjectSourceRepository(string packageId)
		{
			return fakeProject.FakeSourceRepository.AddFakePackage(packageId);
		}
		
		[Test]
		public void Execute_PackageIsSet_InstallsPackageIntoProject()
		{
			CreateAction();
			installPackageHelper.InstallTestPackage();
			
			var actualPackage = fakeProject.PackagePassedToInstallPackage;
			var expectedPackage = installPackageHelper.TestPackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageIsSet_InstallsPackageUsingPackageOperations()
		{
			CreateAction();
			var expectedOperations = new List<PackageOperation>();
			installPackageHelper.PackageOperations = expectedOperations;
			installPackageHelper.InstallTestPackage();
			
			var actualOperations = fakeProject.PackageOperationsPassedToInstallPackage;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void Execute_PackageIsSet_InstallsPackageNotIgnoringDependencies()
		{
			CreateAction();
			installPackageHelper.IgnoreDependencies = false;
			installPackageHelper.InstallTestPackage();
			
			bool ignored = fakeProject.IgnoreDependenciesPassedToInstallPackage;
			
			Assert.IsFalse(ignored);
		}
		
		[Test]
		public void Execute_PackageIsSetAndIgnoreDependencies_IsTrueInstallsPackageIgnoringDependencies()
		{
			CreateAction();
			installPackageHelper.IgnoreDependencies = true;
			installPackageHelper.InstallTestPackage();
			
			bool ignored = fakeProject.IgnoreDependenciesPassedToInstallPackage;
			
			Assert.IsTrue(ignored);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassed_RepositoryCreatedForPackageSource()
		{
			CreateAction();
			installPackageHelper.InstallPackageById("PackageId");
			
			var expectedPackageSource = installPackageHelper.PackageSource;
			var actualPackageSource = fakeSolution.PackageSourcePassedToCreateProject;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void IgnoreDependencies_DefaultValue_IsFalse()
		{
			CreateAction();
			Assert.IsFalse(action.IgnoreDependencies);
		}
		
		[Test]
		public void Execute_PackageAndPackageRepositoryPassed_CreatesProjectWithPackageRepository()
		{
			CreateAction();
			installPackageHelper.InstallTestPackage();
			
			var expectedRepository = installPackageHelper.PackageRepository;
			var actualRepository = fakeSolution.RepositoryPassedToCreateProject;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void Execute_PackageAndPackageRepositoryPassed_PackageInstallNotificationRaisedWithInstalledPackage()
		{
			CreateAction();
			installPackageHelper.InstallTestPackage();
			
			var expectedPackage = installPackageHelper.TestPackage;
			var actualPackage = fakePackageManagementEvents.PackagePassedToOnParentPackageInstalled;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassed_PackageOperationsRetrievedFromProject()
		{
			CreateAction();
			fakeProject.AddFakeInstallOperation();
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualOperations = action.Operations;
			var expectedOperations = fakeProject.FakeInstallOperations;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void Execute_PackageSpecifiedButNoPackageOperations_PackageUsedWhenPackageOperationsRetrievedForProject()
		{
			CreateAction();
			installPackageHelper.PackageOperations = null;
			installPackageHelper.InstallTestPackage();
			
			var expectedPackage = installPackageHelper.TestPackage;
			
			var actualPackage = fakeProject.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassed_ProjectIsUsedToCreateProject()
		{
			CreateAction();
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualProject = fakeSolution.ProjectPassedToCreateProject;
			var expectedProject = installPackageHelper.TestableProject;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassedAndIgnoreDependenciesIsTrue_DependenciesIgnoredWhenGettingPackageOperations()
		{
			CreateAction();
			installPackageHelper.IgnoreDependencies = true;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakeProject.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void InstallPackage_PackageIdAndSourceAndProjectPassedAndIgnoreDependenciesIsFalse_DependenciesNotIgnoredWhenGettingPackageOperations()
		{
			CreateAction();
			installPackageHelper.IgnoreDependencies = false;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakeProject.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(result);
		}

		[Test]
		public void Execute_VersionSpecified_VersionUsedWhenSearchingForPackage()
		{
			CreateAction();
			
			var recentPackage = AddOnePackageToProjectSourceRepository("PackageId");
			recentPackage.Version = new Version("1.2.0");
			
			var oldPackage = AddOnePackageToProjectSourceRepository("PackageId");
			oldPackage.Version = new Version("1.0.0");
			
			var package = AddOnePackageToProjectSourceRepository("PackageId");
			var version = new Version("1.1.0");
			package.Version = version;
			
			installPackageHelper.Version = version;
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualPackage = fakeProject.PackagePassedToInstallPackage;
			
			Assert.AreEqual(package, actualPackage);
		}
	}
}
