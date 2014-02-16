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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.PackageManagement;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageViewModel : ViewModelBase<PackageViewModel>
	{
		DelegateCommand addPackageCommand;
		DelegateCommand removePackageCommand;
		DelegateCommand managePackageCommand;
		
		PackageManagementSelectedProjects selectedProjects;
		IPackageManagementEvents packageManagementEvents;
		IPackageFromRepository package;
		IEnumerable<PackageOperation> packageOperations = new PackageOperation[0];
		PackageViewModelOperationLogger logger;
		IPackageActionRunner actionRunner;
		IPackageViewModelParent parent;
		
		public PackageViewModel(
			IPackageViewModelParent parent,
			IPackageFromRepository package,
			PackageManagementSelectedProjects selectedProjects,
			IPackageManagementEvents packageManagementEvents,
			IPackageActionRunner actionRunner,
			ILogger logger)
		{
			this.parent = parent;
			this.package = package;
			this.selectedProjects = selectedProjects;
			this.packageManagementEvents = packageManagementEvents;
			this.actionRunner = actionRunner;
			this.logger = CreateLogger(logger);
			
			CreateCommands();
		}
		
		public IPackageViewModelParent GetParent()
		{
			return parent;
		}
		
		protected virtual PackageViewModelOperationLogger CreateLogger(ILogger logger)
		{
			return new PackageViewModelOperationLogger(logger, package);
		}
		
		void CreateCommands()
		{
			addPackageCommand = new DelegateCommand(param => AddOrManagePackage());
			removePackageCommand = new DelegateCommand(param => RemoveOrManagePackage());
			managePackageCommand = new DelegateCommand(param => ManagePackage());
		}
	
		public ICommand AddPackageCommand {
			get { return addPackageCommand; }
		}
		
		public ICommand RemovePackageCommand {
			get { return removePackageCommand; }
		}
		
		public ICommand ManagePackageCommand {
			get { return managePackageCommand; }
		}
		
		public IPackage GetPackage()
		{
			return package;
		}
		
		public bool HasLicenseUrl {
			get { return LicenseUrl != null; }
		}
		
		public Uri LicenseUrl {
			get { return package.LicenseUrl; }
		}
		
		public bool HasProjectUrl {
			get { return ProjectUrl != null; }
		}
		
		public Uri ProjectUrl {
			get { return package.ProjectUrl; }
		}
		
		public bool HasReportAbuseUrl {
			get { return ReportAbuseUrl != null; }
		}
		
		public Uri ReportAbuseUrl {
			get { return package.ReportAbuseUrl; }
		}
		
		public bool IsAdded {
			get {
				if (selectedProjects.HasSingleProjectSelected()) {
					return selectedProjects.GetSingleProjectSelected(package.Repository).IsPackageInstalled(package)
						|| (selectedProjects.IsPackageInstalledInSolution(package) && !package.HasProjectContent());
				}
				return IsPackageInstalled();
			}
		}
		
		protected bool IsPackageInstalled()
		{
			return selectedProjects.IsPackageInstalledInSolution(package);
		}
		
		public IEnumerable<PackageDependencySet> Dependencies {
			get { return package.DependencySets; }
		}
		
		public bool HasDependencies {
			get { return package.HasDependencies; }
		}
		
		public bool HasNoDependencies {
			get { return !HasDependencies; }
		}
		
		public IEnumerable<string> Authors {
			get { return package.Authors; }
		}
		
		public bool HasDownloadCount {
			get { return package.DownloadCount >= 0; }
		}
		
		public string Id {
			get { return package.Id; }
		}
		
		public string Name {
			get { return package.GetName(); }
		}
		
		public bool HasGalleryUrl {
			get { return GalleryUrl != null; }
		}
		
		public bool HasNoGalleryUrl {
			get { return !HasGalleryUrl; }
		}
		
		public Uri GalleryUrl {
			get { return package.GalleryUrl; }
		}
		
		public Uri IconUrl {
			get { return package.IconUrl; }
		}
		
		public string Summary {
			get { return package.SummaryOrDescription(); }
		}
		
		public SemanticVersion Version {
			get { return package.Version; }
		}
		
		public int DownloadCount {
			get { return package.DownloadCount; }
		}
		
		public string Description {
			get { return package.Description; }
		}
		
		public DateTimeOffset? LastPublished {
			get { return package.Published; }
		}
		
		public bool HasLastPublished {
			get { return package.Published.HasValue; }
		}

		public void OnPackagePropertyChanged() {
			OnPropertyChanged(model => model.IsAdded);
			OnPropertyChanged(model => model.IsManaged);
		}

		public void AddOrManagePackage() {
			if (selectedProjects.HasMultipleProjects()) {
				if (package.HasProjectContent()) {
					ManagePackage();
				}
				else {
					AddPackage();
				}
			}
			else {
				AddPackage();
			}
		}

		public void AddPackage()
		{
			ClearReportedMessages();
			logger.LogAddingPackage();
			
			using (IDisposable operation = StartOperation(package)) {
				if (package.HasProjectContent()) {
					TryInstallingPackage();
				}
				else {
					TryInstallingSolutionPackage();
				}
			}
			
			logger.LogAfterPackageOperationCompletes();
			OnPackagePropertyChanged();
		}
		
		protected virtual IDisposable StartOperation(IPackageFromRepository package)
		{
			return package.StartInstallOperation();
		}
		
		void ClearReportedMessages()
		{
			packageManagementEvents.OnPackageOperationsStarting();
		}
		
		void GetInstallPackageOperations()
		{
			IPackageManagementProject project = GetSingleProjectSelected();
			project.Logger = logger;
			InstallPackageAction installAction = project.CreateInstallPackageAction();
			installAction.AllowPrereleaseVersions = parent.IncludePrerelease;
			packageOperations = project.GetInstallPackageOperations(package, installAction);
		}
		
		IPackageManagementProject GetSingleProjectSelected()
		{
			return selectedProjects.GetSingleProjectSelected(package.Repository);
		}
		
		IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance()
		{
			IList<IPackage> packagesToBeInstalled = GetPackagesToBeInstalled();
			return GetPackagesRequiringLicenseAcceptance(packagesToBeInstalled);
		}
		
		IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IEnumerable<IPackage> packagesToBeInstalled)
		{
			return packagesToBeInstalled.Where(PackageRequiresLicenseAcceptance);
		}
		
		IList<IPackage> GetPackagesToBeInstalled()
		{
			var packages = new List<IPackage>();
			foreach (PackageOperation operation in packageOperations) {
				if (operation.Action == PackageAction.Install) {
					packages.Add(operation.Package);
				}
			}
			return packages;
		}

		bool PackageRequiresLicenseAcceptance(IPackage package)
		{
			return package.RequireLicenseAcceptance && !IsPackageInstalledInSolution(package);
		}
		
		bool IsPackageInstalledInSolution(IPackage package)
		{
			return selectedProjects.IsPackageInstalledInSolution(package);
		}

		void TryInstallingSolutionPackage()
		{
			try {
				var installAction = new InstallPackageAction(null, packageManagementEvents);
				installAction.IgnoreDependencies = false;
				installAction.AllowPrereleaseVersions = parent.IncludePrerelease;

				var solutionRepository = PackageManagementServices.Solution.CreateSolutionPackageRepository();

				var resolverFactory = new PackageOperationsResolverFactory();
				var resolver = resolverFactory.CreateInstallPackageOperationResolver(
					solutionRepository.Repository, 
					package.Repository, 
					logger, 
					installAction);
				packageOperations = resolver.ResolveOperations(package);

				if (LicensesAccepted()) {
					var packageManager = new PackageManager(
						package.Repository, 
						solutionRepository.PackagePathResolver, 
						solutionRepository.FileSystem, 
						solutionRepository.Repository);
					packageManager.InstallPackage(
						package.Id, 
						package.Version, 
						installAction.IgnoreDependencies, 
						installAction.AllowPrereleaseVersions);
					packageManagementEvents.OnParentPackageInstalled(package);
				}
			} catch (Exception ex) {
				ReportError(ex);
				logger.LogError(ex);
			}
		}		

		void TryInstallingPackage()
		{
			try {
				GetInstallPackageOperations();
				if (LicensesAccepted()) {
					InstallPackage();
				}
			} catch (Exception ex) {
				ReportError(ex);
				logger.LogError(ex);
			}
		}
		
		void InstallPackage()
		{
			InstallPackage(packageOperations);
		}
		
		void InstallPackage(IEnumerable<PackageOperation> packageOperations)
		{
			IPackageManagementProject project = GetSingleProjectSelected();
			ProcessPackageOperationsAction action = CreateInstallPackageAction(project);
			action.AllowPrereleaseVersions = parent.IncludePrerelease;
			action.Package = package;
			action.Operations = packageOperations;
			actionRunner.Run(action);
		}
		
		protected virtual ProcessPackageOperationsAction CreateInstallPackageAction(
			IPackageManagementProject project)
		{
			return project.CreateInstallPackageAction();
		}
		
		void ReportError(Exception ex)
		{
			packageManagementEvents.OnPackageOperationError(ex);
		}
		
		public void RemoveOrManagePackage() {
			if (selectedProjects.HasMultipleProjects()) {
				if (package.HasProjectContent()) {
					ManagePackage();
				}
				else {
					RemovePackage();
				}
			}
			else {
				RemovePackage();
			}
		}

		public void RemovePackage()
		{
			ClearReportedMessages();
			logger.LogRemovingPackage();

			if (package.HasProjectContent()) {
				TryUninstallingPackage();
			} else {
				TryUninstallingSolutionPackage();
			}

			logger.LogAfterPackageOperationCompletes();
			OnPackagePropertyChanged();
		}
		
		void LogRemovingPackage()
		{
			logger.LogRemovingPackage();
		}

		void TryUninstallingSolutionPackage()
		{
			try {
				var solutionPackageRepository = PackageManagementServices.Solution.CreateSolutionPackageRepository();
				var packageManager = new NuGet.PackageManager(solutionPackageRepository.Repository, solutionPackageRepository.PackagePathResolver, solutionPackageRepository.FileSystem);
				packageManager.UninstallPackage(package.Id, package.Version);
				packageManagementEvents.OnParentPackageUninstalled(package);
			} catch (Exception ex) {
				ReportError(ex);
				logger.LogError(ex);
			}
		}		

		void TryUninstallingPackage()
		{
			try {
				IPackageManagementProject project = GetSingleProjectSelected();
				UninstallPackageAction action = project.CreateUninstallPackageAction();
				action.Package = package;
				actionRunner.Run(action);
			} catch (Exception ex) {
				ReportError(ex);
				logger.LogError(ex);
			}
		}
		
		public bool IsManaged {
			get {
				if (selectedProjects.HasSingleProjectSelected()) {
					// Single Project selected 
					// Project-level Package Management
					return false; // [Add]/[Remove]
				}
				// Solution selected 
				// Project-level/Solution-level Package Management
				if (IsAdded) {
					if (package.HasProjectContent()) {
						return true; // [Manage] Button
					}
					return false; // [Remove] Button
				}
				// package.HasProjectContent() is too slow on uninstalled packages
				// check ([Add] or [Manage]) after user press on [Add] Package Button
				return false; // [Add] Button -> redirect to [Manage] if (package.HasProjectContent())
			}
		}
		
		public void ManagePackage()
		{
			List<IPackageManagementSelectedProject> projects = GetSelectedProjectsForPackage();
			if (packageManagementEvents.OnSelectProjects(projects)) {
				ManagePackagesForSelectedProjects(projects);
			}
		}
		
		List<IPackageManagementSelectedProject> GetSelectedProjectsForPackage()
		{
			return selectedProjects.GetProjects(package).ToList();
		}
		
		public void ManagePackagesForSelectedProjects(IEnumerable<IPackageManagementSelectedProject> projects)
		{
			ManagePackagesForSelectedProjects(projects.ToList());
		}
		
		void ManagePackagesForSelectedProjects(IList<IPackageManagementSelectedProject> projects)
		{
			ClearReportedMessages();
			logger.LogManagingPackage();
			
			using (IDisposable operation = StartOperation(package)) {
				TryManagePackagesForSelectedProjects(projects);
			}
			
			logger.LogAfterPackageOperationCompletes();
			OnPackagePropertyChanged();
		}
		
		void TryManagePackagesForSelectedProjects(IList<IPackageManagementSelectedProject> projects)
		{
			try {
				IList<ProcessPackageAction> actions = GetProcessPackageActionsForSelectedProjects(projects);
				if (actions.Any() && (IsPackageInstalled() || LicensesAccepted())) {
					RunActionsIfAnyExist(actions);
				}
			} catch (Exception ex) {
				ReportError(ex);
				logger.LogError(ex);
			}
		}
		
		protected virtual bool AnyProjectsSelected(IList<IPackageManagementSelectedProject> projects)
		{
			return projects.Any(project => project.IsSelected);
		}
		
		public IList<ProcessPackageAction> GetProcessPackageActionsForSelectedProjects(
			IList<IPackageManagementSelectedProject> selectedProjects)
		{
			var actions = new List<ProcessPackageAction>();
			foreach (IPackageManagementSelectedProject selectedProject in selectedProjects) {
				ProcessPackageAction action = CreatePackageManageAction(selectedProject);
				if (action != null) {
					actions.Add(action);
				}
			}
			return actions;
		}
		
		protected virtual ProcessPackageAction CreatePackageManageAction(IPackageManagementSelectedProject selectedProject)
		{
			if (selectedProject.IsSelected) {
				return CreateInstallPackageManageActionForSelectedProject(selectedProject);
			}
			return CreateUninstallPackageManageActionForSelectedProject(selectedProject);
		}
		
		protected ProcessPackageAction CreateUpdatePackageManageActionForSelectedProject(IPackageManagementSelectedProject selectedProject)
		{
			if (selectedProject.Project.GetPackages().Where(p => p.Id == package.Id).Any() &&
				!selectedProject.Project.GetPackages().Where(p => p.Id == package.Id && p.Version == package.Version).Any()) {
				return CreateUpdatePackageAction(selectedProject);
			}
			return null;
		}
		
		ProcessPackageAction CreateInstallPackageManageActionForSelectedProject(IPackageManagementSelectedProject selectedProject)
		{
			if (!selectedProject.Project.GetPackages().Where(p => p.Id == package.Id && p.Version == package.Version).Any()) {
				return CreateInstallPackageAction(selectedProject);
			}
			return null;
		}
		
		ProcessPackageAction CreateUninstallPackageManageActionForSelectedProject(IPackageManagementSelectedProject selectedProject)
		{
			if (selectedProject.Project.GetPackages().Where(p => p.Id == package.Id && p.Version == package.Version).Any()) {
				return CreateUninstallPackageAction(selectedProject);
			}
			return null;
		}
		
		bool LicensesAccepted()
		{
			IEnumerable<IPackage> licensedPackages = GetPackagesRequiringLicenseAcceptance();
			if (licensedPackages.Any()) {
				return packageManagementEvents.OnAcceptLicenses(licensedPackages);
			}
			return true;
		}
		
		protected ProcessPackageAction CreateInstallPackageAction(IPackageManagementSelectedProject selectedProject)
		{
			IPackageManagementProject project = selectedProject.Project;
			project.Logger = logger;
			
			ProcessPackageOperationsAction action = CreateInstallPackageAction(project);
			action.Package = package;
			return action;
		}
		
		protected ProcessPackageAction CreateUninstallPackageAction(IPackageManagementSelectedProject selectedProject)
		{
			IPackageManagementProject project = selectedProject.Project;
			project.Logger = logger;
			
			ProcessPackageAction action = project.CreateUninstallPackageAction();
			action.Package = package;
			return action;
		}
		
		protected ProcessPackageAction CreateUpdatePackageAction(IPackageManagementSelectedProject selectedProject)
		{
			IPackageManagementProject project = selectedProject.Project;
			project.Logger = logger;
			
			ProcessPackageOperationsAction action = project.CreateUpdatePackageAction();
			action.Package = package;
			return action;
		}
		
		void RunActionsIfAnyExist(IList<ProcessPackageAction> actions)
		{
			if (actions.Any()) {
				actionRunner.Run(actions);
			}
		}
		
		IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IPackageManagementSelectedProject selectedProject)
		{
			IPackageManagementProject project = selectedProject.Project;
			project.Logger = logger;
			InstallPackageAction installAction = project.CreateInstallPackageAction();
			installAction.AllowPrereleaseVersions = parent.IncludePrerelease;
			IEnumerable<PackageOperation> operations = project.GetInstallPackageOperations(package, installAction);
			return GetPackagesRequiringLicenseAcceptance(operations);
		}
		
		IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IEnumerable<PackageOperation> operations)
		{
			foreach (PackageOperation operation in operations) {
				if (PackageOperationRequiresLicenseAcceptance(operation)) {
					yield return operation.Package;
				}
			}
		}
		
		bool PackageOperationRequiresLicenseAcceptance(PackageOperation operation)
		{
			return
				(operation.Action == PackageAction.Install) &&
				operation.Package.RequireLicenseAcceptance &&
				!IsPackageInstalledInSolution(operation.Package);
		}
	}
}
