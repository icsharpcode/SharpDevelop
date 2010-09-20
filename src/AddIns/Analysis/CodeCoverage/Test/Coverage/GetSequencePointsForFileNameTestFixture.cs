// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class GetSequencePointsForFileNameTestFixture : CodeCoverageResultsTestsBase
	{
		[SetUp]
		public void SetUpFixture()
		{
			string xml = 
				"<PartCoverReport>\r\n" +
				"  <File id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"  <File id=\"2\" url=\"c:\\Projects\\Foo\\SimpleTestFixture.cs\" />\r\n" +
				"  <File id=\"3\" url=\"c:\\Projects\\Foo\\BarTestFixture.cs\" />\r\n" +
				"  <Assembly id=\"1\" name=\"Foo.Tests\" module=\"C:\\Projects\\Test\\Foo.Tests\\bin\\Foo.Tests.DLL\" domain=\"test-domain-Foo.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"  <Assembly id=\"2\" name=\"Bar.Tests\" module=\"C:\\Projects\\Test\\Foo.Tests\\bin\\Bar.Tests.DLL\" domain=\"test-domain-Bar.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"  <Type name=\"Foo.Tests.FooTestFixture\" asmref=\"1\">\r\n" +
				"    <Method name=\"FooTest\">\r\n" +
				"      <pt visit=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"1\" />\r\n" +
				"    </Method>\r\n" +
				"  </Type>\r\n" +
				"  <Type name=\"Foo.Tests.SimpleTestFixture2\" asmref=\"1\">\r\n" +
				"    <Method name=\"SimpleTest2\">\r\n" +
				"      <pt visit=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"2\" />\r\n" +
				"    </Method>\r\n" +
				"  </Type>\r\n" +
				"  <Type name=\"Foo.Tests.SimpleTestFixture3\" asmref=\"1\">\r\n" +
				"    <Method name=\"SimpleTest3\">\r\n" +
				"      <pt visit=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"2\" />\r\n" +
				"    </Method>\r\n" +
				"  </Type>\r\n" +
				"  <Type name=\"Foo.Tests.BarTestFixture\" asmref=\"2\">\r\n" +
				"    <Method name=\"SimpleTest2\">\r\n" +
				"      <pt visit=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"3\" />\r\n" +
				"    </Method>\r\n" +
				"  </Type>\r\n" +
				"</PartCoverReport>";
			
			base.CreateCodeCoverageResults(xml);
		}
		
		[Test]
		public void SequencePointsCount_FooTestFixture_HasSequencePoint()
		{
			List<CodeCoverageSequencePoint> fooTestFixtureSequencePoints = 
				results.GetSequencePoints(@"c:\Projects\Foo\FooTestFixture.cs");
			int count = fooTestFixtureSequencePoints.Count;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void SequencePointsCount_BarTestFixture_HasSequencePoint()
		{
			List<CodeCoverageSequencePoint> barTestFixtureSequencePoints = 
				results.GetSequencePoints(@"c:\Projects\Foo\BarTestFixture.cs");
			int count = barTestFixtureSequencePoints.Count;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void SequencePointsCount_SimpleTestFixture_HasSequencePoints()
		{
			List<CodeCoverageSequencePoint> simpleTestFixtureSequencePoints = 
				results.GetSequencePoints(@"c:\Projects\Foo\SimpleTestFixture.cs");
			int count = simpleTestFixtureSequencePoints.Count;
			Assert.AreEqual(2, count);
		}
		
		[Test]
		public void SequencePointsCount_NonExistentFileName_HasNoSequencePoints()
		{
			List<CodeCoverageSequencePoint> nonExistentFileNameSequencePoints = 
				results.GetSequencePoints(@"c:\Projects\Foo\NoSuchTestFixture.cs");
			int count = nonExistentFileNameSequencePoints.Count;
			Assert.AreEqual(0, count);
		}		
	}
}
