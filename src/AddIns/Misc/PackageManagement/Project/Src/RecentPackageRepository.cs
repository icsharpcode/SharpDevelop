// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RecentPackageRepository : IPackageRepository
	{
		public const int DefaultMaximumPackagesCount = 20;
		
		List<IPackage> packages = new List<IPackage>();
		int maximumPackagesCount = DefaultMaximumPackagesCount;
		IPackageManagementService packageManagementService;
		
		public RecentPackageRepository(IPackageManagementService packageManagementService)
		{
			this.packageManagementService = packageManagementService;
		}
		
		public string Source {
			get { return "RecentPackages"; }
		}
		
		public void AddPackage(IPackage package)
		{
			RemovePackageIfAlreadyAdded(package);
			AddPackageAtBeginning(package);
			RemoveLastPackageIfCurrentPackageCountExceedsMaximum();
			UpdateRecentPackagesInOptions();
		}
		
		void RemovePackageIfAlreadyAdded(IPackage package)
		{
			int index = FindPackage(package);
			if (index >= 0) {
				packages.RemoveAt(index);
			}
		}
		
		int FindPackage(IPackage package)
		{
			return packages.FindIndex(p => PackageEqualityComparer.IdAndVersion.Equals(package, p));
		}
		
		void AddPackageAtBeginning(IPackage package)
		{
			packages.Insert(0, package);
		}
		
		void RemoveLastPackageIfCurrentPackageCountExceedsMaximum()
		{
			if (packages.Count > maximumPackagesCount) {
				RemoveLastPackage();
			}
		}

		void RemoveLastPackage()
		{
			packages.RemoveAt(packages.Count - 1);
		}
		
		void UpdateRecentPackagesInOptions()
		{
			IList<RecentPackageInfo> recentPackages = packageManagementService.Options.RecentPackages;
			recentPackages.Clear();
			recentPackages.AddRange(GetRecentPackagesInfo());
		}
		
		List<RecentPackageInfo> GetRecentPackagesInfo()
		{
			List<RecentPackageInfo> allRecentPackages = new List<RecentPackageInfo>();
			foreach (IPackage package in packages) {
				var recentPackageInfo = new RecentPackageInfo(package);
				allRecentPackages.Add(recentPackageInfo);
			}
			return allRecentPackages;
		}
		
		public void RemovePackage(IPackage package)
		{
		}
		
		public IQueryable<IPackage> GetPackages()
		{
			UpdatePackages();
			return packages.AsQueryable();
		}
		
		void UpdatePackages()
		{
			if (!HasRecentPackagesBeenRead() && HasRecentPackages()) {
				IEnumerable<IPackage> recentPackages = GetRecentPackages();
				packages.AddRange(recentPackages);
			}
		}
		
		bool HasRecentPackagesBeenRead()
		{
			return packages.Count > 0;
		}
		
		bool HasRecentPackages()
		{
			return packageManagementService.Options.RecentPackages.Count > 0;
		}
		
		IEnumerable<IPackage> GetRecentPackages()
		{
			IEnumerable<IPackage> recentPackages = GetRecentPackagesFilteredById();
			return GetRecentPackagesFilteredByVersion(recentPackages);
		}
		
		IEnumerable<IPackage> GetRecentPackagesFilteredById()
		{
			IPackageRepository aggregrateRepository = packageManagementService.CreateAggregatePackageRepository();
			IEnumerable<string> recentPackageIds = GetRecentPackageIds();
			return aggregrateRepository.FindPackages(recentPackageIds);
		}
				
		IEnumerable<string> GetRecentPackageIds()
		{
			foreach (RecentPackageInfo recentPackageInfo in packageManagementService.Options.RecentPackages) {
				yield return recentPackageInfo.Id;
			}
		}
		
		IEnumerable<IPackage> GetRecentPackagesFilteredByVersion(IEnumerable<IPackage> recentPackages)
		{
			List<IPackage> filteredRecentPackages = new List<IPackage>();
			foreach (IPackage recentPackage in recentPackages) {
				foreach (RecentPackageInfo savedRecentPackageInfo in packageManagementService.Options.RecentPackages) {
					if (savedRecentPackageInfo.IsMatch(recentPackage)) {
						filteredRecentPackages.Add(recentPackage);
					}
				}
			}
			return filteredRecentPackages;
		}
		
		public int MaximumPackagesCount {
			get { return maximumPackagesCount; }
			set { maximumPackagesCount = value; }
		}
	}
}
