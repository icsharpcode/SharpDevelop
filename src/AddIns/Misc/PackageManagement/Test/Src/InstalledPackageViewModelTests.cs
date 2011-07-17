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
	public class InstalledPackageViewModelTests
	{
		TestableInstalledPackageViewModel viewModel;
		FakePackageManagementSolution fakeSolution;
		List<IPackageManagementSelectedProject> fakeSelectedProjects;
		IList<ProcessPackageAction> packageActions;
		FakePackageActionRunner fakeActionRunner;
		
		void CreateViewModel()
		{
			viewModel = new TestableInstalledPackageViewModel();
			fakeSolution = viewModel.FakeSolution;
			fakeActionRunner = viewModel.FakeActionRunner;
		}
		
		void CreateEmptyFakeSelectedProjectsList()
		{
			fakeSelectedProjects = new List<IPackageManagementSelectedProject>();			
		}
		
		void CreateOneFakeSelectedProject(string name)
		{
			CreateEmptyFakeSelectedProjectsList();
			AddFakeSelectedProject(name);
		}
		
		void AddFakeSelectedProject(string name)
		{
			fakeSelectedProjects.Add(new FakeSelectedProject(name));
		}
		
		FakeSelectedProject FirstSelectedProject {
			get { return fakeSelectedProjects[0] as FakeSelectedProject; }
		}
		
		void GetPackageActionsForSelectedProjects()
		{
			packageActions = viewModel.GetProcessPackageActionsForSelectedProjects(fakeSelectedProjects);
		}
		
		void AddProjectToSolution()
		{
			TestableProject project = ProjectHelper.CreateTestProject();
			fakeSolution.FakeMSBuildProjects.Add(project);
		}
		
		void CreateViewModelWithTwoProjectsSelected(string projectName1, string projectName2)
		{
			CreateViewModel();
			AddProjectToSolution();
			AddProjectToSolution();
			fakeSolution.FakeMSBuildProjects[0].Name = projectName1;
			fakeSolution.FakeMSBuildProjects[1].Name = projectName2;
			fakeSolution.NoProjectsSelected();
			
			fakeSolution.AddFakeProjectToReturnFromGetProject(projectName1);
			fakeSolution.AddFakeProjectToReturnFromGetProject(projectName2);
		}
		
		[Test]
		public void GetProcessPackageActionsForSelectedProjects_OneProjectIsSelected_ReturnsOneAction()
		{
			CreateViewModel();
			CreateOneFakeSelectedProject("Test");
			FirstSelectedProject.IsSelected = true;
			GetPackageActionsForSelectedProjects();
		
			Assert.AreEqual(1, packageActions.Count);
		}
		
		[Test]
		public void GetProcessPackageActionsForSelectedProjects_NoProjects_ReturnsNoActions()
		{
			CreateViewModel();
			CreateEmptyFakeSelectedProjectsList();
			GetPackageActionsForSelectedProjects();
		
			Assert.AreEqual(0, packageActions.Count);
		}
		
		[Test]
		public void GetProcessPackageActionsForSelectedProjects_OneProjectIsSelected_InstallActionCreatedFromProject()
		{
			CreateViewModel();
			CreateOneFakeSelectedProject("Test");
			FirstSelectedProject.IsSelected = true;
			GetPackageActionsForSelectedProjects();
			
			var action = packageActions[0] as InstallPackageAction;
			InstallPackageAction expectedAction = FirstSelectedProject.FakeProject.FakeInstallPackageAction;
		
			Assert.AreEqual(expectedAction, action);
		}
		
		[Test]
		public void GetProcessPackageActionsForSelectedProjects_OneProjectWithIsSelectedSetToFalse_UninstallActionCreatedFromProject()
		{
			CreateViewModel();
			CreateOneFakeSelectedProject("Test");
			FirstSelectedProject.IsSelected = false;
			GetPackageActionsForSelectedProjects();
			
			var action = packageActions[0] as UninstallPackageAction;
			UninstallPackageAction expectedAction = FirstSelectedProject.FakeProject.FakeUninstallPackageAction;
		
			Assert.AreEqual(expectedAction, action);
		}
		
		[Test]
		public void GetProcessPackageActionsForSelectedProjects_OneProjectIsSelected_InstallActionHasViewModelPackage()
		{
			CreateViewModel();
			CreateOneFakeSelectedProject("Test");
			FirstSelectedProject.IsSelected = true;
			GetPackageActionsForSelectedProjects();
			
			var action = packageActions[0] as InstallPackageAction;
			IPackage actualPackage = action.Package;
			FakePackage expectedPackage = viewModel.FakePackage;
		
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void GetProcessPackageActionsForSelectedProjects_OneProjectWithIsSelectedSetToFalse_UninstallActionHasViewModelPackage()
		{
			CreateViewModel();
			CreateOneFakeSelectedProject("Test");
			FirstSelectedProject.IsSelected = false;
			GetPackageActionsForSelectedProjects();
			
			var action = packageActions[0] as UninstallPackageAction;
			IPackage actualPackage = action.Package;
			FakePackage expectedPackage = viewModel.FakePackage;
		
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void GetProcessPackageActionsForSelectedProjects_OneProjectIsSelected_ProjectUsesViewModelLogger()
		{
			CreateViewModel();
			CreateOneFakeSelectedProject("Test");
			FirstSelectedProject.IsSelected = true;
			GetPackageActionsForSelectedProjects();
			
			ILogger logger = FirstSelectedProject.Project.Logger;
			ILogger expectedLogger = viewModel.OperationLoggerCreated;
		
			Assert.AreEqual(expectedLogger, logger);
		}
		
		[Test]
		public void GetProcessPackageActionsForSelectedProjects_OneProjectWithIsSelectedSetToFalse_ProjectUsesViewModelLogger()
		{
			CreateViewModel();
			CreateOneFakeSelectedProject("Test");
			FirstSelectedProject.IsSelected = false;
			GetPackageActionsForSelectedProjects();
			
			ILogger logger = FirstSelectedProject.Project.Logger;
			ILogger expectedLogger = viewModel.OperationLoggerCreated;
		
			Assert.AreEqual(expectedLogger, logger);
		}
		
		[Test]
		public void ManagePackage_SolutionWithTwoProjectsAndUserUnselectsBothProjects_TwoProjectsAreUninstalled()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			viewModel.FakePackageManagementEvents.OnSelectProjectsReturnValue = true;
			viewModel.ManagePackage();
			
			List<ProcessPackageAction> actions = fakeActionRunner.GetActionsRunInOneCallAsList();
			var firstAction = actions[0] as UninstallPackageAction;
			var secondAction = actions[1] as UninstallPackageAction;
			
			Assert.AreEqual(2, actions.Count);
			Assert.AreEqual(viewModel.FakePackage, firstAction.Package);
			Assert.AreEqual(viewModel.FakePackage, secondAction.Package);
		}
	}
}
