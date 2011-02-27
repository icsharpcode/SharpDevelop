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
		
		public RecentPackageRepository()
		{
		}
		
		public string Source {
			get { return "RecentPackages"; }
		}
		
		public void AddPackage(IPackage package)
		{
			RemovePackageIfAlreadyAdded(package);
			AddPackageAtBeginning(package);
			RemoveLastPackageIfCurrentPackageCountExceedsMaximum();
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
		
		public void RemovePackage(IPackage package)
		{
		}
		
		public IQueryable<IPackage> GetPackages()
		{
			return packages.AsQueryable();
		}
		
		public int MaximumPackagesCount {
			get { return maximumPackagesCount; }
			set { maximumPackagesCount = value; }
		}
	}
}
