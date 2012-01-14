// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageRepository : IRecentPackageRepository
	{
		public List<FakePackage> FakePackages = new List<FakePackage>();
		public bool IsRemovePackageCalled;
		public List<IPackage> PackagesAdded = new List<IPackage>();
		
		public IPackage FirstPackageAdded {
			get { return PackagesAdded[0]; }
		}
		
		public IQueryable<IPackage> GetPackages()
		{
			return FakePackages.AsQueryable();
		}
		
		public void AddPackage(IPackage package)
		{
			PackagesAdded.Add(package);
		}
		
		public void RemovePackage(IPackage package)
		{
			IsRemovePackageCalled = true;
			FakePackages.Remove(package as FakePackage);
		}
		
		public string Source { get; set; }
		
		public FakePackage AddFakePackage(string packageId)
		{
			var package = new FakePackage(packageId);
			FakePackages.Add(package);
			return package;
		}
		
		public FakePackage AddFakePackageWithVersion(string packageId, string version)
		{
			var package = FakePackage.CreatePackageWithVersion(packageId, version);
			FakePackages.Add(package);
			return package;
		}
		
		public void Clear()
		{
		}
		
		public bool HasRecentPackages { get; set; }
		public bool SupportsPrereleasePackages { get; set; }
	}
}
