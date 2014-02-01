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

namespace PackageManagement.Tests
{
	[TestFixture]
	public class UpdateSolutionPackagesActionTests
	{
		TestableUpdateSolutionPackagesAction action;
		IPackageManagementSolution solution;
		List<IPackageManagementProject> projects;
		IPackageRepository sourceRepository;
		IPackageManagementEvents packageEvents;
		
		void CreateAction()
		{
			CreateSolution();
			packageEvents = MockRepository.GenerateStub<IPackageManagementEvents>();
			action = new TestableUpdateSolutionPackagesAction(solution, packageEvents);
		}
		
		void CreateActionWithOperations(params PackageOperation[] operations)
		{
			CreateAction();
			action.AddOperations(operations);
		}
		
		void CreateSolution()
		{
			projects = new List<IPackageManagementProject>();
			sourceRepository = MockRepository.GenerateStub<IPackageRepository>();
			
			solution = MockRepository.GenerateStub<IPackageManagementSolution>();
			solution
				.Stub(s => s.GetProjects(sourceRepository))
				.Return(projects);
		}
		
		IPackageFromRepository CreatePackage(string packageId, string version)
		{
			var package = MockRepository.GenerateStub<IPackageFromRepository>();
			package.Stub(p => p.Id).Return(packageId);
			package.Stub(p => p.Version).Return(new SemanticVersion(version));
			package.Stub(p => p.Repository).Return(sourceRepository);
			return package;
		}
		
		void ProjectHasOlderVersionOfPackage(IPackageManagementProject project, IPackageFromRepository package)
		{
			project.Stub(p => p.HasOlderPackageInstalled(package)).Return(true);
		}
		
		PackageOperation CreateInstallOperationWithFile(string fileName)
		{
			return PackageOperationHelper.CreateInstallOperationWithFile(fileName);
		}
		
		IPackageScriptRunner CreatePackageScriptRunner()
		{
			return MockRepository.GenerateStub<IPackageScriptRunner>();
		}
		
		IPackageManagementProject AddProjectToSolution()
		{
			IPackageManagementProject project = MockRepository.GenerateStub<IPackageManagementProject>();
			projects.Add(project);
			return project;
		}
		
