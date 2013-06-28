// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.SharpDevelop;
using NuGet;

namespace ICSharpCode.AddInManager2.ViewModel
{
	public class UpdatedAddInsViewModel : NuGetAddInsViewModelBase
	{
		private IQueryable<IPackage> installedPackages;
		private string errorMessage = String.Empty;
		private bool hasSavedException = false;
		
		public UpdatedAddInsViewModel()
			: base()
		{
			Initialize();
		}
		
		public UpdatedAddInsViewModel(IAddInManagerServices services)
			: base(services)
		{
			Initialize();
		}
		
		private void Initialize()
		{
			IsSearchable = true;
			ShowPackageSources = true;
			HasFilterForPrereleases = true;
			Title = SD.ResourceService.GetString("AddInManager2.Views.Updates");
			
			ShowPrereleases = AddInManager.Settings.ShowPrereleases;
			
			AddInManager.Events.AddInInstalled += NuGetPackagesChanged;
			AddInManager.Events.AddInUninstalled += NuGetPackagesChanged;
			AddInManager.Events.AddInStateChanged += InstalledAddInStateChanged;
		}
		
		protected override void OnDispose()
		{
			AddInManager.Events.AddInInstalled -= NuGetPackagesChanged;
			AddInManager.Events.AddInUninstalled -= NuGetPackagesChanged;
			AddInManager.Events.AddInStateChanged -= InstalledAddInStateChanged;
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			if (hasSavedException)
			{
				ThrowSavedException();
			}
			return GetUpdatedPackages();
		}
		
		protected override void UpdateRepositoryBeforeReadPackagesTaskStarts()
		{
			try
			{
				installedPackages = GetInstalledPackages();
			}
			catch (Exception ex)
			{
				hasSavedException = true;
				errorMessage = ex.Message;
			}
		}
		
		private IQueryable<IPackage> GetInstalledPackages()
		{
			return AddInManager.NuGet.Packages.LocalRepository.GetPackages();
		}
		
		private IQueryable<IPackage> GetUpdatedPackages()
		{
			IQueryable<IPackage> localPackages = installedPackages;
			localPackages = FilterPackages(localPackages);
			
			int allUpdatesCount = 0;
			IEnumerable<IPackage> updatedPackages = null;
			
			var allRepositories = AddInManager.Repositories.RegisteredPackageRepositories;
			if (allRepositories != null)
			{
				// Run through all repositories and collect counts of updated packages
				foreach (var repository in allRepositories)
				{
					var updatesForThisRepository = GetUpdatedPackages(repository, localPackages);
					
					if (ActiveRepository.Source == repository.Source)
					{
						// This is also the user-selected repository we need the package list from
						updatedPackages = updatesForThisRepository;
					}
					
					// Set update count for repository in list
					var packageRepositoryModel = PackageRepositories.Where(pr => pr.SourceUrl == repository.Source).FirstOrDefault();
					if (packageRepositoryModel != null)
					{
						int updatesCount = updatesForThisRepository.Count();
						packageRepositoryModel.HighlightCount = updatesCount;
						allUpdatesCount += updatesCount;
					}
				}
			}
			
			if (updatedPackages == null)
			{
				// Just as fallback, if something goes wrong in upper loop
				updatedPackages = GetUpdatedPackages(AddInManager.Repositories.AllRegistered, localPackages);
				allUpdatesCount = updatedPackages.Count();
			}
			
			HighlightCount = allUpdatesCount;
			return updatedPackages.AsQueryable();
		}
		
		private IEnumerable<IPackage> GetUpdatedPackages(IPackageRepository sourceRepository, IQueryable<IPackage> localPackages)
		{
			return sourceRepository.GetUpdates(localPackages, ShowPrereleases, false);
		}
		
		private IQueryable<IPackage> FilterPackages(IQueryable<IPackage> localPackages)
		{
			return localPackages.Find(SearchTerms);
		}
		
		private void ThrowSavedException()
		{
			throw new ApplicationException(errorMessage);
		}
		
		protected override void UpdatePrereleaseFilter()
		{
			AddInManager.Settings.ShowPrereleases = ShowPrereleases;
			ReadPackages();
		}
		
		private void NuGetPackagesChanged(object sender, AddInInstallationEventArgs e)
		{
			ReadPackages();
		}
		
		private void InstalledAddInStateChanged(object sender, AddInInstallationEventArgs e)
		{
			UpdateInstallationState();
		}
	}
}
