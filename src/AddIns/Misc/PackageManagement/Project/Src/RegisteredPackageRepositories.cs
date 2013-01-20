// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RegisteredPackageRepositories : IRegisteredPackageRepositories
	{
		IPackageRepositoryCache repositoryCache;
		PackageManagementOptions options;
		RegisteredPackageSources registeredPackageSources;
		PackageSource activePackageSource;
		IPackageRepository activePackageRepository;
		
		public RegisteredPackageRepositories(
			IPackageRepositoryCache repositoryCache,
			PackageManagementOptions options)
		{
			this.repositoryCache = repositoryCache;
			this.options = options;
			registeredPackageSources = options.PackageSources;
		}
		
		public IRecentPackageRepository RecentPackageRepository {
			get { return repositoryCache.RecentPackageRepository; }
		}
		
		public IPackageRepository CreateRepository(PackageSource source)
		{
			return repositoryCache.CreateRepository(source.Source);
		}
		
		public IPackageRepository CreateAggregateRepository()
		{
			return repositoryCache.CreateAggregateRepository();
		}
		
		public RegisteredPackageSources PackageSources {
			get { return options.PackageSources; }
		}
		
		public bool HasMultiplePackageSources {
			get { return registeredPackageSources.HasMultipleEnabledPackageSources; }
		}
		
		public PackageSource ActivePackageSource {
			get {
				activePackageSource = options.ActivePackageSource;
				if (activePackageSource == null) {
					if (options.PackageSources.Any()) {
						activePackageSource = options.PackageSources[0];
					}
				}
				return activePackageSource;
			}
			set {
				if (activePackageSource != value) {
					activePackageSource = value;
					options.ActivePackageSource = value;
					activePackageRepository = null;
				}
			}
		}
		
		public IPackageRepository ActiveRepository {
			get {
				if (activePackageRepository == null) {
					CreateActiveRepository();
				}
				return activePackageRepository;
			}
		}
		
		void CreateActiveRepository()
		{
			if (ActivePackageSource.IsAggregate()) {
				activePackageRepository = CreateAggregateRepository();
			} else {
				activePackageRepository = repositoryCache.CreateRepository(ActivePackageSource.Source);
			}
		}
	}
}
