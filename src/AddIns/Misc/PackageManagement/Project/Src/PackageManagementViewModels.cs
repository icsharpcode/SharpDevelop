// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementViewModels
	{
		ManagePackagesViewModel managePackagesViewModel;
		RegisteredPackageSourcesViewModel registeredPackageSourcesViewModel;
		RegisteredPackageSourcesViewModel registeredProjectTemplatePackageSourcesViewModel;
		PackageManagementOptionsViewModel packageManagementOptionsViewModel;
		PackageManagementConsoleViewModel packageManagementConsoleViewModel;
		IPackageManagementSolution solution;
		IRegisteredPackageRepositories registeredPackageRepositories;
		
		public ManagePackagesViewModel ManagePackagesViewModel {
			get {
				CreateManagePackagesViewModel();
				return managePackagesViewModel;
			}
		}
		
		void CreateManagePackagesViewModel()
		{
			CreateRegisteredPackageRepositories();
			CreateSolution();
			ThreadSafePackageManagementEvents packageManagementEvents = CreateThreadSafePackageManagementEvents();
			PackagesViewModels packagesViewModels = CreatePackagesViewModels(packageManagementEvents);

			managePackagesViewModel = 
				new ManagePackagesViewModel(
					packagesViewModels,
					new ManagePackagesViewTitle(solution),
					packageManagementEvents,
					new ManagePackagesUserPrompts(packageManagementEvents));
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
		
		ThreadSafePackageManagementEvents CreateThreadSafePackageManagementEvents()
		{
			return new ThreadSafePackageManagementEvents(
				PackageManagementServices.PackageManagementEvents);
		}
		
		PackagesViewModels CreatePackagesViewModels(IThreadSafePackageManagementEvents packageManagementEvents)
		{
			return new PackagesViewModels(
				solution,
				registeredPackageRepositories,
				packageManagementEvents,
				PackageManagementServices.PackageActionRunner,
				new PackageManagementTaskFactory());
		}
		
		bool IsInDesignMode()
		{
			return WpfDesigner.IsInDesignMode();
		}
		
		public RegisteredPackageSourcesViewModel RegisteredPackageSourcesViewModel {
			get {
				if (registeredPackageSourcesViewModel == null) {
					RegisteredPackageSources packageSources = GetRegisteredPackageSources();
					registeredPackageSourcesViewModel = 
						CreateRegisteredPackageSourcesViewModel(packageSources);
				}
				return registeredPackageSourcesViewModel;
			}
		}
		
		RegisteredPackageSources GetRegisteredPackageSources()
		{
			if (IsInDesignMode()) {
				return CreateDesignTimeRegisteredPackageSources();
			} else {
				return PackageManagementServices.Options.PackageSources;
			}
		}
		
		RegisteredPackageSources CreateDesignTimeRegisteredPackageSources()
		{
			return new RegisteredPackageSources(new PackageSource[0]);
		}
		
		RegisteredPackageSourcesViewModel CreateRegisteredPackageSourcesViewModel(RegisteredPackageSources packageSources)
		{
			CreateRegisteredPackageRepositories();
			if (IsInDesignMode()) {
				return new DesignTimeRegisteredPackageSourcesViewModel();
			} else {
				return new RegisteredPackageSourcesViewModel(packageSources);
			}
		}
		
		public RegisteredPackageSourcesViewModel RegisteredProjectTemplatePackageSourcesViewModel {
			get {
				if (registeredProjectTemplatePackageSourcesViewModel == null) {
					RegisteredPackageSources packageSources = GetProjectTemplatePackageSources();
					registeredProjectTemplatePackageSourcesViewModel =
						CreateRegisteredPackageSourcesViewModel(packageSources);
				}
				return registeredProjectTemplatePackageSourcesViewModel;
			}
		}
		
		RegisteredPackageSources GetProjectTemplatePackageSources()
		{
			if (IsInDesignMode()) {
				return CreateDesignTimeRegisteredPackageSources();
			} else {
				return PackageManagementServices.ProjectTemplatePackageSources;
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
