// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class AddPackageReferenceViewModel : ViewModelBase<AddPackageReferenceViewModel>
	{
		IPackageManagementService packageManagementService;
		InstalledPackagesViewModel installedPackagesViewModel;
		AvailablePackagesViewModel availablePackagesViewModel;
		PackageUpdatesViewModel packageUpdatesViewModel;
		RecentPackagesViewModel recentPackagesViewModel;
		
		public AddPackageReferenceViewModel(
			IPackageManagementService packageManagementService,
			ITaskFactory taskFactory)
		{
			this.packageManagementService = packageManagementService;
			this.packageManagementService.OutputMessagesView.Clear();
			
			availablePackagesViewModel = new AvailablePackagesViewModel(packageManagementService, taskFactory);
			installedPackagesViewModel = new InstalledPackagesViewModel(packageManagementService, taskFactory);
			packageUpdatesViewModel = new PackageUpdatesViewModel(packageManagementService, taskFactory);
			recentPackagesViewModel = new RecentPackagesViewModel(packageManagementService, taskFactory);
			
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
	}
}
