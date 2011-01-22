// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageSourceViewModelTests
	{
		PackageSourceViewModel viewModel;
		PackageSource packageSource;
		
		void CreatePackageSource()
		{
			CreatePackageSource("http://sharpdevelop.codeplex.com", "Test");
		}
		
		void CreatePackageSource(string source, string name)
		{
			packageSource = new PackageSource(source, name);
		}
		
		void CreatePackageSourceWithName(string  name)
		{
			CreatePackageSource("http://sharpdevelop.codeplex.com", name);
		}
		
		void  CreatePackageSourceWithSourceUrl(string  sourceUrl)
		{
			CreatePackageSource(sourceUrl, "Test");
		}
		
		void CreateViewModel(PackageSource packageSource)
		{
			viewModel = new PackageSourceViewModel(packageSource);
		}
		
		[Test]
		public void Name_InstanceCreatedWithRegisteredPackageSource_MatchesRegisteredPackageSourceName()
		{
			CreatePackageSourceWithName("Test");
			CreateViewModel(packageSource);
			
			Assert.AreEqual("Test", viewModel.Name);
		}
		
		[Test]
		public void Name_Changed_NamePropertyIsChanged()
		{
			CreatePackageSourceWithName("Test");
			CreateViewModel(packageSource);
			viewModel.Name = "changed";
			
			Assert.AreEqual("changed", viewModel.Name);
		}
		
		[Test]
		public void SourceUrl_InstanceCreatedWithRegisteredPackageSource_MatchesRegisteredPackageSourceSourceUrl()
		{
			CreatePackageSourceWithSourceUrl("Test-url");
			CreateViewModel(packageSource);
			
			Assert.AreEqual("Test-url", viewModel.SourceUrl);
		}
		
		[Test]
		public void Source_Changed_SourcePropertyIsChanged()
		{
			CreatePackageSourceWithSourceUrl("source-url");
			CreateViewModel(packageSource);
			viewModel.SourceUrl = "changed";
			
			Assert.AreEqual("changed", viewModel.SourceUrl);
		}
	}
}
