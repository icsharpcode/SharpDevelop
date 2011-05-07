// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class AddPackageReferenceViewModel : ViewModelBase<AddPackageReferenceViewModel>, IMessageReporter
	{
		IPackageManagementService packageManagementService;
		InstalledPackagesViewModel installedPackagesViewModel;
		AvailablePackagesViewModel availablePackagesViewModel;
		PackageUpdatesViewModel packageUpdatesViewModel;
		RecentPackagesViewModel recentPackagesViewModel;
		string message;
		bool hasError;
		
		public AddPackageReferenceViewModel(
			IPackageManagementService packageManagementService,
			ITaskFactory taskFactory)
		{
			this.packageManagementService = packageManagementService;
			this.packageManagementService.OutputMessagesView.Clear();
			
			availablePackagesViewModel = new AvailablePackagesViewModel(packageManagementService, this, taskFactory);
			installedPackagesViewModel = new InstalledPackagesViewModel(packageManagementService, this, taskFactory);
			packageUpdatesViewModel = new PackageUpdatesViewModel(packageManagementService, this, taskFactory);
			recentPackagesViewModel = new RecentPackagesViewModel(packageManagementService, this, taskFactory);
			
			availablePackagesViewModel.ReadPackages();
			installedPackagesViewModel.ReadPackages();
			packageUpdatesViewModel.ReadPackages();
			recentPackagesViewModel.ReadPackages();
		}
		
		public InstalledPackagesViewModel InstalledPackagesViewModel {
			get { return installedPackagesViewModel; }
		}
		
		public AvailablePackagesViewModel AvailablePackagesViewModel {
			get { return availablePackagesViewModel; }
		}
		
		public PackageUpdatesViewModel PackageUpdatesViewModel {
			get { return packageUpdatesViewModel; }
		}
		
		public RecentPackagesViewModel RecentPackagesViewModel {
			get { return recentPackagesViewModel; }
		}
		
		public void ShowErrorMessage(string message)
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
		
		public void ClearMessage()
		{
			this.Message = null;
			this.HasError = false;
		}
	}
}
