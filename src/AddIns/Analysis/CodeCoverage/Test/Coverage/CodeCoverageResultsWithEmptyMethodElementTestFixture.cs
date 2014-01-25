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
	public class CodeCoverageResultsWithEmptyMethodElementTestFixture : CodeCoverageResultsTestsBase
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
				"\t\t\t\t\t\t\t<Name>System.Void Foo.Tests.FooTestFixture::.SimpleTest()</Name>\r\n" +
				"\t\t\t\t\t\t\t<FileRef uid=\"1\" />\r\n" +
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
		public void ModulesCount_MethodWithNoSequencePointsInModule_ReturnsOneModule()
		{
			int count = results.Modules.Count;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void MethodUnvisitedCodeLength_MethodWithNoSequencePoints_ReturnsBodySize()
		{
			int unvisitedCodeLength = FirstModuleFirstMethod.GetUnvisitedCodeLength();
			int expectedUnvisitedCodeLength = 0;
			Assert.AreEqual(expectedUnvisitedCodeLength, unvisitedCodeLength);
		}
	}
}
