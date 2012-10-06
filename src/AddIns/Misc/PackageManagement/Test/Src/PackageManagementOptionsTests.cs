// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageManagementOptionsTests
	{
		Properties properties;
		PackageManagementOptions options;
		FakeSettings fakeSettings;
		
		void CreateOptions()
		{
			CreateProperties();
			CreateSettings();
			CreateOptions(properties, fakeSettings);
		}
		
		void CreateProperties()
		{
			properties = new Properties();
		}
		
		void CreateSettings()
		{
			fakeSettings = new FakeSettings();
		}
		
		void CreateOptions(Properties properties)
		{
			CreateSettings();
			CreateOptions(properties, fakeSettings);
		}
		
		void CreateOptions(FakeSettings fakeSettings)
		{
			CreateProperties();
			options = new PackageManagementOptions(properties, fakeSettings);
		}
		
		void CreateOptions(Properties properties, FakeSettings fakeSettings)
		{
			options = new PackageManagementOptions(properties, fakeSettings);
		}
		
		void SaveOptions()
		{
			StringBuilder xml = new StringBuilder();
			StringWriter stringWriter = new StringWriter(xml);
			using (XmlTextWriter writer = new XmlTextWriter(stringWriter)) {
				properties.WriteProperties(writer);
			}
		}
		
		RecentPackageInfo AddRecentPackageToOptions(string id, string version)
		{
			var recentPackage = new RecentPackageInfo(id, new SemanticVersion(version));
			options.RecentPackages.Add(recentPackage);
			return recentPackage;
		}
		
		void EnablePackageRestoreInSettings()
		{
			fakeSettings.SetPackageRestoreSetting(true);
		}
		
		[Test]
		public void PackageSources_OnePackageSourceInSettings_ContainsSinglePackageSourceFromSettings()
		{
			CreateSettings();
			var packageSource = new PackageSource("http://codeplex.com", "Test");
			fakeSettings.AddFakePackageSource(packageSource);
			CreateOptions(fakeSettings);
			
			RegisteredPackageSources actualSources = options.PackageSources;
			
			List<PackageSource> expectedSources = new List<PackageSource>();
			expectedSources.Add(packageSource);
			
			Assert.AreEqual(expectedSources, actualSources);
		}
		
		[Test]
		public void PackageSources_NoPackageSourceInSavedSettings_ContainsDefaultPackageSource()
		{
			CreateSettings();
			CreateOptions(fakeSettings);
			
			List<PackageSource> expectedSources = new List<PackageSource>();
			expectedSources.Add(RegisteredPackageSources.DefaultPackageSource);
			
			RegisteredPackageSources actualPackageSources = options.PackageSources;
			
			CollectionAssert.AreEqual(expectedSources, actualPackageSources);
		}
		
		[Test]
		public void PackageSources_NoPackageSourceInSavedSettings_DefaultPackageSourceAddedToSettings()
		{
			CreateSettings();
			CreateOptions(fakeSettings);
			
			RegisteredPackageSources packageSources = options.PackageSources;
			
			PackageSource defaultSource = RegisteredPackageSources.DefaultPackageSource;
			
			var expectedSavedPackageSourceSettings = new List<KeyValuePair<string, string>>();
			string name = defaultSource.Name;
			string sourceUrl = defaultSource.Source;
			expectedSavedPackageSourceSettings.Add(new KeyValuePair<string, string>(name, sourceUrl));
			
			IList<KeyValuePair<string, string>> actualSavedPackageSourceSettings = fakeSettings.GetValuesPassedToSetValuesForPackageSourcesSection();
			
			Assert.AreEqual(expectedSavedPackageSourceSettings, actualSavedPackageSourceSettings);
		}
		
		[Test]
		public void PackageSources_OnePackageSourceAdded_PackageSourceSavedInSettings()
		{
			CreateSettings();
			CreateOptions(fakeSettings);
			RegisteredPackageSources registeredPackageSources = options.PackageSources;
			
			var packageSource = new PackageSource("http://codeplex.com", "Test");
			registeredPackageSources.Clear();
			registeredPackageSources.Add(packageSource);
			
			var expectedSavedPackageSourceSettings = new List<KeyValuePair<string, string>>();
			expectedSavedPackageSourceSettings.Add(new KeyValuePair<string, string>("Test", "http://codeplex.com"));
			
			IList<KeyValuePair<string, string>> actualSavedPackageSourceSettings = fakeSettings.GetValuesPassedToSetValuesForPackageSourcesSection();
			
			Assert.AreEqual(expectedSavedPackageSourceSettings, actualSavedPackageSourceSettings);
		}
		
		[Test]
		public void PackageSources_OnePackageSourceAdded_PackageSourcesSectionDeletedFromSettings()
		{
			CreateSettings();
			CreateOptions(fakeSettings);
			RegisteredPackageSources registeredPackageSources = options.PackageSources;	
			
			var packageSource = new PackageSource("http://codeplex.com", "Test");
			registeredPackageSources.Clear();
			registeredPackageSources.Add(packageSource);
			
			bool sectionDeleted = fakeSettings.IsPackageSourcesSectionDeleted;
			
			Assert.IsTrue(sectionDeleted);
		}
		
		[Test]
		public void PackageSources_SettingsFilesDoesNotExistSoSettingsReturnsNullForPackageSourcesSection_DoesNotThrowException()
		{
			CreateSettings();
			fakeSettings.MakePackageSourceSectionsNull();
			CreateOptions(fakeSettings);
			
			RegisteredPackageSources packageSources = null;
			Assert.DoesNotThrow(() => packageSources = options.PackageSources);
		}
		
		[Test]
		public void ActivePackageSource_NoInitialPropertiesSet_ReturnsNull()
		{
			CreateOptions();
			PackageSource actualSource = options.ActivePackageSource;
			Assert.IsNull(options.ActivePackageSource);
		}
		
		[Test]
		public void ActivePackageSource_SourceChanged_ActivePackageSourceUpdatedInSettings()
		{
			CreateOptions();
			var packageSource = new PackageSource("http://sharpdevelop.com", "Test");
			
			options.ActivePackageSource = packageSource;
			
			var expectedKeyValuePair = new KeyValuePair<string, string>("Test", "http://sharpdevelop.com");
			KeyValuePair<string, string> actualKeyValuePair = fakeSettings.GetValuePassedToSetValueForActivePackageSourceSection();
			
			Assert.AreEqual(expectedKeyValuePair, actualKeyValuePair);
		}
		
		[Test]
		public void ActivePackageSource_ActivePackageSourceNonNullInSettings_ReturnsActivatePackageSourceInSavedSettings()
		{
			CreateSettings();
			var expectedSource = new PackageSource("http://source-url", "Test");
			fakeSettings.SetFakeActivePackageSource(expectedSource);
			fakeSettings.AddFakePackageSource(expectedSource);
			CreateOptions(fakeSettings);
			
			PackageSource actualSource = options.ActivePackageSource;
			
			Assert.AreEqual(expectedSource, actualSource);
		}
		
		[Test]
		public void ActivePackageSource_SetToNullAfterHavingNonNullValue_ReturnsNull()
		{
			CreateOptions();
			var source = new PackageSource("http://source-url", "Test");
			options.ActivePackageSource = source;
			
			options.ActivePackageSource = null;
			
			PackageSource actualSource = options.ActivePackageSource;
			Assert.IsNull(actualSource);
		}
	
		[Test]
		public void ActivePackageSource_ActivePackageSourceRemovedFromPackageSources_ActivePackageSourceSetToNull()
		{
			CreateOptions();
			var source = new PackageSource("http://source-url", "Test");
			options.PackageSources.Add(source);
			
			var differentSource = new PackageSource("http://different-url", "Test2");
			options.ActivePackageSource = differentSource;
			
			PackageSource activeSource = options.ActivePackageSource;
			
			Assert.IsNull(activeSource);
		}
		
		[Test]
		public void ActivePackageSource_ActivePackageSourceChanged_ActivePackageSourceSectionRemovedFromSettings()
		{
			CreateOptions();
			
			options.ActivePackageSource = new PackageSource("http://sharpdevelop.com", "Test");
			
			bool sectionDeleted = fakeSettings.IsActivePackageSourceSectionDeleted;
			
			Assert.IsTrue(sectionDeleted);
		}
		
		[Test]
		public void ActivePackageSource_OneActivePackageSourceInSettingsAndActivePackageSourceSetToNull_ActivePackageSourceSectionRemoved()
		{
			CreateSettings();
			var expectedSource = new PackageSource("http://source-url", "Test");
			fakeSettings.SetFakeActivePackageSource(expectedSource);
			fakeSettings.AddFakePackageSource(expectedSource);
			CreateOptions(fakeSettings);
			
			options.ActivePackageSource = null;
			
			bool sectionRemoved = fakeSettings.IsActivePackageSourceSectionDeleted;
			
			Assert.IsTrue(sectionRemoved);
		}
		
		[Test]
		public void ActivePackageSource_SettingsFilesDoesNotExistSoSettingsReturnsNullForActivePackageSourceSection_DoesNotThrowException()
		{
			CreateSettings();
			fakeSettings.MakeActivePackageSourceSectionNull();
			CreateOptions(fakeSettings);
			
			PackageSource activePackageSource = null;
			Assert.DoesNotThrow(() => activePackageSource = options.ActivePackageSource);
		}
		
		[Test]
		public void RecentPackages_OneRecentPackageAddedAndOptionsReloadedFromSavedProperties_ContainsOneRecentPackageThatWasSavedPreviously()
		{
			CreateOptions();
			var package = new FakePackage("Test");
			var recentPackage = new RecentPackageInfo(package);
			options.RecentPackages.Add(recentPackage);
			CreateOptions(properties);
			
			IList<RecentPackageInfo> recentPackages = options.RecentPackages;
			
			var expectedRecentPackages = new RecentPackageInfo[] {
				new RecentPackageInfo(package)
			};
			
			RecentPackageInfoCollectionAssert.AreEqual(expectedRecentPackages, recentPackages);
		}
		
		[Test]
		public void RecentPackages_SaveRecentPackages_DoesNotThrowInvalidOperationException()
		{
			CreateOptions();
			RecentPackageInfo recentPackage = AddRecentPackageToOptions("id", "1.0");
			
			Assert.DoesNotThrow(() => SaveOptions());
		}
		
		[Test]
		public void ActivePackageSource_AggregatePackageSourceIsActivePackageSourceInSettings_ReturnsAggregatePackageSource()
		{
			CreateSettings();
			PackageSource expectedPackageSource = RegisteredPackageSourceSettings.AggregatePackageSource;
			fakeSettings.SetFakeActivePackageSource(expectedPackageSource);
			CreateOptions(fakeSettings);
			
			PackageSource activePackageSource = options.ActivePackageSource;
			
			Assert.AreEqual(expectedPackageSource, activePackageSource);
		}
		
		[Test]
		public void PackageSources_OneEnabledPackageSourceInSettings_ContainsSingleEnabledPackageSourceFromSettings()
		{
			CreateSettings();
			var packageSource = new PackageSource("http://codeplex.com", "Test") { IsEnabled = true };
			fakeSettings.AddFakePackageSource(packageSource);
			CreateOptions(fakeSettings);
			
			RegisteredPackageSources actualSources = options.PackageSources;
			
			Assert.IsTrue(actualSources[0].IsEnabled);
		}
		
		[Test]
		public void PackageSources_OneDisabledPackageSourceInSettings_ContainsSingleDisabledPackageSourceFromSettings()
		{
			CreateSettings();
			var packageSource = new PackageSource("http://codeplex.com", "Test") { IsEnabled = false };
			fakeSettings.AddFakePackageSource(packageSource);
			fakeSettings.AddDisabledPackageSource(packageSource);
			CreateOptions(fakeSettings);
			
			RegisteredPackageSources actualSources = options.PackageSources;
			
			Assert.IsFalse(actualSources[0].IsEnabled);
		}
		
		[Test]
		public void PackageSources_OnePackageSourceAdded_DisabledPackageSourcesSectionDeletedFromSettings()
		{
			CreateSettings();
			CreateOptions(fakeSettings);
			RegisteredPackageSources registeredPackageSources = options.PackageSources;	
			
			var packageSource = new PackageSource("http://codeplex.com", "Test");
			registeredPackageSources.Clear();
			registeredPackageSources.Add(packageSource);
			
			bool sectionDeleted = fakeSettings.IsDisabledPackageSourcesSectionDeleted;
			
			Assert.IsTrue(sectionDeleted);
		}
		
		[Test]
		public void PackageSources_OneDisabledPackageSourceAdded_DisabledPackageSourcesSectionSaved()
		{
			CreateSettings();
			CreateOptions(fakeSettings);
			RegisteredPackageSources registeredPackageSources = options.PackageSources;	
			
			var packageSource = new PackageSource("http://codeplex.com", "Test") { IsEnabled = false };
			registeredPackageSources.Clear();
			registeredPackageSources.Add(packageSource);
			
			var expectedSavedPackageSourceSettings = new List<KeyValuePair<string, string>>();
			expectedSavedPackageSourceSettings.Add(new KeyValuePair<string, string>(packageSource.Name, "true"));
			
			IList<KeyValuePair<string, string>> actualSavedPackageSourceSettings = 
				fakeSettings.GetValuesPassedToSetValuesForDisabledPackageSourcesSection();
			Assert.AreEqual(expectedSavedPackageSourceSettings, actualSavedPackageSourceSettings);
		}
		
		[Test]
		public void PackageSources_OneEnabledPackageSourceAdded_DisabledPackageSourcesSectionNotChanged()
		{
			CreateSettings();
			CreateOptions(fakeSettings);
			RegisteredPackageSources registeredPackageSources = options.PackageSources;	
			
			var packageSource = new PackageSource("http://codeplex.com", "Test") { IsEnabled = true };
			registeredPackageSources.Clear();
			registeredPackageSources.Add(packageSource);
			
			bool result = fakeSettings.AnyValuesPassedToSetValuesForDisabledPackageSourcesSection;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsPackageRestoreEnabled_EnabledInSettings_ReturnsTrue()
		{
			CreateSettings();
			EnablePackageRestoreInSettings();
			CreateOptions(fakeSettings);
			
			bool enabled = options.IsPackageRestoreEnabled;
			
			Assert.IsTrue(enabled);
		}
		
		[Test]
		public void IsPackageRestoreEnabled_PackageRestoreNotInSettings_ReturnsFalse()
		{
			CreateOptions();
			
			bool enabled = options.IsPackageRestoreEnabled;
			
			Assert.IsFalse(enabled);
		}
		
		[Test]
		public void IsPackageRestoreEnabled_NotInSettingsOriginallyButSetToTrue_PackageRestoreEnabledInSettings()
		{
			CreateOptions();
			
			options.IsPackageRestoreEnabled = true;
			
			KeyValuePair<string, string> keyPair = fakeSettings.GetValuePassedToSetValueForPackageRestoreSection();
			
			Assert.AreEqual("enabled", keyPair.Key);
			Assert.AreEqual("True", keyPair.Value);
		}
		
		[Test]
		public void IsPackageRestoreEnabled_OriginallyEnabledInSettingsButSetToTrue_PackageRestoreSectionDeletedFromSettings()
		{
			CreateSettings();
			EnablePackageRestoreInSettings();
			CreateOptions(fakeSettings);
			
			options.IsPackageRestoreEnabled = false;
			
			bool deleted = fakeSettings.IsPackageRestoreSectionDeleted;
			Assert.IsTrue(deleted);
		}
	}
}
