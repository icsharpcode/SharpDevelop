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
		FakePackageManagementSolution fakeSolution;
		FakePackageManagementEvents fakePackageManagementEvents;
		UninstallPackageHelper uninstallPackageHelper;
		FakePackageManagementProject fakeProject;
		
		void CreateAction()
		{
			fakeSolution = new FakePackageManagementSolution();
			fakePackageManagementEvents = new FakePackageManagementEvents();
			fakeProject = fakeSolution.FakeProject;
			action = new UninstallPackageAction(fakeSolution, fakePackageManagementEvents);
			uninstallPackageHelper = new UninstallPackageHelper(action);
		}
		
		FakePackage AddOnePackageToProjectSourceRepository(string packageId)
		{
			return fakeProject.FakeSourceRepository.AddFakePackage(packageId);
		}
		
		[Test]
		public void Execute_PackageObjectPassed_UninstallsPackageFromProject()
		{
			CreateAction();
			
			uninstallPackageHelper.UninstallTestPackage();
			
			var actualPackage = fakeProject.PackagePassedToUninstallPackage;
			var expectedPackage = uninstallPackageHelper.TestPackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageObjectPassed_PackageRepositoryUsedToCreateProject()
		{
			CreateAction();
			
			uninstallPackageHelper.UninstallTestPackage();
			
			var actualRepository = fakeSolution.RepositoryPassedToCreateProject;
	
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
		public void Execute_PackageObjectPassedAndForceRemoveIsFalse_PackageIsNotForcefullyRemoved()
		{
			CreateAction();
			
			uninstallPackageHelper.ForceRemove = false;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool forceRemove = fakeProject.ForceRemovePassedToUninstallPackage;
			
			Assert.IsFalse(forceRemove);
		}
		
		[Test]
		public void Execute_PackageObjectPassedAndForceRemoveIsTrue_PackageIsForcefullyRemoved()
		{
			CreateAction();
			
			uninstallPackageHelper.ForceRemove = true;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool forceRemove = fakeProject.ForceRemovePassedToUninstallPackage;
			
			Assert.IsTrue(forceRemove);
		}
		
		[Test]
		public void Execute_PackageObjectPassedAndRemoveDependenciesIsFalse_PackageDependenciesAreNotRemoved()
		{
			CreateAction();
			
			uninstallPackageHelper.RemoveDependencies = false;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool removeDependencies = fakeProject.RemoveDependenciesPassedToUninstallPackage;
			
			Assert.IsFalse(removeDependencies);
		}
		
		[Test]
		public void Execute_PackageObjectPassedAndRemoveDependenciesIsTrue_PackageDependenciesAreRemoved()
		{
			CreateAction();
			
			uninstallPackageHelper.RemoveDependencies = true;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool removeDependencies = fakeProject.RemoveDependenciesPassedToUninstallPackage;
			
			Assert.IsTrue(removeDependencies);
		}
		
		[Test]
		public void Execute_PackageIdSpecified_PackageSourcePassedIsUsedToCreateRepository()
		{
			CreateAction();
			
			var expectedPackageSource = new PackageSource("http://test");
			uninstallPackageHelper.PackageSource = expectedPackageSource;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			var actualPackageSource = fakeSolution.PackageSourcePassedToCreateProject;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void Execute_PackageIdSpecified_MSBuildProjectPassedIsUsedToCreateRepository()
		{
			CreateAction();
			var expectedProject = ProjectHelper.CreateTestProject();
			uninstallPackageHelper.Project = expectedProject;
			
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			var actualProject = fakeSolution.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void Execute_PackageIdSpecifiedAndForceRemoveIsTrue_PackageIsForcefullyRemoved()
		{
			CreateAction();
			
			uninstallPackageHelper.ForceRemove = true;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool forceRemove = fakeProject.ForceRemovePassedToUninstallPackage;
			
			Assert.IsTrue(forceRemove);
		}
		
		[Test]
		public void Execute_PackageIdSpecifiedAndRemoveDependenciesIsTrue_PackageDependenciesAreRemoved()
		{
			CreateAction();
			
			uninstallPackageHelper.RemoveDependencies = true;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool removeDependencies = fakeProject.RemoveDependenciesPassedToUninstallPackage;
			
			Assert.IsTrue(removeDependencies);
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
			
			uninstallPackageHelper.Version = version;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			var actualPackage = fakeProject.PackagePassedToUninstallPackage;
			
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
