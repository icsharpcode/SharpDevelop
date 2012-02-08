// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class FakePackageTests
	{
		FakePackage lhs;
		FakePackage rhs;
		
		[SetUp]
		public void Init()
		{
			lhs = new FakePackage();
			rhs = new FakePackage();
		}
		
		[Test]
		public void Equals_IdAndVersionAreSame_ReturnsTrue()
		{
			lhs.Id = "Test";
			lhs.Version = new SemanticVersion(1, 0, 0, 0);
			rhs.Id = "Test";
			rhs.Version = new SemanticVersion(1, 0, 0, 0);
			
			bool result = lhs.Equals(rhs);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Equals_DifferentIds_ReturnsFalse()
		{
			lhs.Id = "id 1";
			lhs.Version = new SemanticVersion(1, 0, 0, 0);
			rhs.Id = "id 2";
			rhs.Version = new SemanticVersion(1, 0, 0, 0);
			
			bool result = lhs.Equals(rhs);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Equals_DifferentVersions_ReturnsFalse()
		{
			lhs.Id = "test";
			lhs.Version = new SemanticVersion(1, 0, 0, 0);
			rhs.Id = "test";
			rhs.Version = new SemanticVersion(2, 2, 0, 0);
			
			bool result = lhs.Equals(rhs);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Equals_NullPassed_ReturnsFalse()
		{
			bool result = lhs.Equals(null);
			
			Assert.IsFalse(result);
		}
	}
}
