// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.IO;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class ModuleVisitedSequencePointsTestFixture : CodeCoverageResultsTestsBase
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string xml = 
				"<PartCoverReport>\r\n" +
				"\t<File id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\"/>\r\n" +
				"\t<File id=\"2\" url=\"c:\\Projects\\Foo\\FooTestFixture2.cs\"/>\r\n" +
				"\t<File id=\"3\" url=\"c:\\Projects\\Foo\\BarTestFixture.cs\"/>\r\n" +
				"\t<Assembly id=\"1\" name=\"Foo.Tests\" module=\"C:\\Projects\\Test\\bin\\Foo.Tests.DLL\" domain=\"test-domain-Foo.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"\t<Assembly id=\"2\" name=\"Bar.Tests\" module=\"C:\\Projects\\Test\\bin\\Bar.Tests.DLL\" domain=\"test-domain-Bar.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"\t<Type name=\"Foo.Tests.FooTestFixture1\" asmref=\"1\">\r\n" +
				"\t\t<Method name=\"SimpleTest1\">\r\n" +
				"\t\t\t<pt visit=\"12\" len=\"2\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"1\" />\r\n" +
				"\t\t\t<pt visit=\"4\" len=\"3\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" fid=\"1\" />\r\n" +
				"\t\t\t<pt visit=\"0\" len=\"1\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" fid=\"1\"/>\r\n" +
				"\t\t</Method>\r\n" +
				"\t</Type>\r\n" +
				"\t<Type name=\"Foo.Tests.FooTestFixture2\" asmref=\"1\">\r\n" +
				"\t\t<Method name=\"SimpleTest2\">\r\n" +
				"\t\t\t<pt visit=\"1\" len=\"3\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"2\" />\r\n" +
				"\t\t\t<pt visit=\"10\" len=\"1\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" fid=\"2\"/>\r\n" +
				"\t\t\t<pt visit=\"0\" len=\"2\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" fid=\"2\" />\r\n" +
				"\t\t</Method>\r\n" +
				"\t</Type>\r\n" +
				"\t<Type name=\"Bar.Tests.FooTestFixture3\" asmref=\"2\">\r\n" +
				"\t\t<Method name=\"SimpleTest3\">\r\n" +
				"\t\t\t<pt visit=\"5\" len=\"6\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" fid=\"3\" />\r\n" +
				"\t\t\t<pt visit=\"1\" len=\"5\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" fid=\"3\" />\r\n" +
				"\t\t\t<pt visit=\"0\" len=\"4\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" fid=\"3\" />\r\n" +
				"\t\t</Method>\r\n" +
				"\t</Type>\r\n" +
				"</PartCoverReport>";
			
			base.CreateCodeCoverageResults(xml);
		}
		
		CodeCoverageModule FooModule {
			get { return FirstModule; }
		}

		CodeCoverageModule BarModule {
			get { return SecondModule; }
		}
		
		[Test]
		public void ModuleGetVisitedCodeLength_FooModule_ReturnsTotalLengthOfAllVisitedMethodSequencePoints()
		{
			int length = FooModule.GetVisitedCodeLength();
			int expectedLength = 9;
			Assert.AreEqual(expectedLength, length);
		}
		
		[Test]
		public void ModuleGetUnvisitedCodeLength_FooModule_ReturnsTotalLengthOfAllNonVisitedMethodSequencePoints()
		{
			int length = FooModule.GetUnvisitedCodeLength();
			int expectedLength = 3;
			Assert.AreEqual(expectedLength, length);
		}
		
		[Test]
		public void ModuleGetVisitedCodeLength_BarModule_ReturnsTotalLengthOfAllVisitedMethodSequencePoints()
		{
			int length = BarModule.GetVisitedCodeLength();
			int expectedLength = 11;
			Assert.AreEqual(expectedLength, length);
		}
		
		[Test]
		public void ModuleGetUnvisitedCodeLength_BarModule_ReturnsTotalLengthOfAllNonVisitedMethodSequencePoints()
		{
			int length = BarModule.GetUnvisitedCodeLength();
			int expectedLength = 4;
			Assert.AreEqual(expectedLength, length);
		}
		
		[Test]
		public void ModulesCount_TwoModulesInCodeCoverageResults_ReturnsTwo()
		{
			int count = results.Modules.Count;
			Assert.AreEqual(2, count);
		}
	}
}
