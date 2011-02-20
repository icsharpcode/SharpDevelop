// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageRepository : IPackageRepository
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
		}
		
		public string Source { get; set; }
	}
}
