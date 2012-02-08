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
		
		[Test]
		public void PackageSources_OnePackageSourceInSettings_ContainsSinglePackageSourceFromSettings()
		{
			CreateSettings();
			var packageSource = new PackageSource("http://codeplex.com", "Test");
			fakeSettings.AddFakePackageSource(packageSource);
			CreateOptions(fakeSettings);
			
			var actualSources = options.PackageSources;
			
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
			
			var actualPackageSources = options.PackageSources;
			
			CollectionAssert.AreEqual(expectedSources, actualPackageSources);
		}
		
		[Test]
		public void PackageSources_NoPackageSourceInSavedSettings_DefaultPackageSourceAddedToSettings()
		{
			CreateSettings();
			CreateOptions(fakeSettings);
			
			var packageSources = options.PackageSources;
			
			var defaultSource = RegisteredPackageSources.DefaultPackageSource;
			
			var expectedSavedPackageSourceSettings = new List<KeyValuePair<string, string>>();
			string name = defaultSource.Name;
			string sourceUrl = defaultSource.Source;
			expectedSavedPackageSourceSettings.Add(new KeyValuePair<string, string>(name, sourceUrl));
			
			var actualSavedPackageSourceSettings = fakeSettings.GetValuesPassedToSetValuesForPackageSourcesSection();
			
			Assert.AreEqual(expectedSavedPackageSourceSettings, actualSavedPackageSourceSettings);
		}
		
		[Test]
		public void PackageSources_OnePackageSourceAdded_PackageSourceSavedInSettings()
		{
			CreateSettings();
			CreateOptions(fakeSettings);
			var registeredPackageSources = options.PackageSources;
			
			var packageSource = new PackageSource("http://codeplex.com", "Test");
			registeredPackageSources.Clear();
			registeredPackageSources.Add(packageSource);
			
			var expectedSavedPackageSourceSettings = new List<KeyValuePair<string, string>>();
			expectedSavedPackageSourceSettings.Add(new KeyValuePair<string, string>("Test", "http://codeplex.com"));
			
			var actualSavedPackageSourceSettings = fakeSettings.GetValuesPassedToSetValuesForPackageSourcesSection();
			
			Assert.AreEqual(expectedSavedPackageSourceSettings, actualSavedPackageSourceSettings);
		}
		
		[Test]
		public void PackageSources_OnePackageSourceAdded_PackageSourcesSectionDeletedFromSettings()
		{
			CreateSettings();
			CreateOptions(fakeSettings);
			var registeredPackageSources = options.PackageSources;	
			
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
			var actualKeyValuePair = fakeSettings.GetValuePassedToSetValueForActivePackageSourceSection();
			
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
			
			var activeSource = options.ActivePackageSource;
			
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
			
			var recentPackages = options.RecentPackages;
			
			var expectedRecentPackages = new RecentPackageInfo[] {
				new RecentPackageInfo(package)
			};
			
			RecentPackageInfoCollectionAssert.AreEqual(expectedRecentPackages, recentPackages);
		}
		
		[Test]
		public void RecentPackages_SaveRecentPackages_DoesNotThrowInvalidOperationException()
		{
			CreateOptions();
			var recentPackage = AddRecentPackageToOptions("id", "1.0");
			
			Assert.DoesNotThrow(() => SaveOptions());
		}
		
		[Test]
		public void ActivePackageSource_AggregatePackageSourceIsActivePackageSourceInSettings_ReturnsAggregatePackageSource()
		{
			CreateSettings();
			var expectedPackageSource = RegisteredPackageSourceSettings.AggregatePackageSource;
			fakeSettings.SetFakeActivePackageSource(expectedPackageSource);
			CreateOptions(fakeSettings);
			
			var activePackageSource = options.ActivePackageSource;
			
			Assert.AreEqual(expectedPackageSource, activePackageSource);
		}
	}
}
