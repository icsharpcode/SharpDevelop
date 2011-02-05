// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;

namespace ICSharpCode.PackageManagement
{
	public class ViewModelLocator
	{
		AddPackageReferenceViewModel addPackageReferenceViewModel;
		PackageManagementOptionsViewModel packageManagementOptionsViewModel;
		IPackageManagementService packageManagementService;
		
		public AddPackageReferenceViewModel AddPackageReferenceViewModel {
			get {
				if (addPackageReferenceViewModel == null) {
					CreateAddPackageReferenceViewModel();
				}
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
		
		public PackageManagementOptionsViewModel PackageManagementOptionsViewModel {
			get {
				if (packageManagementOptionsViewModel == null) {
					CreatePackageManagementOptionsViewModel();
				}
				return packageManagementOptionsViewModel;
			}
		}
		
		void CreatePackageManagementOptionsViewModel()
		{
			CreatePackageManagementService();
			if (IsInDesignMode()) {
				packageManagementOptionsViewModel = new DesignTimePackageManagementOptionsViewModel();
			} else {
				packageManagementOptionsViewModel = new PackageManagementOptionsViewModel(packageManagementService.Options);
			}
		}
	}
}
