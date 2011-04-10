// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		
		public RegisteredPackageRepositories(PackageManagementOptions options)
			: this(new PackageRepositoryCache(options.PackageSources, options.RecentPackages), options)
		{
		}
		
		public RegisteredPackageRepositories(
			IPackageRepositoryCache repositoryCache,
			PackageManagementOptions options)
		{
			this.repositoryCache = repositoryCache;
			this.options = options;
			registeredPackageSources = options.PackageSources;
		}
		
		public IPackageRepository RecentPackageRepository {
			get { return repositoryCache.RecentPackageRepository; }
		}
		
		public IPackageRepository CreateRepository(PackageSource source)
		{
			return repositoryCache.CreateRepository(source);
		}
		
		public IPackageRepository CreateAggregateRepository()
		{
			return repositoryCache.CreateAggregateRepository();
		}
		
		public RegisteredPackageSources PackageSources {
			get { return options.PackageSources; }
		}
		
		public bool HasMultiplePackageSources {
			get { return registeredPackageSources.HasMultiplePackageSources; }
		}
		
		public PackageSource ActivePackageSource {
			get {
				activePackageSource = options.ActivePackageSource;
				if (activePackageSource == null) {
					activePackageSource = options.PackageSources[0];
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
			activePackageRepository = repositoryCache.CreateRepository(ActivePackageSource);
		}
	}
}
