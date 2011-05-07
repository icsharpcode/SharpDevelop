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
	public class FakeMachinePackageCache : IMachinePackageCache
	{
		public List<FakePackage> FakePackages = new List<FakePackage>();
		
		public IQueryable<IPackage> GetPackages()
		{
			return FakePackages.AsQueryable();
		}
		
		public bool IsClearCalled;
		
		public void Clear()
		{
			IsClearCalled = true;
		}
		
		public string Source { get; set; }
	}
}
