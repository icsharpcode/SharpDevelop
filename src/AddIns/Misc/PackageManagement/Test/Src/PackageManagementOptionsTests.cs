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
		
		void CreateOptions()
		{
			CreateProperties();
			CreateOptions(properties);
		}
		
		void CreateProperties()
		{
			properties = new Properties();
		}
		
		void CreateOptions(Properties properties)
		{
			options = new PackageManagementOptions(properties);
		}
		
		void SaveOptions()
		{
			StringBuilder xml = new StringBuilder();
			StringWriter stringWriter = new StringWriter(xml);
			using (XmlTextWriter writer = new XmlTextWriter(stringWriter)) {
				properties.WriteProperties(writer);
			}
		}
		
		[Test]
		public void ActivePackageSource_NoInitialPropertiesSet_ReturnsNull()
		{
			CreateOptions();
			PackageSource actualSource = options.ActivePackageSource;
			Assert.IsNull(options.ActivePackageSource);
		}
		
		[Test]
		public void ActivePackageSource_ActivePackageSourceNonNullWhenPropertiesSaved_OnReloadingPropertiesActivatePackageSourceRemembered()
		{
			CreateOptions();
			var expectedSource = new PackageSource("http://source-url", "Test");
			options.PackageSources.Add(expectedSource);
			options.ActivePackageSource = expectedSource;
			CreateOptions(properties);
			
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
		public void ActivePackageSource_SaveActivePackageSource_DoesNotThrowSerializationError()
		{
			CreateOptions();
			var source = new PackageSource("http://source-url", "Test");
			options.ActivePackageSource = source;
			
			Assert.DoesNotThrow(() => SaveOptions());
		}
		
		[Test]
		public void ActivePackageSource_SaveDefaultNullActivePackageSource_DoesNotThrowSerializationError()
		{
			CreateOptions();
			var source = options.ActivePackageSource;
			
			Assert.DoesNotThrow(() => SaveOptions());
			Assert.IsNull(source);
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
			var recentPackage = new RecentPackageInfo("id", new Version(1, 0));
			options.RecentPackages.Add(recentPackage);
			
			Assert.DoesNotThrow(() => SaveOptions());
		}
	}
}
