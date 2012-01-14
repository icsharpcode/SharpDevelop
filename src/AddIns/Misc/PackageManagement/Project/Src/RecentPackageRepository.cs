// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RecentPackageRepository : IRecentPackageRepository
	{
		public const int DefaultMaximumPackagesCount = 20;
		
		List<IPackage> packages = new List<IPackage>();
		int maximumPackagesCount = DefaultMaximumPackagesCount;
		IList<RecentPackageInfo> savedRecentPackages;
		IPackageRepository aggregateRepository;
		IPackageManagementEvents packageManagementEvents;
		
		public RecentPackageRepository(
			IList<RecentPackageInfo> recentPackages,
			IPackageRepository aggregateRepository,
			IPackageManagementEvents packageManagementEvents)
		{
			this.savedRecentPackages = recentPackages;
			this.aggregateRepository = aggregateRepository;
			this.packageManagementEvents = packageManagementEvents;
			
			this.packageManagementEvents.ParentPackageInstalled += ParentPackageInstalled;
		}
		
		void ParentPackageInstalled(object sender, ParentPackageOperationEventArgs e)
		{
			AddPackage(e.Package);
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
			savedRecentPackages.Clear();
			savedRecentPackages.AddRange(GetRecentPackagesInfo());
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
			if (!HasRecentPackagesBeenRead() && HasRecentPackages) {
				IEnumerable<IPackage> recentPackages = GetRecentPackages();
				packages.AddRange(recentPackages);
			}
		}
		
		bool HasRecentPackagesBeenRead()
		{
			return packages.Count > 0;
		}
		
		public bool HasRecentPackages {
			get { return savedRecentPackages.Count > 0; }
		}
		
		IEnumerable<IPackage> GetRecentPackages()
		{
			IEnumerable<IPackage> recentPackages = GetRecentPackagesFilteredById();
			return GetRecentPackagesFilteredByVersion(recentPackages);
		}
		
		IEnumerable<IPackage> GetRecentPackagesFilteredById()
		{
			IEnumerable<string> recentPackageIds = GetRecentPackageIds();
			return aggregateRepository.FindPackages(recentPackageIds);
		}
				
		IEnumerable<string> GetRecentPackageIds()
		{
			foreach (RecentPackageInfo recentPackageInfo in savedRecentPackages) {
				yield return recentPackageInfo.Id;
			}
		}
		
		IEnumerable<IPackage> GetRecentPackagesFilteredByVersion(IEnumerable<IPackage> recentPackages)
		{
			List<IPackage> filteredRecentPackages = new List<IPackage>();
			foreach (IPackage recentPackage in recentPackages) {
				foreach (RecentPackageInfo savedRecentPackageInfo in savedRecentPackages) {
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
		
		public void Clear()
		{
			packages.Clear();
			UpdateRecentPackagesInOptions();
		}
		
		public bool SupportsPrereleasePackages {
			get { return false; }
		}
	}
}
