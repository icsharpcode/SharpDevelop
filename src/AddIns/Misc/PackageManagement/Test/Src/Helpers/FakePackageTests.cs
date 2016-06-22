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
