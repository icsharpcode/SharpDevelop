// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class RunPackageScriptsActionTests
	{
		RunPackageScriptsAction action;
		FakePackageManagementProject fakeProject;
		FakePackageScriptFactory fakeScriptFactory;
		FakePackageScriptRunner fakeScriptRunner;
			
		void CreateAction()
		{
			fakeProject = new FakePackageManagementProject();
			fakeScriptFactory = new FakePackageScriptFactory();
			fakeScriptRunner = new FakePackageScriptRunner();
			action = new RunPackageScriptsAction(fakeProject, fakeScriptRunner, fakeScriptFactory);
		}
		
		PackageOperationEventArgs CreatePackageOperationEventArgs()
		{
			return CreatePackageOperationEventArgs(@"d:\projects\myproject\packages\test");
		}
		
		PackageOperationEventArgs CreatePackageOperationEventArgs(string installPath)
		{
			var package = new FakePackage("Test");
			return CreatePackageOperationEventArgs(package, installPath);
		}
		
		PackageOperationEventArgs CreatePackageOperationEventArgs(FakePackage package)
		{
			string installPath = @"d:\projects\myproject\packages\test";
			return CreatePackageOperationEventArgs(package, installPath);
		}
		
		PackageOperationEventArgs CreatePackageOperationEventArgs(FakePackage package, string installPath)
		{
			return new PackageOperationEventArgs(package, null, installPath);
		}
		
		[Test]
		public void Constructor_PackageIsInstalled_PackageInitScriptIsRun()
		{
			CreateAction();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			fakeProject.FirePackageInstalledEvent(eventArgs);
			
			IPackageScript actualScript = fakeScriptRunner.FirstScriptRun;
			FakePackageScript expectedScript = fakeScriptFactory.FirstPackageInitializeScriptCreated;
			
			Assert.AreEqual(expectedScript, actualScript);
		}
		
		[Test]
		public void Constructor_PackageIsInstalled_PackageInitScriptIsCreated()
		{
			CreateAction();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs(@"d:\projects\myproject\packages\test");

			fakeProject.FirePackageInstalledEvent(eventArgs);
			
			string path = fakeScriptFactory.FirstPackageInstallDirectoryPassed;
			
			Assert.AreEqual(@"d:\projects\myproject\packages\test", path);
		}
		
		[Test]
		public void Dispose_PackageIsInstalled_PackageInitScriptIsNotRun()
		{
			CreateAction();
			action.Dispose();
			
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			fakeProject.FirePackageInstalledEvent(eventArgs);
			
			int count = fakeScriptFactory.FakePackageInstallScriptsCreated.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void Constructor_PackageIsInstalled_PackageInitScriptIsPassedPackage()
		{
			CreateAction();
			var expectedPackage = new FakePackage();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs(expectedPackage);
			fakeProject.FirePackageInstalledEvent(eventArgs);
			
			IPackage package = fakeScriptFactory.FirstPackageInitializeScriptCreated.Package;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void Constructor_PackageReferenceIsAdded_PackageInstallScriptIsRun()
		{
			CreateAction();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			fakeProject.FirePackageReferenceAddedEvent(eventArgs);
			
			IPackageScript actualScript = fakeScriptRunner.FirstScriptRun;
			FakePackageScript expectedScript = fakeScriptFactory.FirstPackageInstallScriptCreated;
			
			Assert.AreEqual(expectedScript, actualScript);
		}
		
		[Test]
		public void Constructor_PackageReferenceIsAdded_PackageInstallScriptIsCreated()
		{
			CreateAction();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs(@"d:\projects\myproject\packages\test");
			fakeProject.FirePackageReferenceAddedEvent(eventArgs);
			
			string path = fakeScriptFactory.FirstPackageInstallDirectoryPassed;
			
			Assert.AreEqual(@"d:\projects\myproject\packages\test", path);
		}
		
		[Test]
		public void Dispose_PackageReferenceIsAdded_PackageInstallScriptIsNotRun()
		{
			CreateAction();
			action.Dispose();
			
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			fakeProject.FirePackageReferenceAddedEvent(eventArgs);
			
			int count = fakeScriptFactory.FakePackageInstallScriptsCreated.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void Constructor_PackageReferenceIsAdded_InstallScriptIsPassedProject()
		{
			CreateAction();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			fakeProject.FirePackageReferenceAddedEvent(eventArgs);
			
			IPackageManagementProject project = fakeScriptFactory.FirstPackageInstallScriptCreated.Project;
			
			Assert.AreEqual(fakeProject, project);
		}
		
		[Test]
		public void Constructor_PackageReferenceIsAdded_InstallScriptIsPassedPackageFromPackageOperationEventArgs()
		{
			CreateAction();
			var expectedPackage = new FakePackage();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs(expectedPackage);
			fakeProject.FirePackageReferenceAddedEvent(eventArgs);
			
			IPackage package = fakeScriptFactory.FirstPackageInstallScriptCreated.Package;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void Constructor_PackageReferenceIsRemoved_PackageUninstallScriptIsRun()
		{
			CreateAction();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			fakeProject.FirePackageReferenceRemovedEvent(eventArgs);
			
			IPackageScript actualScript = fakeScriptRunner.FirstScriptRun;
			FakePackageScript expectedScript = fakeScriptFactory.FirstPackageUninstallScriptCreated;
			
			Assert.AreEqual(expectedScript, actualScript);
		}
		
		[Test]
		public void Constructor_PackageReferenceIsRemoved_PackageUninstallScriptIsCreated()
		{
			CreateAction();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs(@"d:\projects\myproject\packages\test");
			fakeProject.FirePackageReferenceRemovedEvent(eventArgs);
			
			string path = fakeScriptFactory.FirstPackageInstallDirectoryPassed;
			
			Assert.AreEqual(@"d:\projects\myproject\packages\test", path);
		}
		
		[Test]
		public void Dispose_PackageReferenceIsRemoved_PackageUninstallScriptIsNotRun()
		{
			CreateAction();
			action.Dispose();
			
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			fakeProject.FirePackageReferenceRemovedEvent(eventArgs);
			
			int count = fakeScriptFactory.FakePackageUninstallScriptsCreated.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void Constructor_PackageReferenceIsRemoved_UninstallScriptIsPassedProject()
		{
			CreateAction();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			fakeProject.FirePackageReferenceRemovedEvent(eventArgs);
			
			IPackageManagementProject project = fakeScriptFactory.FirstPackageUninstallScriptCreated.Project;
			
			Assert.AreEqual(fakeProject, project);
		}
		
		[Test]
		public void Constructor_PackageReferenceIsRemoved_UninstallScriptIsPassedPackageFromPackageOperationEventArgs()
		{
			CreateAction();
			var expectedPackage = new FakePackage();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs(expectedPackage);
			fakeProject.FirePackageReferenceRemovedEvent(eventArgs);
			
			IPackage package = fakeScriptFactory.FirstPackageUninstallScriptCreated.Package;
			
			Assert.AreEqual(expectedPackage, package);
		}
	}
}
