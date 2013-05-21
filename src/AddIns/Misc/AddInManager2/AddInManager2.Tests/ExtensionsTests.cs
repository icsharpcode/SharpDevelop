// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
