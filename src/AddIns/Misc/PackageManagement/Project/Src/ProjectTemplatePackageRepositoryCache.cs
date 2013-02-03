// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	/// <summary>
	/// Supports a configurable set of package repositories for project templates that can be
	/// different to the registered package repositories used with the Add Package Reference dialog.
	/// </summary>
	public class ProjectTemplatePackageRepositoryCache : IPackageRepositoryCache
	{
		IPackageRepositoryCache packageRepositoryCache;
		RegisteredProjectTemplatePackageSources registeredPackageSources;
		
		/// <summary>
		/// Creates a new instance of the ProjectTemplatePackageRepositoryCache.
		/// </summary>
		/// <param name="packageRepositoryCache">The main package repository cache used
		/// with the Add Package Reference dialog.</param>
		public ProjectTemplatePackageRepositoryCache(
			IPackageRepositoryCache packageRepositoryCache,
			RegisteredProjectTemplatePackageSources registeredPackageSources)
		{
			this.packageRepositoryCache = packageRepositoryCache;
			this.registeredPackageSources = registeredPackageSources;
		}
		
		public IRecentPackageRepository RecentPackageRepository {
			get { throw new NotImplementedException(); }
		}
		
		public IPackageRepository CreateAggregateRepository()
		{
			IEnumerable<IPackageRepository> repositories = GetRegisteredPackageRepositories();
			return CreateAggregateRepository(repositories);
		}
		
		IEnumerable<IPackageRepository> GetRegisteredPackageRepositories()
		{
			foreach (PackageSource packageSource in GetEnabledPackageSources()) {
				yield return CreateRepository(packageSource.Source);
			}
		}
		
		public IEnumerable<PackageSource> GetEnabledPackageSources()
		{
			return registeredPackageSources.PackageSources.GetEnabledPackageSources();
		}
		
		public ISharedPackageRepository CreateSharedRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem, IFileSystem configSettingsFileSystem)
		{
			throw new NotImplementedException();
		}
		
		public IRecentPackageRepository CreateRecentPackageRepository(IList<RecentPackageInfo> recentPackages, IPackageRepository aggregateRepository)
		{
			throw new NotImplementedException();
		}
		
		public IPackageRepository CreateAggregateRepository(IEnumerable<IPackageRepository> repositories)
		{
			return packageRepositoryCache.CreateAggregateRepository(repositories);
		}
		
		public IPackageRepository CreateRepository(string packageSource)
		{
			return packageRepositoryCache.CreateRepository(packageSource);
		}
	}
}
