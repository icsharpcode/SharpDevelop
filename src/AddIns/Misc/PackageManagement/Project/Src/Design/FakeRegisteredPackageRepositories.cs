// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakeRegisteredPackageRepositories : IRegisteredPackageRepositories
	{
		public FakeRegisteredPackageRepositories()
		{
			PackageSources = new RegisteredPackageSources(new PackageSource[0]);
		}
		
		public FakePackageRepository FakeActiveRepository = new FakePackageRepository();
		
		public virtual IPackageRepository ActiveRepository {
			get { return FakeActiveRepository; }
		}
		
		public FakePackageRepository FakeRecentPackageRepository = new FakePackageRepository();
		
		public IRecentPackageRepository RecentPackageRepository {
			get { return FakeRecentPackageRepository; }
		}
		
		public bool HasMultiplePackageSources { get; set; }
		
		public PackageSource ActivePackageSource { get; set; }
		
		public RegisteredPackageSources PackageSources { get; set; }
		
		public FakePackageRepository FakePackageRepository = new FakePackageRepository();
		public PackageSource PackageSourcePassedToCreateRepository;
		
		public IPackageRepository CreateRepository(PackageSource source)
		{
			PackageSourcePassedToCreateRepository = source;
			return FakePackageRepository;
		}
		
		public FakePackageRepository FakeAggregateRepository = new FakePackageRepository();
		
		public IPackageRepository CreateAggregateRepository()
		{
			return FakeAggregateRepository;
		}
		
		public void ClearPackageSources()
		{
			PackageSources.Clear();
		}
		
		public PackageSource AddOnePackageSource()
		{
			return AddOnePackageSource("Test");
		}
		
		public PackageSource AddOnePackageSource(string name)
		{
			var source = new PackageSource("http://sharpdevelop.codeplex.com", name);
			PackageSources.Add(source);
			return source;
		}
		
		public void AddPackageSources(IEnumerable<PackageSource> sources)
		{
			PackageSources.AddRange(sources);
		}
		
		public FakePackage AddFakePackageWithVersionToActiveRepository(string version)
		{
			var package = FakePackage.CreatePackageWithVersion("Test", version);
			FakeActiveRepository.FakePackages.Add(package);
			return package;
		}
		
		public FakePackage AddFakePackageWithVersionToAggregrateRepository(string id, string version)
		{
			var package = FakePackage.CreatePackageWithVersion(id, version);
			FakeAggregateRepository.FakePackages.Add(package);
			return package;
		}
	}
}
