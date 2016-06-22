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

namespace PackageManagement.Tests
{
	[TestFixture]
	public class UpdatePackagesActionTests
	{
		TestableUpdatePackagesAction action;
		IPackageManagementProject project;
		IPackageManagementEvents packageEvents;
		
		void CreateAction()
		{
			CreateSolution();
			packageEvents = MockRepository.GenerateStub<IPackageManagementEvents>();
			action = new TestableUpdatePackagesAction(project, packageEvents);
		}
		
		void CreateActionWithOperations(params PackageOperation[] operations)
		{
			CreateAction();
			action.AddOperations(operations);
		}
		
		void CreateSolution()
		{
			project = MockRepository.GenerateStub<IPackageManagementProject>();
		}
		
		IPackage CreatePackage()
		{
			return MockRepository.GenerateStub<IPackage>();
		}
		
		PackageOperation CreateInstallOperationWithFile(string fileName)
		{
			return PackageOperationHelper.CreateInstallOperationWithFile(fileName);
		}
		
		IPackageScriptRunner CreatePackageScriptRunner()
		{
			return MockRepository.GenerateStub<IPackageScriptRunner>();
		}
		
		FakePackage AddPackageToAction(string id, string version)
		{
			var package = new FakePackage(id, version);
			action.AddPackages(new FakePackage[] { package });
			return package;
		}
		
		[Test]
		public void UpdateDependencies_DefaultValue_IsTrue()
		{
			CreateActionWithOperations();
			
			bool result = action.UpdateDependencies;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void HasPackageScriptsToRun_PackageHasInitPowerShellScript_ReturnsTrue()
		{
			PackageOperation operation = CreateInstallOperationWithFile(@"tools\install.ps1");
			CreateActionWithOperations(operation);
			
			bool result = action.HasPackageScriptsToRun();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void HasPackageScriptsToRun_PackageHasOneTextFile_ReturnsFalse()
		{
			PackageOperation operation = CreateInstallOperationWithFile(@"tools\readme.txt");
			CreateActionWithOperations(operation);
			
			bool result = action.HasPackageScriptsToRun();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Execute_UpdatePackagesAction_PackagesUpdatedUsingProject()
		{
			CreateAction();
			
			action.Execute();
			
			project.AssertWasCalled(p => p.UpdatePackages(action));
		}
		
		[Test]
		public void Execute_UpdatePackagesActionWithOnePackageOperation_PackagesUpdatedUsingProject()
		{
			PackageOperation operation = CreateInstallOperationWithFile(@"tools\readme.txt");
			CreateActionWithOperations(operation);
			
			action.Execute();
			
			project.AssertWasCalled(p => p.UpdatePackages(action));
		}
		
		[Test]
		public void Execute_PackageScriptRunnerSet_RunPackageScriptsActionCreatedUsingPackageScriptRunner()
		{
			CreateAction();
			IPackageScriptRunner expectedRunner = CreatePackageScriptRunner();
			action.PackageScriptRunner = expectedRunner;
			
			action.Execute();
			
			IPackageScriptRunner actualRunner = action.ScriptRunnerPassedToCreateRunPackageScriptsAction;
			Assert.AreEqual(expectedRunner, actualRunner);
		}
		
		[Test]
		public void Execute_PackageScriptRunnerSet_RunPackageScriptsActionCreatedUsingProject()
		{
			CreateAction();
			action.PackageScriptRunner = CreatePackageScriptRunner();
			
			action.Execute();
			
			IPackageManagementProject actualProject = action.ProjectPassedToCreateRunPackageScriptsAction;
			Assert.AreEqual(project, actualProject);
		}
		
		[Test]
		public void Execute_PackageScriptRunnerSet_RunPackageScriptsActionIsDisposed()
		{
			CreateAction();
			action.PackageScriptRunner = CreatePackageScriptRunner();
			
			action.Execute();
			
			Assert.IsTrue(action.IsRunPackageScriptsActionDisposed);
		}
		
		[Test]
		public void Execute_NullPackageScriptRunner_RunPackageScriptsActionIsNotCreated()
		{
			CreateAction();
			action.PackageScriptRunner = null;
			
			action.Execute();
			
			Assert.IsFalse(action.IsRunPackageScriptsActionCreated);
		}
		
		[Test]
		public void Execute_PackageScriptRunnerSet_PackagesAreUpdated()
		{
			CreateAction();
			PackageOperation operation = CreateInstallOperationWithFile(@"tools\readme.txt");
			CreateActionWithOperations(operation);
			action.PackageScriptRunner = CreatePackageScriptRunner();
			
			action.Execute();
			
			project.AssertWasCalled(p => p.UpdatePackages(action));
		}
		
		[Test]
		public void Execute_ActionHasOnePackage_ParentPackagesUpdatedEventIsFired()
		{
			CreateAction();
			AddPackageToAction("Test", "1.0");
			
			action.Execute();
			
			packageEvents.AssertWasCalled(events => events.OnParentPackagesUpdated(action.Packages));
		}
	}
}
