// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.PackageManagement.Scripting;
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
		
		public PackageViewModel(
			IPackageFromRepository package,
			PackageManagementSelectedProjects selectedProjects,
			IPackageManagementEvents packageManagementEvents,
			IPackageActionRunner actionRunner,
			ILogger logger)
		{
			this.package = package;
			this.selectedProjects = selectedProjects;
			this.packageManagementEvents = packageManagementEvents;
			this.actionRunner = actionRunner;
			this.logger = CreateLogger(logger);
			
			CreateCommands();
		}
		
		protected virtual PackageViewModelOperationLogger CreateLogger(ILogger logger)
		{
			return new PackageViewModelOperationLogger(logger, package);
		}
		
		void CreateCommands()
		{
			addPackageCommand = new DelegateCommand(param => AddPackage());
			removePackageCommand = new DelegateCommand(param => RemovePackage());
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
			get { return IsPackageInstalled(); }
		}
		
		bool IsPackageInstalled()
		{
			return selectedProjects.IsPackageInstalled(package);
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
		
		public void AddPackage()
		{
			ClearReportedMessages();
			logger.LogAddingPackage();
			TryInstallingPackage();
			logger.LogAfterPackageOperationCompletes();
		}
		
		void ClearReportedMessages()
		{
			packageManagementEvents.OnPackageOperationsStarting();
		}
		
		void GetPackageOperations()
		{
			IPackageManagementProject project = GetSingleProjectSelected();
			project.Logger = logger;
			var installAction = project.CreateInstallPackageAction();
			packageOperations = project.GetInstallPackageOperations(package, installAction);
		}
		
		IPackageManagementProject GetSingleProjectSelected()
		{
			return selectedProjects.GetSingleProjectSelected(package.Repository);
		}
		
		bool CanInstallPackage()
		{
			IEnumerable<IPackage> packages = GetPackagesRequiringLicenseAcceptance();
			if (packages.Any()) {
				return packageManagementEvents.OnAcceptLicenses(packages);
			}
			return true;
		}
		
		IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance()
		{
			IList<IPackage> packagesToBeInstalled = GetPackagesToBeInstalled();
			return GetPackagesRequiringLicenseAcceptance(packagesToBeInstalled);
		}
		
		IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IList<IPackage> packagesToBeInstalled)
		{
			return packagesToBeInstalled.Where(package => PackageRequiresLicenseAcceptance(package));
		}
		
		IList<IPackage> GetPackagesToBeInstalled()
		{
			List<IPackage> packages = new List<IPackage>();
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
		
		void TryInstallingPackage()
		{
			try {
				GetPackageOperations();
				if (CanInstallPackage()) {
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
			OnPropertyChanged(model => model.IsAdded);
		}
		
		void InstallPackage(IEnumerable<PackageOperation> packageOperations)
		{
			IPackageManagementProject project = GetSingleProjectSelected();
			ProcessPackageOperationsAction action = CreateInstallPackageAction(project);
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
		
		public void RemovePackage()
		{
			ClearReportedMessages();
			logger.LogRemovingPackage();
			TryUninstallingPackage();
			logger.LogAfterPackageOperationCompletes();
			
			OnPropertyChanged(model => model.IsAdded);
		}
		
		void LogRemovingPackage()
		{
			logger.LogRemovingPackage();
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
				if (selectedProjects.HasMultipleProjects()) {
					return true;
				}
				return !selectedProjects.HasSingleProjectSelected();
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
			TryInstallingPackagesForSelectedProjects(projects);
			logger.LogAfterPackageOperationCompletes();
			OnPropertyChanged(model => model.IsAdded);
		}
		
		void TryInstallingPackagesForSelectedProjects(IList<IPackageManagementSelectedProject> projects)
		{
			try {
				if (AnyProjectsSelected(projects)) {
					InstallPackagesForSelectedProjects(projects);
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
		
		void InstallPackagesForSelectedProjects(IList<IPackageManagementSelectedProject> projects)
		{
			if (CanInstallPackage(projects)) {
				IList<ProcessPackageAction> actions = GetProcessPackageActionsForSelectedProjects(projects);
				RunActionsIfAnyExist(actions);
			}
		}
		
		public virtual IList<ProcessPackageAction> GetProcessPackageActionsForSelectedProjects(
			IList<IPackageManagementSelectedProject> selectedProjects)
		{
			var actions = new List<ProcessPackageAction>();
			foreach (IPackageManagementSelectedProject selectedProject in selectedProjects) {
				if (selectedProject.IsSelected) {
					ProcessPackageAction action = CreateInstallPackageAction(selectedProject);
					actions.Add(action);
				}
			}
			return actions;
		}
		
		bool CanInstallPackage(IList<IPackageManagementSelectedProject> projects)
		{
			IPackageManagementSelectedProject project = projects.FirstOrDefault();
			if (project != null) {
				return CanInstallPackage(project);
			}
			return false;
		}
		
		bool CanInstallPackage(IPackageManagementSelectedProject selectedProject)
		{
			IEnumerable<IPackage> licensedPackages = GetPackagesRequiringLicenseAcceptance(selectedProject);
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
			IEnumerable<PackageOperation> operations = project.GetInstallPackageOperations(package, project.CreateInstallPackageAction());
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
