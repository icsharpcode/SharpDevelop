// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageViewModelTests
	{
		TestablePackageViewModel viewModel;
		FakePackage package;
		FakePackageManagementService packageManagementService;
		FakePackageRepository sourcePackageRepository;
		FakeLicenseAcceptanceService licenseAcceptanceService;
		FakeMessageReporter messageReporter;
		ExceptionThrowingPackageManagementService exceptionThrowingPackageManagementService;
		FakeInstallPackageAction fakeInstallPackageTask;
		FakeUninstallPackageAction fakeUninstallPackageAction;
		
		void CreateViewModel()
		{
			packageManagementService = new FakePackageManagementService();
			CreateViewModel(packageManagementService);
		}
		
		void CreateViewModelWithExceptionThrowingPackageManagementService()
		{
			exceptionThrowingPackageManagementService = new ExceptionThrowingPackageManagementService();
			CreateViewModel(exceptionThrowingPackageManagementService);
		}
		
		void CreateViewModel(FakePackageManagementService packageManagementService)
		{
			viewModel = new TestablePackageViewModel(packageManagementService);
			package = viewModel.FakePackage;
			this.packageManagementService = packageManagementService;
			sourcePackageRepository = packageManagementService.FakeActivePackageRepository;
			licenseAcceptanceService = viewModel.FakeLicenseAcceptanceService;
			messageReporter = viewModel.FakeMessageReporter;
			fakeInstallPackageTask = packageManagementService.ActionToReturnFromCreateInstallPackageAction;
			fakeUninstallPackageAction = packageManagementService.ActionToReturnFromCreateUninstallPackageAction;
		}

		[Test]
		public void AddPackageCommand_CommandExecuted_InstallsPackage()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			
			viewModel.AddPackageCommand.Execute(null);
						
			Assert.AreEqual(package, fakeInstallPackageTask.Package);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageAddedFromSourcePackageRepository()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			
			viewModel.AddPackage();
						
			Assert.AreEqual(sourcePackageRepository, fakeInstallPackageTask.PackageRepository);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageIsInstalled()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			
			viewModel.AddPackage();
						
			Assert.IsTrue(fakeInstallPackageTask.IsExecuteCalled);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageOperationsUsedWhenInstallingPackage()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
		
			PackageOperation[] expectedOperations = new PackageOperation[] {
				new PackageOperation(package, PackageAction.Install)
			};
			
			CollectionAssert.AreEqual(expectedOperations, fakeInstallPackageTask.Operations);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PropertyNotifyChangedFiredForIsAddedProperty()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();

			string propertyChangedName = null;
			viewModel.PropertyChanged += (sender, e) => propertyChangedName = e.PropertyName;
			viewModel.AddPackage();
			
			Assert.AreEqual("IsAdded", propertyChangedName);
		}

		[Test]
		public void AddPackage_PackageAddedSuccessfully_PropertyNotifyChangedFiredAfterPackageInstalled()
		{
			CreateViewModel();
			IPackage packagePassedToInstallPackageWhenPropertyNameChanged = null;
			viewModel.PropertyChanged += (sender, e) => {
				packagePassedToInstallPackageWhenPropertyNameChanged = fakeInstallPackageTask.Package;
			};
			viewModel.AddPackage();
			
			Assert.AreEqual(package, packagePassedToInstallPackageWhenPropertyNameChanged);
		}

		[Test]
		public void HasLicenseUrl_PackageHasLicenseUrl_ReturnsTrue()
		{
			CreateViewModel();
			package.LicenseUrl = new Uri("http://sharpdevelop.com");
			
			Assert.IsTrue(viewModel.HasLicenseUrl);
		}
		
		[Test]
		public void HasLicenseUrl_PackageHasNoLicenseUrl_ReturnsFalse()
		{
			CreateViewModel();
			package.LicenseUrl = null;
			
			Assert.IsFalse(viewModel.HasLicenseUrl);
		}
		
		[Test]
		public void HasProjectUrl_PackageHasProjectUrl_ReturnsTrue()
		{
			CreateViewModel();
			package.ProjectUrl = new Uri("http://sharpdevelop.com");
			
			Assert.IsTrue(viewModel.HasProjectUrl);
		}
		
		[Test]
		public void HasProjectUrl_PackageHasNoProjectUrl_ReturnsFalse()
		{
			CreateViewModel();
			package.ProjectUrl = null;
			
			Assert.IsFalse(viewModel.HasProjectUrl);
		}
		
		[Test]
		public void HasReportAbuseUrl_PackageHasReportAbuseUrl_ReturnsTrue()
		{
			CreateViewModel();
			package.ReportAbuseUrl = new Uri("http://sharpdevelop.com");
			
			Assert.IsTrue(viewModel.HasReportAbuseUrl);
		}
		
		[Test]
		public void HasReportAbuseUrl_PackageHasNoReportAbuseUrl_ReturnsFalse()
		{
			CreateViewModel();
			package.ReportAbuseUrl = null;
			
			Assert.IsFalse(viewModel.HasReportAbuseUrl);
		}
		
		[Test]
		public void IsAdded_ProjectHasPackageAdded_ReturnsTrue()
		{
			CreateViewModel();
			packageManagementService.FakeActiveProjectManager.IsInstalledReturnValue = true;
			
			Assert.IsTrue(viewModel.IsAdded);
		}
		
		[Test]
		public void IsAdded_ProjectDoesNotHavePackageInstalled_ReturnsFalse()
		{
			CreateViewModel();
			packageManagementService.FakeActiveProjectManager.IsInstalledReturnValue = false;
			
			Assert.IsFalse(viewModel.IsAdded);
		}
		
		[Test]
		public void RemovePackageCommand_CommandExecuted_UninstallsPackage()
		{
			CreateViewModel();
			viewModel.RemovePackageCommand.Execute(null);
						
			Assert.AreEqual(package, fakeUninstallPackageAction.Package);
		}
		
		[Test]
		public void RemovePackage_PackageRemovedSuccessfully_RepositoryUsedToUninstallPackage()
		{
			CreateViewModel();
			viewModel.RemovePackage();
			
			Assert.AreEqual(sourcePackageRepository, fakeUninstallPackageAction.PackageRepository);
		}
		
		[Test]
		public void RemovePackage_PackageRemovedSuccessfully_PropertyNotifyChangedFiredForIsAddedProperty()
		{
			CreateViewModel();
			string propertyChangedName = null;
			viewModel.PropertyChanged += (sender, e) => propertyChangedName = e.PropertyName;
			viewModel.RemovePackage();
			
			Assert.AreEqual("IsAdded", propertyChangedName);
		}
		
		[Test]
		public void RemovePackage_PackageRemovedSuccessfully_PropertyNotifyChangedFiredAfterPackageUninstalled()
		{
			CreateViewModel();
			IPackage packagePassedToUninstallPackageWhenPropertyNameChanged = null;
			viewModel.PropertyChanged += (sender, e) => {
				packagePassedToUninstallPackageWhenPropertyNameChanged = fakeUninstallPackageAction.Package;
			};
			viewModel.RemovePackage();
			
			Assert.AreEqual(package, packagePassedToUninstallPackageWhenPropertyNameChanged);
		}
		
		[Test]
		public void HasDependencies_PackageHasNoDependencies_ReturnsFalse()
		{
			CreateViewModel();
			package.DependenciesList.Clear();
			
			Assert.IsFalse(viewModel.HasDependencies);
		}
		
		[Test]
		public void HasDependencies_PackageHasOneDependency_ReturnsTrue()
		{
			CreateViewModel();
			package.DependenciesList.Add(new PackageDependency("Test"));
			
			Assert.IsTrue(viewModel.HasDependencies);
		}
		
		[Test]
		public void HasNoDependencies_PackageHasNoDependencies_ReturnsTrue()
		{
			CreateViewModel();
			package.DependenciesList.Clear();
			
			Assert.IsTrue(viewModel.HasNoDependencies);
		}
		
		[Test]
		public void HasNoDependencies_PackageHasOneDependency_ReturnsFalse()
		{
			CreateViewModel();
			package.DependenciesList.Add(new PackageDependency("Test"));
			
			Assert.IsFalse(viewModel.HasNoDependencies);
		}
		
		[Test]
		public void HasDownloadCount_DownloadCountIsZero_ReturnsTrue()
		{
			CreateViewModel();
			package.DownloadCount = 0;
			
			Assert.IsTrue(viewModel.HasDownloadCount);
		}
		
		[Test]
		public void HasDownloadCount_DownloadCountIsMinusOne_ReturnsFalse()
		{
			CreateViewModel();
			package.DownloadCount = -1;
			
			Assert.IsFalse(viewModel.HasDownloadCount);
		}
				
		[Test]
		public void AddPackage_PackageRequiresLicenseAgreementAcceptance_UserAskedToAcceptLicenseAgreementForPackageBeforeInstalling()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			package.RequireLicenseAcceptance = true;
			licenseAcceptanceService.AcceptLicensesReturnValue = true;
			
			viewModel.AddPackage();
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			var actualPackages = licenseAcceptanceService.PackagesPassedToAcceptLicenses;
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void AddPackage_PackageDoesNotRequireLicenseAgreementAcceptance_UserNotAskedToAcceptLicenseAgreementBeforeInstalling()
		{
			CreateViewModel();
			package.RequireLicenseAcceptance = false;
			viewModel.AddPackage();
			
			Assert.IsFalse(licenseAcceptanceService.IsAcceptLicensesCalled);
		}
		
		[Test]
		public void AddPackage_PackageRequiresLicenseAgreementAcceptanceAndUserDeclinesAgreement_PackageIsNotInstalled()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			package.RequireLicenseAcceptance = true;
			licenseAcceptanceService.AcceptLicensesReturnValue = false;
			
			viewModel.AddPackage();
			
			Assert.IsFalse(fakeInstallPackageTask.IsExecuteCalled);
		}
		
		[Test]
		public void AddPackage_PackageRequiresLicenseAgreementAcceptanceAndUserDeclinesAgreement_PropertyChangedEventNotFired()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			package.RequireLicenseAcceptance = true;
			licenseAcceptanceService.AcceptLicensesReturnValue = false;
			bool propertyChangedEventFired = false;
			viewModel.PropertyChanged += (sender, e) => propertyChangedEventFired = true;
			
			viewModel.AddPackage();
			
			Assert.IsFalse(propertyChangedEventFired);
		}
		
		[Test]
		public void AddPackage_OnePackageOperationIsToUninstallPackageWhichRequiresLicenseAcceptance_UserIsNotAskedToAcceptLicenseAgreementForPackageToBeUninstalled()
		{
			CreateViewModel();
			package.RequireLicenseAcceptance = false;
			var operation = viewModel.AddOneFakeUninstallPackageOperation();
			FakePackage packageToUninstall = operation.Package as FakePackage;
			packageToUninstall.RequireLicenseAcceptance = true;
			viewModel.AddPackage();
			
			Assert.IsFalse(licenseAcceptanceService.IsAcceptLicensesCalled);
		}
		
		[Test]
		public void AddPackage_CheckLoggerUsed_OutputMessagesLoggerUsedWhenResolvingPackageOperations()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			ILogger expectedLogger = packageManagementService.FakeOutputMessagesView;
			ILogger actualLogger = packageManagementService
				.FakePackageManagerToReturnFromCreatePackageManager
				.Logger;
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_InstallingPackageMessageIsFirstMessageLogged()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			package.Id = "Test.Package";
			package.Version = new Version(1, 2, 0, 55);
			viewModel.PackageViewModelAddingPackageMessageFormat = "Updating...{0}";
			viewModel.AddPackage();
			
			string expectedMessage = "------- Updating...Test.Package 1.2.0.55 -------";
			string actualMessage = packageManagementService.FakeOutputMessagesView.FirstFormattedMessageLogged;
						
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_NextToLastMessageLoggedMarksEndOfInstallation()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			string expectedMessage = "==============================";
			string actualMessage = packageManagementService.FakeOutputMessagesView.NextToLastFormattedMessageLogged;
						
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_LastMessageLoggedIsEmptyLine()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			string expectedMessage = String.Empty;
			string actualMessage = packageManagementService.FakeOutputMessagesView.LastFormattedMessageLogged;
						
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void RemovePackage_PackageRemovedSuccessfully_UninstallingPackageMessageIsFirstMessageLogged()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			package.Id = "Test.Package";
			package.Version = new Version(1, 2, 0, 55);
			viewModel.PackageViewModelRemovingPackageMessageFormat = "Removing...{0}";
			viewModel.RemovePackage();
			
			string expectedMessage = "------- Removing...Test.Package 1.2.0.55 -------";
			string actualMessage = packageManagementService.FakeOutputMessagesView.FirstFormattedMessageLogged;
						
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void RemovePackage_PackageRemovedSuccessfully_NextToLastMessageLoggedMarksEndOfInstallation()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.RemovePackage();
			
			string expectedMessage = "==============================";
			string actualMessage = packageManagementService.FakeOutputMessagesView.NextToLastFormattedMessageLogged;
						
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void RemovePackage_PackageRemovedSuccessfully_LastMessageLoggedIsEmptyLine()
		{
			CreateViewModel();
			viewModel.RemovePackage();
			
			string expectedMessage = String.Empty;
			string actualMessage = packageManagementService.FakeOutputMessagesView.LastFormattedMessageLogged;
						
			Assert.AreEqual(expectedMessage, actualMessage);
		}
		
		[Test]
		public void AddPackage_ExceptionWhenInstallingPackage_ExceptionErrorMessageReported()
		{
			CreateViewModelWithExceptionThrowingPackageManagementService();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			Exception ex = new Exception("Test");
			exceptionThrowingPackageManagementService.ExeptionToThrowWhenCreateInstallPackageTaskCalled = ex;
			viewModel.AddPackage();
			
			Assert.AreEqual("Test", messageReporter.MessagePassedToShowErrorMessage);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_MessagesReportedPreviouslyAreCleared()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			Assert.IsTrue(messageReporter.IsClearMessageCalled);
		}
		
		[Test]
		public void AddPackage_ExceptionWhenInstallingPackage_ExceptionLogged()
		{
			CreateViewModelWithExceptionThrowingPackageManagementService();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			Exception ex = new Exception("Exception error message");
			exceptionThrowingPackageManagementService.ExeptionToThrowWhenCreateInstallPackageTaskCalled = ex;
			viewModel.AddPackage();
			
			string actualMessage = packageManagementService.FakeOutputMessagesView.SecondFormattedMessageLogged;
			bool containsExceptionErrorMessage = actualMessage.Contains("Exception error message");
			
			Assert.IsTrue(containsExceptionErrorMessage, actualMessage);
		}
		
		[Test]
		public void RemovePackage_ExceptionWhenUninstallingPackage_ExceptionErrorMessageReported()
		{
			CreateViewModelWithExceptionThrowingPackageManagementService();
			Exception ex = new Exception("Test");
			exceptionThrowingPackageManagementService.ExeptionToThrowWhenCreateUninstallPackageActionCalled = ex;
			viewModel.RemovePackage();
			
			Assert.AreEqual("Test", messageReporter.MessagePassedToShowErrorMessage);
		}
		
		[Test]
		public void RemovePackage_PackageUninstalledSuccessfully_MessagesReportedPreviouslyAreCleared()
		{
			CreateViewModel();
			viewModel.RemovePackage();
			
			Assert.IsTrue(messageReporter.IsClearMessageCalled);
		}
		
		[Test]
		public void RemovePackage_ExceptionWhenUninstallingPackage_ExceptionLogged()
		{
			CreateViewModelWithExceptionThrowingPackageManagementService();
			Exception ex = new Exception("Exception error message");
			exceptionThrowingPackageManagementService.ExeptionToThrowWhenCreateUninstallPackageActionCalled = ex;
			viewModel.RemovePackage();
			
			string actualMessage = packageManagementService.FakeOutputMessagesView.SecondFormattedMessageLogged;
			bool containsExceptionErrorMessage = actualMessage.Contains("Exception error message");
			
			Assert.IsTrue(containsExceptionErrorMessage, actualMessage);
		}
		
		[Test]
		public void AddPackage_ExceptionThrownWhenResolvingPackageOperations_ExceptionReported()
		{
			CreateViewModelWithExceptionThrowingPackageManagementService();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			
			exceptionThrowingPackageManagementService.ExceptionToThrowWhenCreatePackageManagerForActiveProjectCalled =
				new Exception("Test");
			viewModel.AddPackage();
			
			Assert.AreEqual("Test", messageReporter.MessagePassedToShowErrorMessage);
		}
		
		[Test]
		public void AddPackage_PackagesInstalledSuccessfully_ViewModelPackageUsedWhenResolvingPackageOperations()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			var expectedPackage = package;
			var actualPackage = packageManagementService
				.FakePackageManagerToReturnFromCreatePackageManager
				.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void AddPackage_PackagesInstalledSuccessfully_PackageDependenciesNotIgnoredWhenCheckingForPackageOperations()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.AddPackage();
			
			bool result = packageManagementService
				.FakePackageManagerToReturnFromCreatePackageManager
				.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void RemovePackage_PackageRemovedSuccessfully_PackageIsRemoved()
		{
			CreateViewModel();
			viewModel.AddOneFakeInstallPackageOperationForViewModelPackage();
			viewModel.RemovePackage();
			
			Assert.IsTrue(fakeUninstallPackageAction.IsExecuted);
		}
	}
}
