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
	public class AvailableAddInsViewModel : NuGetAddInsViewModelBase
	{
		public AvailableAddInsViewModel()
			: base()
		{
			Initialize();
		}
		
		public AvailableAddInsViewModel(IAddInManagerServices services)
			: base(services)
		{
			Initialize();
		}
		
		private void Initialize()
		{
			IsSearchable = true;
			ShowPackageSources = true;
			Title = SD.ResourceService.GetString("AddInManager2.Views.Available");
			
			AddInManager.Events.AddInInstalled += AddInInstallationStateChanged;
			AddInManager.Events.AddInUninstalled += AddInInstallationStateChanged;
			AddInManager.Events.AddInStateChanged += AddInInstallationStateChanged;
		}
		
		protected override void OnDispose()
		{
			AddInManager.Events.AddInInstalled -= AddInInstallationStateChanged;
			AddInManager.Events.AddInUninstalled -= AddInInstallationStateChanged;
			AddInManager.Events.AddInStateChanged += AddInInstallationStateChanged;
		}

		protected override IQueryable<IPackage> GetAllPackages()
		{
			return (ActiveRepository ?? AddInManager.Repositories.AllRegistered).GetPackages();
		}
		
		protected override IEnumerable<IPackage> GetFilteredPackagesBeforePagingResults(IQueryable<IPackage> allPackages)
		{
			return base.GetFilteredPackagesBeforePagingResults(allPackages)
				.Where(package => package.IsReleaseVersion())
				.DistinctLast(PackageEqualityComparer.Id);
		}
		
		protected override IQueryable<IPackage> OrderPackages(IQueryable<IPackage> packages)
		{
			return packages.OrderByDescending(package => package.DownloadCount);
		}
		
		protected override void UpdatePrereleaseFilter()
		{
			ReadPackages();
		}
		
		private void AddInInstallationStateChanged(object sender, AddInInstallationEventArgs e)
		{
			UpdateInstallationState();
		}
	}
}
