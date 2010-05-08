// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.IO;

namespace ICSharpCode.CodeCoverage.Tests
{
	[TestFixture]
	public class CodeCoverageResultsWithEmptyMethodElementTestFixture
	{
		CodeCoverageModule module;
		CodeCoverageResults results;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string xml = "<PartCoverReport date=\"2008-07-10T02:59:13.7198656+01:00\">\r\n" +
				"    <File id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"    <Assembly id=\"1\" name=\"Foo.Tests\" module=\"C:\\Projects\\Test\\Foo.Tests\\bin\\Foo.Tests.DLL\" domain=\"test-domain-Foo.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"    <Type asmref=\"1\" name=\"Foo.Tests.FooTestFixture\" flags=\"1232592\">\r\n" +
				"        <Method name=\"SimpleTest\" sig=\"void  ()\" bodysize=\"42\" flags=\"2182\" iflags=\"0\" />\r\n" +
				"    </Type>\r\n" +
				"</PartCoverReport>";
			results = new CodeCoverageResults(new StringReader(xml));
			if (results.Modules.Count > 0) {
				module = results.Modules[0];
			}
		}
		
		[Test]
		public void NoModules()
		{
			Assert.AreEqual(0, results.Modules.Count);
		}
	}
}
