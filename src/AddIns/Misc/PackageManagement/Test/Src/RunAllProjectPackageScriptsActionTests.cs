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
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class RunAllProjectPackageScriptsActionTests
	{
		RunAllProjectPackageScriptsAction action;
		List<IPackageManagementProject> projects;
		IPackageScriptRunner scriptRunner;
		IPackageScriptFactory scriptFactory;
		IGlobalMSBuildProjectCollection msbuildProjectCollection;
		
		[SetUp]
		public void Init()
		{
			CreateProjectsList();
		}
		
		void CreateProjectsList()
		{
			projects = new List<IPackageManagementProject>();
		}
		
		void CreateAction()
		{
			scriptRunner = MockRepository.GenerateStub<IPackageScriptRunner>();
			scriptFactory = MockRepository.GenerateStub<IPackageScriptFactory>();
			msbuildProjectCollection = MockRepository.GenerateStub<IGlobalMSBuildProjectCollection>();
			action = new RunAllProjectPackageScriptsAction(
				scriptRunner,
				projects,
				scriptFactory,
				msbuildProjectCollection);
		}
		
		IPackageManagementProject AddProject()
		{
			IPackageManagementProject project = MockRepository.GenerateStub<IPackageManagementProject>();
			projects.Add(project);
			return project;
		}
		
		PackageOperationEventArgs CreatePackageOperationEventArgs()
		{
			IPackage package = CreatePackage();
			return CreatePackageOperationEventArgs(package);
		}
		
		IPackage CreatePackage()
		{
			return new TestPackageHelper("Test", "1.0").Package;
		}
		
		PackageOperationEventArgs CreatePackageOperationEventArgs(
			IPackage package,
			string installPath = @"d:\projects\myproject\packages\test")
		{
			return new PackageOperationEventArgs(package, null, installPath);
		}
		
		void FirePackageInstalledEvent(IPackageManagementProject project, PackageOperationEventArgs eventArgs)
		{
			project.Raise(p => p.PackageInstalled += null, null, eventArgs);
		}
		
		void FirePackageReferenceAddedEvent(IPackageManagementProject project, PackageOperationEventArgs eventArgs)
		{
			project.Raise(p => p.PackageReferenceAdded += null, null, eventArgs);
		}
		
		void FirePackageReferenceRemovingEvent(IPackageManagementProject project, PackageOperationEventArgs eventArgs)
		{
			project.Raise(p => p.PackageReferenceRemoving += null, null, eventArgs);
		}
		
		IPackageScript CreatePackageScript()
		{
			return MockRepository.GenerateStub<IPackageScript>();
		}
		
		void SetInitScriptToReturnFromScriptFactory(IPackageScript initScript)
		{
			scriptFactory.Stub(factory => factory.CreatePackageInitializeScript(
				Arg<IPackage>.Is.Anything,
				Arg<string>.Is.Anything))
				.Return(initScript);
		}
		
		void SetInstallScriptToReturnFromScriptFactory(IPackageScript installScript)
		{
			scriptFactory.Stub(factory => factory.CreatePackageInstallScript(
				Arg<IPackage>.Is.Anything,
				Arg<string>.Is.Anything))
				.Return(installScript);
		}
		
		void SetUninstallScriptToReturnFromScriptFactory(IPackageScript uninstallScript)
		{
			scriptFactory.Stub(factory => factory.CreatePackageUninstallScript(
				Arg<IPackage>.Is.Anything,
				Arg<string>.Is.Anything))
				.Return(uninstallScript);
		}
		
		[Test]
		public void Constructor_OneProjectAndOnePackageIsInstalled_PackageInitializeScriptCreated()
		{
			IPackageManagementProject project = AddProject();
			CreateAction();
			IPackageScript initScript = CreatePackageScript();
			SetInitScriptToReturnFromScriptFactory(initScript);
			IPackage package = CreatePackage();
			string installPath = @"d:\projects\MyProject\packages\foo";
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs(package, installPath);
			
			FirePackageInstalledEvent(project, eventArgs);
			
			scriptFactory.AssertWasCalled(factory => factory.CreatePackageInitializeScript(package, installPath));
		}
		
		[Test]
		public void Constructor_OneProjectAndOnePackageIsInstalled_PackageInitializeScriptIsRun()
		{
			IPackageManagementProject project = AddProject();
			CreateAction();
			IPackageScript initScript = CreatePackageScript();
			SetInitScriptToReturnFromScriptFactory(initScript);
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			
			FirePackageInstalledEvent(project, eventArgs);
			
			scriptRunner.AssertWasCalled(runner => runner.Run(initScript));
		}
		
		[Test]
		public void Constructor_OneProjectAndOnePackageIsInstalled_ProjectSetForPackageScript()
		{
			IPackageManagementProject project = AddProject();
			CreateAction();
			IPackageScript initScript = CreatePackageScript();
			SetInitScriptToReturnFromScriptFactory(initScript);
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			
			FirePackageInstalledEvent(project, eventArgs);
			
			Assert.AreEqual(project, initScript.Project);
		}
		
		[Test]
		public void Constructor_TwoProjectsAndPackageIsInstalledForSecondProject_PackageInitializeScriptIsRun()
		{
			IPackageManagementProject project1 = AddProject();
			IPackageManagementProject project2 = AddProject();
			CreateAction();
			IPackageScript initScript = CreatePackageScript();
			SetInitScriptToReturnFromScriptFactory(initScript);
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			
			FirePackageInstalledEvent(project2, eventArgs);
			
			scriptRunner.AssertWasCalled(runner => runner.Run(initScript));
		}
		
		[Test]
		public void Constructor_TwoProjectsAndPackageIsInstalledForSecondProject_SecondProjectSetForPackageScript()
		{
			IPackageManagementProject project1 = AddProject();
			IPackageManagementProject project2 = AddProject();
			CreateAction();
			IPackageScript initScript = CreatePackageScript();
			SetInitScriptToReturnFromScriptFactory(initScript);
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			
			FirePackageInstalledEvent(project2, eventArgs);
			
			Assert.AreEqual(project2, initScript.Project);
		}
		
		[Test]
		public void Dispose_OneProjectAndOnePackageIsInstalledAfterScriptsActionIsDisposed_PackageInitializeScriptIsNotRun()
		{
			IPackageManagementProject project = AddProject();
			CreateAction();
			IPackageScript initScript = CreatePackageScript();
			SetInitScriptToReturnFromScriptFactory(initScript);
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			
			action.Dispose();
			FirePackageInstalledEvent(project, eventArgs);
			
			scriptRunner.AssertWasNotCalled(runner => runner.Run(Arg<IPackageScript>.Is.Anything));
		}
		
		[Test]
		public void Dispose_TwoProjectsAndOnePackageIsInstalledForSecondProjectAfterScriptsActionIsDisposed_PackageInitializeScriptIsNotRun()
		{
			IPackageManagementProject project1 = AddProject();
			IPackageManagementProject project2 = AddProject();
			CreateAction();
			IPackageScript initScript = CreatePackageScript();
			SetInitScriptToReturnFromScriptFactory(initScript);
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			
			action.Dispose();
			FirePackageInstalledEvent(project2, eventArgs);
			
			scriptRunner.AssertWasNotCalled(runner => runner.Run(Arg<IPackageScript>.Is.Anything));
		}
		
		[Test]
		public void Constructor_OneProjectAndPackageReferencedAddedToProject_PackageInstallScriptCreated()
		{
			IPackageManagementProject project = AddProject();
			CreateAction();
			IPackageScript installScript = CreatePackageScript();
			SetInstallScriptToReturnFromScriptFactory(installScript);
			IPackage package = CreatePackage();
			string installPath = @"d:\projects\MyProject\packages\foo";
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs(package, installPath);
			
			FirePackageReferenceAddedEvent(project, eventArgs);
			
			scriptFactory.AssertWasCalled(factory => factory.CreatePackageInstallScript(package, installPath));
		}
		
		[Test]
		public void Constructor_TwoProjectsAndPackageReferencedAddedToSecondProject_PackageInstallScriptIsRun()
		{
			IPackageManagementProject project1 = AddProject();
			IPackageManagementProject project2 = AddProject();
			CreateAction();
			IPackageScript installScript = CreatePackageScript();
			SetInstallScriptToReturnFromScriptFactory(installScript);
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			
			FirePackageReferenceAddedEvent(project2, eventArgs);
			
			scriptRunner.AssertWasCalled(runner => runner.Run(installScript));
		}
		
		[Test]
		public void Constructor_TwoProjectsAndPackageReferencedIsAddedForSecondProject_SecondProjectSetForPackageScript()
		{
			IPackageManagementProject project1 = AddProject();
			IPackageManagementProject project2 = AddProject();
			CreateAction();
			IPackageScript installScript = CreatePackageScript();
			SetInstallScriptToReturnFromScriptFactory(installScript);
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			
			FirePackageReferenceAddedEvent(project2, eventArgs);
			
			Assert.AreEqual(project2, installScript.Project);
		}
		
		[Test]
		public void Dispose_OneProjectAndPackageReferencedAddedToProjectAfterActionIsDisposed_PackageInstallScriptIsNotRun()
		{
			IPackageManagementProject project = AddProject();
			CreateAction();
			IPackageScript installScript = CreatePackageScript();
			SetInstallScriptToReturnFromScriptFactory(installScript);
			IPackage package = CreatePackage();
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			
			action.Dispose();
			FirePackageReferenceAddedEvent(project, eventArgs);
			
			scriptRunner.AssertWasNotCalled(runner => runner.Run(installScript));
		}
		
		[Test]
		public void Constructor_OneProjectAndPackageReferencedRemovedFromProject_PackageUninstallScriptCreated()
		{
			IPackageManagementProject project = AddProject();
			CreateAction();
			IPackageScript uninstallScript = CreatePackageScript();
			SetUninstallScriptToReturnFromScriptFactory(uninstallScript);
			IPackage package = CreatePackage();
			string installPath = @"d:\projects\MyProject\packages\foo";
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs(package, installPath);
			
			FirePackageReferenceRemovingEvent(project, eventArgs);
			
			scriptFactory.AssertWasCalled(factory => factory.CreatePackageUninstallScript(package, installPath));
		}
		
		[Test]
		public void Constructor_TwoProjectsAndPackageReferencedRemovedFromSecondProject_PackageUninstallScriptIsRun()
		{
			IPackageManagementProject project1 = AddProject();
			IPackageManagementProject project2 = AddProject();
			CreateAction();
			IPackageScript uninstallScript = CreatePackageScript();
			SetUninstallScriptToReturnFromScriptFactory(uninstallScript);
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			
			FirePackageReferenceRemovingEvent(project2, eventArgs);
			
			scriptRunner.AssertWasCalled(runner => runner.Run(uninstallScript));
		}
		
		[Test]
		public void Constructor_TwoProjectsAndPackageReferencedIsRemovedFromSecondProject_SecondProjectSetForPackageScript()
		{
			IPackageManagementProject project1 = AddProject();
			IPackageManagementProject project2 = AddProject();
			CreateAction();
			IPackageScript uninstallScript = CreatePackageScript();
			SetUninstallScriptToReturnFromScriptFactory(uninstallScript);
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			
			FirePackageReferenceRemovingEvent(project2, eventArgs);
			
			Assert.AreEqual(project2, uninstallScript.Project);
		}
		
		[Test]
		public void Dispose_OneProjectAndPackageReferencedRemovedFromProjectAfterActionIsDisposed_PackageUninstallScriptIsNotRun()
		{
			IPackageManagementProject project = AddProject();
			CreateAction();
			IPackageScript uninstallScript = CreatePackageScript();
			SetUninstallScriptToReturnFromScriptFactory(uninstallScript);
			PackageOperationEventArgs eventArgs = CreatePackageOperationEventArgs();
			
			action.Dispose();
			FirePackageReferenceRemovingEvent(project, eventArgs);
			
			scriptRunner.AssertWasNotCalled(runner => runner.Run(uninstallScript));
		}
		
		[Test]
		public void Dispose_OneProject_GlobalMSBuildProjectsCollectionIsDisposed()
		{
			IPackageManagementProject project = AddProject();
			CreateAction();
			
			action.Dispose();
			
			msbuildProjectCollection.AssertWasCalled(projectCollection => projectCollection.Dispose());
		}
		
		[Test]
		public void Constructor_TwoProjects_BothProjectsAddedToGlobalMSBuildProjectCollection()
		{
			IPackageManagementProject project1 = AddProject();
			IPackageManagementProject project2 = AddProject();
			
			CreateAction();
			
			msbuildProjectCollection.AssertWasCalled(projectCollection => projectCollection.AddProject(project1));
			msbuildProjectCollection.AssertWasCalled(projectCollection => projectCollection.AddProject(project2));
		}
	}
}
