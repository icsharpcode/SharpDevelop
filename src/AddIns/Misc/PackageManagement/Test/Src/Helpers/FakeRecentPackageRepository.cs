// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakeRecentPackageRepository : IRecentPackageRepository
	{		
		public string Source { get; set; }
		public List<FakePackage> FakePackages = new List<FakePackage>();
		
		public IQueryable<IPackage> GetPackages()
		{
			return FakePackages.AsQueryable();
		}
		
		public void AddPackage(IPackage package)
		{
		}
		
		public void RemovePackage(IPackage package)
		{
		}
		
		public bool IsClearCalled;
		
		public void Clear()
		{
			IsClearCalled = true;
		}
		
		public bool HasRecentPackages { get; set; }
		public bool SupportsPrereleasePackages { get; set; }
	}
}
