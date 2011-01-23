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
		IProjectManager projectManager;
		IEnumerable<IPackage> allPackages;
		string searchTerms;
		
		public PackagesViewModel(IPackageManagementService packageManagementService)
			: this(packageManagementService, new LicenseAcceptanceService())
		{
		}
		
		public PackagesViewModel(
			IPackageManagementService packageManagementService,
			ILicenseAcceptanceService licenseAcceptanceService)
			: this(
				packageManagementService, 
				new PackageViewModelFactory(packageManagementService, licenseAcceptanceService))
		{
		}
		
		public PackagesViewModel(
			IPackageManagementService packageManagementService, 
			IPackageViewModelFactory packageViewModelFactory)
		{
			this.packageManagementService = packageManagementService;
			this.packageViewModelFactory = packageViewModelFactory;
			this.projectManager = packageManagementService.ActiveProjectManager;
			
			CreateCommands();
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
		
		public void ReadPackages()
		{
			allPackages = null;
			SelectedPageNumber = 1;
			UpdatePackageViewModels();
		}
		
		void PagesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdatePackageViewModels();
			base.OnPropertyChanged(null);
		}
		
		void UpdatePackageViewModels()
		{
			pages.CollectionChanged -= PagesChanged;
			
			IEnumerable<IPackage> packages = GetPackagesForSelectedPage();
			pages.TotalItemsOnSelectedPage = packages.Count();
			UpdatePackageViewModels(packages);
			
			pages.CollectionChanged += PagesChanged;
		}
		
		IEnumerable<IPackage> GetPackagesForSelectedPage()
		{
			IEnumerable<IPackage> filteredPackages = GetFilteredPackagesBeforePagingResults();
			return GetPackagesForSelectedPage(filteredPackages);
		}
		
		IEnumerable<IPackage> GetPackagesForSelectedPage(IEnumerable<IPackage> allPackages)
		{
			int packagesToSkip = pages.ItemsBeforeFirstPage;
			return allPackages
				.Skip(packagesToSkip)
				.Take(pages.PageSize);
		}
		
		IEnumerable<IPackage> GetFilteredPackagesBeforePagingResults()
		{
			if (allPackages == null) {
				IQueryable<IPackage> packages = GetAllPackages();
				packages = packages.Find(searchTerms);
				pages.TotalItems = packages.Count();
				allPackages = GetFilteredPackagesBeforePagingResults(packages);
			}
			return allPackages;
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
			PackageViewModels.Clear();
			PackageViewModels.AddRange(newPackageViewModels);
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
		
		public bool IsSearchable {
			get; set;
		}
		
		public string SearchTerms {
			get { return searchTerms; }
			set { searchTerms = value; }
		}
		
		public void Search()
		{
			ReadPackages();
			OnPropertyChanged(null);
		}
	}
}
