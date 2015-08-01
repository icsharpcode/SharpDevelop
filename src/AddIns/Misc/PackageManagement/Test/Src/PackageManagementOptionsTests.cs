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
		SettingsProvider settingsProvider;
		FakePackageManagementProjectService projectService;
		
		[TearDown]
		public void TearDown()
		{
			// This resets SettingsProvider.LoadDefaultSettings.
			TestablePackageManagementOptions.CreateSettingsProvider(fakeSettings, projectService);
		}
		
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
			CreateSettingsProvider(fakeSettings);
			options = new PackageManagementOptions(properties, settingsProvider);
		}
		
		void CreateOptions(Properties properties, ISettingsProvider provider)
		{
			CreateSettings();
			options = new PackageManagementOptions(properties, provider);
		}
		
		void CreateSettingsProvider(FakeSettings fakeSettings)
		{
			projectService = new FakePackageManagementProjectService();
			settingsProvider = TestablePackageManagementOptions.CreateSettingsProvider(fakeSettings, projectService);
		}
		
		void ChangeSettingsReturnedBySettingsProvider()
		{
			fakeSettings = new FakeSettings();
			TestablePackageManagementOptions.ChangeSettingsReturnedBySettingsProvider(fakeSettings);
		}
		
		void CreateOptions(Properties properties, FakeSettings fakeSettings)
		{
			CreateSettingsProvider(fakeSettings);
			options = new PackageManagementOptions(properties, settingsProvider);
		}
		
		void SaveOptions()
		{
			properties.Save();
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
		
		void OpenSolution()
		{
			var helper = new SolutionHelper(@"d:\projects\MyProject\MySolution.sln");
			projectService.FireSolutionOpenedEvent(helper.MSBuildSolution);
		}
		
		void CloseSolution()
		{
			var helper = new SolutionHelper(@"d:\projects\MyProject\MySolution.sln");
			projectService.FireSolutionClosedEvent(helper.MSBuildSolution);
		}
		
		[Test]
		public void PackageSources_OnePackageSourceInSettings_ContainsSinglePackageSourceFromSettingsAndDefaultPackageSource()
		{
			CreateSettings();
			var packageSource = new PackageSource("http://codeplex.com", "Test");
			fakeSettings.AddFakePackageSource(packageSource);
			CreateOptions(fakeSettings);
			
			RegisteredPackageSources actualSources = options.PackageSources;
			
			var expectedSources = new List<PackageSource>();
			expectedSources.Add(packageSource);
				expectedSources.Add(new PackageSource("https://www.nuget.org/api/v2/", "nuget.org"));
			
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
		public void PackageSources_OnePackageSourceAdded_PackageSourceSavedInSettings()
		{
			CreateSettings();
			CreateOptions(fakeSettings);
			RegisteredPackageSources registeredPackageSources = options.PackageSources;
			
			var packageSource = new PackageSource("http://codeplex.com", "Test");
			registeredPackageSources.Clear();
			registeredPackageSources.Add(packageSource);
			
			var expectedSavedPackageSourceSettings = new List<SettingValue>();
			expectedSavedPackageSourceSettings.Add(new SettingValue("Test", "http://codeplex.com", false));
			
			IList<SettingValue> actualSavedPackageSourceSettings = fakeSettings.GetValuesPassedToSetValuesForPackageSourcesSection();
			
			Assert.AreEqual(expectedSavedPackageSourceSettings, actualSavedPackageSourceSettings);
		}
		
		[Test]
		public void PackageSources_OnePackageSourceAdded_PackageSourcesSectionUpdated()
		{
			CreateSettings();
			CreateOptions(fakeSettings);
			RegisteredPackageSources registeredPackageSources = options.PackageSources;	
			
			var packageSource = new PackageSource("http://codeplex.com", "Test");
			registeredPackageSources.Clear();
			registeredPackageSources.Add(packageSource);
			
			IList<SettingValue> settings = fakeSettings.SectionsUpdated[RegisteredPackageSourceSettings.PackageSourcesSectionName];
			
			Assert.AreEqual(1, settings.Count);
			Assert.AreEqual("Test", settings[0].Key);
			Assert.AreEqual("http://codeplex.com", settings[0].Value);
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
			
			var expectedKeyValuePair = new SettingValue("Test", "http://sharpdevelop.com", false);
			SettingValue actualSetting = fakeSettings.GetValuePassedToSetValueForActivePackageSourceSection();
			
			Assert.AreEqual(expectedKeyValuePair, actualSetting);
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
			
			IList<SettingValue> settings = fakeSettings.SectionsUpdated[RegisteredPackageSourceSettings.DisabledPackageSourceSectionName];
			
			Assert.AreEqual(0, settings.Count);
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
			
			var expectedSavedPackageSourceSettings = new List<SettingValue>();
			expectedSavedPackageSourceSettings.Add(new SettingValue(packageSource.Name, "true", false));
			
			IList<SettingValue> actualSavedPackageSourceSettings = 
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
			
			IList<SettingValue> actualSavedPackageSourceSettings = 
				fakeSettings.GetValuesPassedToSetValuesForDisabledPackageSourcesSection();
			Assert.AreEqual(0, actualSavedPackageSourceSettings.Count);
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
		public void IsPackageRestoreEnabled_PackageRestoreNotInSettings_ReturnsTrue()
		{
			CreateOptions();
			
			bool enabled = options.IsPackageRestoreEnabled;
			
			Assert.IsTrue(enabled);
		}
		
		[Test]
		public void IsPackageRestoreEnabled_NotInSettingsOriginallyButSetToTrue_PackageRestoreEnabledInSettings()
		{
			CreateOptions();
			
			options.IsPackageRestoreEnabled = true;
			
			SettingValue setting = fakeSettings.GetValuePassedToSetValueForPackageRestoreSection();
			
			Assert.AreEqual("enabled", setting.Key);
			Assert.AreEqual("True", setting.Value);
		}
		
		[Test]
		public void IsPackageRestoreEnabled_OriginallyEnabledInSettingsButSetToTrue_PackageRestoreSectionIsNotDeletedFromSettings()
		{
			CreateSettings();
			EnablePackageRestoreInSettings();
			CreateOptions(fakeSettings);
			
			options.IsPackageRestoreEnabled = false;
			
			bool deleted = fakeSettings.IsPackageRestoreSectionDeleted;
			Assert.IsFalse(deleted);
		}
		
		[Test]
		public void IsPackageRestoreEnabled_OriginallyEnabledInSettingsButSetToTrue_PackageRestoreIsFalseInSettings()
		{
			CreateSettings();
			EnablePackageRestoreInSettings();
			CreateOptions(fakeSettings);
			
			options.IsPackageRestoreEnabled = false;
			
			SettingValue setting = fakeSettings.GetValuePassedToSetValueForPackageRestoreSection();
			Assert.AreEqual("False", setting.Value);
		}
		
		[Test]
		public void PackageSources_SolutionOpenedAfterInitialPackageSourcesLoaded_ContainsPackageSourceFromSolutionSpecificSettings()
		{
			CreateSettings();
			var packageSource = new PackageSource("https://www.nuget.org/api/v2/", "Official NuGet Gallery");
			fakeSettings.AddFakePackageSource(packageSource);
			CreateOptions(fakeSettings);
			RegisteredPackageSources initialSources = options.PackageSources;
			var expectedInitialSources = new List<PackageSource>();
			expectedInitialSources.Add(packageSource);
			ChangeSettingsReturnedBySettingsProvider();
			packageSource = new PackageSource("https://www.nuget.org/api/v2/", "Official NuGet Gallery");
			fakeSettings.AddFakePackageSource(packageSource);
			var expectedSources = new List<PackageSource>();
			expectedSources.Add(packageSource);
			packageSource = new PackageSource("http://codeplex.com", "ProjectSource");
			fakeSettings.AddFakePackageSource(packageSource);
			expectedSources.Add(packageSource);
			OpenSolution();
			
			RegisteredPackageSources actualSources = options.PackageSources;
			
			Assert.AreEqual(expectedInitialSources, initialSources);
			Assert.AreEqual(expectedSources, actualSources);
		}
		
		[Test]
		public void PackageSources_SolutionClosedAfterInitialPackageSourcesLoaded_PackageSourcesReloaded()
		{
			CreateSettings();
			var packageSource = new PackageSource("https://www.nuget.org/api/v2/", "Official NuGet Gallery");
			fakeSettings.AddFakePackageSource(packageSource);
			var expectedInitialSources = new List<PackageSource>();
			expectedInitialSources.Add(packageSource);
			packageSource = new PackageSource("http://projectsource.org", "ProjectSource");
			fakeSettings.AddFakePackageSource(packageSource);
			expectedInitialSources.Add(packageSource);
			OpenSolution();
			CreateOptions(fakeSettings);
			RegisteredPackageSources initialSources = options.PackageSources;
			ChangeSettingsReturnedBySettingsProvider();
			packageSource = new PackageSource("https://www.nuget.org/api/v2/", "Official NuGet Gallery");
			fakeSettings.AddFakePackageSource(packageSource);
			var expectedSources = new List<PackageSource>();
			expectedSources.Add(packageSource);
			CloseSolution();
			
			RegisteredPackageSources actualSources = options.PackageSources;
			
			Assert.AreEqual(expectedInitialSources, initialSources);
			Assert.AreEqual(expectedSources, actualSources);
		}
		
		[Test]
		public void PackageSources_SolutionClosedAfterInitialPackageSourcesLoaded_ActivePackageSourceReloaded()
		{
			CreateSettings();
			var packageSource = new PackageSource("https://www.nuget.org/api/v2/", "Official NuGet Gallery");
			fakeSettings.AddFakePackageSource(packageSource);
			var expectedInitialSources = new List<PackageSource>();
			expectedInitialSources.Add(packageSource);
			var initialActivePackageSource = new PackageSource("http://projectsource.org", "ProjectSource");
			fakeSettings.AddFakePackageSource(initialActivePackageSource);
			fakeSettings.SetFakeActivePackageSource(initialActivePackageSource);
			expectedInitialSources.Add(initialActivePackageSource);
			OpenSolution();
			CreateOptions(fakeSettings);
			RegisteredPackageSources actualInitialPackageSources = options.PackageSources;
			PackageSource actualInitialActivePackageSource = options.ActivePackageSource;
			ChangeSettingsReturnedBySettingsProvider();
			var expectedActivePackageSource = new PackageSource("https://www.nuget.org/api/v2/", "Official NuGet Gallery");
			fakeSettings.SetFakeActivePackageSource(expectedActivePackageSource);
			fakeSettings.AddFakePackageSource(expectedActivePackageSource);
			CloseSolution();
			
			PackageSource actualSource = options.ActivePackageSource;
			
			Assert.AreEqual(initialActivePackageSource, actualInitialActivePackageSource);
			Assert.AreEqual(expectedActivePackageSource, actualSource);
			Assert.AreEqual(expectedInitialSources, actualInitialPackageSources);
			Assert.AreEqual(new PackageSource[] { expectedActivePackageSource }, options.PackageSources);
		}
		
		[Test]
		public void PackageSources_ReadOnlyNuGetConfigFile_DoesNotThrowException()
		{
			var settings = new FakeReadOnlySettings();
			CreateOptions(settings);
			
			int count = 0;
			Assert.DoesNotThrow(() => count = options.PackageSources.Count);
			Assert.AreEqual(1, count);
			Assert.AreEqual(options.PackageSources[0], new PackageSource("https://www.nuget.org/api/v2/", "nuget.org"));
		}
		
		[Test]
		public void PackageSources_UpdateActivePackageSourceWhenNuGetConfigAccessIsUnauthorized_DoesNotThrowException()
		{
			CreateProperties();
			fakeSettings = new FakeReadOnlySettings();
			CreateSettingsProvider(fakeSettings);
			SettingsProvider.LoadDefaultSettings = (fileSystem, configFile, machineSettings) => {
				throw new UnauthorizedAccessException();
			};
			CreateOptions(properties, settingsProvider);
			
			var packageSource = new PackageSource("http://test.com");
			Assert.DoesNotThrow(() => options.ActivePackageSource = packageSource);
		}
	}
}
