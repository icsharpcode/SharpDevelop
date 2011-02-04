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
		IPackageRepository localRepository;
		IPackageRepository sourceRepository;
		
		public PackageUpdatesViewModel(
			IPackageManagementService packageManagementService,
			ITaskFactory taskFactory)
			: base(packageManagementService, taskFactory)
		{
			this.packageManagementService = packageManagementService;
		}
		
		protected override void UpdateRepositoryBeforeReadPackagesTaskStarts()
		{
			IProjectManager projectManager = packageManagementService.ActiveProjectManager;
			localRepository = projectManager.LocalRepository;
			
			sourceRepository = packageManagementService.CreateAggregatePackageRepository();
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			IQueryable<IPackage> localPackages = localRepository.GetPackages();
			return GetUpdatedPackages(localPackages);
		}
		
		IQueryable<IPackage> GetUpdatedPackages(IQueryable<IPackage> localPackages)
		{
			return sourceRepository.GetUpdates(localPackages).AsQueryable();
		}
	}
}
