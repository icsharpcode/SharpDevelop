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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.SharpDevelop;
using NuGet;

namespace ICSharpCode.AddInManager2.ViewModel
{
	public class PackageRepositoriesViewModel : Model<PackageRepositoriesViewModel>
	{
		ObservableCollection<PackageRepository> packageRepositories =
			new ObservableCollection<PackageRepository>();
		ObservableCollection<PackageSource> packageSources;
		
		DelegateCommand addPackageSourceCommmand;
		DelegateCommand removePackageSourceCommand;
		DelegateCommand movePackageSourceUpCommand;
		DelegateCommand movePackageSourceDownCommand;
		DelegateCommand browsePackageFolderCommand;
		
		PackageRepository newPackageSource = new PackageRepository();
		PackageRepository selectedPackageRepository;
		
		public PackageRepositoriesViewModel()
			: base()
		{
			Initialize();
		}
		
		public PackageRepositoriesViewModel(IAddInManagerServices services)
			: base(services)
		{
			Initialize();
		}
		
		private void Initialize()
		{
			this.packageSources = new ObservableCollection<PackageSource>();
			CreateCommands();
		}
		
		private void CreateCommands()
		{
			addPackageSourceCommmand =
				new DelegateCommand(param => AddPackageSource(),
				                    param => CanAddPackageSource);
			
			removePackageSourceCommand =
				new DelegateCommand(param => RemovePackageSource(),
				                    param => CanRemovePackageSource);
			
			movePackageSourceUpCommand =
				new DelegateCommand(param => MovePackageSourceUp(),
				                    param => CanMovePackageSourceUp);
			
			movePackageSourceDownCommand =
				new DelegateCommand(param => MovePackageSourceDown(),
				                    param => CanMovePackageSourceDown);
			
			browsePackageFolderCommand =
				new DelegateCommand(param => BrowsePackageFolder());
		}
		
		public ICommand AddPackageSourceCommand
		{
			get
			{
				return addPackageSourceCommmand;
			}
		}
		
		public ICommand RemovePackageSourceCommand
		{
			get
			{
				return removePackageSourceCommand;
			}
		}
		
		public ICommand MovePackageSourceUpCommand
		{
			get
			{
				return movePackageSourceUpCommand;
			}
		}
		
		public ICommand MovePackageSourceDownCommand
		{
			get
			{
				return movePackageSourceDownCommand;
			}
		}
		
		public ICommand BrowsePackageFolderCommand
		{
			get
			{
				return browsePackageFolderCommand;
			}
		}
		
		public ObservableCollection<PackageRepository> PackageRepositories
		{
			get
			{
				return packageRepositories;
			}
		}
		
		public void Load()
		{
			packageSources.Clear();
			NuGet.CollectionExtensions.AddRange(packageSources, AddInManager.Repositories.RegisteredPackageSources);
			foreach (PackageSource packageSource in packageSources)
			{
				AddPackageSourceToViewModel(packageSource);
			}
		}
		
		private void AddPackageSourceToViewModel(PackageSource packageSource)
		{
			var packageRepository = new PackageRepository(packageSource);
			packageRepositories.Add(packageRepository);
		}
		
		public void Save()
		{
			packageSources.Clear();
			foreach (PackageRepository packageRepository in packageRepositories)
			{
				PackageSource source = packageRepository.ToPackageSource();
				packageSources.Add(source);
			}
			AddInManager.Repositories.RegisteredPackageSources = packageSources;
		}
		
		public string NewPackageSourceName
		{
			get
			{
				return newPackageSource.Name;
			}
			set
			{
				newPackageSource.Name = value;
				OnPropertyChanged(viewModel => viewModel.NewPackageSourceName);
			}
		}
		
		public string NewPackageSourceUrl
		{
			get
			{
				return newPackageSource.SourceUrl;
			}
			set
			{
				newPackageSource.SourceUrl = value;
				OnPropertyChanged(viewModel => viewModel.NewPackageSourceUrl);
			}
		}
		
