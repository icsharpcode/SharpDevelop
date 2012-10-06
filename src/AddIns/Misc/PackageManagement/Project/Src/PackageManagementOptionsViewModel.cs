// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Windows.Input;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementOptionsViewModel : ViewModelBase<PackageManagementOptionsViewModel>
	{
		PackageManagementOptions options;
		IRecentPackageRepository recentPackageRepository;
		IMachinePackageCache machinePackageCache;
		IProcess process;
		
		public PackageManagementOptionsViewModel(IRecentPackageRepository recentPackageRepository)
			: this(PackageManagementServices.Options, recentPackageRepository, new MachinePackageCache(), new Process())
		{
		}
		
		public PackageManagementOptionsViewModel(
			PackageManagementOptions options,
			IRecentPackageRepository recentPackageRepository,
			IMachinePackageCache machinePackageCache,
			IProcess process)
		{
			this.options = options;
			this.recentPackageRepository = recentPackageRepository;
			this.machinePackageCache = machinePackageCache;
			this.process = process;
			
			this.HasNoRecentPackages = !RecentPackageRepositoryHasPackages();
			this.HasNoCachedPackages = !MachinePackageCacheHasPackages();
			this.IsPackageRestoreEnabled = options.IsPackageRestoreEnabled;
			
			CreateCommands();
		}
		
		public bool HasNoRecentPackages { get; private set; }
		public bool HasNoCachedPackages { get; private set; }
		public bool IsPackageRestoreEnabled { get; set; }
		
		bool MachinePackageCacheHasPackages()
		{
			return machinePackageCache.GetPackages().Any();
		}
		
		bool RecentPackageRepositoryHasPackages()
		{
			return recentPackageRepository.HasRecentPackages;
		}
		
		void CreateCommands()
		{
			ClearRecentPackagesCommand =
				new DelegateCommand(param => ClearRecentPackages(), param => !HasNoRecentPackages);
			ClearCachedPackagesCommand =
				new DelegateCommand(param => ClearCachedPackages(), param => !HasNoCachedPackages);
			BrowseCachedPackagesCommand =
				new DelegateCommand(param => BrowseCachedPackages(), param => !HasNoCachedPackages);
		}
		
		public ICommand ClearRecentPackagesCommand { get; private set; }
		public ICommand ClearCachedPackagesCommand { get; private set; }
		public ICommand BrowseCachedPackagesCommand { get; private set; }
		
		public void ClearRecentPackages()
		{
			recentPackageRepository.Clear();
			HasNoRecentPackages = true;
			OnPropertyChanged(viewModel => viewModel.HasNoRecentPackages);
		}
		
		public void ClearCachedPackages()
		{
			machinePackageCache.Clear();
			HasNoCachedPackages = true;
			OnPropertyChanged(viewModel => viewModel.HasNoCachedPackages);
		}
		
		public void BrowseCachedPackages()
		{
			process.Start(machinePackageCache.Source);
		}
		
		public void SaveOptions()
		{
			options.IsPackageRestoreEnabled = IsPackageRestoreEnabled;
		}
	}
}
