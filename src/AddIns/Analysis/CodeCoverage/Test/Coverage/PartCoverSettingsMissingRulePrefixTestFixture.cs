// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	/// <summary>
	/// Tests that the PartCoverSettings class does not throw an index out of
	/// range exception when the Rule elements are empty strings.
	/// </summary>
	[TestFixture]
	public class PartCoverSettingsMissingRulePrefixTestFixture
	{
		PartCoverSettings settings;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string xml = "<PartCoverSettings>\r\n" +
						"   <Rule/>\r\n" +
						"   <Rule></Rule>\r\n" +
						"</PartCoverSettings>";
			
			settings = new PartCoverSettings(new StringReader(xml));
		}
		
		[Test]
		public void NoIncludes()
		{
			Assert.AreEqual(0, settings.Include.Count);
		}
		
		[Test]
		public void NoExcludes()
		{
			Assert.AreEqual(0, settings.Exclude.Count);
		}		
	}
}
