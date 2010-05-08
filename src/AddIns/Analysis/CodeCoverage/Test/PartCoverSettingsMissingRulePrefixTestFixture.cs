// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests
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
