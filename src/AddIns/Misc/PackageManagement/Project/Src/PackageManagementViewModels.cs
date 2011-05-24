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
		RegisteredPackageSourcesViewModel registeredPackageSourcesViewModel;
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
					PackageManagementServices.PackageActionRunner,
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
			CreateRegisteredPackageRepositories();
			if (IsInDesignMode()) {
				registeredPackageSourcesViewModel = new DesignTimeRegisteredPackageSourcesViewModel();
			} else {
				registeredPackageSourcesViewModel = new RegisteredPackageSourcesViewModel(PackageManagementServices.Options);
			}
		}
		
		public PackageManagementOptionsViewModel PackageManagementOptionsViewModel {
			get {
				if (packageManagementOptionsViewModel == null) {
					CreateRegisteredPackageRepositories();
					IRecentPackageRepository recentRepository = registeredPackageRepositories.RecentPackageRepository;
					packageManagementOptionsViewModel = new PackageManagementOptionsViewModel(recentRepository);
				}
				return packageManagementOptionsViewModel;
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
			CreateRegisteredPackageRepositories();
			var consoleHost = PackageManagementServices.ConsoleHost;
			packageManagementConsoleViewModel = 
				new PackageManagementConsoleViewModel(
					registeredPackageRepositories.PackageSources,
					PackageManagementServices.ProjectService,
					consoleHost);
		}
	}
}
