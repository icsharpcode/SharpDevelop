// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
				"<CoverageSession xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
				"\t<Modules>\r\n" +
				"\t\t<Module hash=\"44-54-B6-13-97-49-45-F8-6A-74-9E-49-0C-77-87-C6-9C-54-47-7A\">\r\n" +
				"\t\t\t<FullName>C:\\Projects\\Test\\Foo.Tests\\bin\\Foo.Tests.DLL</FullName>\r\n" +
				"\t\t\t<ModuleName>Foo.Tests</ModuleName>\r\n" +
				"\t\t\t<Files>\r\n" +
				"\t\t\t\t<File uid=\"1\" fullPath=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t\t<File uid=\"2\" fullPath=\"c:\\Projects\\Foo\\FooTestFixture2.cs\" />\r\n" +
				"\t\t\t</Files>\r\n" +
				"\t\t\t<Classes>\r\n" +
				"\t\t\t\t<Class>\r\n" +
				"\t\t\t\t\t<FullName>Foo.Tests.FooTestFixture1</FullName>\r\n" +
				"\t\t\t\t\t<Methods>\r\n" +
				"\t\t\t\t\t\t<Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"false\" isSetter=\"false\">\r\n" +
				"\t\t\t\t\t\t\t<MetadataToken>100663297</MetadataToken>\r\n" +
				"\t\t\t\t\t\t\t<Name>System.Void Foo.Tests.FooTestFixture::SimpleTest1()</Name>\r\n" +
				"\t\t\t\t\t\t\t<FileRef uid=\"1\" />\r\n" +
				"\t\t\t\t\t\t\t<SequencePoints>\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"12\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"4\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t</SequencePoints>\r\n" +
				"\t\t\t\t\t\t</Method>\r\n" +
				"\t\t\t\t\t</Methods>\r\n" +
				"\t\t\t\t</Class>\r\n" +
				"\t\t\t\t<Class>\r\n" +
				"\t\t\t\t\t<FullName>Foo.Tests.FooTestFixture2</FullName>\r\n" +
				"\t\t\t\t\t<Methods>\r\n" +
				"\t\t\t\t\t\t<Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"false\" isSetter=\"false\">\r\n" +
				"\t\t\t\t\t\t\t<MetadataToken>100663297</MetadataToken>\r\n" +
				"\t\t\t\t\t\t\t<Name>System.Void Foo.Tests.FooTestFixture2::SimpleTest2()</Name>\r\n" +
				"\t\t\t\t\t\t\t<FileRef uid=\"2\" />\r\n" +
				"\t\t\t\t\t\t\t<SequencePoints>\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"10\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t</SequencePoints>\r\n" +
				"\t\t\t\t\t\t</Method>\r\n" +
				"\t\t\t\t\t</Methods>\r\n" +
				"\t\t\t\t</Class>\r\n" +
				"\t\t\t</Classes>\r\n" +
				"\t\t</Module>\r\n" +
				"\t\t<Module hash=\"44-54-B6-13-97-49-45-F8-6A-74-9E-49-0C-77-87-C6-9C-54-47-7B\">\r\n" +
				"\t\t\t<FullName>C:\\Projects\\Test\\Foo.Tests\\bin\\Bar.Tests.DLL</FullName>\r\n" +
				"\t\t\t<ModuleName>Bar.Tests</ModuleName>\r\n" +
				"\t\t\t<Files>\r\n" +
				"\t\t\t\t<File uid=\"3\" fullPath=\"c:\\Projects\\Foo\\BarTestFixture.cs\" />\r\n" +
				"\t\t\t</Files>\r\n" +
				"\t\t\t<Classes>\r\n" +
				"\t\t\t\t<Class>\r\n" +
				"\t\t\t\t\t<FullName>Foo.Tests.FooTestFixture3</FullName>\r\n" +
				"\t\t\t\t\t<Methods>\r\n" +
				"\t\t\t\t\t\t<Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"false\" isSetter=\"false\">\r\n" +
				"\t\t\t\t\t\t\t<MetadataToken>100663297</MetadataToken>\r\n" +
				"\t\t\t\t\t\t\t<Name>System.Void Foo.Tests.BarTestFixture::.SimpleTest3()</Name>\r\n" +
				"\t\t\t\t\t\t\t<FileRef uid=\"3\" />\r\n" +
				"\t\t\t\t\t\t\t<SequencePoints>\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"5\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
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
		
		CodeCoverageModule FooModule {
			get { return FirstModule; }
		}

		CodeCoverageModule BarModule {
			get { return SecondModule; }
		}
		
		[Test]
		public void ModuleGetVisitedCodeLength_FooModule_ReturnsTotalLengthOfAllVisitedMethodSequencePoints()
		{
			Assert.Inconclusive("SequencePoint.Length is computed from text-source not present in this test");
			int length = FooModule.GetVisitedCodeLength();
			int expectedLength = 4;
			Assert.AreEqual(expectedLength, length);
		}
		
		[Test]
		public void ModuleGetUnvisitedCodeLength_FooModule_ReturnsTotalLengthOfAllNonVisitedMethodSequencePoints()
		{
			Assert.Inconclusive("SequencePoint.Length is computed from text-source not present in this test");
			int length = FooModule.GetUnvisitedCodeLength();
			int expectedLength = 2;
			Assert.AreEqual(expectedLength, length);
		}
		
		[Test]
		public void ModuleGetVisitedCodeLength_BarModule_ReturnsTotalLengthOfAllVisitedMethodSequencePoints()
		{
			Assert.Inconclusive("SequencePoint.Length is computed from text-source not present in this test");
			int length = BarModule.GetVisitedCodeLength();
			int expectedLength = 2;
			Assert.AreEqual(expectedLength, length);
		}
		
		[Test]
		public void ModuleGetUnvisitedCodeLength_BarModule_ReturnsTotalLengthOfAllNonVisitedMethodSequencePoints()
		{
			Assert.Inconclusive("SequencePoint.Length is computed from text-source not present in this test");
			int length = BarModule.GetUnvisitedCodeLength();
			int expectedLength = 1;
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
