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
				"\t<File id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\"/>\r\n" +
				"\t<File id=\"2\" url=\"c:\\Projects\\Foo\\FooTestFixture2.cs\"/>\r\n" +
				"\t<File id=\"3\" url=\"c:\\Projects\\Foo\\BarTestFixture.cs\"/>\r\n" +
				"\t<Assembly id=\"1\" name=\"Foo.Tests\" module=\"C:\\Projects\\Test\\bin\\Foo.Tests.DLL\" domain=\"test-domain-Foo.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"\t<Assembly id=\"2\" name=\"Bar.Tests\" module=\"C:\\Projects\\Test\\bin\\Bar.Tests.DLL\" domain=\"test-domain-Bar.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"\t<Type name=\"Foo.Tests.FooTestFixture1\" asmref=\"1\">\r\n" +
				"\t\t<Method name=\"SimpleTest1\">\r\n" +
				"\t\t\t<pt visit=\"12\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"1\" />\r\n" +
				"\t\t\t<pt visit=\"4\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" fid=\"1\" />\r\n" +
				"\t\t\t<pt visit=\"0\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" fid=\"1\"/>\r\n" +
				"\t\t</Method>\r\n" +
				"\t</Type>\r\n" +
				"\t<Type name=\"Foo.Tests.FooTestFixture2\" asmref=\"1\">\r\n" +
				"\t\t<Method name=\"SimpleTest2\">\r\n" +
				"\t\t\t<pt visit=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"2\" />\r\n" +
				"\t\t\t<pt visit=\"10\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" fid=\"2\"/>\r\n" +
				"\t\t\t<pt visit=\"0\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" fid=\"2\" />\r\n" +
				"\t\t</Method>\r\n" +
				"\t</Type>\r\n" +
				"\t<Type name=\"Bar.Tests.FooTestFixture3\" asmref=\"2\">\r\n" +
				"\t\t<Method name=\"SimpleTest3\">\r\n" +
				"\t\t\t<pt visit=\"5\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"3\" />\r\n" +
				"\t\t\t<pt visit=\"1\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" fid=\"3\" />\r\n" +
				"\t\t\t<pt visit=\"0\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" fid=\"3\" />\r\n" +
				"\t\t</Method>\r\n" +
				"\t</Type>\r\n" +				
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
