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
	public class UninstallPackageActionTests
	{
		UninstallPackageAction action;
		FakePackageManagementEvents fakePackageManagementEvents;
		UninstallPackageHelper uninstallPackageHelper;
		FakePackageManagementProject fakeProject;
		
		void CreateAction()
		{
			fakePackageManagementEvents = new FakePackageManagementEvents();
			fakeProject = new FakePackageManagementProject();
			action = new UninstallPackageAction(fakeProject, fakePackageManagementEvents);
			uninstallPackageHelper = new UninstallPackageHelper(action);
		}
		
		FakePackage AddOnePackageToProjectSourceRepository(string packageId)
		{
			return fakeProject.FakeSourceRepository.AddFakePackage(packageId);
		}
		
		FakePackage AddOnePackageToProjectSourceRepository(string packageId, string version)
		{
			return fakeProject.FakeSourceRepository.AddFakePackageWithVersion(packageId, version);
		}
		
		void AddFileToPackageBeingUninstalled(string fileName)
		{
			var package = new FakePackage();
			package.AddFile(fileName);
			
			action.Package = package;
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
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			uninstallPackageHelper.ForceRemove = false;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool forceRemove = fakeProject.ForceRemovePassedToUninstallPackage;
			
			Assert.IsFalse(forceRemove);
		}
		
		[Test]
		public void Execute_PackageObjectPassedAndForceRemoveIsTrue_PackageIsForcefullyRemoved()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			uninstallPackageHelper.ForceRemove = true;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool forceRemove = fakeProject.ForceRemovePassedToUninstallPackage;
			
			Assert.IsTrue(forceRemove);
		}
		
		[Test]
		public void Execute_PackageObjectPassedAndRemoveDependenciesIsFalse_PackageDependenciesAreNotRemoved()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			uninstallPackageHelper.RemoveDependencies = false;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool removeDependencies = fakeProject.RemoveDependenciesPassedToUninstallPackage;
			
			Assert.IsFalse(removeDependencies);
		}
		
		[Test]
		public void Execute_PackageObjectPassedAndRemoveDependenciesIsTrue_PackageDependenciesAreRemoved()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			uninstallPackageHelper.RemoveDependencies = true;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool removeDependencies = fakeProject.RemoveDependenciesPassedToUninstallPackage;
			
			Assert.IsTrue(removeDependencies);
		}
		
		[Test]
		public void Execute_PackageIdSpecifiedAndForceRemoveIsTrue_PackageIsForcefullyRemoved()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			uninstallPackageHelper.ForceRemove = true;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool forceRemove = fakeProject.ForceRemovePassedToUninstallPackage;
			
			Assert.IsTrue(forceRemove);
		}
		
		[Test]
		public void Execute_PackageIdSpecifiedAndRemoveDependenciesIsTrue_PackageDependenciesAreRemoved()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			uninstallPackageHelper.RemoveDependencies = true;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			bool removeDependencies = fakeProject.RemoveDependenciesPassedToUninstallPackage;
			
			Assert.IsTrue(removeDependencies);
		}
		
		[Test]
		public void Execute_VersionSpecified_VersionUsedWhenSearchingForPackage()
		{
			CreateAction();
			
			FakePackage recentPackage = AddOnePackageToProjectSourceRepository("PackageId", "1.2.0.0");
			FakePackage oldPackage = AddOnePackageToProjectSourceRepository("PackageId", "1.0.0.0");
			FakePackage package = AddOnePackageToProjectSourceRepository("PackageId", "1.1.0");
			
			uninstallPackageHelper.Version = package.Version;
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
		
		[Test]
		public void HasPackageScriptsToRun_OnePackageInOperationsHasUninstallPowerShellScript_ReturnsTrue()
		{
			CreateAction();
			AddFileToPackageBeingUninstalled(@"tools\uninstall.ps1");
			
			bool hasPackageScripts = action.HasPackageScriptsToRun();
			
			Assert.IsTrue(hasPackageScripts);
		}
		
		[Test]
		public void HasPackageScriptsToRun_OnePackageInOperationsHasNoFiles_ReturnsFalse()
		{
			CreateAction();
			action.Package = new FakePackage();
			
			bool hasPackageScripts = action.HasPackageScriptsToRun();
			
			Assert.IsFalse(hasPackageScripts);
		}
		
		[Test]
		public void HasPackageScriptsToRun_OnePackageInOperationsHasUninstallPowerShellScriptInUpperCase_ReturnsTrue()
		{
			CreateAction();
			AddFileToPackageBeingUninstalled(@"tools\UNINSTALL.PS1");
			
			bool hasPackageScripts = action.HasPackageScriptsToRun();
			
			Assert.IsTrue(hasPackageScripts);
		}
		
		[Test]
		public void AllowPreleasePackages_DefaultValue_IsTrue()
		{
			CreateAction();
			
			Assert.IsTrue(action.AllowPrereleaseVersions);
		}
		
	}
}
