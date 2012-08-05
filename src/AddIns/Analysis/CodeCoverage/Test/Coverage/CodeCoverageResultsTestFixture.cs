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
				"<CoverageSession xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
				"\t<Modules>\r\n" +
				"\t\t<Module hash=\"44-54-B6-13-97-49-45-F8-6A-74-9E-49-0C-77-87-C6-9C-54-47-7A\">\r\n" +
				"\t\t\t<FullName>C:\\Projects\\Test\\Foo.Tests\\bin\\Foo.Tests.DLL</FullName>\r\n" +
				"\t\t\t<ModuleName>Foo.Tests</ModuleName>\r\n" +
				"\t\t\t<Files>\r\n" +
				"\t\t\t\t<File uid=\"1\" fullPath=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t</Files>\r\n" +
				"\t\t\t<Classes>\r\n" +
				"\t\t\t\t<Class>\r\n" +
				"\t\t\t\t\t<FullName>Foo.Tests.FooTestFixture</FullName>\r\n" +
				"\t\t\t\t\t<Methods>\r\n" +
				"\t\t\t\t\t\t<Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"false\" isSetter=\"false\">\r\n" +
				"\t\t\t\t\t\t\t<MetadataToken>100663297</MetadataToken>\r\n" +
				"\t\t\t\t\t\t\t<Name>System.Void Foo.Tests.FooTestFixture::SimpleTest()</Name>\r\n" +
				"\t\t\t\t\t\t\t<FileRef uid=\"1\" />\r\n" +
				"\t\t\t\t\t\t\t<SequencePoints>\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"1\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t</SequencePoints>\r\n" +
				"\t\t\t\t\t\t</Method>\r\n" +
				"\t\t\t\t\t</Methods>\r\n" +
				"\t\t\t\t</Class>\r\n" +
				"\t\t\t</Classes>\r\n" +
				"\t\t</Module>\r\n" +
				"\t</Modules>\r\n" +
				"</CoverageSession>";
			
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
			point.Length = 1;
			
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
			Assert.AreEqual(2, length);
		}
		
		[Test]
		public void GetUnvisitedCodeLength_FirstMethod_ReturnsSummedLengthOfUnvisitedSequencePoints()
		{
			int length = FirstModuleFirstMethod.GetUnvisitedCodeLength();
			Assert.AreEqual(1, length);
		}
	}
}
