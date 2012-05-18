// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdatedPackages
	{
		IPackageRepository sourceRepository;
		IQueryable<IPackage> installedPackages;
		
		public UpdatedPackages(
			IPackageManagementProject project,
			IPackageRepository aggregateRepository)
			: this(
				project.GetPackages(),
				aggregateRepository)
		{
		}
		
		public UpdatedPackages(
			IQueryable<IPackage> installedPackages,
			IPackageRepository aggregrateRepository)
		{
			this.installedPackages = installedPackages;
			this.sourceRepository = aggregrateRepository;
		}
		
		public string SearchTerms { get; set; }
		
		public IEnumerable<IPackage> GetUpdatedPackages()
		{
			IQueryable<IPackage> localPackages = installedPackages;
			localPackages = FilterPackages(localPackages);
			return GetUpdatedPackages(sourceRepository, localPackages);
		}
		
		IQueryable<IPackage> GetInstalledPackages()
		{
			return installedPackages;
		}
		
		IQueryable<IPackage> FilterPackages(IQueryable<IPackage> localPackages)
		{
			return localPackages.Find(SearchTerms);
		}
		
		IEnumerable<IPackage> GetUpdatedPackages(IPackageRepository sourceRepository, IQueryable<IPackage> localPackages)
		{
			return sourceRepository.GetUpdates(localPackages, false, false);
		}
	}
}
