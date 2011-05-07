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
		ISettings settings;
		List<RecentPackageInfo> recentPackages;
		
		public PackageManagementOptions(Properties properties, ISettings settings)
		{
			this.properties = properties;
			this.settings = settings;
		}
		
		public PackageManagementOptions(Properties properties)
			: this(properties, Settings.UserSettings)
		{
		}
		
		public PackageManagementOptions()
			: this(PropertyService.Get("PackageManagementSettings", new Properties()))
		{
		}
		
		public RegisteredPackageSources PackageSources {
			get {
				if (packageSources == null) {
					packageSources = new RegisteredPackageSources(settings);
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
			get { return PackageSources.ActivePackageSource; }
			set { PackageSources.ActivePackageSource = value; }
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
