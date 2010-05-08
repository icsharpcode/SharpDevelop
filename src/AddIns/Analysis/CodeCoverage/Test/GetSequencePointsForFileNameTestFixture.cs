// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.CodeCoverage.Tests
{
	[TestFixture]
	public class GetSequencePointsForFileNameTestFixture
	{
		CodeCoverageResults results;
		List<CodeCoverageSequencePoint> fooTestFixtureSequencePoints;
		List<CodeCoverageSequencePoint> barTestFixtureSequencePoints;
		List<CodeCoverageSequencePoint> simpleTestFixtureSequencePoints;
		List<CodeCoverageSequencePoint> nonExistentFileNameSequencePoints;
		
		[SetUp]
		public void SetUpFixture()
		{
			string xml = "<PartCoverReport>\r\n" +
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
			
			results = new CodeCoverageResults(new StringReader(xml));
			fooTestFixtureSequencePoints = results.GetSequencePoints(@"c:\Projects\Foo\FooTestFixture.cs");
			barTestFixtureSequencePoints = results.GetSequencePoints(@"c:\Projects\Foo\BarTestFixture.cs");
			simpleTestFixtureSequencePoints = results.GetSequencePoints(@"c:\Projects\Foo\SimpleTestFixture.cs");
			nonExistentFileNameSequencePoints = results.GetSequencePoints(@"c:\Projects\Foo\NoSuchTestFixture.cs");
		}
		
		[Test]
		public void FooTestFixtureHasSequencePoint()
		{
			Assert.AreEqual(1, fooTestFixtureSequencePoints.Count);
		}
		
		[Test]
		public void BarTestFixtureHasSequencePoint()
		{
			Assert.AreEqual(1, barTestFixtureSequencePoints.Count);
		}
		
		[Test]
		public void SimpleTestFixtureHasSequencePoints()
		{
			Assert.AreEqual(2, simpleTestFixtureSequencePoints.Count);
		}
		
		[Test]
		public void NonExistentFileNameHasNoSequencePoints()
		{
			Assert.AreEqual(0, nonExistentFileNameSequencePoints.Count);
		}		
	}
}
