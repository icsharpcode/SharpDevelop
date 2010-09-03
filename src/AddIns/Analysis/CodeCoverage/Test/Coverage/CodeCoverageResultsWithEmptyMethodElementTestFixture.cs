// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.IO;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class CodeCoverageResultsWithEmptyMethodElementTestFixture : CodeCoverageResultsTestsBase
	{	
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string xml = 
				"<PartCoverReport date=\"2008-07-10T02:59:13.7198656+01:00\">\r\n" +
				"    <File id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"    <Assembly id=\"1\" name=\"Foo.Tests\" module=\"C:\\Projects\\Test\\Foo.Tests\\bin\\Foo.Tests.DLL\" domain=\"test-domain-Foo.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"    <Type asmref=\"1\" name=\"Foo.Tests.FooTestFixture\" flags=\"1232592\">\r\n" +
				"        <Method name=\"SimpleTest\" sig=\"void  ()\" bodysize=\"42\" flags=\"2182\" iflags=\"0\" />\r\n" +
				"    </Type>\r\n" +
				"</PartCoverReport>";
			
			base.CreateCodeCoverageResults(xml);
		}
		
		[Test]
		public void ModulesCount_MethodWithNoSequencePointsInModule_ReturnsOneModule()
		{
			int count = results.Modules.Count;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void MethodUnvisitedCodeLength_MethodWithNoSequencePoints_ReturnsBodySize()
		{
			int unvisitedCodeLength = FirstModuleFirstMethod.GetUnvisitedCodeLength();
			int expectedUnvisitedCodeLength = 42;
			Assert.AreEqual(expectedUnvisitedCodeLength, unvisitedCodeLength);
		}
	}
}
