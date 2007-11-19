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
			string xml = "<PartCoverReport>\r\n" +
				"\t<file id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\"/>\r\n" +
				"\t<file id=\"2\" url=\"c:\\Projects\\Foo\\FooTestFixture2.cs\"/>\r\n" +
				"\t<file id=\"3\" url=\"c:\\Projects\\Foo\\BarTestFixture.cs\"/>\r\n" +
				"\t<type name=\"Foo.Tests.FooTestFixture1\" asm=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest1\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t<pt visit=\"12\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"1\" />\r\n" +
				"\t\t\t<pt visit=\"4\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" fid=\"1\" />\r\n" +
				"\t\t\t<pt visit=\"0\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" fid=\"1\"/>\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t</type>\r\n" +
				"\t<type name=\"Foo.Tests.FooTestFixture2\" asm=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest2\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t<pt visit=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"2\" />\r\n" +
				"\t\t\t<pt visit=\"10\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" fid=\"2\"/>\r\n" +
				"\t\t\t<pt visit=\"0\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" fid=\"2\" />\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t</type>\r\n" +
				"\t<type name=\"Bar.Tests.FooTestFixture3\" asm=\"Bar.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest3\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t<pt visit=\"5\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"3\" />\r\n" +
				"\t\t\t<pt visit=\"1\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" fid=\"3\" />\r\n" +
				"\t\t\t<pt visit=\"0\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" fid=\"3\" />\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t</type>\r\n" +				
				"</PartCoverReport>";
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
		
		[Test]
		public void TwoModulesInTotal()
		{
			Assert.AreEqual(2, results.Modules.Count);
		}
	}
}
