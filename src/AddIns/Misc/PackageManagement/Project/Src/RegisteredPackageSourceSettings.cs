// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RegisteredPackageSourceSettings
	{
		public static readonly string PackageSourcesSectionName = "packageSources";
		public static readonly string ActivePackageSourceSectionName = "activePackageSource";
		public static readonly string DisabledPackageSourceSectionName = "disabledPackageSources";
		
		public static readonly PackageSource AggregatePackageSource = 
			new PackageSource("(Aggregate source)", "All");
		
		ISettings settings;
		ISettingsProvider settingsProvider;
		PackageSource defaultPackageSource;
		RegisteredPackageSources packageSources;
		PackageSource activePackageSource;
		
		public RegisteredPackageSourceSettings(ISettingsProvider settingsProvider)
			: this(settingsProvider, RegisteredPackageSources.DefaultPackageSource)
		{
		}
		
		public RegisteredPackageSourceSettings(
			ISettingsProvider settingsProvider,
			PackageSource defaultPackageSource)
		{
			this.settingsProvider = settingsProvider;
			this.defaultPackageSource = defaultPackageSource;
			
			settings = settingsProvider.LoadSettings();
			
			ReadActivePackageSource();
			RegisterSolutionEvents();
		}

		void RegisterSolutionEvents()
		{
			settingsProvider.SettingsChanged += SettingsChanged;
		}
		
		void ReadActivePackageSource()
		{
			IList<KeyValuePair<string, string>> packageSources = settings.GetValues(ActivePackageSourceSectionName);
			activePackageSource = PackageSourceConverter.ConvertFromFirstKeyValuePair(packageSources);
		}
		
		public RegisteredPackageSources PackageSources {
			get {
				if (packageSources == null) {
					ReadPackageSources();
				}
				return packageSources;
			}
		}
		
		void ReadPackageSources()
		{
			IEnumerable<PackageSource> savedPackageSources = GetPackageSourcesFromSettings();
			packageSources = new RegisteredPackageSources(savedPackageSources, defaultPackageSource);
			packageSources.CollectionChanged += PackageSourcesChanged;
			
			if (!savedPackageSources.Any()) {
				UpdatePackageSourceSettingsWithChanges();
			}
		}
		
		IEnumerable<PackageSource> GetPackageSourcesFromSettings()
		{
			IList<KeyValuePair<string, string>> savedPackageSources = settings.GetValues(PackageSourcesSectionName);
			foreach (PackageSource packageSource in PackageSourceConverter.ConvertFromKeyValuePairs(savedPackageSources)) {
				packageSource.IsEnabled = IsPackageSourceEnabled(packageSource);
				yield return packageSource;
			}
		}
		
		bool IsPackageSourceEnabled(PackageSource packageSource)
		{
			string disabled = settings.GetValue(DisabledPackageSourceSectionName, packageSource.Name);
			return String.IsNullOrEmpty(disabled);
		}
		
		void PackageSourcesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdatePackageSourceSettingsWithChanges();
		}
		
		void UpdatePackageSourceSettingsWithChanges()
		{
			IList<KeyValuePair<string, string>> newPackageSourceSettings = GetSettingsFromPackageSources();
			SavePackageSourceSettings(newPackageSourceSettings);
			IList<KeyValuePair<string, string>> disabledPackageSourceSettings = GetSettingsForDisabledPackageSources();
			SaveDisabledPackageSourceSettings(disabledPackageSourceSettings);
		}
		
		IList<KeyValuePair<string, string>> GetSettingsFromPackageSources()
		{
			return PackageSourceConverter.ConvertToKeyValuePairList(packageSources);
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
		
		IList<KeyValuePair<string, string>> GetSettingsForDisabledPackageSources()
		{
			return packageSources
				.Where(source => !source.IsEnabled)
				.Select(source => new KeyValuePair<string, string>(source.Name, "true"))
				.ToList();
		}
		
		void SaveDisabledPackageSourceSettings(IList<KeyValuePair<string, string>> disabledPackageSourceSettings)
		{
			settings.DeleteSection(DisabledPackageSourceSectionName);
			if (disabledPackageSourceSettings.Any()) {
				settings.SetValues(DisabledPackageSourceSectionName, disabledPackageSourceSettings);
			}
		}
		
		public PackageSource ActivePackageSource {
			get {
				if (activePackageSource != null) {
					if (activePackageSource.IsAggregate()) {
						return activePackageSource;
					}
					if (PackageSources.Contains(activePackageSource)) {
						return activePackageSource;
					}
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
			
			KeyValuePair<string, string> activePackageSourceSetting = PackageSourceConverter.ConvertToKeyValuePair(activePackageSource);
			SaveActivePackageSourceSetting(activePackageSourceSetting);
		}
		
		void SaveActivePackageSourceSetting(KeyValuePair<string, string> activePackageSource)
		{
			settings.SetValue(ActivePackageSourceSectionName, activePackageSource.Key, activePackageSource.Value);
		}
		
		void SettingsChanged(object sender, EventArgs e)
		{
			settings = settingsProvider.LoadSettings();
			ReadActivePackageSource();
			ResetPackageSources();
		}
		
		void ResetPackageSources()
		{
			if (packageSources != null) {
				packageSources.CollectionChanged -= PackageSourcesChanged;
				packageSources = null;
			}
		}
		
		public ISettings Settings {
			get { return settings; }
		}
	}
}
