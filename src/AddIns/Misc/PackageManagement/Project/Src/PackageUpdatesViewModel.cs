// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageUpdatesViewModel : PackagesViewModel
	{
		List<IPackage> packages = new List<IPackage>();
		IPackageManagementService packageManagementService;
		
		public PackageUpdatesViewModel(IPackageManagementService packageManagementService)
			: base(packageManagementService)
		{
			this.packageManagementService = packageManagementService;
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			IQueryable<IPackage> localPackages = GetLocalRepositoryPackages();
			return GetUpdatedPackages(localPackages);
		}
		
		IQueryable<IPackage> GetLocalRepositoryPackages()
		{
			IProjectManager projectManager = packageManagementService.ActiveProjectManager;
			IPackageRepository localRepository = projectManager.LocalRepository;
			return localRepository.GetPackages();
		}
		
		IQueryable<IPackage> GetUpdatedPackages(IQueryable<IPackage> localPackages)
		{
			IPackageRepository sourceRepository = packageManagementService.CreateAggregatePackageRepository();
			return sourceRepository.GetUpdates(localPackages).AsQueryable();
		}
	}
}
