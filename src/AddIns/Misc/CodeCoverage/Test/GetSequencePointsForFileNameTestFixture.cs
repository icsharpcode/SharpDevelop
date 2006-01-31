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
			string xml = "<coverage>\r\n" +
				"\t<module name=\"C:\\Projects\\Foo.Tests\\bin\\Debug\\Foo.Tests.dll\" assembly=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"FooTest\" class=\"Foo.Tests.FooTestFixture\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"20\" column=\"3\" endline=\"20\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t\t<method name=\"SimpleTest2\" class=\"Foo.Tests.SimpleTestFixture2\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"20\" column=\"3\" endline=\"20\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\SimpleTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t\t<method name=\"SimpleTest3\" class=\"Foo.Tests.SimpleTestFixture3\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"20\" column=\"3\" endline=\"20\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\SimpleTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t</module>\r\n" +
				"\t<module name=\"C:\\Projects\\Foo.Tests\\bin\\Debug\\Bar.Tests.dll\" assembly=\"Bar.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest2\" class=\"Foo.Tests.BarTestFixture\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"20\" column=\"3\" endline=\"20\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\BarTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t</module>\r\n" +
				"</coverage>";
			
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
