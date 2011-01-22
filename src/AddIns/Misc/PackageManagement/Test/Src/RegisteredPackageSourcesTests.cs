// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.Core;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class RegisteredPackageSourcesTests
	{
		RegisteredPackageSources registeredPackageSources;
		PackageManagementOptions options;
		Properties properties;
		
		void CreateProperties()
		{
			properties = new Properties();
		}
		
		void CreateRegisteredPackageSources()
		{
			CreateProperties();
			CreateRegisteredPackageSources(properties);
		}
		
		void CreateRegisteredPackageSources(Properties properties)
		{
			options = new PackageManagementOptions(properties);
			registeredPackageSources = options.PackageSources;
		}
		
		[Test]
		public void Constructor_NoExistingSavedProperties_NewInstanceContainsDefaultPackageSource()
		{
			CreateRegisteredPackageSources();
			
			List<PackageSource> expectedSources = new List<PackageSource>();
			expectedSources.Add(RegisteredPackageSources.DefaultPackageSource);
			
			CollectionAssert.AreEqual(expectedSources, registeredPackageSources);
		}
		
		[Test]
		public void Constructor_OnePackageSourceInSavedProperties_NewInstanceContainsSinglePackageSourceFromProperties()
		{
			CreateProperties();
			CreateRegisteredPackageSources(properties);
			
			var packageSource = new PackageSource("http://codeplex.com", "Test");
			registeredPackageSources.Clear();
			registeredPackageSources.Add(packageSource);
			
			CreateRegisteredPackageSources(properties);
			
			List<PackageSource> expectedSources = new List<PackageSource>();
			expectedSources.Add(packageSource);
			
			Assert.AreEqual(expectedSources, registeredPackageSources);
		}
	}
}
