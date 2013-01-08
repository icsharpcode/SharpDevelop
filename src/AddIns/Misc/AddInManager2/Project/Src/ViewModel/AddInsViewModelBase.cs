// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ICSharpCode.AddInManager2.Model;
using NuGet;

namespace ICSharpCode.AddInManager2.ViewModel
{
	public abstract class AddInsViewModelBase : Model<AddInsViewModelBase>, IDisposable
	{
		private Pages _pages;
		private int _highlightCount;
		private string _title;
		
		public AddInsViewModelBase()
		{
			_pages = new Pages();
			_highlightCount = 0;
			AddInPackages = new ObservableCollection<AddInPackageViewModelBase>();
			ErrorMessage = String.Empty;
			
			CreateCommands();
		}
		
		void CreateCommands()
		{
			ShowNextPageCommand = new DelegateCommand(param => ShowNextPage());
			ShowPreviousPageCommand = new DelegateCommand(param => ShowPreviousPage());
			ShowPageCommand = new DelegateCommand(param => ExecuteShowPageCommand(param));
			SearchCommand = new DelegateCommand(param => Search());
			UpdatePreinstalledFilterCommand = new DelegateCommand(param => UpdatePreinstalledFilter());
			UpdatePrereleaseFilterCommand = new DelegateCommand(param => UpdatePrereleaseFilter());
			InstallFromArchiveCommand = new DelegateCommand(param => InstallFromArchive());
		}
		
		public ICommand ShowNextPageCommand
		{
			get;
			private set;
		}
		
		public ICommand ShowPreviousPageCommand
		{
			get;
			private set;
		}
		
		public ICommand ShowPageCommand
		{
			get;
			private set;
		}
		
		public ICommand SearchCommand
		{
			get;
			private set;
		}
		
		public ICommand UpdatePreinstalledFilterCommand
		{
			get;
			private set;
		}
		
		public ICommand UpdatePrereleaseFilterCommand
		{
			get;
			private set;
		}
		
		public ICommand InstallFromArchiveCommand
		{
			get;
			private set;
		}
		
		public void Dispose()
		{
			OnDispose();
			IsDisposed = true;
		}
		
		protected virtual void OnDispose()
		{
		}
		
		public bool IsDisposed
		{
			get;
			protected set;
		}
		
		public bool HasError
		{
			get;
			protected set;
		}
		
		public string ErrorMessage
		{
			get;
			protected set;
		}
		
		public int HighlightCount
		{
			get
			{
				return _highlightCount;
			}
			protected set
			{
				_highlightCount = value;
				OnPropertyChanged(vm => vm.HighlightCount);
				OnPropertyChanged(vm => vm.HasHighlightCount);
				OnPropertyChanged(vm => vm.TitleWithHighlight);
			}
		}
		
