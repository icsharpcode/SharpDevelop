// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public abstract class PackagesViewModel : ViewModelBase<PackagesViewModel>
	{
		DelegateCommand showNextPageCommand;
		DelegateCommand showPreviousPageCommand;
		DelegateCommand showPageCommand;
		DelegateCommand searchCommand;
		
		ObservableCollection<PackageViewModel> packageViewModels = 
			new ObservableCollection<PackageViewModel>();
		Pages pages = new Pages();
		
		IPackageManagementService packageManagementService;
		IPackageViewModelFactory packageViewModelFactory;
		ITaskFactory taskFactory;
		IProjectManager projectManager;
		IEnumerable<IPackage> allPackages;
		string searchTerms;
		bool isReadingPackages;
		ITask<PackagesForSelectedPageResult> task;
		int totalItems;
		bool hasError;
		string errorMessage = String.Empty;
		
		public PackagesViewModel(IPackageManagementService packageManagementService, ITaskFactory taskFactory)
			: this(
				packageManagementService,
				new LicenseAcceptanceService(),
				taskFactory)
		{
		}
		
		public PackagesViewModel(
			IPackageManagementService packageManagementService,
			ILicenseAcceptanceService licenseAcceptanceService,
			ITaskFactory taskFactory)
			: this(
				packageManagementService, 
				new PackageViewModelFactory(packageManagementService, licenseAcceptanceService),
				taskFactory)
		{
		}
		
		public PackagesViewModel(
			IPackageManagementService packageManagementService, 
			IPackageViewModelFactory packageViewModelFactory,
			ITaskFactory taskFactory)
		{
			this.packageManagementService = packageManagementService;
			this.packageViewModelFactory = packageViewModelFactory;
			this.taskFactory = taskFactory;
			
			GetActiveProjectManager();

			CreateCommands();
		}
		
		void GetActiveProjectManager()
		{
			try {
				this.projectManager = packageManagementService.ActiveProjectManager;
			} catch (Exception ex) {
				SaveError(ex);
			}
		}
		
		void SaveError(Exception ex)
		{
			hasError = true;
			errorMessage = ex.Message;
		}
		
		void CreateCommands()
		{
			showNextPageCommand = new DelegateCommand(param => ShowNextPage());
			showPreviousPageCommand = new DelegateCommand(param => ShowPreviousPage());
			showPageCommand = new DelegateCommand(param => ExecuteShowPageCommand(param));
			searchCommand = new DelegateCommand(param => Search());
		}
		
		public ICommand ShowNextPageCommand {
			get { return showNextPageCommand; }
		}
		
		public ICommand ShowPreviousPageCommand {
			get { return showPreviousPageCommand; }
		}
		
		public ICommand ShowPageCommand {
			get { return showPageCommand; }
		}
		
		public ICommand SearchCommand {
			get { return searchCommand; }
		}
		
		public bool HasError {
			get { return hasError; }
		}
		
		public string ErrorMessage {
			get { return errorMessage; }
		}
		
		public ObservableCollection<PackageViewModel> PackageViewModels {
			get { return packageViewModels; }
			set { packageViewModels = value; }
		}
		
		public IPackageManagementService PackageManagementService { 
			get { return packageManagementService; }
		}
		
		public IProjectManager ProjectManager {
			get { return projectManager; }
		}
		
		public bool IsReadingPackages {
			get { return isReadingPackages; }
		}
		
		public void ReadPackages()
		{
			allPackages = null;
			SelectedPageNumber = 1;
			StartReadPackagesTask();
		}
		
		void StartReadPackagesTask()
		{
			isReadingPackages = true;
			ClearPackages();
			CancelReadPackagesTask();
			CreateReadPackagesTask();
			task.Start();
		}
		
		void CancelReadPackagesTask()
		{
			if (task != null) {
				task.Cancel();
			}
		}
		
		void CreateReadPackagesTask()
		{
			task = taskFactory.CreateTask(
				() => GetPackagesForSelectedPageResult(),
				(result) => OnPackagesReadForSelectedPage(result));
		}
		
		PackagesForSelectedPageResult GetPackagesForSelectedPageResult()
		{
			IEnumerable<IPackage> packages = GetPackagesForSelectedPage();
			return new PackagesForSelectedPageResult(packages, totalItems);
		}
		
		void OnPackagesReadForSelectedPage(ITask<PackagesForSelectedPageResult> task)
		{
			isReadingPackages = false;
			if (task.IsFaulted) {
				SaveError(task.Exception);
			} else if (task.IsCancelled) {
				// Ignore
			} else {
				UpdatePackagesForSelectedPage(task.Result);
			}
			base.OnPropertyChanged(null);
		}
		
		void UpdatePackagesForSelectedPage(PackagesForSelectedPageResult result)
		{			
			pages.CollectionChanged -= PagesChanged;
			
			pages.TotalItems = result.TotalPackages;
			pages.TotalItemsOnSelectedPage = result.TotalPackagesOnPage;
			UpdatePackageViewModels(result.Packages);
			
			pages.CollectionChanged += PagesChanged;
		}
		
		void PagesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			StartReadPackagesTask();
			base.OnPropertyChanged(null);
		}
		
		IEnumerable<IPackage> GetPackagesForSelectedPage()
		{
			IEnumerable<IPackage> filteredPackages = GetFilteredPackagesBeforePagingResults();
			return GetPackagesForSelectedPage(filteredPackages);
		}
		
		IEnumerable<IPackage> GetFilteredPackagesBeforePagingResults()
		{
			if (allPackages == null) {
				IQueryable<IPackage> packages = GetAllPackages();
				packages = packages.Find(searchTerms);
				totalItems = packages.Count();
				allPackages = GetFilteredPackagesBeforePagingResults(packages);
			}
			return allPackages;
		}
		
		IEnumerable<IPackage> GetPackagesForSelectedPage(IEnumerable<IPackage> allPackages)
		{
			int packagesToSkip = pages.ItemsBeforeFirstPage;
			return allPackages
				.Skip(packagesToSkip)
				.Take(pages.PageSize);
		}
		
		/// <summary>
		/// Returns all the packages.
		/// </summary>
		protected virtual IQueryable<IPackage> GetAllPackages()
		{
			return null;
		}
		
		/// <summary>
		/// Allows filtering of the packages before paging the results. Call base class method
		/// to run default filtering.
		/// </summary>
		protected virtual IEnumerable<IPackage> GetFilteredPackagesBeforePagingResults(IQueryable<IPackage> allPackages)
		{
			IEnumerable<IPackage> bufferedPackages = GetBufferedPackages(allPackages);
			return bufferedPackages;
		}
		
		IEnumerable<IPackage> GetBufferedPackages(IQueryable<IPackage> allPackages)
		{
			return allPackages.AsBufferedEnumerable(30);
		}
		
		void UpdatePackageViewModels(IEnumerable<IPackage> packages)
		{
			IEnumerable<PackageViewModel> currentViewModels = ConvertToPackageViewModels(packages);
			UpdatePackageViewModels(currentViewModels);
		}
		
		void UpdatePackageViewModels(IEnumerable<PackageViewModel> newPackageViewModels)
		{
			ClearPackages();
			PackageViewModels.AddRange(newPackageViewModels);
		}
		
		void ClearPackages()
		{
			PackageViewModels.Clear();
		}
		
		public IEnumerable<PackageViewModel> ConvertToPackageViewModels(IEnumerable<IPackage> packages)
		{
			foreach (IPackage package in packages) {
				yield return CreatePackageViewModel(package);
			}
		}
		
		PackageViewModel CreatePackageViewModel(IPackage package)
		{
			return packageViewModelFactory.CreatePackageViewModel(package);
		}
		
		public int SelectedPageNumber {
			get { return pages.SelectedPageNumber; }
			set { pages.SelectedPageNumber = value; }
		}
		
		public int PageSize {
			get { return pages.PageSize; }
			set { pages.PageSize = value;  }
		}
		
		public bool IsPaged {
			get { return pages.IsPaged; }
		}
				
		public ObservableCollection<Page> Pages {
			get { return pages; }
		}
		
		public bool HasPreviousPage {
			get { return pages.HasPreviousPage; }
		}
		
		public bool HasNextPage {
			get { return pages.HasNextPage; }
		}
		
		public int MaximumSelectablePages {
			get { return pages.MaximumSelectablePages; }
			set { pages.MaximumSelectablePages = value; }
		}
		
		public int TotalItems {
			get { return totalItems; }
		}
		
		public void ShowNextPage()
		{
			SelectedPageNumber += 1;
		}
		
		public void ShowPreviousPage()
		{
			SelectedPageNumber -= 1;
		}
		
		void ExecuteShowPageCommand(object param)
		{
			int pageNumber = (int)param;
			ShowPage(pageNumber);
		}
		
		public void ShowPage(int pageNumber)
		{
			SelectedPageNumber = pageNumber;
		}
		
		public bool IsSearchable { get; set; }
		
		public string SearchTerms {
			get { return searchTerms; }
			set { searchTerms = value; }
		}
		
		public void Search()
		{
			ReadPackages();
			OnPropertyChanged(null);
		}
		
		public bool ShowPackageSources { get; set; }
		
		public IEnumerable<PackageSource> PackageSources {
			get { return packageManagementService.Options.PackageSources; }
		}
		
		public PackageSource SelectedPackageSource {
			get { return packageManagementService.ActivePackageSource; }
			set {
				if (packageManagementService.ActivePackageSource != value) {
					packageManagementService.ActivePackageSource = value;
					ReadPackages();
					OnPropertyChanged(null);
				}
			}
		}
	}
}
