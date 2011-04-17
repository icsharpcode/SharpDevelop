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
		IPackageManagementSolution solution;
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
			CreateSolution();
			addPackageReferenceViewModel = 
				new AddPackageReferenceViewModel(
					solution,
					registeredPackageRepositories,
					PackageManagementServices.PackageManagementEvents,
					new LicenseAcceptanceService(),
					new PackageManagementTaskFactory());
		}
		
		void CreateSolution()
		{
			if (solution == null) {
				if (IsInDesignMode()) {
					solution = new FakePackageManagementSolution();
				} else {
					solution = PackageManagementServices.Solution;
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
			CreateSolution();
			if (IsInDesignMode()) {
				packageManagementOptionsViewModel = new DesignTimePackageManagementOptionsViewModel();
			} else {
				packageManagementOptionsViewModel = new PackageManagementOptionsViewModel(PackageManagementServices.Options);
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
			CreateSolution();
			var consoleHost = PackageManagementServices.ConsoleHost;
			packageManagementConsoleViewModel = 
				new PackageManagementConsoleViewModel(
					PackageManagementServices.RegisteredPackageRepositories.PackageSources,
					consoleHost);
		}
	}
}
