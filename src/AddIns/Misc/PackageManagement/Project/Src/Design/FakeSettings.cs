// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakeSettings : ISettings
	{
		public List<KeyValuePair<string, string>> PackageSources 
			= new List<KeyValuePair<string, string>>();
		
		public List<KeyValuePair<string, string>> ActivePackageSourceSettings =
			new List<KeyValuePair<string, string>>();
		
		public Dictionary<string, IList<KeyValuePair<string, string>>> Sections
			= new Dictionary<string, IList<KeyValuePair<string, string>>>();
			
		public FakeSettings()
		{
			Sections.Add(RegisteredPackageSources.PackageSourcesSectionName, PackageSources);
			Sections.Add(RegisteredPackageSources.ActivePackageSourceSectionName, ActivePackageSourceSettings);
		}
		
		public string GetValue(string section, string key)
		{
			throw new NotImplementedException();
		}
		
		public IList<KeyValuePair<string, string>> GetValues(string section)
		{
			return Sections[section];
		}
		
		public void AddFakePackageSource(PackageSource packageSource)
		{
			var valuePair = new KeyValuePair<string, string>(packageSource.Name, packageSource.Source);
			PackageSources.Add(valuePair);
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
			return SavedSectionValues[RegisteredPackageSources.ActivePackageSourceSectionName];
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
			return SavedSectionValueLists[RegisteredPackageSources.PackageSourcesSectionName];
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
				return SectionsDeleted.Contains(RegisteredPackageSources.PackageSourcesSectionName);
			}
		}
		
		public bool IsActivePackageSourceSectionDeleted {
			get {
				return SectionsDeleted.Contains(RegisteredPackageSources.ActivePackageSourceSectionName);
			}
		}
		
		public void SetFakeActivePackageSource(PackageSource packageSource)
		{
			ActivePackageSourceSettings.Clear();
			var valuePair = new KeyValuePair<string, string>(packageSource.Name, packageSource.Source);
			ActivePackageSourceSettings.Add(valuePair);
		}
		
		public void MakeActivePackageSourceSectionNull()
		{
			Sections.Remove(RegisteredPackageSources.ActivePackageSourceSectionName);
			Sections.Add(RegisteredPackageSources.ActivePackageSourceSectionName, null);
		}
		
		public void MakePackageSourceSectionsNull()
		{
			Sections.Remove(RegisteredPackageSources.PackageSourcesSectionName);
			Sections.Add(RegisteredPackageSources.PackageSourcesSectionName, null);			
		}
	}
}
