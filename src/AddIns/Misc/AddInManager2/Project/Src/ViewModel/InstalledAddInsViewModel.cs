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
using Microsoft.Win32;
using NuGet;

namespace ICSharpCode.AddInManager2.ViewModel
{
	/// <summary>
	/// Model for view of installed SharpDevelop AddIns.
	/// </summary>
	public class InstalledAddInsViewModel : NuGetAddInsViewModelBase
	{
		public InstalledAddInsViewModel()
			: base()
		{
			Initialize();
		}
		
		public InstalledAddInsViewModel(IAddInManagerServices services)
			: base(services)
		{
			Initialize();
		}
		
		private void Initialize()
		{
			AllowInstallFromArchive = true;
			HasFilterForPreinstalled = true;
			Title = SD.ResourceService.GetString("AddInManager2.Views.Installed");

			// Load preinstalled AddIn filter
			LoadPreinstalledAddInFilter();
			
			AddInManager.Events.AddInInstalled += InstalledAddInsChanged;
			AddInManager.Events.AddInUninstalled += InstalledAddInsChanged;
			AddInManager.Events.AddInStateChanged += InstalledAddInStateChanged;
		}
		
		protected override void OnDispose()
		{
			AddInManager.Events.AddInInstalled -= InstalledAddInsChanged;
			AddInManager.Events.AddInUninstalled -= InstalledAddInsChanged;
			AddInManager.Events.AddInStateChanged -= InstalledAddInStateChanged;
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			return AddInManager.NuGet.Packages.LocalRepository.GetPackages();
		}
		
		protected override IEnumerable<IPackage> GetFilteredPackagesBeforePagingResults(IQueryable<IPackage> allPackages)
		{
			return base.GetFilteredPackagesBeforePagingResults(allPackages)
				.Where(package => package.IsReleaseVersion())
				.DistinctLast(PackageEqualityComparer.Id);
		}
		
		protected override void UpdatePackageViewModels(IEnumerable<IPackage> packages)
		{
			IEnumerable<AddInPackageViewModelBase> offlineAddInViewModels = GetInstalledAddIns(packages);
//			IEnumerable<AddInPackageViewModelBase> nuGetViewModels = ConvertToAddInViewModels(packages);
			
			// Merge lists of offline entries (internal AddIn objects) and online entries (installed NuGet packages)
//			IEnumerable<AddInPackageViewModelBase> viewModels = CombineOnlineAndOfflineAddIns(nuGetViewModels, offlineAddInViewModels);
//			UpdatePackageViewModels(viewModels.OrderBy(vm => vm.Name));
			
			UpdatePackageViewModels(offlineAddInViewModels.OrderBy(vm => vm.Name));
		}
		
		private IEnumerable<AddInPackageViewModelBase> CombineOnlineAndOfflineAddIns(
			IEnumerable<AddInPackageViewModelBase> onlineAddIns, IEnumerable<AddInPackageViewModelBase> offlineAddIns)
		{
			return offlineAddIns.GroupJoin(
				onlineAddIns,
				offlinevm => offlinevm.Id,
				onlinevm => onlinevm.Id,
				(offlinevm, e) => e.ElementAtOrDefault(0) ?? offlinevm);
		}
		
		private IEnumerable<AddInPackageViewModelBase> GetInstalledAddIns(IEnumerable<IPackage> installedPackages)
		{
			AddInPackageViewModelBase addInPackage;
			
			// Fill set of ID of installed NuGet packages, so we can later quickly check, whether NuGet package is installed for an AddIn
			HashSet<string> nuGetPackageIDs = new HashSet<string>();
			foreach (IPackage package in installedPackages)
			{
				if (!nuGetPackageIDs.Contains(package.Id))
				{
					nuGetPackageIDs.Add(package.Id);
				}
			}
			
			List<ManagedAddIn> addInList = new List<ManagedAddIn>(AddInManager.Setup.AddInsWithMarkedForInstallation);
			addInList.Sort(delegate(ManagedAddIn a, ManagedAddIn b)
			               {
			               	return a.AddIn.Name.CompareTo(b.AddIn.Name);
			               });
			foreach (ManagedAddIn addIn in addInList)
			{
				if (string.Equals(addIn.AddIn.Properties["addInManagerHidden"], "true", StringComparison.OrdinalIgnoreCase))
				{
					// This excludes the SharpDevelop application appearing as AddIn in the tree
					continue;
				}
				if (!ShowPreinstalledAddIns && AddInManager.Setup.IsAddInPreinstalled(addIn.AddIn))
				{
					continue;
				}
				
				string nuGetPackageID = addIn.LinkedNuGetPackageID;
				if (!string.IsNullOrEmpty(nuGetPackageID))
				{
					if (nuGetPackageIDs.Contains(nuGetPackageID))
					{
						addIn.InstallationSource = AddInInstallationSource.NuGetRepository;
					}
				}
				
				addInPackage = new OfflineAddInsViewModel(AddInManager, addIn);
				yield return addInPackage;
			}
		}
		
		protected override void UpdatePreinstalledFilter()
		{
			// Save the preinstalled AddIn filter
			SavePreinstalledAddInFilter();
			
			// Update the list
			Search();
		}
		
		private void InstalledAddInsChanged(object sender, AddInInstallationEventArgs e)
		{
			ReadPackages();
		}
		
		private void InstalledAddInStateChanged(object sender, AddInInstallationEventArgs e)
		{
			UpdateInstallationState();
		}
		
		private void LoadPreinstalledAddInFilter()
		{
			ShowPreinstalledAddIns = AddInManager.Settings.ShowPreinstalledAddIns;
		}
		
		private void SavePreinstalledAddInFilter()
		{
			AddInManager.Settings.ShowPreinstalledAddIns = ShowPreinstalledAddIns;
		}
		
		protected override void InstallFromArchive()
		{
			// Notify about new operation
			AddInManager.Events.OnOperationStarted();
			
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = SD.ResourceService.GetString("AddInManager2.SDAddInFileFilter");
			dlg.Multiselect = true;
			var showDialogResult = dlg.ShowDialog();
			if (showDialogResult.HasValue && showDialogResult.Value)
			{
				foreach (var file in dlg.FileNames)
				{
					AddInManager.Setup.InstallAddIn(file);
				}
			}
		}
	}
}
