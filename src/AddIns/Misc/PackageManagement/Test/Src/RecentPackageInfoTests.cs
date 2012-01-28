// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class RecentPackageInfoTests
	{
		[Test]
		public void ToString_IdAndVersionSpecified_ContainsIdAndVersion()
		{
			var recentPackageInfo = new RecentPackageInfo("id", new SemanticVersion("1.0"));
			
			string actual = recentPackageInfo.ToString();
						
			string expected = "[RecentPackageInfo Id=id, Version=1.0]";
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void IsMatch_PackageWithSameIdAndVersionPassed_ReturnsTrue()
		{
			string id = "id";
			var version = new SemanticVersion(1, 0, 0, 0);
			var recentPackageInfo = new RecentPackageInfo(id, version);
			var package = new FakePackage(id);
			package.Version = version;
			
			bool result = recentPackageInfo.IsMatch(package);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsMatch_PackageWithSameIdButDifferentVersionPassed_ReturnsFalse()
		{
			string id = "id";
			var version = new SemanticVersion(1, 0, 0, 0);
			var recentPackageInfo = new RecentPackageInfo(id, version);
			var package = new FakePackage(id);
			package.Version = new SemanticVersion(2, 0, 0, 0);
			
			bool result = recentPackageInfo.IsMatch(package);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsMatch_PackageWithDifferentIdButSameVersionPassed_ReturnsFalse()
		{
			var version = new SemanticVersion(1, 0, 0, 0);
			var recentPackageInfo = new RecentPackageInfo("id", version);
			var package = new FakePackage("different-id");
			package.Version = version;
			
			bool result = recentPackageInfo.IsMatch(package);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Version_SerializeThenDeserializeRecentPackageInfoInPropertiesObject_ReturnsSameValueAfterDeserialization()
		{
			var version = new SemanticVersion(1, 0, 0, 0);
			var recentPackageInfo = new RecentPackageInfo("id", version);
			var properties = new Properties();
			properties.Set<RecentPackageInfo>("RecentPackageInfo", recentPackageInfo);
			
			var xml = new StringBuilder();
			var stringWriter = new StringWriter(xml);
			var writer = new XmlTextWriter(stringWriter);
			properties.Save(writer);
			
			var stringReader = new StringReader(xml.ToString());
			var reader = new XmlTextReader(stringReader);
			properties = Properties.Load(reader);
			
			var deserializedRecentPackageInfo = properties.Get<RecentPackageInfo>("RecentPackageInfo", null);
			
			Assert.AreEqual(recentPackageInfo.Version, deserializedRecentPackageInfo.Version);
		}
	}
}
