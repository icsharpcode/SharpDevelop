// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class IPackageExtensionsTests
	{
		FakePackage package;
		
		void CreatePackageWithSummary(string summary)
		{
			package = new FakePackage() { Summary = summary };
		}
		
		[Test]
		public void SummaryOrDescription_PackageHasSummary_ReturnsSummary()
		{
			CreatePackageWithSummary("summary");
			
			string result = package.SummaryOrDescription();
			
			Assert.AreEqual("summary", result);
		}
		
		[Test]
		public void SummaryOrDescription_PackageHasDescriptionButNullSummary_ReturnsDescription()
		{
			CreatePackageWithSummary(null);
			package.Description = "description";
			
			string result = package.SummaryOrDescription();
			
			Assert.AreEqual("description", result);
		}
		
		[Test]
		public void SummaryOrDescription_PackageHasDescriptionButEmptySummary_ReturnsDescription()
		{
			CreatePackageWithSummary(String.Empty);
			package.Description = "description";
			
			string result = package.SummaryOrDescription();
			
			Assert.AreEqual("description", result);
		}
	}
}
