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
				"\t<file id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t<file id=\"2\" url=\"c:\\Projects\\Foo\\SimpleTestFixture.cs\" />\r\n" +
				"\t<file id=\"3\" url=\"c:\\Projects\\Foo\\BarTestFixture.cs\" />\r\n" +
				"\t<type name=\"Foo.Tests.FooTestFixture\" asm=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"FooTest\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t\t<pt visit=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"1\" />\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t</type>\r\n" +
				"\t<type name=\"Foo.Tests.SimpleTestFixture2\" asm=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest2\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t\t\t<pt visit=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"2\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t</type>\r\n" +
				"\t<type name=\"Foo.Tests.SimpleTestFixture3\" asm=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest3\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t\t<pt visit=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"2\" />\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t</type>\r\n" +
				"\t<type name=\"Foo.Tests.BarTestFixture\" asm=\"Bar.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest2\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t\t<pt visit=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"3\" />\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t</type>\r\n" +
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
