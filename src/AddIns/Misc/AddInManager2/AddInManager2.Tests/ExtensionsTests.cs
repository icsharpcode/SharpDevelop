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
using NuGet;
using NUnit;
using NUnit.Framework;

namespace ICSharpCode.AddInManager2.Tests
{
	public class ExtensionsTests
	{
		[Test]
		public void SemanticVersionConvertShortVersion()
		{
			SemanticVersion semVersion = new SemanticVersion("2.1");
			Version convertedVersion = semVersion.ToVersion();
			Assert.That(convertedVersion.Major, Is.EqualTo(2));
			Assert.That(convertedVersion.Minor, Is.EqualTo(1));
			Assert.That(convertedVersion.Build, Is.EqualTo(-1));
			Assert.That(convertedVersion.Revision, Is.EqualTo(-1));
		}
		
		[Test]
		public void SemanticVersionConvertShortVersionWithZeros()
		{
			SemanticVersion semVersion = new SemanticVersion("2.1.0.0");
			Version convertedVersion = semVersion.ToVersion();
			Assert.That(convertedVersion.Major, Is.EqualTo(2));
			Assert.That(convertedVersion.Minor, Is.EqualTo(1));
			Assert.That(convertedVersion.Build, Is.EqualTo(0));
			Assert.That(convertedVersion.Revision, Is.EqualTo(0));
		}
		
		[Test]
		public void SemanticVersionConvertFullVersion()
		{
			SemanticVersion semVersion = new SemanticVersion("2.1.3.4");
			Version convertedVersion = semVersion.ToVersion();
			Assert.That(convertedVersion.Major, Is.EqualTo(2));
			Assert.That(convertedVersion.Minor, Is.EqualTo(1));
			Assert.That(convertedVersion.Build, Is.EqualTo(3));
			Assert.That(convertedVersion.Revision, Is.EqualTo(4));
		}
		
		[Test]
		public void SemanticVersionConvertFullVersionWithSpecial()
		{
			SemanticVersion semVersion = new SemanticVersion(new Version("2.1.3.4"), "56789");
			Version convertedVersion = semVersion.ToVersion();
			Assert.That(convertedVersion.Major, Is.EqualTo(2));
			Assert.That(convertedVersion.Minor, Is.EqualTo(1));
			Assert.That(convertedVersion.Build, Is.EqualTo(3));
			Assert.That(convertedVersion.Revision, Is.EqualTo(4));
		}
	}
}
