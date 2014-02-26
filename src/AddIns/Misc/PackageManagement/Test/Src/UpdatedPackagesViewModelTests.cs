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
		PackageManagementEvents packageManagementEvents;
		
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
		
		void CreateViewModelWithRealPackageManagementEvents()
		{
			CreateSolution();
			CreateRegisteredPackageRepositories();
			packageManagementEvents = new PackageManagementEvents();
			var actionRunner = new FakePackageActionRunner();
			var packageViewModelFactory = new PackageViewModelFactory(solution, packageManagementEvents, actionRunner);
			updatedPackageViewModelFactory = new UpdatedPackageViewModelFactory(packageViewModelFactory);
			taskFactory = new FakeTaskFactory();
			
			viewModel = new UpdatedPackagesViewModel(
				solution,
				packageManagementEvents,
				registeredPackageRepositories,
				updatedPackageViewModelFactory,
				taskFactory);
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
				packageManagementEvents,
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
		
		FakePackage AddPackageToActiveRepository(string id, string version)
		{
			return registeredPackageRepositories.AddFakePackageWithVersionToActiveRepository(id, version);
		}
		
		FakePackage AddPackageToSolution(string id, string version)
		{
			FakePackage package = FakePackage.CreatePackageWithVersion(version);
			package.Id = id;
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
		
		UpdateSolutionPackagesAction GetUpdateSolutionPackagesActionRun()
		{
			return packageViewModelFactory
				.FakeActionRunner
				.ActionPassedToRun as UpdateSolutionPackagesAction;
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
		
		void ViewModelHasTwoPackagesInSolutionThatCanBeUpdatedAfterReadingPackages()
		{
			AddPackageToSolution("First", "1.0.0.0");
			AddPackageToActiveRepository("First", "1.1.0.0");
			AddPackageToSolution("Second", "1.0.0.0");
			AddPackageToActiveRepository("Second", "1.1.0.0");
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
		}
		
		FakePackageManagementProject AddProjectToSolution()
		{
			return solution.AddFakeProject("MyProject");
		}
		
		[Test]
		public void ReadPackages_OneNewerPackageVersionAvailable_NewerPackageVersionDisplayed()
		{
			CreateViewModel();
			AddPackageToLocalRepository("Test", "1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("Test", "1.1.0.0");
			
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
			AddPackageToLocalRepository("Test", "1.0.0.0");
			AddPackageToActiveRepository("Test", "1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("Test", "1.1.0.0");
			
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
			AddPackageToLocalRepository("Test", "1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("Test", "1.1.0.0");
			
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
			AddPackageToSolution("Test", "1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("Test", "1.1.0.0");
			
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
			AddPackageToSolution("Test", "1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("Test", "1.1.0.0");
			
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
			AddPackageToLocalRepository("Test", "1.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("Test", "1.1.0-alpha");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ReadPackages_PrereleasePackageVersionAvailableAndIncludePrereleaseIsTrue_UpdateFound()
		{
			CreateViewModel();
			viewModel.IncludePrerelease = true;
			AddPackageToLocalRepository("Test", "1.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("Test", "1.1.0-alpha");
			var expectedPackages = new FakePackage[] { newerPackage };
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
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
			Assert.AreEqual(action, project.SettingsPassedToGetUpdatePackagesOperations);
			Assert.AreEqual(action.Packages, project.PackagesOnUpdatePackagesActionPassedToGetUpdatePackagesOperations);
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
		
		[Test]
		public void UpdateAllPackagesCommand_TwoProjectsAndPackagesUpdatedForSolution_UpdateAllPackagesInSolutionActionIsRun()
		{
			CreateSolution();
			NoProjectsSelected();
			FakePackageManagementProject project1 = AddProjectToSolution();
			FakePackageManagementProject project2 = AddProjectToSolution();
			CreateViewModel(solution);
			ViewModelHasTwoPackagesInSolutionThatCanBeUpdatedAfterReadingPackages();
			
			RunUpdateAllPackagesCommand();
			
			UpdateSolutionPackagesAction action = GetUpdateSolutionPackagesActionRun();
			Assert.IsNotNull(action);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_TwoProjectsAndPackagesUpdatedForSolution_UpdateAllPackagesInSolutionActionRunHasSolutionSet()
		{
			CreateSolution();
			NoProjectsSelected();
			FakePackageManagementProject project1 = AddProjectToSolution();
			FakePackageManagementProject project2 = AddProjectToSolution();
			CreateViewModel(solution);
			ViewModelHasTwoPackagesInSolutionThatCanBeUpdatedAfterReadingPackages();
			
			RunUpdateAllPackagesCommand();
			
			UpdateSolutionPackagesAction action = GetUpdateSolutionPackagesActionRun();
			Assert.AreEqual(solution, action.Solution);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_TwoProjectsAndPackagesUpdatedForSolution_UpdateAllPackagesInSolutionActionRunHasTwoPackages()
		{
			CreateSolution();
			NoProjectsSelected();
			FakePackageManagementProject project1 = AddProjectToSolution();
			FakePackageManagementProject project2 = AddProjectToSolution();
			CreateViewModel(solution);
			AddPackageToSolution("First", "1.0.0.0");
			FakePackage firstUpdatedPackage = AddPackageToActiveRepository("First", "1.1.0.0");
			AddPackageToSolution("Second", "1.0.0.0");
			FakePackage secondUpdatedPackage = AddPackageToActiveRepository("Second", "1.1.0.0");
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			RunUpdateAllPackagesCommand();
			
			UpdateSolutionPackagesAction action = GetUpdateSolutionPackagesActionRun();
			IPackage firstPackage = action.Packages.FirstOrDefault(p => p.Id == "First");
			IPackage secondPackage = action.Packages.FirstOrDefault(p => p.Id == "Second");
			Assert.AreEqual(firstUpdatedPackage, firstPackage);
			Assert.AreEqual(secondUpdatedPackage, secondPackage);
			Assert.AreEqual(2, action.Packages.Count());
		}
		
		[Test]
		public void UpdateAllPackagesCommand_TwoProjectsAndPackagesUpdatedForSolution_ActionHasPackageOperations()
		{
			CreateSolution();
			NoProjectsSelected();
			FakePackageManagementProject project1 = AddProjectToSolution();
			FakePackageManagementProject project2 = AddProjectToSolution();
			CreateViewModel(solution);
			ViewModelHasTwoPackagesInSolutionThatCanBeUpdatedAfterReadingPackages();
			List<PackageOperation> operations = PackageOperationHelper.CreateListWithOneInstallOperationWithFile("readme.txt");
			project1.PackageOperationsToReturnFromGetUpdatePackagesOperations = operations;
			
			RunUpdateAllPackagesCommand();
			
			UpdateSolutionPackagesAction action = GetUpdateSolutionPackagesActionRun();
			CollectionAssert.AreEqual(operations, action.Operations);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_TwoProjectsAndPackagesUpdatedForSolution_PackageOperationsDeterminedFromConfiguredPackageRepository()
		{
			CreateSolution();
			NoProjectsSelected();
			FakePackageManagementProject project1 = AddProjectToSolution();
			FakePackageManagementProject project2 = AddProjectToSolution();
			CreateViewModel(solution);
			ViewModelHasTwoPackagesInSolutionThatCanBeUpdatedAfterReadingPackages();
			List<PackageOperation> operations = PackageOperationHelper.CreateListWithOneInstallOperationWithFile("readme.txt");
			project1.PackageOperationsToReturnFromGetUpdatePackagesOperations = operations;
			IPackageFromRepository package = viewModel.PackageViewModels[0].GetPackage() as IPackageFromRepository;
			
			RunUpdateAllPackagesCommand();
			
			Assert.AreEqual(package.Repository, solution.SourceRepositoryPassedToGetProjects);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_TwoProjectsAndPackagesUpdatedForSolution_LoggerSetWhenResolvingPackageOperations()
		{
			CreateSolution();
			NoProjectsSelected();
			FakePackageManagementProject project1 = AddProjectToSolution();
			FakePackageManagementProject project2 = AddProjectToSolution();
			CreateViewModel(solution);
			ViewModelHasTwoPackagesInSolutionThatCanBeUpdatedAfterReadingPackages();
			
			RunUpdateAllPackagesCommand();
			
			ILogger expectedLogger = updatedPackageViewModelFactory.Logger;
			ILogger actualLogger = project1.Logger;
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_TwoProjectsAndPackagesUpdatedForSolution_LoggerSetOnUpdateSolutionPackagesAction()
		{
			CreateSolution();
			NoProjectsSelected();
			FakePackageManagementProject project1 = AddProjectToSolution();
			FakePackageManagementProject project2 = AddProjectToSolution();
			CreateViewModel(solution);
			ViewModelHasTwoPackagesInSolutionThatCanBeUpdatedAfterReadingPackages();
			
			RunUpdateAllPackagesCommand();
			
			ILogger expectedLogger = updatedPackageViewModelFactory.Logger;
			UpdateSolutionPackagesAction action = GetUpdateSolutionPackagesActionRun();
			Assert.AreEqual(expectedLogger, action.Logger);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_TwoPackagesBeingUpdated_PreviouslyLoggedMessagesAreCleared()
		{
			CreateViewModel();
			ViewModelHasTwoPackagesThatCanBeUpdatedAfterReadingPackages();
			
			RunUpdateAllPackagesCommand();
			
			Assert.IsTrue(packageViewModelFactory.FakePackageManagementEvents.IsOnPackageOperationsStartingCalled);
		}
		
		[Test]
		public void PackageViewModels_PackagesUpdated_PackagesAreUpdatedInViewModelList()
		{
			CreateViewModelWithRealPackageManagementEvents();
			AddPackageToLocalRepository("Test", "1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("Test", "1.1.0.0");
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			solution.FakeProjectToReturnFromGetProject.FakePackages.Clear();
			AddPackageToLocalRepository("Second", "1.0.0.0");
			AddPackageToLocalRepository("Test", "1.1.0.0");
			FakePackage expectedPackage = AddPackageToActiveRepository("Second", "1.1.0.0");
			var expectedPackages = new FakePackage[] { expectedPackage };
			
			packageManagementEvents.OnParentPackagesUpdated(new FakePackage[] { newerPackage });
			CompleteReadPackagesTask();
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void PackageViewModels_PackagesUpdatedAfterViewModelIsDisposed_PackagesAreNotUpdatedInViewModelList()
		{
			CreateViewModelWithRealPackageManagementEvents();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			viewModel.Dispose();
			AddPackageToLocalRepository("Test", "1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("Test", "1.1.0.0");
			var expectedPackages = new FakePackage[] { newerPackage };
			
			packageManagementEvents.OnParentPackagesUpdated(new FakePackage[] { newerPackage });
			CompleteReadPackagesTask();
			
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ShowPrerelease_ByDefault_ReturnsTrue()
		{
			CreateViewModel();
			
			bool show = viewModel.ShowPrerelease;
			
			Assert.IsTrue(show);
		}
		
		[Test]
		public void PackageViewModels_ChildPackageViewModelParent_IsUpdatedPackagesViewModel()
		{
			CreateViewModel();
			AddPackageToLocalRepository("Test", "1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("Test", "1.1.0.0");
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			PackageViewModel childViewModel = viewModel.PackageViewModels.First();
			IPackageViewModelParent parent = childViewModel.GetParent();
			Assert.AreEqual(viewModel, parent);
		}
		
		[Test]
		public void UpdateAllPackagesCommand_SourceRepositoryIsOperationAware_UpdateOperationStartedAndDisposed()
		{
			CreateViewModel();
			var operationAwareRepository = new FakeOperationAwarePackageRepository();
			registeredPackageRepositories.FakeActiveRepository = operationAwareRepository;
			AddPackageToLocalRepository("Test", "1.0.0.0");
			AddPackageToActiveRepository("Test", "1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("Test", "1.1.0.0");
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			RunUpdateAllPackagesCommand();
			
			operationAwareRepository.AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Update, null);
		}
	}
}
