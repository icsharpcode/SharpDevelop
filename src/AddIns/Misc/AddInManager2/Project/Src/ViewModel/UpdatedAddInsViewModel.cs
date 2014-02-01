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
