// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class RunPackageScriptsActionTests
	{
		RunPackageScriptsAction action;
		FakePackageManagementProject fakeProject;
		FakePackageScriptFactory fakeScriptFactory;
		FakePackageScriptRunner fakeScriptRunner;
		IGlobalMSBuildProjectCollection globalMSBuildProjectCollection;
		
		void CreateAction()
		{
			fakeProject = new FakePackageManagementProject();
			fakeScriptFactory = new FakePackageScriptFactory();
			fakeScriptRunner = new FakePackageScriptRunner();
			globalMSBuildProjectCollection = MockRepository.GenerateStub<IGlobalMSBuildProjectCollection>();
			action = new RunPackageScriptsAction(
				fakeProject,
				fakeScriptRunner,
				fakeScriptFactory,
				globalMSBuildProjectCollection);
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
		public void Constructor_PackageReferenceIsBeingRemoved_PackageUninstallScriptIsRun()
		{
			CreateAction();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			fakeProject.FirePackageReferenceRemovingEvent(eventArgs);
			
			IPackageScript actualScript = fakeScriptRunner.FirstScriptRun;
			FakePackageScript expectedScript = fakeScriptFactory.FirstPackageUninstallScriptCreated;
			
			Assert.AreEqual(expectedScript, actualScript);
		}
		
		[Test]
		public void Constructor_PackageReferenceIsBeingRemoved_PackageUninstallScriptIsCreated()
		{
			CreateAction();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs(@"d:\projects\myproject\packages\test");
			fakeProject.FirePackageReferenceRemovingEvent(eventArgs);
			
			string path = fakeScriptFactory.FirstPackageInstallDirectoryPassed;
			
			Assert.AreEqual(@"d:\projects\myproject\packages\test", path);
		}
		
		[Test]
		public void Dispose_PackageReferenceIsBeingRemoved_PackageUninstallScriptIsNotRun()
		{
			CreateAction();
			action.Dispose();
			
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			fakeProject.FirePackageReferenceRemovingEvent(eventArgs);
			
			int count = fakeScriptFactory.FakePackageUninstallScriptsCreated.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void Constructor_PackageReferenceIsBeingRemoved_UninstallScriptIsPassedProject()
		{
			CreateAction();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			fakeProject.FirePackageReferenceRemovingEvent(eventArgs);
			
			IPackageManagementProject project = fakeScriptFactory.FirstPackageUninstallScriptCreated.Project;
			
			Assert.AreEqual(fakeProject, project);
		}
		
		[Test]
		public void Constructor_PackageReferenceIsBeingRemoved_UninstallScriptIsPassedPackageFromPackageOperationEventArgs()
		{
			CreateAction();
			var expectedPackage = new FakePackage();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs(expectedPackage);
			fakeProject.FirePackageReferenceRemovingEvent(eventArgs);
			
			IPackage package = fakeScriptFactory.FirstPackageUninstallScriptCreated.Package;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void Dispose_OneProjectAddedToGlobalMSBuildProjectCollection_GlobalMSBuildProjectCollectionIsDisposed()
		{
			CreateAction();
			
			action.Dispose();
			
			globalMSBuildProjectCollection.AssertWasCalled(projectCollection => projectCollection.Dispose());
		}
		
		[Test]
		public void Constructor_NewInstance_OneProjectAddedToGlobalMSBuildProjectCollection()
		{
			CreateAction();
			
			globalMSBuildProjectCollection.AssertWasCalled(collection => collection.AddProject(fakeProject));
		}
	}
}
