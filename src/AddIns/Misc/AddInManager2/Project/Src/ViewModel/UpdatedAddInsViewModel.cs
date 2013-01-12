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
using ICSharpCode.Core;
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
			IsSearchable = true;
			HasFilterForPrereleases = true;
			Title = SD.ResourceService.GetString("AddInManager2.Views.Updates");;
			
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
			var updatedPackages = GetUpdatedPackages(AddInManager.Repositories.Registered, localPackages);
			HighlightCount = updatedPackages.Count();
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
