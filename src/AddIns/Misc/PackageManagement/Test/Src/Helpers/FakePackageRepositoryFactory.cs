// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageRepositoryFactory : ISharpDevelopPackageRepositoryFactory
	{
		public List<PackageSource> PackageSourcesPassedToCreateRepository
			= new List<PackageSource>();
		
		public FakePackageRepository FakePackageRepository = new FakePackageRepository();
		
		public Dictionary<PackageSource, FakePackageRepository> FakePackageRepositories =
			new Dictionary<PackageSource, FakePackageRepository>();
	
		public IPackageRepository CreateRepository(PackageSource packageSource)
		{
			PackageSourcesPassedToCreateRepository.Add(packageSource);
			
			FakePackageRepository repository = null;
			if (FakePackageRepositories.TryGetValue(packageSource, out repository)) {
				return repository;
			}
			
			return FakePackageRepository;
		}
		
		public ISharedPackageRepository CreateSharedRepository(string path)
		{
			return new FakeSharedPackageRepository(path);
		}
	}
}