		public bool HasHighlightCount
		{
			get
			{
				return (_highlightCount > 0);
			}
		}
		
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
				OnPropertyChanged(vm => vm.HighlightCount);
				OnPropertyChanged(vm => vm.HasHighlightCount);
				OnPropertyChanged(vm => vm.TitleWithHighlight);
			}
		}
		
		public string TitleWithHighlight
		{
			get
			{
				if (_highlightCount > 0)
				{
					return String.Format("{0} ({1})", Title, _highlightCount);
				}
				else
				{
					return Title;
				}
			}
		}
		
		public ObservableCollection<AddInPackageViewModelBase> AddInPackages
		{
			get;
			private set;
		}
		
		public ObservableCollection<Page> Pages
		{
			get
			{
				return _pages;
			}
		}
		
		protected Pages PagesCollection
		{
			get
			{
				return _pages;
			}
		}
		
		public bool IsReadingPackages
		{
			get;
			protected set;
		}
		
		public virtual void ReadPackages()
		{
			_pages.SelectedPageNumber = 1;
		}
		
		protected void SaveError(AggregateException ex)
		{
			HasError = true;
			ErrorMessage = GetErrorMessage(ex);
			ICSharpCode.Core.LoggingService.Debug(ex);
		}
		
		protected string GetErrorMessage(AggregateException ex)
		{
			StringBuilder errorMessage = new StringBuilder();
			BuildErrorMessage(ex.InnerExceptions, errorMessage);
			return errorMessage.ToString().TrimEnd();
		}
		
		private void BuildErrorMessage(IEnumerable<Exception> exceptions, StringBuilder errorMessage)
		{
			foreach (Exception ex in exceptions)
			{
				var aggregateEx = ex as AggregateException;
				if (aggregateEx != null)
				{
					BuildErrorMessage(aggregateEx.InnerExceptions, errorMessage);
				}
				else
				{
					errorMessage.AppendLine(ex.Message);
				}
			}
		}
		
		protected void UpdatePackageViewModels(IEnumerable<AddInPackageViewModelBase> newPackageViewModels)
		{
			ClearPackages();
			AddInPackages.AddRange(newPackageViewModels);
			UpdateInstallationState();
		}
		
		protected void ClearPackages()
		{
			AddInPackages.Clear();
		}
		
		public virtual int SelectedPageNumber
		{
			get
			{
				return _pages.SelectedPageNumber;
			}
			set
			{
				if (_pages.SelectedPageNumber != value)
				{
					_pages.SelectedPageNumber = value;
				}
			}
		}
		
		public int PageSize
		{
			get
			{
				return _pages.PageSize;
			}
			set
			{
				_pages.PageSize = value;
			}
		}
		
		public bool IsPaged
		{
			get
			{
				return _pages.IsPaged;
			}
		}
		
		public bool HasPreviousPage
		{
			get
			{
				return _pages.HasPreviousPage;
			}
		}
		
		public bool HasNextPage
		{
			get
			{
				return _pages.HasNextPage;
			}
		}
		
		public int MaximumSelectablePages
		{
			get
			{
				return _pages.MaximumSelectablePages;
			}
			set
			{
				_pages.MaximumSelectablePages = value;
			}
		}
		
		public int TotalItems
		{
			get;
			protected set;
		}
		
		public void ShowNextPage()
		{
			SelectedPageNumber += 1;
		}
		
		public void ShowPreviousPage()
		{
			SelectedPageNumber -= 1;
		}
		
		private void ExecuteShowPageCommand(object param)
		{
			int pageNumber = (int)param;
			ShowPage(pageNumber);
		}
		
		public void ShowPage(int pageNumber)
		{
			SelectedPageNumber = pageNumber;
		}
		
		public bool IsSearchable
		{
			get;
			set;
		}
		
		public bool HasFilterForPreinstalled
		{
			get;
			set;
		}
		
		public bool HasFilterForPrereleases
		{
			get;
			set;
		}
		
		public bool AllowInstallFromArchive
		{
			get;
			set;
		}
		
		public string SearchTerms
		{
			get;
			set;
		}
		
		public void Search()
		{
			ReadPackages();
			OnPropertyChanged(null);
		}
		
		protected virtual void UpdatePreinstalledFilter()
		{
		}
		
		protected virtual void UpdatePrereleaseFilter()
		{
		}
		
		protected virtual void InstallFromArchive()
		{
		}
		
		public bool ShowPackageSources
		{
			get;
			set;
		}
		
		public IEnumerable<PackageSource> PackageSources
		{
			get
			{
				foreach (PackageSource packageSource in AddInManager.Repositories.RegisteredPackageSources)
				{
					yield return packageSource;
				}
			}
		}
		
		public PackageSource SelectedPackageSource
		{
			get
			{
				return AddInManager.Repositories.ActiveSource;
			}
			set
			{
				AddInManager.Repositories.ActiveSource = value;
				ReadPackages();
			}
		}
		
		public bool ShowPreinstalledAddIns
		{
			get;
			set;
		}
		
		public bool ShowPrereleases
		{
			get;
			set;
		}
		
		public void UpdateInstallationState()
		{
			// Update installation-state-related properties of all AddIn items in here
			foreach (var packageViewModel in AddInPackages)
			{
				packageViewModel.UpdateInstallationState();
			}
		}
	}
}
