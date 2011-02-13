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
		
		public IQueryable<IPackage> GetPackages()
		{
			return FakePackages.AsQueryable();
		}
		
		public void AddPackage(IPackage package)
		{
		}
		
		public void RemovePackage(IPackage package)
		{
			IsRemovePackageCalled = true;
		}
		
		public string Source { get; set; }
	}
}
