// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakeSolutionPackageRepository : ISolutionPackageRepository
	{
		public string InstallPathToReturn;
		public IPackage PackagePassedToGetInstallPath;
		
		public string GetInstallPath(IPackage package)
		{
			PackagePassedToGetInstallPath = package;
			return InstallPathToReturn;
		}
		
		public List<FakePackage> FakePackages = new List<FakePackage>();
		
		public IEnumerable<IPackage> GetPackagesByDependencyOrder()
		{
			return FakePackages;
		}
	}
}
