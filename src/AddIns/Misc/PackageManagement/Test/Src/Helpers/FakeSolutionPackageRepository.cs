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
	public class FakeSolutionPackageRepository : ISolutionPackageRepository
	{
		public FakeSharedPackageRepository FakeSharedRepository = new FakeSharedPackageRepository();
		
		public List<FakePackage> FakePackages;
		
		public FakeSolutionPackageRepository()
		{
			FakePackages = FakeSharedRepository.FakePackages;
		}
		
		public string InstallPathToReturn;
		public IPackage PackagePassedToGetInstallPath;
		
		public string GetInstallPath(IPackage package)
		{
			PackagePassedToGetInstallPath = package;
			return InstallPathToReturn;
		}
		
		public IEnumerable<IPackage> GetPackagesByDependencyOrder()
		{
			return FakePackages;
		}
		
		public List<FakePackage> FakePackagesByReverseDependencyOrder = new List<FakePackage>();
		
		public IEnumerable<IPackage> GetPackagesByReverseDependencyOrder()
		{
			return FakePackagesByReverseDependencyOrder;
		}
		
		public bool IsInstalled(IPackage package)
		{
			return FakeSharedRepository.FakePackages.Exists(p => p == package);
		}
		
		public IQueryable<IPackage> GetPackages()
		{
			return FakeSharedRepository.FakePackages.AsQueryable();
		}
		
		public ISharedPackageRepository Repository {
			get { return FakeSharedRepository; }
		}
		
		public IFileSystem FileSystem { get; set; }
		
		public IPackagePathResolver PackagePathResolver { get; set; }
	}
}