		IPackageFromRepository AddPackageToAction(string packageId, string version)
		{
			IPackageFromRepository package = CreatePackage(packageId, version);
			action.AddPackages(new IPackageFromRepository[] { package });
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
		public void Execute_OneProjectThatHasOlderVersionOfPackageBeingUpdated_PackageOperationsAreRun()
		{
			PackageOperation operation = CreateInstallOperationWithFile(@"tools\readme.txt");
			CreateActionWithOperations(operation);
			IPackageFromRepository package = AddPackageToAction("Test", "1.0");
			IPackageManagementProject project = AddProjectToSolution();
			var expectedOperations = new PackageOperation[] { operation };
			
			action.Execute();
			
			project.AssertWasCalled(p => p.RunPackageOperations(expectedOperations));
		}
		
		[Test]
		public void Execute_OneProjectThatHasOlderVersionOfPackageBeingUpdated_PackageReferenceUpdatedInProject()
		{
			CreateAction();
			IPackageFromRepository package = AddPackageToAction("Test", "1.0");
			IPackageManagementProject project = AddProjectToSolution();
			ProjectHasOlderVersionOfPackage(project, package);
			
			action.Execute();
			
			project.AssertWasCalled(p => p.UpdatePackageReference(package, action));
		}
		
		[Test]
		public void Execute_OneProjectThatHasOlderVersionsOfTwoPackagesBeingUpdated_PackageReferenceUpdatedInProjectForBothPackages()
		{
			CreateAction();
			IPackageFromRepository package1 = AddPackageToAction("A", "1.0");
			IPackageFromRepository package2 = AddPackageToAction("B", "1.0");
			IPackageManagementProject project = AddProjectToSolution();
			ProjectHasOlderVersionOfPackage(project, package1);
			ProjectHasOlderVersionOfPackage(project, package2);
			
			action.Execute();
			
			project.AssertWasCalled(p => p.UpdatePackageReference(package1, action));
			project.AssertWasCalled(p => p.UpdatePackageReference(package2, action));
		}
		
		[Test]
		public void Execute_TwoProjectsThatHaveOlderVersionsOfPackageBeingUpdated_PackageReferenceUpdatedInBothProjects()
		{
			CreateAction();
			IPackageFromRepository package = AddPackageToAction("A", "1.0");
			IPackageManagementProject project1 = AddProjectToSolution();
			IPackageManagementProject project2 = AddProjectToSolution();
			ProjectHasOlderVersionOfPackage(project1, package);
			ProjectHasOlderVersionOfPackage(project2, package);
			
			action.Execute();
			
			project1.AssertWasCalled(p => p.UpdatePackageReference(package, action));
			project2.AssertWasCalled(p => p.UpdatePackageReference(package, action));
		}
		
		[Test]
		public void Execute_TwoProjectsAndOnlyOneHasOlderVersionsOfPackageBeingUpdated_PackageReferenceUpdatedInOnlyOneProject()
		{
			CreateAction();
			IPackageFromRepository package = AddPackageToAction("A", "1.0");
			IPackageManagementProject project1 = AddProjectToSolution();
			IPackageManagementProject project2 = AddProjectToSolution();
			ProjectHasOlderVersionOfPackage(project2, package);
			
			action.Execute();
			
			project1.AssertWasNotCalled(p => p.UpdatePackageReference(package, action));
			project2.AssertWasCalled(p => p.UpdatePackageReference(package, action));
		}
		
		[Test]
		public void Execute_PackageScriptRunnerSet_RunPackageScriptsActionCreatedUsingPackageScriptRunner()
		{
			CreateAction();
			IPackageScriptRunner expectedRunner = CreatePackageScriptRunner();
			action.PackageScriptRunner = expectedRunner;
			AddPackageToAction("Test", "1.0");
			AddProjectToSolution();
			
			action.Execute();
			
			IPackageScriptRunner actualRunner = action.ScriptRunnerPassedToCreateRunPackageScriptsAction;
			Assert.AreEqual(expectedRunner, actualRunner);
		}
		
		[Test]
		public void Execute_PackageScriptRunnerSet_RunPackageScriptsActionCreatedUsingProjects()
		{
			CreateAction();
			AddPackageToAction("Test", "1.0");
			IPackageManagementProject project1 = AddProjectToSolution();
			IPackageManagementProject project2 = AddProjectToSolution();
			var expectedProjects = new IPackageManagementProject[] {
				project1,
				project2
			};
			action.PackageScriptRunner = CreatePackageScriptRunner();
			
			action.Execute();
			
			Assert.AreEqual(expectedProjects, action.ProjectsPassedToCreateRunPackageScriptsAction);
		}
		
		[Test]
		public void Execute_PackageScriptRunnerSet_RunPackageScriptsActionIsDisposed()
		{
			CreateAction();
			AddPackageToAction("Test", "1.0");
			AddProjectToSolution();
			action.PackageScriptRunner = CreatePackageScriptRunner();
			
			action.Execute();
			
			Assert.IsTrue(action.IsRunPackageScriptsActionDisposed);
		}
		
		[Test]
		public void Execute_NullPackageScriptRunner_RunPackageScriptsActionIsNotCreated()
		{
			CreateAction();
			AddPackageToAction("Test", "1.0");
			AddProjectToSolution();
			action.PackageScriptRunner = null;
			
			action.Execute();
			
			Assert.IsFalse(action.IsRunPackageScriptsActionCreated);
		}
		
		[Test]
		public void Execute_PackageScriptRunnerSet_PackageReferenceUpdatedInProject()
		{
			CreateAction();
			IPackageFromRepository package = AddPackageToAction("Test", "1.0");
			IPackageManagementProject project = AddProjectToSolution();
			ProjectHasOlderVersionOfPackage(project, package);
			action.PackageScriptRunner = CreatePackageScriptRunner();
			
			action.Execute();
			
			project.AssertWasCalled(p => p.UpdatePackageReference(package, action));
		}
		
		[Test]
		public void Execute_PackageScriptRunnerSet_ProjectsFromSolutionReadOnlyOnce()
		{
			CreateAction();
			IPackageFromRepository package = AddPackageToAction("Test", "1.0");
			IPackageManagementProject project = AddProjectToSolution();
			ProjectHasOlderVersionOfPackage(project, package);
			action.PackageScriptRunner = CreatePackageScriptRunner();
			
			action.Execute();
			
			solution.AssertWasCalled(
				s => s.GetProjects(sourceRepository),
				setupConstraint => setupConstraint.Repeat.Once());
		}
		
		[Test]
		public void Execute_NullPackageScriptRunner_ProjectsFromSolutionReadOnlyOnce()
		{
			CreateAction();
			IPackageFromRepository package = AddPackageToAction("Test", "1.0");
			IPackageManagementProject project = AddProjectToSolution();
			ProjectHasOlderVersionOfPackage(project, package);
			action.PackageScriptRunner = null;
			
			action.Execute();
			
			solution.AssertWasCalled(
				s => s.GetProjects(sourceRepository),
				setupConstraint => setupConstraint.Repeat.Once());
		}
		
		[Test]
		public void Execute_TwoProjectsBeingUpdated_LoggerSetOnBothProjects()
		{
			CreateAction();
			ILogger logger = MockRepository.GenerateStub<ILogger>();
			action.Logger = logger;
			IPackageFromRepository package = AddPackageToAction("A", "1.0");
			IPackageManagementProject project1 = AddProjectToSolution();
			IPackageManagementProject project2 = AddProjectToSolution();
			
			action.Execute();
			
			Assert.AreEqual(logger, project1.Logger);
			Assert.AreEqual(logger, project2.Logger);
		}
		
		[Test]
		public void Execute_OneProjectThatHasOlderVersionOfPackageBeingUpdated_PackagesUpdatedEventIsFired()
		{
			PackageOperation operation = CreateInstallOperationWithFile(@"tools\readme.txt");
			CreateActionWithOperations(operation);
			IPackageFromRepository package = AddPackageToAction("Test", "1.0");
			IPackageManagementProject project = AddProjectToSolution();
			var expectedOperations = new PackageOperation[] { operation };
			
			action.Execute();
			
			packageEvents.AssertWasCalled(events => events.OnParentPackagesUpdated(action.Packages));
		}
	}
}
