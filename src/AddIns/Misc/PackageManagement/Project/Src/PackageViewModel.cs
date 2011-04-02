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
		
		IPackageManagementService packageManagementService;
		ILicenseAcceptanceService licenseAcceptanceService;
		IMessageReporter messageReporter;
		IPackage package;
		IEnumerable<PackageOperation> packageOperations = new PackageOperation[0];
		IPackageRepository sourcePackageRepository;
		IPackageRepository localPackageRepository;
		bool? hasDependencies;
		
		public PackageViewModel(
			IPackage package,
			IPackageManagementService packageManagementService,
			ILicenseAcceptanceService licenseAcceptanceService,
			IMessageReporter messageReporter)
		{
			this.package = package;
			this.sourcePackageRepository = packageManagementService.ActivePackageRepository;
			this.packageManagementService = packageManagementService;
			this.licenseAcceptanceService = licenseAcceptanceService;
			this.messageReporter = messageReporter;
			
			CreateCommands();
		}
		
		void CreateCommands()
		{
			addPackageCommand = new DelegateCommand(param => AddPackage());
			removePackageCommand = new DelegateCommand(param => RemovePackage());
		}
		
		public IPackageRepository SourcePackageRepository {
			get { return sourcePackageRepository; }
		}
		
		public IPackageRepository LocalPackageRepository {
			get {
				if (localPackageRepository == null) {
					GetLocalPackageRepository();
				}
				return localPackageRepository;
			}
		}
		
		void GetLocalPackageRepository()
		{
			localPackageRepository = packageManagementService.ActiveProjectManager.LocalRepository;
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
			return packageManagementService.ActiveProjectManager.IsInstalled(package);
		}
		
		public IEnumerable<PackageDependency> Dependencies {
			get { return package.Dependencies; }
		}
		
		public bool HasDependencies {
			get {
				if (!hasDependencies.HasValue) {
					IEnumerator<PackageDependency> enumerator = Dependencies.GetEnumerator();
					hasDependencies = enumerator.MoveNext();
				}
				return hasDependencies.Value;
			}
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
			LogAddingPackage();
			TryInstallingPackage();
			LogAfterPackageOperationCompletes();
		}
		
		void ClearReportedMessages()
		{
			messageReporter.ClearMessage();
		}
		
		void LogAddingPackage()
		{
			string message = GetFormattedStartPackageOperationMessage(AddingPackageMessageFormat);
			Log(message);
		}
				
		void Log(string message)
		{
			Logger.Log(MessageLevel.Info, message);
		}
		
		string GetFormattedStartPackageOperationMessage(string format)
		{
			string message = String.Format(format, package.ToString());
			return GetStartPackageOperationMessage(message);
		}
		
		string GetStartPackageOperationMessage(string message)
		{
			return String.Format("------- {0} -------", message);
		}
		
		ILogger Logger {
			get { return packageManagementService.OutputMessagesView; }
		}
		
		protected virtual string AddingPackageMessageFormat {
			get { return "Installing...{0}"; }
		}
		
		void LogAfterPackageOperationCompletes()
		{
			LogEndMarkerLine();
			LogEmptyLine();
		}
		
		void LogEndMarkerLine()
		{
			string message = new String('=', 30);
			Log(message);
		}

		void LogEmptyLine()
		{
			Log(String.Empty);
		}
		
		void GetPackageOperations()
		{
			IPackageOperationResolver resolver = CreatePackageOperationResolver();
			packageOperations = resolver.ResolveOperations(package);
		}
		
		IPackageOperationResolver CreatePackageOperationResolver()
		{
			return CreatePackageOperationResolver(Logger);
		}
		
		protected virtual IPackageOperationResolver CreatePackageOperationResolver(ILogger logger)
		{
			return new InstallWalker(LocalPackageRepository,
			                         sourcePackageRepository,
			                         logger,
			                         ignoreDependencies: false);
		}
		
		bool CanInstallPackage()
		{
			IEnumerable<IPackage> packages = GetPackagesRequiringLicenseAcceptance();
			if (packages.Any()) {
				return licenseAcceptanceService.AcceptLicenses(packages);
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
				Log(ex.ToString());
			}
		}
		
		void InstallPackage()
		{
			InstallPackage(sourcePackageRepository, package, packageOperations);
			OnPropertyChanged(model => model.IsAdded);
		}
		
		protected virtual void InstallPackage(
			IPackageRepository sourcePackageRepository,
			IPackage package,
			IEnumerable<PackageOperation> packageOperations)
		{
			packageManagementService.InstallPackage(sourcePackageRepository, package, packageOperations);
		}
		
		void ReportError(Exception ex)
		{
			messageReporter.ShowErrorMessage(ex.Message);
		}
		
		public void RemovePackage()
		{
			ClearReportedMessages();
			LogRemovingPackage();
			TryUninstallingPackage();
			LogAfterPackageOperationCompletes();
			
			OnPropertyChanged(model => model.IsAdded);
		}
		
		void LogRemovingPackage()
		{
			string message =  GetFormattedStartPackageOperationMessage(RemovingPackageMessageFormat);
			Log(message);
		}
				
		protected virtual string RemovingPackageMessageFormat {
			get { return "Uninstalling...{0}"; }
		}
		
		void TryUninstallingPackage()
		{
			try {
				packageManagementService.UninstallPackage(sourcePackageRepository, package);
			} catch (Exception ex) {
				ReportError(ex);
				Log(ex.ToString());
			}
		}
	}
}
