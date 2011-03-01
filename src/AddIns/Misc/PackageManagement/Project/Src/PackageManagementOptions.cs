// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using ICSharpCode.Core;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementOptions
	{
		const string PackageSourcesPropertyName = "PackageSources";
		const string PackageDirectoryPropertyName = "PackagesDirectory";
		const string ActivePackageSourcePropertyName = "ActivePackageSource";
		const string RecentPackagesPropertyName = "RecentPackages";

		RegisteredPackageSources packageSources;
		Properties properties;
		List<RecentPackageInfo> recentPackages;
		
		public PackageManagementOptions(Properties properties)
		{
			this.properties = properties;
			properties.Set<Version>("MyVersion", new Version(1, 0));
		}
		
		public PackageManagementOptions()
			: this(PropertyService.Get("PackageManagementSettings", new Properties()))
		{
		}
		
		public RegisteredPackageSources PackageSources {
			get {
				if (packageSources == null) {
					packageSources = new RegisteredPackageSources(GetSavedPackageSources());
					packageSources.CollectionChanged += PackageSourcesChanged;
				}
				return packageSources;
			}
		}
		
		IEnumerable<PackageSource> GetSavedPackageSources()
		{
			return PackageSourceConverter.ConvertFromRegisteredPackageSources(SavedRegisteredPackageSources);
		}

		void PackageSourcesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateSavedPackageSources();
		}
		
		void UpdateSavedPackageSources()
		{
			SavedRegisteredPackageSources = 
				PackageSourceConverter.ConvertToRegisteredPackageSourcesList(packageSources);
		}
		
		/// <summary>
		/// Use RegisteredPackageSource since PackageSource cannot be serialized.
		/// </summary>
		IList<RegisteredPackageSource> SavedRegisteredPackageSources {
			get { 
				List<RegisteredPackageSource> defaultSources = new List<RegisteredPackageSource>();
				return properties.Get<List<RegisteredPackageSource>>(PackageSourcesPropertyName, defaultSources);
			}
			set { properties.Set(PackageSourcesPropertyName, value); }
		}
		
		public string PackagesDirectory {
			get { return properties.Get(PackageDirectoryPropertyName, "packages"); }
			set { properties.Set(PackageDirectoryPropertyName, value); }
		}
		
		public PackageSource ActivePackageSource {
			get {
				if (properties.Contains(ActivePackageSourcePropertyName)) {
					var registeredPackageSource = properties.Get<RegisteredPackageSource>(ActivePackageSourcePropertyName, null);
					var packageSource = registeredPackageSource.ToPackageSource();
					if (PackageSources.Contains(packageSource)) {
						return packageSource;
					}
				}
				return null;
			}
			set {
				if (value == null) {
					properties.Remove(ActivePackageSourcePropertyName);
				} else {
					var packageSource = new RegisteredPackageSource(value);
					properties.Set(ActivePackageSourcePropertyName, packageSource);
				}
			}
		}
		
		public IList<RecentPackageInfo> RecentPackages {
			get {
				if (recentPackages == null) {
					ReadRecentPackages();
				}
				return recentPackages;
			}
		}
		
		void ReadRecentPackages()
		{
			var defaultRecentPackages = new List<RecentPackageInfo>();
			recentPackages = properties.Get<List<RecentPackageInfo>>(RecentPackagesPropertyName, defaultRecentPackages);
		}
	}
}
