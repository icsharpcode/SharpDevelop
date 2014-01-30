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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	/// <summary>
	/// Code coverage results when two classes have similar names that match
	/// at the start:
	/// 
	/// FooResult
	/// FooResultParser
	/// </summary>
	[TestFixture]
	public class TwoClassNamesMatchingAtStartTests : CodeCoverageResultsTestsBase
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string xml =
				"<CoverageSession xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
				"  <Modules>\r\n" +
				"    <Module hash=\"44-54-B6-13-97-49-45-F8-6A-74-9E-49-0C-77-87-C6-9C-54-47-7A\">\r\n" +
				"      <FullName>C:\\Projects\\Test\\Foo\\bin\\Foo.DLL</FullName>\r\n" +
				"      <ModuleName>Foo.Tests</ModuleName>\r\n" +
				"      <Files>\r\n" +
				"        <File uid=\"1\" fullPath=\"c:\\Projects\\Foo\\FooResults.cs\" />\r\n" +
				"        <File uid=\"2\" fullPath=\"c:\\Projects\\Foo\\FooResultsParser.cs\" />\r\n" +
				"      </Files>\r\n" +
				"      <Classes>\r\n" +
				"        <Class>\r\n" +
				"          <FullName>Foo.Results</FullName>\r\n" +
				"          <Methods>\r\n" +
				"            <Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"false\" isSetter=\"false\">\r\n" +
				"              <MetadataToken>100663297</MetadataToken>\r\n" +
				"              <Name>System.String Foo.Results::GetResult()</Name>\r\n" +
				"              <FileRef uid=\"2\" />\r\n" +
				"              <SequencePoints>\r\n" +
				"                <SequencePoint vc=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
				"              </SequencePoints>\r\n" +
				"            </Method>\r\n" +
				"          </Methods>\r\n" +
				"        </Class>\r\n" +
				"        <Class>\r\n" +
				"          <FullName>Foo.ResultsParser</FullName>\r\n" +
				"          <Methods>\r\n" +
				"            <Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"false\" isSetter=\"false\">\r\n" +
				"              <MetadataToken>100663297</MetadataToken>\r\n" +
				"              <Name>System.Void Foo.ResultsParser::Parse()</Name>\r\n" +
				"              <FileRef uid=\"2\" />\r\n" +
				"              <SequencePoints>\r\n" +
				"                <SequencePoint vc=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
				"              </SequencePoints>\r\n" +
				"            </Method>\r\n" +
				"          </Methods>\r\n" +
				"        </Class>\r\n" +
				"      </Classes>\r\n" +
				"    </Module>\r\n" +
				"  </Modules>\r\n" +
				"</CoverageSession>";
			
			base.CreateCodeCoverageResults(xml);
		}
		
		[Test]
		public void ResultsClass_Methods_HasOneMethodCalledResults()
		{
			List<CodeCoverageMethod> methods =
				FirstModule.Methods.Where(m => m.ClassName == "Results").ToList();
			
			Assert.AreEqual("GetResult", methods[0].Name);
			Assert.AreEqual(1, methods.Count);
		}
		
		[Test]
		public void ResultsParserClass_Methods_HasOneMethodCalledParse()
		{
			List<CodeCoverageMethod> methods =
				FirstModule.Methods.Where(m => m.ClassName == "ResultsParser").ToList();
			
			Assert.AreEqual("Parse", methods[0].Name);
			Assert.AreEqual(1, methods.Count);
		}
	}
}
