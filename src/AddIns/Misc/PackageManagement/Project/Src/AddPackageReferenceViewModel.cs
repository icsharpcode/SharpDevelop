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
		
		public AddPackageReferenceViewModel(
			IPackageManagementService packageManagementService,
			ITaskFactory taskFactory)
		{
			this.packageManagementService = packageManagementService;
			this.packageManagementService.OutputMessagesView.Clear();
			
			installedPackagesViewModel = new InstalledPackagesViewModel(packageManagementService, taskFactory);
			availablePackagesViewModel = new AvailablePackagesViewModel(packageManagementService, taskFactory);
			packageUpdatesViewModel = new PackageUpdatesViewModel(packageManagementService, taskFactory);
			
			installedPackagesViewModel.ReadPackages();
			availablePackagesViewModel.ReadPackages();
			packageUpdatesViewModel.ReadPackages();
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
	}
}
