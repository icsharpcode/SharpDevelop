// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class UpdatedPackagesViewModelTests
	{
		UpdatedPackagesViewModel viewModel;
		FakePackageManagementSolution solution;
		FakeTaskFactory taskFactory;
		FakeRegisteredPackageRepositories registeredPackageRepositories;
		ExceptionThrowingPackageManagementSolution exceptionThrowingSolution;
		FakePackageViewModelFactory packageViewModelFactory;
		FakePackageManagementEvents fakePackageManagementEvents;
		UpdatedPackageViewModelFactory updatedPackageViewModelFactory;
		
		void CreateViewModel()
		{
			CreateSolution();
			CreateViewModel(solution);
		}
		
		void CreateSolution()
		{
			solution = new FakePackageManagementSolution();
			solution.FakeActiveMSBuildProject = ProjectHelper.CreateTestProject();
		}
		
		void NoProjectsSelected()
		{
			solution.NoProjectsSelected();
		}
		
		void CreateViewModel(FakePackageManagementSolution solution)
		{
			CreateRegisteredPackageRepositories();
			CreateViewModel(solution, registeredPackageRepositories);
		}
		
		void CreateViewModel(FakePackageActionRunner actionRunner)
		{
			CreateSolution();
			CreateRegisteredPackageRepositories();
			CreateViewModel(solution, registeredPackageRepositories, actionRunner);
		}
		
		void CreateViewModel(FakePackageManagementSolution solution, FakeRegisteredPackageRepositories registeredPackageRepositories)
		{
			packageViewModelFactory = new FakePackageViewModelFactory { FakeSolution = solution };
			CreateViewModel(solution, registeredPackageRepositories, packageViewModelFactory);
		}
		
		void CreateViewModel(
			FakePackageManagementSolution solution,
			FakeRegisteredPackageRepositories registeredPackageRepositories,
			FakePackageActionRunner actionRunner)
		{
			packageViewModelFactory = new FakePackageViewModelFactory {
				FakeSolution = solution,
				FakeActionRunner = actionRunner
			};
			CreateViewModel(solution, registeredPackageRepositories, packageViewModelFactory);
		}
		
		void CreateViewModel(
			FakePackageManagementSolution solution,
			FakeRegisteredPackageRepositories registeredPackageRepositories,
			FakePackageViewModelFactory packageViewModelFactory)
		{
			taskFactory = new FakeTaskFactory();
			this.packageViewModelFactory = packageViewModelFactory;
			fakePackageManagementEvents = packageViewModelFactory.FakePackageManagementEvents;
			updatedPackageViewModelFactory = new UpdatedPackageViewModelFactory(packageViewModelFactory);
			viewModel = new UpdatedPackagesViewModel(
				solution,
				registeredPackageRepositories,
				updatedPackageViewModelFactory,
				taskFactory);
		}
		
		void CreateRegisteredPackageRepositories()
		{
			registeredPackageRepositories = new FakeRegisteredPackageRepositories();
		}
		
		void CreateViewModel(FakeRegisteredPackageRepositories registeredPackageRepositories)
		{
			CreateSolution();
			CreateViewModel(solution, registeredPackageRepositories);
		}
		
		void CreateExceptionThrowingSolution()
		{
			exceptionThrowingSolution = new ExceptionThrowingPackageManagementSolution();
			exceptionThrowingSolution.FakeActiveMSBuildProject = ProjectHelper.CreateTestProject();
		}
		
		void CompleteReadPackagesTask()
		{
			taskFactory.ExecuteAllFakeTasks();
		}

		FakePackage AddPackageToLocalRepository(string version)
		{
			return AddPackageToLocalRepository("Test", version);
		}
		
		FakePackage AddPackageToLocalRepository(string id, string version)
		{
			FakePackage package = FakePackage.CreatePackageWithVersion(id, version);
			solution.FakeProjectToReturnFromGetProject.FakePackages.Add(package);
			return package;
		}
		
		FakePackage AddPackageToActiveRepository(string version)
		{
			return AddPackageToActiveRepository("Test", version);
		}
		
		FakePackage AddPackageToActiveRepository(string id, string version)
		{
			return registeredPackageRepositories.AddFakePackageWithVersionToActiveRepository(id, version);
		}
		
		FakePackage AddPackageToSolution(string version)
		{
			FakePackage package = FakePackage.CreatePackageWithVersion(version);
			solution.FakeInstalledPackages.Add(package);
			return package;
		}
		
		void AddOnePackageSourceToRegisteredSources()
		{
			registeredPackageRepositories.ClearPackageSources();
			registeredPackageRepositories.AddOnePackageSource();
			registeredPackageRepositories.HasMultiplePackageSources = false;
		}
		
		void AddTwoPackageSourcesToRegisteredSources()
		{
			var expectedPackageSources = new PackageSource[] {
				new PackageSource("http://first.com", "First"),
				new PackageSource("http://second.com", "Second")
			};
			AddPackageSourcesToRegisteredSources(expectedPackageSources);
			registeredPackageRepositories.HasMultiplePackageSources = true;
		}
		
		void AddPackageSourcesToRegisteredSources(PackageSource[] sources)
		{
			registeredPackageRepositories.ClearPackageSources();
			registeredPackageRepositories.AddPackageSources(sources);
		}
		
		UpdatePackagesAction GetUpdatePackagesActionRun()
		{
			return packageViewModelFactory
				.FakeActionRunner
				.ActionPassedToRun as UpdatePackagesAction;
		}
		
		void RunUpdateAllPackagesCommand()
		{
			viewModel.UpdateAllPackagesCommand.Execute(null);
		}
		
		void ViewModelHasTwoPackagesThatCanBeUpdatedAfterReadingPackages()
		{
			AddPackageToLocalRepository("First", "1.0.0.0");
			AddPackageToActiveRepository("First", "1.1.0.0");
			AddPackageToLocalRepository("Second", "1.0.0.0");
			AddPackageToActiveRepository("Second", "1.1.0.0");
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
		}
		
		[Test]
		public void ReadPackages_OneNewerPackageVersionAvailable_NewerPackageVersionDisplayed()
		{
			CreateViewModel();
			AddPackageToLocalRepository("1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				newerPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void ReadPackages_TwoPackagesInSourceRepositoryAndOneNewerPackageVersionAvailable_NewerPackageVersionDisplayed()
		{
			CreateViewModel();
			AddPackageToLocalRepository("1.0.0.0");
			AddPackageToActiveRepository("1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				newerPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void ReadPackages_OneNewerPackageVersionAvailable_ProjectNotCreatedByBackgroundThread()
		{
			CreateViewModel();
			AddPackageToLocalRepository("1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			
			registeredPackageRepositories.FakeAggregateRepository = null;
			CompleteReadPackagesTask();
			
			Assert.AreEqual(1, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ReadPackages_GetProjectThrowsException_ErrorMessageFromExceptionNotOverriddenByReadPackagesCall()
		{
			CreateExceptionThrowingSolution();
			exceptionThrowingSolution.ExceptionToThrowWhenGetProjectCalled =
				new Exception("Test");
			CreateViewModel(exceptionThrowingSolution);
			viewModel.ReadPackages();
			
			ApplicationException ex = Assert.Throws<ApplicationException>(() => CompleteReadPackagesTask());
			Assert.AreEqual("Test", ex.Message);
		}
		
		[Test]
		public void ReadPackages_OnlySolutionSelectedAndOneNewerPackageVersionAvailable_NewerPackageVersionDisplayed()
		{
			CreateSolution();
			NoProjectsSelected();
			CreateViewModel(solution);
			AddPackageToSolution("1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				newerPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void ReadPackages_SolutionInitiallySelectedWithOneNewerPackageAndProjectSelectedBeforeSecondReadOfPackages_NewerPackageVersionDisplayed()
		{
			CreateSolution();
			NoProjectsSelected();
			CreateViewModel(solution);
			AddPackageToSolution("1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			TestableProject msbuildProject = ProjectHelper.CreateTestProject("Test");
			solution.FakeActiveMSBuildProject = msbuildProject;
			solution.FakeActiveProject = new FakePackageManagementProject("Test");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				newerPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void ReadPackages_PrereleasePackageVersionAvailable_NoUpdatesFound()
		{
			CreateViewModel();
			
			AddPackageToLocalRepository("1.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("1.1.0-alpha");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ShowSources_TwoPackageSources_ReturnsTrue()
		{
			CreateRegisteredPackageRepositories();
			AddTwoPackageSourcesToRegisteredSources();
			CreateViewModel(registeredPackageRepositories);
			
			Assert.IsTrue(viewModel.ShowPackageSources);
		}
		
		[Test]
		public void ShowPackageSources_OnePackageSources_ReturnsTrue()
		{
			CreateRegisteredPackageRepositories();
			AddOnePackageSourceToRegisteredSources();
			CreateViewModel(registeredPackageRepositories);
			
			Assert.IsTrue(viewModel.ShowPackageSources);
		}
		
		[Test]
		public void ShowUpdateAllPackages_DefaultValue_ReturnsTrue()
		{
			CreateViewModel();
			
			bool result = viewModel.ShowUpdateAllPackages;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsUpdateAllPackagesEnabled_NoPackages_ReturnsFalse()
		{
			CreateViewModel();
			
			bool enabled = viewModel.IsUpdateAllPackagesEnabled;
			
			Assert.IsFalse(enabled);
		}
		
		[Test]
		public void IsUpdateAllPackagesEnabled_TwoNewerPackagesAvailable_ReturnsTrue()
		{
			CreateViewModel();
			ViewModelHasTwoPackagesThatCanBeUpdatedAfterReadingPackages();
			
			bool enabled = viewModel.IsUpdateAllPackagesEnabled;
			
			Assert.IsTrue(enabled);
		}
		
		[Test]
		public void IsUpdateAllPackagesEnabled_OneNewerPackageAvailable_ReturnsFalse()
		{
			CreateViewModel();
			AddPackageToLocalRepository("First", "1.0.0.0");
			AddPackageToActiveRepository("First", "1.1.0.0");
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			bool enabled = viewModel.IsUpdateAllPackagesEnabled;
			
			Assert.IsFalse(enabled);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_TwoPackagesToBeUpdated_BothPackagesUpdated()
		{
			CreateViewModel();
			AddPackageToLocalRepository("First", "1.0.0.0");
			FakePackage firstUpdatedPackage = AddPackageToActiveRepository("First", "1.1.0.0");
			AddPackageToLocalRepository("Second", "1.0.0.0");
			FakePackage secondUpdatedPackage = AddPackageToActiveRepository("Second", "1.1.0.0");
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			RunUpdateAllPackagesCommand();
			
			UpdatePackagesAction action = GetUpdatePackagesActionRun();
			IPackage firstPackage = action.Packages.FirstOrDefault(p => p.Id == "First");
			IPackage secondPackage = action.Packages.FirstOrDefault(p => p.Id == "Second");
			Assert.AreEqual(firstUpdatedPackage, firstPackage);
			Assert.AreEqual(secondUpdatedPackage, secondPackage);
			Assert.AreEqual(2, action.Packages.Count());
		}
		
		[Test]
		public void UpdateAllPackagesCommand_ExceptionThrownWhenUpdatingAllPackages_ExceptionReported()
		{
			var actionRunner =  new ExceptionThrowingPackageActionRunner();
			CreateViewModel(actionRunner);
			ViewModelHasTwoPackagesThatCanBeUpdatedAfterReadingPackages();
			var exception = new Exception("test");
			actionRunner.ExceptionToThrow = exception;
			
			RunUpdateAllPackagesCommand();
			
			Assert.AreEqual(exception, packageViewModelFactory.FakePackageManagementEvents.ExceptionPassedToOnPackageOperationError);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_ExceptionThrownWhenUpdatingAllPackages_ExceptionLogged()
		{
			var actionRunner =  new ExceptionThrowingPackageActionRunner();
			CreateViewModel(actionRunner);
			ViewModelHasTwoPackagesThatCanBeUpdatedAfterReadingPackages();
			var exception = new Exception("Exception error message");
			actionRunner.ExceptionToThrow = exception;
			
			RunUpdateAllPackagesCommand();
			
			string actualMessage = fakePackageManagementEvents.FormattedStringPassedToOnPackageOperationMessageLogged;
			bool containsExceptionErrorMessage = actualMessage.Contains("Exception error message");
			Assert.IsTrue(containsExceptionErrorMessage, actualMessage);
			Assert.AreEqual(MessageLevel.Error, fakePackageManagementEvents.MessageLevelPassedToOnPackageOperationMessageLogged);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_PackagesUpdated_ProjectCreatedUsingSourcePackageRepository()
		{
			CreateViewModel();
			ViewModelHasTwoPackagesThatCanBeUpdatedAfterReadingPackages();
			var package = viewModel.PackageViewModels[0].GetPackage() as IPackageFromRepository;
			solution.RepositoryPassedToGetProject = null;
			
			RunUpdateAllPackagesCommand();
			
			Assert.AreEqual(package.Repository, solution.RepositoryPassedToGetProject);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_PackagesUpdated_UpdatePackagesActionCreatedFromProject()
		{
			CreateViewModel();
			ViewModelHasTwoPackagesThatCanBeUpdatedAfterReadingPackages();
			
			RunUpdateAllPackagesCommand();
			
			UpdatePackagesAction action = GetUpdatePackagesActionRun();
			FakePackageManagementProject project = solution.FakeProjectToReturnFromGetProject;
			UpdatePackagesAction expectedAction = project.UpdatePackagesActionsCreated.FirstOrDefault();
			Assert.AreEqual(expectedAction, action);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_CheckLoggerUsed_PackageViewModelLoggerUsedWhenResolvingPackageOperations()
		{
			CreateViewModel();
			ViewModelHasTwoPackagesThatCanBeUpdatedAfterReadingPackages();
			
			RunUpdateAllPackagesCommand();
			
			ILogger expectedLogger = updatedPackageViewModelFactory.Logger;
			ILogger actualLogger = solution.FakeProjectToReturnFromGetProject.Logger;
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_PackageOperations_CreatedFromProjectUsingPackagesAndUpdatePackagesAction()
		{
			CreateViewModel();
			ViewModelHasTwoPackagesThatCanBeUpdatedAfterReadingPackages();
			
			RunUpdateAllPackagesCommand();
			
			UpdatePackagesAction action = GetUpdatePackagesActionRun();
			FakePackageManagementProject project = solution.FakeProjectToReturnFromGetProject;
			Assert.AreEqual(action, project.UpdatePackagesActionPassedToGetUpdatePackagesOperations);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_PackageOperations_ActionHasPackageOperationsReturnedFromProject()
		{
			CreateViewModel();
			ViewModelHasTwoPackagesThatCanBeUpdatedAfterReadingPackages();
			FakePackageManagementProject project = solution.FakeProjectToReturnFromGetProject;
			List<PackageOperation> operations = PackageOperationHelper.CreateListWithOneInstallOperationWithFile("readme.txt");
			project.PackageOperationsToReturnFromGetUpdatePackagesOperations = operations;
			
			RunUpdateAllPackagesCommand();
			
			UpdatePackagesAction action = GetUpdatePackagesActionRun();
			CollectionAssert.AreEqual(operations, action.Operations);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_PackageOperations_PackagesUsedToDeterminePackageOperations()
		{
			CreateViewModel();
			ViewModelHasTwoPackagesThatCanBeUpdatedAfterReadingPackages();
			FakePackageManagementProject project = solution.FakeProjectToReturnFromGetProject;
			List<FakePackage> expectedPackages = registeredPackageRepositories.FakeActiveRepository.FakePackages;
			
			RunUpdateAllPackagesCommand();
			
			CollectionAssert.AreEqual(expectedPackages, project.PackagesOnUpdatePackagesActionPassedToGetUpdatePackagesOperations);
		}
	}
}
