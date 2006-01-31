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
	public class ModuleVisitedSequencePointsTestFixture
	{
		CodeCoverageModule fooModule;
		CodeCoverageModule barModule;
		CodeCoverageResults results;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string xml = "<coverage>\r\n" +
				"\t<module name=\"C:\\Projects\\Foo.Tests\\bin\\Debug\\Foo.Tests.dll\" assembly=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest1\" class=\"Foo.Tests.FooTestFixture1\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"12\" line=\"20\" column=\"3\" endline=\"20\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"4\" line=\"21\" column=\"13\" endline=\"21\" endcolumn=\"32\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"0\" line=\"24\" column=\"3\" endline=\"24\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t\t<method name=\"SimpleTest2\" class=\"Foo.Tests.FooTestFixture2\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"20\" column=\"3\" endline=\"20\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture2.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"10\" line=\"21\" column=\"13\" endline=\"21\" endcolumn=\"32\" document=\"c:\\Projects\\Foo\\FooTestFixture2.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"0\" line=\"24\" column=\"3\" endline=\"24\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture2.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t</module>\r\n" +
				"\t<module name=\"C:\\Projects\\Foo.Tests\\bin\\Debug\\Bar.Tests.dll\" assembly=\"Bar.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest3\" class=\"Bar.Tests.FooTestFixture3\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"5\" line=\"20\" column=\"3\" endline=\"20\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\BarTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"21\" column=\"13\" endline=\"21\" endcolumn=\"32\" document=\"c:\\Projects\\Foo\\BarTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"0\" line=\"24\" column=\"3\" endline=\"24\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\BarTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t</module>\r\n" +				
				"</coverage>";
			results = new CodeCoverageResults(new StringReader(xml));
			fooModule = results.Modules[0];
			barModule = results.Modules[1];
		}

		[Test]
		public void FooModuleVisitedCount()
		{
			Assert.AreEqual(4, fooModule.VisitedSequencePointsCount);
		}
		
		[Test]
		public void FooModuleNotVisitedCount()
		{
			Assert.AreEqual(2, fooModule.NotVisitedSequencePointsCount);
		}
		
		[Test]
		public void BarModuleVisitedCount()
		{
			Assert.AreEqual(2, barModule.VisitedSequencePointsCount);
		}
		
		[Test]
		public void BarModuleNotVisitedCount()
		{
			Assert.AreEqual(1, barModule.NotVisitedSequencePointsCount);
		}
	}
}
