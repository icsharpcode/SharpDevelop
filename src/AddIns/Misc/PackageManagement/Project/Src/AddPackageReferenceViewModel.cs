// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class AddPackageReferenceViewModel : ViewModelBase<AddPackageReferenceViewModel>, IDisposable
	{
		IPackageManagementService packageManagementService;
		IPackageManagementEvents packageManagementEvents;
		ILicenseAcceptanceService licenseAcceptanceService;
		string message;
		bool hasError;
		
		public AddPackageReferenceViewModel(
			IPackageManagementService packageManagementService,
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagementEvents packageManagementEvents,
			ILicenseAcceptanceService licenseAcceptanceService,
			ITaskFactory taskFactory)
		{
			this.packageManagementService = packageManagementService;
			this.packageManagementEvents = packageManagementEvents;
			this.licenseAcceptanceService = licenseAcceptanceService;
			
			packageManagementEvents.PackageOperationError += PackageOperationError;
			packageManagementEvents.PackageOperationsStarting += PackageOperationsStarting;
			packageManagementEvents.AcceptLicenses += AcceptLicenses;
			
			var packageViewModelFactory = new PackageViewModelFactory(registeredPackageRepositories, packageManagementService, packageManagementEvents);
			
			AvailablePackagesViewModel = new AvailablePackagesViewModel(registeredPackageRepositories, packageViewModelFactory, taskFactory);
			InstalledPackagesViewModel = new InstalledPackagesViewModel(packageManagementService, packageManagementEvents, registeredPackageRepositories, packageViewModelFactory, taskFactory);
			UpdatedPackagesViewModel = new UpdatedPackagesViewModel(packageManagementService, registeredPackageRepositories, packageViewModelFactory, taskFactory);
			RecentPackagesViewModel = new RecentPackagesViewModel(packageManagementEvents, registeredPackageRepositories, packageViewModelFactory, taskFactory);
			
			AvailablePackagesViewModel.ReadPackages();
			InstalledPackagesViewModel.ReadPackages();
			UpdatedPackagesViewModel.ReadPackages();
			RecentPackagesViewModel.ReadPackages();
		}
		
		public AvailablePackagesViewModel AvailablePackagesViewModel { get; private set; }
		public InstalledPackagesViewModel InstalledPackagesViewModel { get; private set; }
		public RecentPackagesViewModel RecentPackagesViewModel { get; private set; }
		public UpdatedPackagesViewModel UpdatedPackagesViewModel { get; private set; }
		
		public void Dispose()
		{
			AvailablePackagesViewModel.Dispose();
			InstalledPackagesViewModel.Dispose();
			RecentPackagesViewModel.Dispose();
			UpdatedPackagesViewModel.Dispose();
			
			packageManagementEvents.AcceptLicenses -= AcceptLicenses;
			packageManagementEvents.PackageOperationError -= PackageOperationError;
			packageManagementEvents.PackageOperationsStarting -= PackageOperationsStarting;
		}
		
		void PackageOperationError(object sender, PackageOperationExceptionEventArgs e)
		{
			ShowErrorMessage(e.Exception.Message);
		}
		
		void ShowErrorMessage(string message)
		{
			this.Message = message;
			this.HasError = true;
		}
		
		public string Message {
			get { return message; }
			set {
				message = value;
				OnPropertyChanged(model => model.Message);
			}
		}
		
		public bool HasError {
			get { return hasError; }
			set {
				hasError = value;
				OnPropertyChanged(model => model.HasError);
			}
		}
		
		void PackageOperationsStarting(object sender, EventArgs e)
		{
			ClearMessage();
		}
		
		void ClearMessage()
		{
			this.Message = null;
			this.HasError = false;
		}
		
		void AcceptLicenses(object sender, AcceptLicensesEventArgs e)
		{
			e.IsAccepted = licenseAcceptanceService.AcceptLicenses(e.Packages);
		}
	}
}