		public PackageRepository SelectedPackageRepository
		{
			get
			{
				return selectedPackageRepository;
			}
			set
			{
				selectedPackageRepository = value;
				OnPropertyChanged(viewModel => viewModel.SelectedPackageRepository);
				OnPropertyChanged(viewModel => viewModel.CanAddPackageSource);
			}
		}
		
		public void AddPackageSource()
		{
			AddNewPackageSourceToViewModel();
			SelectLastPackageSourceViewModel();
		}
		
		private void AddNewPackageSourceToViewModel()
		{
			var packageSource = newPackageSource.ToPackageSource();
			AddPackageSourceToViewModel(packageSource);
		}
		
		private void SelectLastPackageSourceViewModel()
		{
			SelectedPackageRepository = GetLastPackageSourceViewModel();
		}
		
		public bool CanAddPackageSource
		{
			get
			{
				return NewPackageSourceHasUrl && NewPackageSourceHasName;
			}
		}
		
		private bool NewPackageSourceHasUrl
		{
			get
			{
				return !String.IsNullOrEmpty(NewPackageSourceUrl);
			}
		}
		
		private bool NewPackageSourceHasName
		{
			get
			{
				return !String.IsNullOrEmpty(NewPackageSourceName);
			}
		}
		
		public void RemovePackageSource()
		{
			RemoveSelectedPackageSourceViewModel();
		}
		
		public bool CanRemovePackageSource
		{
			get
			{
				return selectedPackageRepository != null;
			}
		}
		
		private void RemoveSelectedPackageSourceViewModel()
		{
			packageRepositories.Remove(selectedPackageRepository);
		}
		
		public void MovePackageSourceUp()
		{
			int selectedPackageSourceIndex = GetSelectedPackageSourceViewModelIndex();
			int destinationPackageSourceIndex = selectedPackageSourceIndex--;
			packageRepositories.Move(selectedPackageSourceIndex, destinationPackageSourceIndex);
		}
		
		private int GetSelectedPackageSourceViewModelIndex()
		{
			return packageRepositories.IndexOf(selectedPackageRepository);
		}
		
		public bool CanMovePackageSourceUp
		{
			get
			{
				return HasAtLeastTwoPackageSources() && !IsFirstPackageSourceSelected();
			}
		}
		
		private bool IsFirstPackageSourceSelected()
		{
			return selectedPackageRepository == packageRepositories[0];
		}
		
		public void MovePackageSourceDown()
		{
			int selectedPackageSourceIndex = GetSelectedPackageSourceViewModelIndex();
			int destinationPackageSourceIndex = selectedPackageSourceIndex++;
			packageRepositories.Move(selectedPackageSourceIndex, destinationPackageSourceIndex);
		}
		
		public bool CanMovePackageSourceDown
		{
			get
			{
				return HasAtLeastTwoPackageSources() && !IsLastPackageSourceSelected();
			}
		}
		
		private bool HasAtLeastTwoPackageSources()
		{
			return packageRepositories.Count >= 2;
		}
		
		private bool IsLastPackageSourceSelected()
		{
			PackageRepository lastViewModel = GetLastPackageSourceViewModel();
			return lastViewModel == selectedPackageRepository;
		}
		
		private PackageRepository GetLastPackageSourceViewModel()
		{
			return packageRepositories.Last();
		}
		
		public void BrowsePackageFolder()
		{
			using (var dialog = new FolderBrowserDialog())
			{
				var owner = SD.WinForms.MainWin32Window;
				if (dialog.ShowDialog(owner) == DialogResult.OK)
				{
					UpdateNewPackageSourceUsingSelectedFolder(dialog.SelectedPath);
				}
			}
		}
		
		private void UpdateNewPackageSourceUsingSelectedFolder(string folder)
		{
			NewPackageSourceUrl = folder;
			NewPackageSourceName = GetPackageSourceNameFromFolder(folder);
		}
		
		private string GetPackageSourceNameFromFolder(string folder)
		{
			return Path.GetFileName(folder);
		}
	}
}
