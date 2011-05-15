// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;

namespace ICSharpCode.PackageManagement
{
	public class ViewModelLocator
	{
		AddPackageReferenceViewModel addPackageReferenceViewModel;
		RegisteredPackageSourcesViewModel registeredPackageSourcesViewModel;
		PackageManagementOptionsViewModel packageManagementOptionsViewModel;
		IPackageManagementService packageManagementService;
		
		public AddPackageReferenceViewModel AddPackageReferenceViewModel {
			get {
				CreateAddPackageReferenceViewModel();
				return addPackageReferenceViewModel;
			}
		}
		
		void CreateAddPackageReferenceViewModel()
		{
			CreatePackageManagementService();
			addPackageReferenceViewModel = new AddPackageReferenceViewModel(packageManagementService, new PackageManagementTaskFactory());
		}
		
		void CreatePackageManagementService()
		{
			if (packageManagementService == null) {
				if (IsInDesignMode()) {
					packageManagementService = new DesignTimePackageManagementService();
				}
				packageManagementService = ServiceLocator.PackageManagementService;
			}
		}
		
		bool IsInDesignMode()
		{
			return WpfDesigner.IsInDesignMode();
		}
		
		public RegisteredPackageSourcesViewModel RegisteredPackageSourcesViewModel {
			get {
				if (registeredPackageSourcesViewModel == null) {
					CreateRegisteredPackageSourcesViewModel();
				}
				return registeredPackageSourcesViewModel;
			}
		}
		
		void CreateRegisteredPackageSourcesViewModel()
		{
			CreatePackageManagementService();
			if (IsInDesignMode()) {
				registeredPackageSourcesViewModel = new DesignTimeRegisteredPackageSourcesViewModel();
			} else {
				registeredPackageSourcesViewModel = new RegisteredPackageSourcesViewModel(packageManagementService.Options);
			}
		}
		
		public PackageManagementOptionsViewModel PackageManagementOptionsViewModel {
			get {
				if (packageManagementOptionsViewModel == null) {
					CreatePackageManagementService();
					IRecentPackageRepository recentRepository = packageManagementService.RecentPackageRepository;
					packageManagementOptionsViewModel = new PackageManagementOptionsViewModel(recentRepository);
				}
				return packageManagementOptionsViewModel;
			}
		}
	}
}
