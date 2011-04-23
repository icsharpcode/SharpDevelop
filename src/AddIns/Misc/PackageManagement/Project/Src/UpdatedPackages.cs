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
		IPackageManagementProject project;
		IPackageRepository sourceRepository;
		
		public UpdatedPackages(
			IPackageManagementProject project,
			IPackageRepository aggregateRepository)
		{
			this.project = project;
			this.sourceRepository = aggregateRepository;
		}
		
		public string SearchTerms { get; set; }
		
		public IEnumerable<IPackage> GetUpdatedPackages()
		{
			IQueryable<IPackage> localPackages = project.GetPackages();
			localPackages = FilterPackages(localPackages);
			return GetUpdatedPackages(sourceRepository, localPackages);
		}
		
		IQueryable<IPackage> FilterPackages(IQueryable<IPackage> localPackages)
		{
			return localPackages.Find(SearchTerms);
		}
		
		IEnumerable<IPackage> GetUpdatedPackages(IPackageRepository sourceRepository, IQueryable<IPackage> localPackages)
		{
			return sourceRepository.GetUpdates(localPackages);
		}
	}
}
