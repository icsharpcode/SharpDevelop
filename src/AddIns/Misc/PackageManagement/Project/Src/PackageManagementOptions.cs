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
		Properties properties;
		const string PackageSourcesPropertyName = "PackageSources";
		const string PackageDirectoryPropertyName = "PackagesDirectory";
		RegisteredPackageSources packageSources;
		
		public PackageManagementOptions(Properties properties)
		{
			this.properties = properties;
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
	}
}
