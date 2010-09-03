// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.IO;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class CodeCoverageResultsTestFixture : CodeCoverageResultsTestsBase
	{		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string xml = 
				"<PartCoverReport date=\"2008-07-10T02:59:13.7198656+01:00\">\r\n" +
				"    <File id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"    <Assembly id=\"1\" name=\"Foo.Tests\" module=\"C:\\Projects\\Test\\Foo.Tests\\bin\\Foo.Tests.DLL\" domain=\"test-domain-Foo.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"    <Type asmref=\"1\" name=\"Foo.Tests.FooTestFixture\" flags=\"1232592\">\r\n" +
				"        <Method name=\"SimpleTest\" sig=\"void  ()\" bodysize=\"42\" flags=\"2182\" iflags=\"0\">\r\n" +
				"            <pt visit=\"1\" pos=\"0\" len=\"2\" fid=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
				"            <pt visit=\"1\" pos=\"0\" len=\"1\" fid=\"1\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" />\r\n" +
				"            <pt visit=\"0\" pos=\"0\" len=\"4\" fid=\"1\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" />\r\n" +
				"        </Method>\r\n" +
				"    </Type>\r\n" +
				"</PartCoverReport>";
			
			base.CreateCodeCoverageResults(xml);			
		}
		
		[Test]
		public void ModuleName_OneModuleInCodeCoverageResults_AssemblyNameReturned()
		{
			string name = base.FirstModule.Name;
			string expectedName = "Foo.Tests";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void ModulesCount_OneModuleInCodeCoverageResults_ReturnsOneModule()
		{
			int count = results.Modules.Count;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void MethodsCount_ModuleHasOneMethod_ReturnsOne()
		{
			int count = FirstModule.Methods.Count;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void MethodName_ModuleHasOneMethod_ReturnsExpectedMethodName()
		{
			string name = FirstModuleFirstMethod.Name;
			string expectedName = "SimpleTest";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void MethodFullClassName_ModuleHasOneMethod_ReturnsExpectedFullClassName()
		{
			string name = FirstModuleFirstMethod.FullClassName;
			string expectedName = "Foo.Tests.FooTestFixture";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void MethodClassName_ModuleHasOneMethod_ReturnsExpectedClassName()
		{
			string name = FirstModuleFirstMethod.ClassName;
			string expectedClassName = "FooTestFixture";
			Assert.AreEqual(expectedClassName, name);
		}
		
		[Test]
		public void MethodClassNamespace_ModuleHasOneMethod_ReturnsExpectedNamespace()
		{
			string name = FirstModuleFirstMethod.ClassNamespace;
			string expectedName = "Foo.Tests";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void SequencePointsCount_MethodHasThreeSequencePoints_ReturnsThree()
		{
			int count = FirstModuleFirstMethod.SequencePoints.Count;
			Assert.AreEqual(3, count);
		}
		
		[Test]
		public void SequencePoint_FirstSequencePoint_HasExpectedPropertyValues()
		{
			CodeCoverageSequencePoint point = base.CreateSequencePoint();
			point.Document = @"c:\Projects\Foo\FooTestFixture.cs";
			point.VisitCount = 1;
			point.Column = 3;
			point.EndColumn = 4;
			point.Line = 20;
			point.EndLine = 20;
			point.Length = 2;
			
			Assert.AreEqual(point, FirstModuleFirstMethodFirstSequencePoint);
		}
		
		[Test]
		public void SequencePointVisitCount_SecondSequencePoint_ReturnsOneVisit()
		{
			int count = FirstModuleFirstMethodSecondSequencePoint.VisitCount;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void SequencePointVistCount_ThirdSequencePoint_ReturnsNoVisits()
		{
			int count = FirstModuleFirstMethodThirdSequencePoint.VisitCount;
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void VisitedSequencePointsCount_FirstMethod_ReturnsTwo()
		{
			int count = FirstModuleFirstMethod.VisitedSequencePointsCount;
			Assert.AreEqual(2, count);
		}
		
		[Test]
		public void NotVisitedSequencePointsCount_FirstMethod_ReturnsOne()
		{
			int count = FirstModuleFirstMethod.NotVisitedSequencePointsCount;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void GetVisitedCodeLength_FirstMethod_ReturnsSummedLengthOfVisitedSequencePoints()
		{
			int length = FirstModuleFirstMethod.GetVisitedCodeLength();
			Assert.AreEqual(3, length);
		}
		
		[Test]
		public void GetUnvisitedCodeLength_FirstMethod_ReturnsSummedLengthOfUnvisitedSequencePoints()
		{
			int length = FirstModuleFirstMethod.GetUnvisitedCodeLength();
			Assert.AreEqual(4, length);
		}
	}
}
