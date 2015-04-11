// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakeSettings : ISettings
	{
		public List<SettingValue> PackageSources 
			= new List<SettingValue> ();
		
		public List<SettingValue> DisabledPackageSources
			= new List<SettingValue> ();
		
		public List<SettingValue> ActivePackageSourceSettings =
			new List<SettingValue> ();
		
		public Dictionary<string, IList<SettingValue>> Sections
			= new Dictionary<string, IList<SettingValue>> ();
		
		public FakeSettings()
		{
			Sections.Add(RegisteredPackageSourceSettings.PackageSourcesSectionName, PackageSources);
			Sections.Add(RegisteredPackageSourceSettings.ActivePackageSourceSectionName, ActivePackageSourceSettings);
			Sections.Add(RegisteredPackageSourceSettings.DisabledPackageSourceSectionName, DisabledPackageSources);
		}
		
		public string GetValue(string section, string key, bool isPath)
		{
			if (!Sections.ContainsKey(section))
				return null;
			
			IList<SettingValue> values = Sections[section];
			foreach (SettingValue setting in values) {
				if (setting.Key == key) {
					return setting.Value;
				}
			}
			return null;
		}
		
		public IList<SettingValue> GetValues(string section, bool isPath)
		{
			return Sections[section];
		}
		
		public void AddFakePackageSource(PackageSource packageSource)
		{
			var setting = new SettingValue (packageSource.Name, packageSource.Source, false);
			PackageSources.Add(setting);
		}
		
		public Dictionary<string, KeyValuePair<string, string>> SavedSectionValues =
			new Dictionary<string, KeyValuePair<string, string>>();
		
		public void SetValue(string section, string key, string value)
		{
			SavedSectionValues.Remove(section);
			SavedSectionValues.Add(section, new KeyValuePair<string, string>(key, value));
		}
		
		public KeyValuePair<string, string> GetValuePassedToSetValueForActivePackageSourceSection()
		{
			return SavedSectionValues[RegisteredPackageSourceSettings.ActivePackageSourceSectionName];
		}
		
		public void SetValues(string section, IList<KeyValuePair<string, string>> values)
		{
			SavedSectionValueLists.Remove(section);
			SavedSectionValueLists.Add(section, values);
		}
		
		public Dictionary<string, IList<KeyValuePair<string, string>>> SavedSectionValueLists
			= new Dictionary<string, IList<KeyValuePair<string, string>>>();
		
		public IList<KeyValuePair<string, string>> GetValuesPassedToSetValuesForPackageSourcesSection()
		{
			return SavedSectionValueLists[RegisteredPackageSourceSettings.PackageSourcesSectionName];
		}
		
		public bool DeleteValue(string section, string key)
		{
			throw new NotImplementedException();
		}
		
		public List<string> SectionsDeleted = new List<string>();
		
		public bool DeleteSection(string section)
		{
			SectionsDeleted.Add(section);
			return true;
		}
		
		public bool IsPackageSourcesSectionDeleted {
			get {
				return SectionsDeleted.Contains(RegisteredPackageSourceSettings.PackageSourcesSectionName);
			}
		}
		
		public bool IsDisabledPackageSourcesSectionDeleted {
			get {
				return SectionsDeleted.Contains(RegisteredPackageSourceSettings.DisabledPackageSourceSectionName);
			}
		}
		
		public bool IsActivePackageSourceSectionDeleted {
			get {
				return SectionsDeleted.Contains(RegisteredPackageSourceSettings.ActivePackageSourceSectionName);
			}
		}
		
		public void SetFakeActivePackageSource(PackageSource packageSource)
		{
			ActivePackageSourceSettings.Clear();
			var setting = new SettingValue(packageSource.Name, packageSource.Source, false);
			ActivePackageSourceSettings.Add(setting);
		}
		
		public void MakeActivePackageSourceSectionNull()
		{
			Sections.Remove(RegisteredPackageSourceSettings.ActivePackageSourceSectionName);
			Sections.Add(RegisteredPackageSourceSettings.ActivePackageSourceSectionName, null);
		}
		
		public void MakePackageSourceSectionsNull()
		{
			Sections.Remove(RegisteredPackageSourceSettings.PackageSourcesSectionName);
			Sections.Add(RegisteredPackageSourceSettings.PackageSourcesSectionName, null);
		}
		
		public void AddFakePackageSources(IEnumerable<PackageSource> packageSources)
		{
			foreach (PackageSource packageSource in packageSources) {
				AddFakePackageSource(packageSource);
			}
		}
		
		public IList<SettingValue> GetNestedValues(string section, string key)
		{
			return new List<SettingValue>();
		}
		
		public virtual void SetNestedValues(string section, string key, IList<KeyValuePair<string, string>> values)
		{
			throw new NotImplementedException();
		}
		
		public void AddDisabledPackageSource(PackageSource packageSource)
		{
			var setting = new SettingValue(packageSource.Name, packageSource.Source, false);
			DisabledPackageSources.Add(setting);
		}
		
		public IList<KeyValuePair<string, string>> GetValuesPassedToSetValuesForDisabledPackageSourcesSection()
		{
			return SavedSectionValueLists[RegisteredPackageSourceSettings.DisabledPackageSourceSectionName];
		}
		
		public bool AnyValuesPassedToSetValuesForDisabledPackageSourcesSection {
			get {
				return SavedSectionValueLists.ContainsKey(RegisteredPackageSourceSettings.DisabledPackageSourceSectionName);
			}
		}
		
		public virtual void SetPackageRestoreSetting(bool enabled)
		{
			var items = new List<SettingValue> ();
			items.Add(new SettingValue("enabled", enabled.ToString(), false));
			Sections.Add("packageRestore", items);
		}
		
		public KeyValuePair<string, string> GetValuePassedToSetValueForPackageRestoreSection()
		{
			return SavedSectionValues["packageRestore"];
		}
		
		public bool IsPackageRestoreSectionDeleted {
			get {
				return SectionsDeleted.Contains("packageRestore");
			}
		}
	}
}
