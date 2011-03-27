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
		IPackageManagementService packageManagementService;
		MSBuildBasedProject project;
		IPackageRepository localRepository;
		IPackageRepository sourceRepository;
		
		public UpdatedPackages(IPackageManagementService packageManagementService)
			: this(packageManagementService, null)
		{
		}
		
		public UpdatedPackages(
			IPackageManagementService packageManagementService,
			MSBuildBasedProject project)
		{
			this.packageManagementService = packageManagementService;
			this.project = project;
			GetRepositories();
		}
		
		void GetRepositories()
		{
			GetSourceRepository();
			GetLocalRepository();
		}
		
		void GetSourceRepository()
		{
			sourceRepository = packageManagementService.CreateAggregatePackageRepository();
		}
		
		void GetLocalRepository()
		{
			IProjectManager projectManager = CreateProjectManager();
			localRepository = projectManager.LocalRepository;
		}
		
		public string SearchTerms { get; set; }
		
		public IEnumerable<IPackage> GetUpdatedPackages()
		{
			IQueryable<IPackage> localPackages = localRepository.GetPackages();
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
		
		IProjectManager CreateProjectManager()
		{
			if (project != null) {
				return packageManagementService.CreateProjectManager(sourceRepository, project);
			}
			return packageManagementService.ActiveProjectManager;
		}
	}
}
