// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ICSharpCode.Core;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RegisteredPackageSources : ObservableCollection<PackageSource>
	{
		public static readonly string DefaultPackageSourceUrl = "http://go.microsoft.com/fwlink/?LinkID=206669";
		public static readonly string DefaultPackageSourceName = "NuGet Official Package Source";
		
		public static readonly string PackageSourcesSectionName = "packageSources";
		public static readonly string ActivePackageSourceSectionName = "activePackageSource";
		
		public static readonly PackageSource DefaultPackageSource = 
			new PackageSource(DefaultPackageSourceUrl, DefaultPackageSourceName);
		
		ISettings settings;
		PackageSource activePackageSource;
		
		public RegisteredPackageSources(IEnumerable<PackageSource> packageSources)
		{
			AddPackageSources(packageSources);
			AddDefaultPackageSourceIfNoRegisteredPackageSources();
		}
		
		public RegisteredPackageSources(ISettings settings)
		{
			this.settings = settings;
			AddPackageSources();
			AddDefaultPackageSourceIfNoRegisteredPackageSources();
			ReadActivePackageSource();
		}
		
		void AddPackageSources()
		{
			foreach (PackageSource source in GetPackageSourcesFromSettings()) {
				Add(source);
			}
		}
		
		IEnumerable<PackageSource> GetPackageSourcesFromSettings()
		{
			IList<KeyValuePair<string, string>> savedPackageSources = settings.GetValues(PackageSourcesSectionName);
			if (HasAny(savedPackageSources)) {
				foreach (KeyValuePair<string, string> packageSource in savedPackageSources) {
					yield return CreatePackageSourceFromKeyValuePair(packageSource);
				}
			}
		}
		
		bool HasAny(IList<KeyValuePair<string, string>> packageSources)
		{
			if (packageSources != null) {
				return packageSources.Any();
			}
			return false;
		}
		
		PackageSource CreatePackageSourceFromKeyValuePair(KeyValuePair<string, string> savedPackageSource)
		{
			string source = savedPackageSource.Value;
			string name = savedPackageSource.Key;
			return new PackageSource(source, name);
		}

		void AddPackageSources(IEnumerable<PackageSource> packageSources)
		{
			foreach (PackageSource source in packageSources) {
				Add(source);
			}
		}
		
		void AddDefaultPackageSourceIfNoRegisteredPackageSources()
		{
			if (HasNoRegisteredPackageSources) {
				Add(DefaultPackageSource);
			}
		}
		
		bool HasNoRegisteredPackageSources {
			get { return Count == 0; }
		}
		
		public bool HasMultiplePackageSources {
			get { return Count > 1; }
		}
		
		void ReadActivePackageSource()
		{
			IList<KeyValuePair<string, string>> packageSources = settings.GetValues(ActivePackageSourceSectionName);
			if (HasAny(packageSources)) {
				activePackageSource = CreatePackageSourceFromKeyValuePair(packageSources[0]);
			}
		}
		
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
			UpdatePackageSourceSettingsWithChanges();
		}
		
		void UpdatePackageSourceSettingsWithChanges()
		{
			if (settings != null) {
				IList<KeyValuePair<string, string>> newPackageSourceSettings = GetSettingsFromPackageSources();
				SavePackageSourceSettings(newPackageSourceSettings);
			}
		}
		
		IList<KeyValuePair<string, string>> GetSettingsFromPackageSources()
		{
			var settings = new List<KeyValuePair<string, string>>();
			foreach (PackageSource source in this) {
				settings.Add(CreateKeyValuePairFromPackageSource(source));
			}
			return settings;
		}
		
		KeyValuePair<string, string> CreateKeyValuePairFromPackageSource(PackageSource source)
		{
			return new KeyValuePair<string, string>(source.Name, source.Source);
		}
		
		void SavePackageSourceSettings(IList<KeyValuePair<string, string>> newPackageSourceSettings)
		{
			settings.DeleteSection(PackageSourcesSectionName);
			settings.SetValues(PackageSourcesSectionName, newPackageSourceSettings);
		}
		
		public PackageSource ActivePackageSource {
			get {
				if (Contains(activePackageSource)) {
					return activePackageSource;
				}
				return null;
			}
			set {
				activePackageSource = value;
				if (activePackageSource == null) {
					RemoveActivePackageSourceSetting();
				} else {
					UpdateActivePackageSourceSetting();
				}
			}
		}
		
		void RemoveActivePackageSourceSetting()
		{
			settings.DeleteSection(ActivePackageSourceSectionName);
		}
		
		void UpdateActivePackageSourceSetting()
		{
			RemoveActivePackageSourceSetting();
			
			KeyValuePair<string, string> activePackageSourceSetting = CreateKeyValuePairFromPackageSource(activePackageSource);
			SaveActivePackageSourceSetting(activePackageSourceSetting);
		}
		
		void SaveActivePackageSourceSetting(KeyValuePair<string, string> activePackageSource)
		{
			settings.SetValue(ActivePackageSourceSectionName, activePackageSource.Key, activePackageSource.Value);
		}
	}
}
