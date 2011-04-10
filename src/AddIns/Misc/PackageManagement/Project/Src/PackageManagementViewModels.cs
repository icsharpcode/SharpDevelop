// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementViewModels
	{
		AddPackageReferenceViewModel addPackageReferenceViewModel;
		PackageManagementOptionsViewModel packageManagementOptionsViewModel;
		PackageManagementConsoleViewModel packageManagementConsoleViewModel;
		IPackageManagementService packageManagementService;
		IRegisteredPackageRepositories registeredPackageRepositories;
		
		public AddPackageReferenceViewModel AddPackageReferenceViewModel {
			get {
				CreateAddPackageReferenceViewModel();
				return addPackageReferenceViewModel;
			}
		}
		
		void CreateAddPackageReferenceViewModel()
		{
			CreateRegisteredPackageRepositories();
			CreatePackageManagementService();
			addPackageReferenceViewModel = 
				new AddPackageReferenceViewModel(
					packageManagementService,
					registeredPackageRepositories,
					new PackageManagementTaskFactory());
		}
		
		void CreatePackageManagementService()
		{
			if (packageManagementService == null) {
				if (IsInDesignMode()) {
					packageManagementService = new FakePackageManagementService();
				} else {
					packageManagementService = PackageManagementServices.PackageManagementService;
				}
			}
		}
		
		void CreateRegisteredPackageRepositories()
		{
			if (registeredPackageRepositories == null) {
				if (IsInDesignMode()) {
					registeredPackageRepositories = new DesignTimeRegisteredPackageRepositories();
				} else {
					registeredPackageRepositories = PackageManagementServices.RegisteredPackageRepositories;
				}
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
		
		public PackageManagementConsoleViewModel PackageManagementConsoleViewModel {
			get { 
				if (packageManagementConsoleViewModel == null) {
					CreatePackageManagementConsoleViewModel();
				}
				return packageManagementConsoleViewModel;
			}
		}
		
		void CreatePackageManagementConsoleViewModel()
		{
			CreatePackageManagementService();
			var consoleHost = PackageManagementServices.ConsoleHost;
			packageManagementConsoleViewModel = 
				new PackageManagementConsoleViewModel(
					PackageManagementServices.RegisteredPackageRepositories.PackageSources,
					consoleHost);
		}
	}
}
