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
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests
{
	[TestFixture]
	   public class PackageViewModelTests
	{
		TestablePackageViewModel viewModel;
		FakePackage fakePackage;
		FakePackageManagementSolution fakeSolution;
		FakePackageManagementEvents fakePackageManagementEvents;
		ExceptionThrowingPackageManagementSolution exceptionThrowingSolution;
		ExceptionThrowingPackageManagementProject exceptionThrowingProject;
		FakeUninstallPackageAction fakeUninstallPackageAction;
		FakeLogger fakeLogger;
		FakePackageActionRunner fakeActionRunner;
		List<FakeSelectedProject> fakeSelectedProjects;
		IPackageViewModelParent viewModelParent;
		
		void CreateFakeSolution()
		{
			fakeSolution = new FakePackageManagementSolution();
			fakeSolution.FakeActiveMSBuildProject = ProjectHelper.CreateTestProject();
		}
		
		void CreateViewModel()
		{
			CreateFakeSolution();
			CreateViewModel(fakeSolution);
		}
		
		void CreateViewModelWithExceptionThrowingSolution()
		{
			exceptionThrowingSolution = new ExceptionThrowingPackageManagementSolution();
			exceptionThrowingSolution.FakeActiveMSBuildProject = ProjectHelper.CreateTestProject();
			CreateViewModel(exceptionThrowingSolution);
		}
		
		void CreateViewModelWithExceptionThrowingProject()
		{
			CreateViewModel();
			exceptionThrowingProject = new ExceptionThrowingPackageManagementProject();
			viewModel.FakeSolution.FakeProjectToReturnFromGetProject = exceptionThrowingProject;
		}
		
		void CreateViewModel(FakePackageManagementSolution solution)
		{
			viewModelParent = MockRepository.GenerateStub<IPackageViewModelParent>();
			viewModel = new TestablePackageViewModel(viewModelParent, solution);
			fakePackage = viewModel.FakePackage;
			this.fakeSolution = solution;
			fakePackageManagementEvents = viewModel.FakePackageManagementEvents;
			fakeLogger = viewModel.FakeLogger;
			fakeUninstallPackageAction = solution.FakeProjectToReturnFromGetProject.FakeUninstallPackageAction;
			fakeActionRunner = viewModel.FakeActionRunner;
		}
		
		void AddProjectToSolution()
		{
			TestableProject project = ProjectHelper.CreateTestProject();
			fakeSolution.FakeMSBuildProjects.Add(project);
		}
		
		void CreateViewModelWithTwoProjectsSelected(string projectName1, string projectName2)
		{
			CreateFakeSolution();
			AddTwoProjectsSelected(projectName1, projectName2);
			CreateViewModel(fakeSolution);
		}

		void AddTwoProjectsSelected(string projectName1, string projectName2)
		{			
			AddProjectToSolution();
			AddProjectToSolution();
			fakeSolution.FakeMSBuildProjects[0].Name = projectName1;
			fakeSolution.FakeMSBuildProjects[1].Name = projectName2;
			fakeSolution.NoProjectsSelected();
			
			fakeSolution.AddFakeProjectToReturnFromGetProject(projectName1);
			fakeSolution.AddFakeProjectToReturnFromGetProject(projectName2);
		}
		
		void SetPackageIdAndVersion(string id, string version)
		{
			fakePackage.Id = id;
			fakePackage.Version = new SemanticVersion(version);
		}
		
		void UserCancelsProjectSelection()
		{
			fakePackageManagementEvents.OnSelectProjectsReturnValue = false;
		}
		
		void UserAcceptsProjectSelection()
		{
			fakePackageManagementEvents.OnSelectProjectsReturnValue = true;
		}
		
		List<FakeSelectedProject> CreateTwoFakeSelectedProjects()
		{
			fakeSelectedProjects = new List<FakeSelectedProject>();
			fakeSelectedProjects.Add(new FakeSelectedProject("Project A"));
			fakeSelectedProjects.Add(new FakeSelectedProject("Project B"));
			return fakeSelectedProjects;
		}
		
		FakePackageOperation AddFakeInstallPackageOperationWithPackageThatRequiresLicenseAcceptance(FakeSelectedProject selectedProject)
		{
			return AddFakeInstallPackageOperationWithPackage(selectedProject, requireLicenseAcceptance: true);
		}
		
		FakePackageOperation AddFakeInstallPackageOperationWithPackageThatDoesNotRequireLicenseAcceptance(FakeSelectedProject selectedProject)
		{
			return AddFakeInstallPackageOperationWithPackage(selectedProject, requireLicenseAcceptance: false);
		}
		
		FakePackageOperation AddFakeInstallPackageOperationWithPackage(FakeSelectedProject selectedProject, bool requireLicenseAcceptance)
		{
			FakePackageOperation operation = selectedProject.AddFakeInstallPackageOperation();
			operation.FakePackage.RequireLicenseAcceptance = requireLicenseAcceptance;
			return operation;
		}
		
		FakePackageOperation AddFakeUninstallPackageOperationWithPackageThatRequiresLicenseAcceptance(FakeSelectedProject selectedProject)
		{
			FakePackageOperation uninstallOperation = selectedProject.AddFakeUninstallPackageOperation();
			uninstallOperation.FakePackage.RequireLicenseAcceptance = true;
			return uninstallOperation;
		}
		
		FakeSelectedProject FirstFakeSelectedProject {
			get { return fakeSelectedProjects[0]; }
		}
		
		FakeSelectedProject SecondFakeSelectedProject {
			get { return fakeSelectedProjects[1]; }
		}
		
		void ParentAllowsPrereleasePackages()
		{
			viewModelParent.Stub(parent => parent.IncludePrerelease).Return(true);
		}
		
		[Test]
		public void AddPackageCommand_CommandExecuted_InstallsPackage()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			
			viewModel.AddPackageCommand.Execute(null);
			
			IPackage package = fakeSolution.FakeProjectToReturnFromGetProject.LastInstallPackageCreated.Package;
			Assert.AreEqual(fakePackage, package);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_SourcePackageRepositoryUsedToCreateProject()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			
			viewModel.AddPackage();
						
			Assert.AreEqual(fakePackage.Repository, fakeSolution.RepositoryPassedToGetProject);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageIsInstalled()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			
			viewModel.AddPackage();
			
			IPackageAction actionExecuted = fakeActionRunner.ActionPassedToRun;
			IPackageAction action = fakeSolution.FakeProjectToReturnFromGetProject.LastInstallPackageCreated;
			Assert.AreEqual(action, actionExecuted);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageOperationsUsedWhenInstallingPackage()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
		
			PackageOperation[] expectedOperations = new PackageOperation[] {
				new PackageOperation(fakePackage, PackageAction.Install)
			};
			
			FakeInstallPackageAction action = fakeSolution.FakeProjectToReturnFromGetProject.LastInstallPackageCreated;
			CollectionAssert.AreEqual(expectedOperations, action.Operations);
		}
		
		[Test]
		public void PackageChanged_PackageAddedSuccessfully_PropertyNotifyChangedFiredForIsAddedProperty()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			string propertyChangedName = null;
			viewModel.PropertyChanged += (sender, e) => propertyChangedName = e.PropertyName;
			viewModel.AddPackage();
		
			viewModel.PackageChanged();
			
			Assert.AreEqual("IsAdded", propertyChangedName);
		}

		[Test]
		public void PackageChanged_PackageAddedSuccessfully_PropertyNotifyChangedFiredAfterPackageInstalled()
		{
			CreateViewModel();
			IPackage packagePassedToInstallPackageWhenPropertyNameChanged = null;
			viewModel.PropertyChanged += (sender, e) => {
				packagePassedToInstallPackageWhenPropertyNameChanged = 
					fakeSolution.FakeProjectToReturnFromGetProject.LastInstallPackageCreated.Package;
			};
			viewModel.AddPackage();
			
			viewModel.PackageChanged();
			
			Assert.AreEqual(fakePackage, packagePassedToInstallPackageWhenPropertyNameChanged);
		}

		[Test]
		public void HasLicenseUrl_PackageHasLicenseUrl_ReturnsTrue()
		{
			CreateViewModel();
			fakePackage.LicenseUrl = new Uri("http://sharpdevelop.com");
			
			Assert.IsTrue(viewModel.HasLicenseUrl);
		}
		
		[Test]
		public void HasLicenseUrl_PackageHasNoLicenseUrl_ReturnsFalse()
		{
			CreateViewModel();
			fakePackage.LicenseUrl = null;
			
			Assert.IsFalse(viewModel.HasLicenseUrl);
		}
		
		[Test]
		public void HasProjectUrl_PackageHasProjectUrl_ReturnsTrue()
		{
			CreateViewModel();
			fakePackage.ProjectUrl = new Uri("http://sharpdevelop.com");
			
			Assert.IsTrue(viewModel.HasProjectUrl);
		}
		
		[Test]
		public void HasProjectUrl_PackageHasNoProjectUrl_ReturnsFalse()
		{
			CreateViewModel();
			fakePackage.ProjectUrl = null;
			
			Assert.IsFalse(viewModel.HasProjectUrl);
		}
		
		[Test]
		public void HasReportAbuseUrl_PackageHasReportAbuseUrl_ReturnsTrue()
		{
			CreateViewModel();
			fakePackage.ReportAbuseUrl = new Uri("http://sharpdevelop.com");
			
			Assert.IsTrue(viewModel.HasReportAbuseUrl);
		}
		
		[Test]
		public void HasReportAbuseUrl_PackageHasNoReportAbuseUrl_ReturnsFalse()
		{
			CreateViewModel();
			fakePackage.ReportAbuseUrl = null;
			
			Assert.IsFalse(viewModel.HasReportAbuseUrl);
		}
		
		[Test]
		public void IsAdded_ProjectHasPackageAdded_ReturnsTrue()
		{
			CreateViewModel();
			fakeSolution.FakeProjectToReturnFromGetProject.FakePackages.Add(fakePackage);
			
			Assert.IsTrue(viewModel.IsAdded);
		}
		
		[Test]
		public void IsAdded_ProjectDoesNotHavePackageInstalled_ReturnsFalse()
		{
			CreateViewModel();
			fakeSolution.FakeProjectToReturnFromGetProject.FakePackages.Clear();
			
			Assert.IsFalse(viewModel.IsAdded);
		}
		
		[Test]
		public void RemovePackageCommand_CommandExecuted_UninstallsPackage()
		{
			CreateViewModel();
			viewModel.RemovePackageCommand.Execute(null);
						
			Assert.AreEqual(fakePackage, fakeUninstallPackageAction.Package);
		}
		
		[Test]
		public void RemovePackage_PackageRemovedSuccessfully_RepositoryUsedToCreateProject()
		{
			CreateViewModel();
			viewModel.RemovePackage();
			
			Assert.AreEqual(fakePackage.Repository, fakeSolution.RepositoryPassedToGetProject);
		}
		
		[Test]
		public void PackageChanged_PackageRemovedSuccessfully_PropertyNotifyChangedFiredForIsAddedProperty()
		{
			CreateViewModel();
			string propertyChangedName = null;
			viewModel.PropertyChanged += (sender, e) => propertyChangedName = e.PropertyName;
			viewModel.RemovePackage();
			
			viewModel.PackageChanged();
			
			Assert.AreEqual("IsAdded", propertyChangedName);
		}
		
		[Test]
		public void PackageChanged_PackageRemovedSuccessfully_PropertyNotifyChangedFiredAfterPackageUninstalled()
		{
			CreateViewModel();
			IPackage packagePassedToUninstallPackageWhenPropertyNameChanged = null;
			viewModel.PropertyChanged += (sender, e) => {
				packagePassedToUninstallPackageWhenPropertyNameChanged = fakeUninstallPackageAction.Package;
			};
			viewModel.RemovePackage();
			
			viewModel.PackageChanged();
			
			Assert.AreEqual(fakePackage, packagePassedToUninstallPackageWhenPropertyNameChanged);
		}
		
		[Test]
		public void HasDependencies_PackageHasNoDependencies_ReturnsFalse()
		{
			CreateViewModel();
			fakePackage.HasDependencies = false;
			
			Assert.IsFalse(viewModel.HasDependencies);
		}
		
		[Test]
		public void HasDependencies_PackageHasDependency_ReturnsTrue()
		{
			CreateViewModel();
			fakePackage.HasDependencies = true;
			
			Assert.IsTrue(viewModel.HasDependencies);
		}
		
		[Test]
		public void HasNoDependencies_PackageHasNoDependencies_ReturnsTrue()
		{
			CreateViewModel();
			fakePackage.HasDependencies = false;
			
			Assert.IsTrue(viewModel.HasNoDependencies);
		}
		
		[Test]
		public void HasNoDependencies_PackageHasOneDependency_ReturnsFalse()
		{
			CreateViewModel();
			fakePackage.HasDependencies = true;
			
			Assert.IsFalse(viewModel.HasNoDependencies);
		}
		
		[Test]
		public void HasDownloadCount_DownloadCountIsZero_ReturnsTrue()
		{
			CreateViewModel();
			fakePackage.DownloadCount = 0;
			
			Assert.IsTrue(viewModel.HasDownloadCount);
		}
		
		[Test]
		public void HasDownloadCount_DownloadCountIsMinusOne_ReturnsFalse()
		{
			CreateViewModel();
			fakePackage.DownloadCount = -1;
			
			Assert.IsFalse(viewModel.HasDownloadCount);
		}
		
		[Test]
		public void HasLastPublished_PackageHasPublishedDate_ReturnsTrue()
		{
			CreateViewModel();
			fakePackage.Published = new DateTime(2011, 1, 2);
			
			Assert.IsTrue(viewModel.HasLastPublished);
		}
		
		[Test]
		public void HasLastPublished_PackageHasNoPublishedDate_ReturnsFalse()
		{
			CreateViewModel();
			fakePackage.Published = null;
			
			Assert.IsFalse(viewModel.HasLastPublished);
		}
		
		[Test]
		public void LastPublished_PackageHasPublishedDate_ReturnsPackagePublishedDate()
		{
			CreateViewModel();
			fakePackage.Published = new DateTime(2011, 1, 2);
			
			Assert.AreEqual(fakePackage.Published, viewModel.LastPublished);
		}
		
		[Test]
		public void LastPublished_PackageHasNoPublishedDate_ReturnsNull()
		{
			CreateViewModel();
			fakePackage.Published = null;
			
			Assert.IsNull(viewModel.LastPublished);
		}
		
		[Test]
		public void AddPackage_PackageRequiresLicenseAgreementAcceptance_UserAskedToAcceptLicenseAgreementForPackageBeforeInstalling()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			fakePackage.RequireLicenseAcceptance = true;
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = true;
			
			viewModel.AddPackage();
			
			var expectedPackages = new FakePackage[] {
				fakePackage
			};
			
			IEnumerable<IPackage> actualPackages = fakePackageManagementEvents.LastPackagesPassedToOnAcceptLicenses;
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void AddPackage_PackageRequiresLicenseAgreementAcceptanceButPackageInstalledInSolutionAlready_UserNotAskedToAcceptLicenseAgreementForPackageBeforeInstalling()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			fakePackage.RequireLicenseAcceptance = true;
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = true;
			fakeSolution.FakeInstalledPackages.Add(fakePackage);
			
			viewModel.AddPackage();
			
			bool acceptLicenses = fakePackageManagementEvents.IsOnAcceptLicensesCalled;
			
			Assert.IsFalse(acceptLicenses);
		}
		
		[Test]
		public void AddPackage_PackageDoesNotRequireLicenseAgreementAcceptance_UserNotAskedToAcceptLicenseAgreementBeforeInstalling()
		{
			CreateViewModel();
			fakePackage.RequireLicenseAcceptance = false;
			viewModel.AddPackage();
			
			Assert.IsFalse(fakePackageManagementEvents.IsOnAcceptLicensesCalled);
		}
		
		[Test]
		public void AddPackage_PackageRequiresLicenseAgreementAcceptanceAndUserDeclinesAgreement_PackageIsNotInstalled()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			fakePackage.RequireLicenseAcceptance = true;
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = false;
			
			viewModel.AddPackage();
			
			FakeInstallPackageAction action = fakeSolution.FakeProjectToReturnFromGetProject.LastInstallPackageCreated;
			Assert.IsFalse(action.IsExecuteCalled);
		}
		
		[Test]
		public void AddPackage_PackageRequiresLicenseAgreementAcceptanceAndUserDeclinesAgreement_PropertyChangedEventNotFired()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			fakePackage.RequireLicenseAcceptance = true;
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = false;
			bool propertyChangedEventFired = false;
			viewModel.PropertyChanged += (sender, e) => propertyChangedEventFired = true;
			
			viewModel.AddPackage();
			
			Assert.IsFalse(propertyChangedEventFired);
		}
		
		[Test]
		public void AddPackage_OnePackageOperationIsToUninstallPackageWhichRequiresLicenseAcceptance_UserIsNotAskedToAcceptLicenseAgreementForPackageToBeUninstalled()
		{
			CreateViewModel();
			fakePackage.RequireLicenseAcceptance = false;
			PackageOperation operation = viewModel.AddOneFakeUninstallPackageOperation();
			FakePackage packageToUninstall = operation.Package as FakePackage;
			packageToUninstall.RequireLicenseAcceptance = true;
			viewModel.AddPackage();
			
			Assert.IsFalse(fakePackageManagementEvents.IsOnAcceptLicensesCalled);
		}
		
		[Test]
		public void AddPackage_CheckLoggerUsed_PackageViewModelLoggerUsedWhenResolvingPackageOperations()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			ILogger expectedLogger = viewModel.OperationLoggerCreated;
			ILogger actualLogger = fakeSolution.FakeProjectToReturnFromGetProject.Logger;
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_InstallingPackageMessageIsFirstMessageLogged()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			fakePackage.Id = "Test.Package";
			fakePackage.Version = new SemanticVersion(1, 2, 0, 55);
			viewModel.AddPackage();
			
			string expectedMessage = "------- Installing...Test.Package 1.2.0.55 -------";
			string actualMessage = fakeLogger.FirstFormattedMessageLogged;
			
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_NextToLastMessageLoggedMarksEndOfInstallation()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			string expectedMessage = "==============================";
			string actualMessage = fakeLogger.NextToLastFormattedMessageLogged;
						
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_LastMessageLoggedIsEmptyLine()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			string expectedMessage = String.Empty;
			string actualMessage = fakeLogger.LastFormattedMessageLogged;
						
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void RemovePackage_PackageRemovedSuccessfully_UninstallingPackageMessageIsFirstMessageLogged()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			fakePackage.Id = "Test.Package";
			fakePackage.Version = new SemanticVersion(1, 2, 0, 55);
			viewModel.RemovePackage();
			
			string expectedMessage = "------- Uninstalling...Test.Package 1.2.0.55 -------";
			string actualMessage = fakeLogger.FirstFormattedMessageLogged;
						
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void RemovePackage_PackageRemovedSuccessfully_NextToLastMessageLoggedMarksEndOfInstallation()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.RemovePackage();
			
			string expectedMessage = "==============================";
			string actualMessage = fakeLogger.NextToLastFormattedMessageLogged;
						
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void RemovePackage_PackageRemovedSuccessfully_LastMessageLoggedIsEmptyLine()
		{
			CreateViewModel();
			viewModel.RemovePackage();
			
			string expectedMessage = String.Empty;
			string actualMessage = fakeLogger.LastFormattedMessageLogged;
						
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void AddPackage_ExceptionWhenInstallingPackage_ExceptionErrorMessageReported()
		{
			CreateViewModelWithExceptionThrowingProject();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			Exception ex = new Exception("Test");
			exceptionThrowingProject.ExceptionToThrowWhenCreateInstallPackageActionCalled = ex;
			viewModel.AddPackage();
			
			Assert.AreEqual(ex, fakePackageManagementEvents.ExceptionPassedToOnPackageOperationError);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_MessagesReportedPreviouslyAreCleared()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			Assert.IsTrue(fakePackageManagementEvents.IsOnPackageOperationsStartingCalled);
		}
		
		[Test]
		public void AddPackage_ExceptionWhenInstallingPackage_ExceptionLogged()
		{
			CreateViewModelWithExceptionThrowingProject();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			Exception ex = new Exception("Exception error message");
			exceptionThrowingProject.ExceptionToThrowWhenCreateInstallPackageActionCalled = ex;
			viewModel.AddPackage();
			
			string actualMessage = fakeLogger.SecondFormattedMessageLogged;
			bool containsExceptionErrorMessage = actualMessage.Contains("Exception error message");
			
			Assert.IsTrue(containsExceptionErrorMessage, actualMessage);
		}
		
		[Test]
		public void RemovePackage_ExceptionWhenUninstallingPackage_ExceptionErrorMessageReported()
		{
			CreateViewModelWithExceptionThrowingProject();
			Exception ex = new Exception("Test");
			exceptionThrowingProject.ExceptionToThrowWhenCreateUninstallPackageActionCalled = ex;
			viewModel.RemovePackage();
			
			Assert.AreEqual(ex, fakePackageManagementEvents.ExceptionPassedToOnPackageOperationError);
		}
		
		[Test]
		public void RemovePackage_PackageUninstalledSuccessfully_MessagesReportedPreviouslyAreCleared()
		{
			CreateViewModel();
			viewModel.RemovePackage();
			
			Assert.IsTrue(fakePackageManagementEvents.IsOnPackageOperationsStartingCalled);
		}
		
		[Test]
		public void RemovePackage_ExceptionWhenUninstallingPackage_ExceptionLogged()
		{
			CreateViewModelWithExceptionThrowingProject();
			Exception ex = new Exception("Exception error message");
			exceptionThrowingProject.ExceptionToThrowWhenCreateUninstallPackageActionCalled = ex;
			viewModel.RemovePackage();
			
			string actualMessage = fakeLogger.SecondFormattedMessageLogged;
			bool containsExceptionErrorMessage = actualMessage.Contains("Exception error message");
			
			Assert.IsTrue(containsExceptionErrorMessage, actualMessage);
		}
		
		[Test]
		public void AddPackage_ExceptionThrownWhenResolvingPackageOperations_ExceptionReported()
		{
			CreateViewModelWithExceptionThrowingSolution();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			
			var exception = new Exception("Test");
			exceptionThrowingSolution.ExceptionToThrowWhenGetProjectCalled = exception;
			viewModel.AddPackage();
			
			Assert.AreEqual(exception, fakePackageManagementEvents.ExceptionPassedToOnPackageOperationError);
		}
		
		[Test]
		public void AddPackage_PackagesInstalledSuccessfully_ViewModelPackageUsedWhenResolvingPackageOperations()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			FakePackage expectedPackage = fakePackage;
			IPackage actualPackage = fakeSolution
				.FakeProjectToReturnFromGetProject
				.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void AddPackage_PackagesInstalledSuccessfully_PackageDependenciesNotIgnoredWhenCheckingForPackageOperations()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			bool result = fakeSolution
				.FakeProjectToReturnFromGetProject
				.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void AddPackage_PackagesInstalledSuccessfully_PrereleaseVersionsNotAllowedWhenCheckingForPackageOperations()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			bool result = fakeSolution
				.FakeProjectToReturnFromGetProject
				.AllowPrereleaseVersionsPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void AddPackage_ParentHasIncludePrereleaseSetToTrueWhenInstalling_PrereleaseVersionsAllowedWhenCheckingForPackageOperations()
		{
			CreateViewModel();
			ParentAllowsPrereleasePackages();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			bool result = fakeSolution
				.FakeProjectToReturnFromGetProject
				.AllowPrereleaseVersionsPassedToGetInstallPackageOperations;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void RemovePackage_PackageRemovedSuccessfully_PackageIsRemoved()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.RemovePackage();
			
			IPackageAction actionExecuted = fakeActionRunner.ActionPassedToRun;
			
			Assert.AreEqual(fakeUninstallPackageAction, actionExecuted);
		}
		
		[Test]
		public void IsAdded_SolutionSelectedContainingOneProjectAndPackageIsInstalledInSolutionSharedRepository_ReturnsTrue()
		{
			CreateFakeSolution();
			AddProjectToSolution();
			fakeSolution.NoProjectsSelected();
			fakeSolution.FakeInstalledPackages.Add(fakePackage);
			CreateViewModel(fakeSolution);
			
			bool added = viewModel.IsAdded;
			
			Assert.IsTrue(added);
		}
		
		[Test]
		public void IsAdded_SolutionSelectedContainingOneProjectAndPackageIsNotInstalledInSolutionSharedRepository_ReturnsFalse()
		{
			CreateViewModel();
			AddProjectToSolution();
			fakeSolution.NoProjectsSelected();
			
			bool added = viewModel.IsAdded;
			
			Assert.IsFalse(added);
		}
		
		[Test]
		public void IsManaged_SolutionSelectedContainingTwoProjects_ReturnsFalsew()
		{
			CreateFakeSolution();
			AddProjectToSolution();
			AddProjectToSolution();
			fakeSolution.NoProjectsSelected();
			CreateViewModel(fakeSolution);
			
			bool managed = viewModel.IsManaged;
			
			// Only installed project-level package "IsManaged"
			Assert.IsFalse(managed);
		}
		
		[Test]
		public void IsManaged_SolutionSelectedContainingOneProject_ReturnsFalse()
		{
			CreateFakeSolution();
			AddProjectToSolution();
			fakeSolution.NoProjectsSelected();
			CreateViewModel(fakeSolution);
			
			bool managed = viewModel.IsManaged;
			
			Assert.IsFalse(managed);
		}
		
		[Test]
		public void IsManaged_SolutionWithOneProjectSelected_ReturnsFalse()
		{
			CreateFakeSolution();
			AddProjectToSolution();
			fakeSolution.FakeActiveMSBuildProject = fakeSolution.FakeMSBuildProjects[0];
			CreateViewModel(fakeSolution);
			
			bool managed = viewModel.IsManaged;
			
			Assert.IsFalse(managed);
		}
		
		[Test]
		public void ManagePackageCommand_TwoProjectsSelectedAndCommandExecuted_UserPromptedToSelectTwoProjects()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			UserCancelsProjectSelection();
			viewModel.ManagePackageCommand.Execute(null);
			
			IEnumerable<IPackageManagementSelectedProject> selectedProjects = 
				fakePackageManagementEvents.SelectedProjectsPassedToOnSelectProjects;
			
			var expectedSelectedProjects = new List<IPackageManagementSelectedProject>();
			expectedSelectedProjects.Add(new FakeSelectedProject("Project A"));
			expectedSelectedProjects.Add(new FakeSelectedProject("Project B"));
			
			SelectedProjectCollectionAssert.AreEqual(expectedSelectedProjects, selectedProjects);
		}
		
		[Test]
		public void ManagePackage_TwoProjectsSelectedAndUserAcceptsSelectedProjects_MessagesReportedPreviouslyAreCleared()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			UserAcceptsProjectSelection();
			
			viewModel.ManagePackage();
			
			Assert.IsTrue(fakePackageManagementEvents.IsOnPackageOperationsStartingCalled);
		}
		
		[Test]
		public void ManagePackage_TwoProjectsSelectedAndUserCancelsSelectedProjects_MessagesReportedPreviouslyAreNotCleared()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			UserCancelsProjectSelection();
			
			viewModel.ManagePackage();
			
			Assert.IsFalse(fakePackageManagementEvents.IsOnPackageOperationsStartingCalled);
		}
		
		[Test]
		public void PackageChanged_TwoProjectsSelectedAndUserAcceptsSelectedProjects_IsAddedPropertyChanged()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			viewModel.FakePackageManagementEvents.ProjectsToSelect.Add("Project A");
			viewModel.FakePackageManagementEvents.ProjectsToSelect.Add("Project B");
			UserAcceptsProjectSelection();
			string propertyChangedName = null;
			viewModel.PropertyChanged += (sender, e) => propertyChangedName = e.PropertyName;
			viewModel.ManagePackage();
			
			viewModel.PackageChanged();
			
			Assert.AreEqual("IsAdded", propertyChangedName);
		}
		
		[Test]
		public void ManagePackage_TwoProjectsSelectedAndUserAcceptsSelectedProjects_NextToLastMessageLoggedMarksEndOfInstallation()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			UserAcceptsProjectSelection();
			
			viewModel.ManagePackage();
			
			string expectedMessage = "==============================";
			string actualMessage = fakeLogger.NextToLastFormattedMessageLogged;
						
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void ManagePackage_TwoProjectsSelectedAndUserAcceptsSelectedProjects_LastMessageLoggedIsEmptyLine()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			UserAcceptsProjectSelection();
			
			viewModel.ManagePackage();
			
			string expectedMessage = String.Empty;
			string actualMessage = fakeLogger.LastFormattedMessageLogged;
			
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_OneProjectIsSelected_OneProjectIsInstalled()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FakeSelectedProject project = fakeSelectedProjects[1];
			project.IsSelected = true;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			List<IPackageAction> actions = fakeActionRunner.GetActionsRunInOneCallAsList();
			InstallPackageAction action = actions[0] as InstallPackageAction;
			InstallPackageAction expectedAction = project.FakeProject.LastInstallPackageCreated;
			Assert.AreEqual(1, actions.Count);
			Assert.AreEqual(fakePackage, action.Package);
			Assert.AreEqual(expectedAction, action);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_ParentAllowsPrereleasePackagesAndOneProjectIsSelected_OneProjectIsInstalledAllowingPrereleasePackages()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			ParentAllowsPrereleasePackages();
			FakeSelectedProject project = fakeSelectedProjects[1];
			project.IsSelected = true;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			List<IPackageAction> actions = fakeActionRunner.GetActionsRunInOneCallAsList();
			InstallPackageAction action = actions[0] as InstallPackageAction;
			Assert.IsTrue(action.AllowPrereleaseVersions);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_ParentDoesNotAllowPrereleasePackagesAndOneProjectIsSelected_OneProjectIsInstalledNotAllowingPrereleasePackages()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FakeSelectedProject project = fakeSelectedProjects[1];
			project.IsSelected = true;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			List<IPackageAction> actions = fakeActionRunner.GetActionsRunInOneCallAsList();
			InstallPackageAction action = actions[0] as InstallPackageAction;
			Assert.IsFalse(action.AllowPrereleaseVersions);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_FirstProjectIsSelectedAndPackageOperationRequiresLicenseAcceptance_UserPromptedToAcceptLicenses()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FakeSelectedProject selectedProject = fakeSelectedProjects[0];
			selectedProject.IsSelected = true;
			FakePackageOperation operation = selectedProject.AddFakeInstallPackageOperation();
			operation.FakePackage.RequireLicenseAcceptance = true;
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = false;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			var expectedPackages = new FakePackage[] {
				operation.FakePackage
			};
			
			List<IPackage> actualPackages = fakePackageManagementEvents.GetPackagesPassedToOnAcceptLicensesAsList();
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_FirstProjectIsSelectedAndPackageOperationRequiresLicenseAcceptance_PackageInViewModelUsedToGetPackageOperations()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FakeSelectedProject selectedProject = fakeSelectedProjects[0];
			selectedProject.IsSelected = true;
			FakePackageOperation operation = selectedProject.AddFakeInstallPackageOperation();
			operation.FakePackage.RequireLicenseAcceptance = true;
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = false;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			IPackage actualPackage = selectedProject.FakeProject.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(fakePackage, actualPackage);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_FirstProjectIsSelectedAndPackageOperationRequiresLicenseAcceptance_PackageDependenciesAreNotIgnored()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FakeSelectedProject selectedProject = fakeSelectedProjects[0];
			selectedProject.IsSelected = true;
			FakePackageOperation operation = selectedProject.AddFakeInstallPackageOperation();
			operation.FakePackage.RequireLicenseAcceptance = true;
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = false;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			bool ignored = selectedProject.FakeProject.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(ignored);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_FirstProjectIsSelectedAndPackageOperationRequiresLicenseAcceptance_PrereleaseVersionsAreNotAllowed()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FakeSelectedProject selectedProject = fakeSelectedProjects[0];
			selectedProject.IsSelected = true;
			FakePackageOperation operation = selectedProject.AddFakeInstallPackageOperation();
			operation.FakePackage.RequireLicenseAcceptance = true;
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = false;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			bool allowed = selectedProject.FakeProject.AllowPrereleaseVersionsPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(allowed);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_ParentAllowsPrereleasesAndFirstProjectIsSelectedAndPackageOperationRequiresLicenseAcceptance_PrereleaseVersionsAreAllowedInPackageOperations()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FakeSelectedProject selectedProject = fakeSelectedProjects[0];
			selectedProject.IsSelected = true;
			FakePackageOperation operation = selectedProject.AddFakeInstallPackageOperation();
			operation.FakePackage.RequireLicenseAcceptance = true;
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = false;
			ParentAllowsPrereleasePackages();
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			bool allowed = selectedProject.FakeProject.AllowPrereleaseVersionsPassedToGetInstallPackageOperations;
			
			Assert.IsTrue(allowed);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_FirstProjectIsSelectedAndTwoPackageOperationsRequireLicenseAcceptance_UserPromptedToAcceptLicensesForTwoPackages()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FirstFakeSelectedProject.IsSelected = true;
			
			FakePackageOperation firstOperation = 
				AddFakeInstallPackageOperationWithPackageThatRequiresLicenseAcceptance(FirstFakeSelectedProject);
			
			FakePackageOperation secondOperation = 
				AddFakeInstallPackageOperationWithPackageThatRequiresLicenseAcceptance(FirstFakeSelectedProject);
			
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = false;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			var expectedPackages = new FakePackage[] {
				firstOperation.FakePackage,
				secondOperation.FakePackage
			};
			
			List<IPackage> actualPackages = fakePackageManagementEvents.GetPackagesPassedToOnAcceptLicensesAsList();
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_TwoPackageOperationsRequireLicenseAcceptanceButOneIsUninstallPackageOperation_UserPromptedToAcceptLicenseForInstallPackageOperationOnly()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FirstFakeSelectedProject.IsSelected = true;
			FakePackageOperation installOperation = 
				AddFakeInstallPackageOperationWithPackageThatRequiresLicenseAcceptance(FirstFakeSelectedProject);
			AddFakeUninstallPackageOperationWithPackageThatRequiresLicenseAcceptance(FirstFakeSelectedProject);
						
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = false;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			var expectedPackages = new FakePackage[] {
				installOperation.FakePackage
			};
			
			List<IPackage> actualPackages = fakePackageManagementEvents.GetPackagesPassedToOnAcceptLicensesAsList();
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_OneInstallPackageOperationsDoesNotRequireLicenseAcceptance_UserIsNotPromptedToAcceptLicense()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FirstFakeSelectedProject.IsSelected = true;
			AddFakeInstallPackageOperationWithPackageThatDoesNotRequireLicenseAcceptance(FirstFakeSelectedProject);
			
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = false;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			Assert.IsFalse(fakePackageManagementEvents.IsOnAcceptLicensesCalled);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_OneInstallPackageOperationsRequiresLicenseAcceptanceButIsInstalledInSolutionAlready_UserIsNotPromptedToAcceptLicense()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FirstFakeSelectedProject.IsSelected = true;
			
			FakePackageOperation installOperation = 
				AddFakeInstallPackageOperationWithPackageThatRequiresLicenseAcceptance(FirstFakeSelectedProject);
			
			fakeSolution.FakeInstalledPackages.Add(installOperation.FakePackage);
			
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = false;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			Assert.IsFalse(fakePackageManagementEvents.IsOnAcceptLicensesCalled);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_FirstProjectIsSelectedAndUserDoesNotAcceptPackageLicense_PackageIsNotInstalled()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FirstFakeSelectedProject.IsSelected = true;
			
			AddFakeInstallPackageOperationWithPackageThatRequiresLicenseAcceptance(FirstFakeSelectedProject);
			
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = false;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			Assert.IsFalse(fakeActionRunner.IsRunCalled);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_TwoProjectsButNeitherIsSelected_NoPackageActionsAreRun()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			Assert.IsFalse(fakeActionRunner.IsRunCalled);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_FirstProjectIsSelectedAndUserAcceptsPackageLicense_PackageIsInstalled()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FirstFakeSelectedProject.IsSelected = true;
			
			AddFakeInstallPackageOperationWithPackageThatRequiresLicenseAcceptance(FirstFakeSelectedProject);
			
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = true;
				
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			InstallPackageAction expectedAction = FirstFakeSelectedProject.FakeProject.LastInstallPackageCreated;
			List<IPackageAction> actions = fakeActionRunner.GetActionsRunInOneCallAsList();
			InstallPackageAction action = actions[0] as InstallPackageAction;
			
			Assert.AreEqual(1, actions.Count);
			Assert.AreEqual(fakePackage, action.Package);
			Assert.AreEqual(expectedAction, action);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_TwoProjectsAreSelectedAndUserAcceptsPackageLicense_UserIsNotPromptedTwiceToAcceptLicenses()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FirstFakeSelectedProject.IsSelected = true;
			AddFakeInstallPackageOperationWithPackageThatRequiresLicenseAcceptance(FirstFakeSelectedProject);
			
			SecondFakeSelectedProject.IsSelected = true;
			AddFakeInstallPackageOperationWithPackageThatRequiresLicenseAcceptance(SecondFakeSelectedProject);
			
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = true;
				
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			List<IEnumerable<IPackage>> packagesPassedToAcceptLicenses =
				fakePackageManagementEvents.PackagesPassedToAcceptLicenses;
			Assert.AreEqual(1, packagesPassedToAcceptLicenses.Count);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_FirstProjectSelectedAndExceptionThrownWhenResolvingPackageOperations_ExceptionReported()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FirstFakeSelectedProject.IsSelected = true;
			var exceptionThrowingProject = new ExceptionThrowingPackageManagementProject();
			FirstFakeSelectedProject.FakeProject = exceptionThrowingProject;
			AddFakeInstallPackageOperationWithPackageThatDoesNotRequireLicenseAcceptance(FirstFakeSelectedProject);
			
			var exception = new Exception("Test");
			exceptionThrowingProject.ExceptionToThrowWhenGetInstallPackageOperationsCalled = exception;
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			Assert.AreEqual(exception, fakePackageManagementEvents.ExceptionPassedToOnPackageOperationError);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_FirstProjectSelectedAndExceptionThrownWhenCreatingInstallAction_ExceptionLogged()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FirstFakeSelectedProject.IsSelected = true;
			var exceptionThrowingProject = new ExceptionThrowingPackageManagementProject();
			FirstFakeSelectedProject.FakeProject = exceptionThrowingProject;
			AddFakeInstallPackageOperationWithPackageThatDoesNotRequireLicenseAcceptance(FirstFakeSelectedProject);
			
			var exception = new Exception("Exception error message");
			exceptionThrowingProject.ExceptionToThrowWhenCreateInstallPackageActionCalled = exception;
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			bool contains = fakeLogger.FormattedMessagesLoggedContainsText("Exception error message");
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_TwoProjectsOneSelectedAndPackageRequiresLicenseAcceptance_PackageViewModelLoggerUsedWhenResolvingPackageOperations()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			FirstFakeSelectedProject.IsSelected = true;
			AddFakeInstallPackageOperationWithPackageThatRequiresLicenseAcceptance(FirstFakeSelectedProject);
			fakePackageManagementEvents.OnAcceptLicensesReturnValue = true;
			
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			ILogger expectedLogger = viewModel.OperationLoggerCreated;
			ILogger actualLogger = FirstFakeSelectedProject.Project.Logger;
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void ManagePackage_UserAcceptsProjectSelection_ManagingPackageMessageIsFirstMessageLogged()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			UserAcceptsProjectSelection();
			fakePackage.Id = "Test.Package";
			fakePackage.Version = new SemanticVersion(1, 2, 0, 55);
			viewModel.ManagePackage();
			
			string expectedMessage = "------- Managing...Test.Package 1.2.0.55 -------";
			string actualMessage = fakeLogger.FirstFormattedMessageLogged;
			
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void ManagePackagesForSelectedProjects_TwoProjectsNoneSelectedAndFirstPackageHasLicense_UserIsNotPromptedToAcceptLicense()
		{
			CreateViewModel();
			CreateTwoFakeSelectedProjects();
			AddFakeInstallPackageOperationWithPackageThatRequiresLicenseAcceptance(FirstFakeSelectedProject);
				
			viewModel.ManagePackagesForSelectedProjects(fakeSelectedProjects);
			
			Assert.IsFalse(fakePackageManagementEvents.IsOnAcceptLicensesCalled);
		}
		
		[Test]
		public void ManagePackage_TwoProjectsAndFirstSelectedInDialog_PackageIsInstalled()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			UserAcceptsProjectSelection();
			fakePackageManagementEvents.ProjectsToSelect.Add("Project A");
			
			viewModel.ManagePackage();
			
			List<IPackageAction> actions = fakeActionRunner.GetActionsRunInOneCallAsList();
			var action = actions[0] as ProcessPackageAction;
			FakePackageManagementProject expectedProject = fakeSolution.FakeProjectsToReturnFromGetProject["Project A"];
			Assert.AreEqual(expectedProject, action.Project);
		}
		
		[Test]
		public void ManagePackage_TwoProjectsAndSecondSelectedInDialog_ProjectHasLoggerSet()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			UserAcceptsProjectSelection();
			fakePackageManagementEvents.ProjectsToSelect.Add("Project B");
			viewModel.ManagePackage();
			
			FakePackageManagementProject project = fakeSolution.FakeProjectsToReturnFromGetProject["Project B"];
			ILogger expectedLogger = viewModel.OperationLoggerCreated;
			ILogger actualLogger = project.Logger;
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void Summary_PackageHasSummary_PackageSummaryReturned()
		{
			CreateViewModel();
			fakePackage.Summary = "Expected summary";
			
			string summary = viewModel.Summary;
			
			Assert.AreEqual("Expected summary", summary);
		}
		
		[Test]
		public void Summary_PackageHasDescriptionButNoSummary_PackageDescriptionReturned()
		{
			CreateViewModel();
			fakePackage.Summary = String.Empty;
			fakePackage.Description = "Expected description";
			
			string summary = viewModel.Summary;
			
			Assert.AreEqual("Expected description", summary);
		}
		
		[Test]
		public void AddPackage_ParentAllowsPrereleasePackages_PackageIsInstalledAllowingPrereleasePackages()
		{
			CreateViewModel();
			ParentAllowsPrereleasePackages();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			
			viewModel.AddPackage();
			
			var action = fakeActionRunner.ActionPassedToRun as InstallPackageAction;
			Assert.IsTrue(action.AllowPrereleaseVersions);
		}
		
		[Test]
		public void AddPackage_ParentDoesNotAllowPrereleasePackages_PackageIsInstalledNotAllowingPrereleasePackages()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			
			viewModel.AddPackage();
			
			var action = fakeActionRunner.ActionPassedToRun as InstallPackageAction;
			Assert.IsFalse(action.AllowPrereleaseVersions);
		}
		
		[Test]
		public void Name_PackageHasIdButNoTitle_ReturnsPackageId()
		{
			CreateViewModel();
			fakePackage.Id = "MyPackage";
			
			string name = viewModel.Name;
			
			Assert.AreEqual("MyPackage", name);
		}
		
		[Test]
		public void Name_PackageHasIdAndTitle_ReturnsPackageId()
		{
			CreateViewModel();
			fakePackage.Id = "MyPackage";
			fakePackage.Title = "My Package Title";
			
			string name = viewModel.Name;
			
			Assert.AreEqual("My Package Title", name);
		}
		
		[Test]
		public void GalleryUrl_PackageHasGalleryUrl_ReturnsUrl()
		{
			CreateViewModel();
			var expectedUrl = new Uri("http://test.com/MyPackage");
			fakePackage.GalleryUrl = expectedUrl;
			
			Uri url = viewModel.GalleryUrl;
			
			Assert.AreEqual(expectedUrl, url);
		}
		
		[Test]
		public void HasGalleryUrl_PackageHasGalleryUrl_ReturnsTrue()
		{
			CreateViewModel();
			var expectedUrl = new Uri("http://test.com/MyPackage");
			fakePackage.GalleryUrl = expectedUrl;
			
			bool result = viewModel.HasGalleryUrl;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void HasNoGalleryUrl_PackageHasNoGalleryUrl_ReturnsFalse()
		{
			CreateViewModel();
			fakePackage.GalleryUrl = null;
			
			bool result = viewModel.HasGalleryUrl;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HasNoGalleryUrl_PackageHasGalleryUrl_ReturnsFalse()
		{
			CreateViewModel();
			var expectedUrl = new Uri("http://test.com/MyPackage");
			fakePackage.GalleryUrl = expectedUrl;
			
			bool result = viewModel.HasNoGalleryUrl;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsGalleryUrlMissing_PackageHasNoGalleryUrl_ReturnsTrue()
		{
			CreateViewModel();
			fakePackage.GalleryUrl = null;
			
			bool result = viewModel.HasNoGalleryUrl;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void AddPackage_PackageRepositoryIsOperationAwareAndPackageAddedSuccessfully_InstallOperationStartedForPackage()
		{
			CreateViewModel();
			var operationAwareRepository = new FakeOperationAwarePackageRepository();
			fakePackage.FakePackageRepository = operationAwareRepository;
			fakePackage.Id = "MyPackage";
			
			viewModel.AddPackage();
			
			operationAwareRepository.AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Install, "MyPackage");
		}
		
		[Test]
		public void AddPackage_PackageRepositoryIsOperationAwareAndExceptionThrownWhenCreatingInstallPackageAction_InstallOperationStartedForPackageBeforeInstallPackageActionCreatedAndPackageOperationsRequested()
		{
			CreateViewModelWithExceptionThrowingProject();
			var operationAwareRepository = new FakeOperationAwarePackageRepository();
			fakePackage.FakePackageRepository = operationAwareRepository;
			fakePackage.Id = "MyPackage";
			Exception ex = new Exception("Test");
			exceptionThrowingProject.ExceptionToThrowWhenCreateInstallPackageActionCalled = ex;
		
			viewModel.AddPackage();
			
			operationAwareRepository.AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Install, "MyPackage");
		}
		
		[Test]
		public void ManagePackage_TwoProjectsAndFirstSelectedAndUserAcceptsSelectedProjectsAndSourceRepositoryIsOperationAware_InstallOperationStartedForPackage()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			UserAcceptsProjectSelection();
			fakePackageManagementEvents.ProjectsToSelect.Add("Project A");
			var operationAwareRepository = new FakeOperationAwarePackageRepository();
			fakePackage.FakePackageRepository = operationAwareRepository;
			fakePackage.Id = "MyPackage";
			
			viewModel.ManagePackage();
			
			operationAwareRepository.AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Install, "MyPackage");
		}
		
		[Test]
		public void ManagePackage_TwoProjectsNeitherSelectedAndSourceRepositoryIsOperationAware_InstallOperationStarted()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			UserAcceptsProjectSelection();
			var operationAwareRepository = new FakeOperationAwarePackageRepository();
			fakePackage.FakePackageRepository = operationAwareRepository;
			fakePackage.Id = "MyPackage";
			
			viewModel.ManagePackage();
			
			operationAwareRepository.AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Install, "MyPackage");
		}
		
		[Test]
		public void AddOrManagePackage_SolutionSelectedAndContainsOnlyOneProject_UserPromptedToSelectOneProject()
		{
			CreateFakeSolution();
			AddProjectToSolution();
			fakeSolution.FakeMSBuildProjects[0].Name = "MyProject";
			fakeSolution.NoProjectsSelected();
			fakeSolution.AddFakeProjectToReturnFromGetProject("MyProject");
			CreateViewModel(fakeSolution);
			UserCancelsProjectSelection();
			
			viewModel.AddOrManagePackage();
			
			IEnumerable<IPackageManagementSelectedProject> selectedProjects = 
				fakePackageManagementEvents.SelectedProjectsPassedToOnSelectProjects;
			
			var expectedSelectedProjects = new List<IPackageManagementSelectedProject>();
			expectedSelectedProjects.Add(new FakeSelectedProject("MyProject"));
			
			var nullReferenceException = new NullReferenceException();
			Assert.IsFalse(fakeLogger.FormattedMessagesLoggedContainsText(nullReferenceException.Message));
			Assert.IsNotNull(selectedProjects);
			SelectedProjectCollectionAssert.AreEqual(expectedSelectedProjects, selectedProjects);
		}
		
		[Test]
		public void AddPackage_ExceptionWhenCheckIfPackageIsForProject_ExceptionErrorMessageReported()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			var ex = new Exception("Test");
			viewModel.IsProjectPackageAction = package => {
				throw ex;
			};
			viewModel.AddPackage();
			
			Assert.AreEqual(ex, fakePackageManagementEvents.ExceptionPassedToOnPackageOperationError);
			Assert.IsTrue(fakeLogger.FormattedMessagesLoggedContainsText("Test"));
		}
		
		[Test]
		public void AddOrManagePackage_ExceptionThrownWhenCheckingIfPackageIsForProject_ExceptionIsLogged()
		{
			CreateFakeSolution();
			AddProjectToSolution();
			fakeSolution.FakeMSBuildProjects[0].Name = "MyProject";
			fakeSolution.NoProjectsSelected();
			fakeSolution.AddFakeProjectToReturnFromGetProject("MyProject");
			CreateViewModel(fakeSolution);
			var ex = new Exception("Test");
			viewModel.IsProjectPackageAction = package => {
				throw ex;
			};
			
			viewModel.AddOrManagePackage();
			
			Assert.AreEqual(ex, fakePackageManagementEvents.ExceptionPassedToOnPackageOperationError);
			Assert.IsTrue(fakeLogger.FormattedMessagesLoggedContainsText("Test"));
		}
		
		[Test]
		public void RemovePackage_ExceptionWhenCheckingIfPackageIsForProject_ExceptionErrorMessageReported()
		{
			CreateViewModel();
			var ex = new Exception("Test");
			viewModel.IsProjectPackageAction = package => {
				throw ex;
			};
			
			viewModel.RemovePackage();
			
			Assert.AreEqual(ex, fakePackageManagementEvents.ExceptionPassedToOnPackageOperationError);
			Assert.IsTrue(fakeLogger.FormattedMessagesLoggedContainsText("Test"));
		}
		
		[Test]
		public void RemoveOrManagePackage_ExceptionThrownWhenCheckingIfPackageIsForProject_ExceptionIsLogged()
		{
			CreateFakeSolution();
			AddProjectToSolution();
			AddProjectToSolution();
			fakeSolution.FakeMSBuildProjects[0].Name = "MyProject";
			fakeSolution.NoProjectsSelected();
			fakeSolution.AddFakeProjectToReturnFromGetProject("MyProject");
			CreateViewModel(fakeSolution);
			var ex = new Exception("Test");
			viewModel.IsProjectPackageAction = package => {
				throw ex;
			};
			
			viewModel.RemoveOrManagePackage();
			
			Assert.AreEqual(ex, fakePackageManagementEvents.ExceptionPassedToOnPackageOperationError);
			Assert.IsTrue(fakeLogger.FormattedMessagesLoggedContainsText("Test"));
		}
	}
}
