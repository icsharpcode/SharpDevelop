// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageViewModel : ViewModelBase<PackageViewModel>
	{
		DelegateCommand addPackageCommand;
		DelegateCommand removePackageCommand;
		
		IPackageManagementSolution solution;
		IPackageManagementEvents packageManagementEvents;
		IPackageFromRepository package;
		IEnumerable<PackageOperation> packageOperations = new PackageOperation[0];
		PackageViewModelOperationLogger logger;
		
		public PackageViewModel(
			IPackageFromRepository package,
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents,
			ILogger logger)
		{
			this.package = package;
			this.solution = solution;
			this.packageManagementEvents = packageManagementEvents;
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
		}
	
		public ICommand AddPackageCommand {
			get { return addPackageCommand; }
		}
		
		public ICommand RemovePackageCommand {
			get { return removePackageCommand; }
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
			return IsPackageInstalled(package);
		}
		
		bool IsPackageInstalled(IPackage package)
		{
			return solution.ActiveProjectManager.IsInstalled(package);
		}
		
		public IEnumerable<PackageDependency> Dependencies {
			get { return package.Dependencies; }
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
			get { return package.Summary; }
		}
		
		public Version Version {
			get { return package.Version; }
		}
		
		public int DownloadCount {
			get { return package.DownloadCount; }
		}
		
		public double Rating {
			get { return package.Rating; }
		}
		
		public string Description {
			get { return package.Description; }
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
			ISharpDevelopPackageManager packageManager = CreatePackageManagerForActiveProject();
			packageOperations = packageManager.GetInstallPackageOperations(package, false);
		}
		
		ISharpDevelopPackageManager CreatePackageManagerForActiveProject()
		{
			ISharpDevelopPackageManager packageManager = solution.CreatePackageManagerForActiveProject();
			packageManager.Logger = logger;
			return packageManager;
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
			return package.RequireLicenseAcceptance && !IsPackageInstalled(package);
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
			InstallPackage(package, packageOperations);
			OnPropertyChanged(model => model.IsAdded);
		}
		
		protected virtual void InstallPackage(
			IPackageFromRepository package,
			IEnumerable<PackageOperation> packageOperations)
		{
			InstallPackageAction task = solution.CreateInstallPackageAction();
			task.PackageRepository = package.Repository;
			task.Package = package;
			task.Operations = packageOperations;
			task.Execute();
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
				var action = solution.CreateUninstallPackageAction();
				action.PackageRepository = package.Repository;
				action.Package = package;
				action.Execute();
			} catch (Exception ex) {
				ReportError(ex);
				logger.LogError(ex);
			}
		}
	}
}
