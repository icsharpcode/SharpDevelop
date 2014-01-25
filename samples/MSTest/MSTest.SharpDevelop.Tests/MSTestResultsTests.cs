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
using System.IO;
using System.Linq;
using System.Xml;
using ICSharpCode.MSTest;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace MSTest.SharpDevelop.Tests
{
	[TestFixture]
	public class MSTestResultsTests
	{
		MSTestResults testResults;
		
		void CreateMSTestResults(string xml)
		{
			using (var reader = new XmlTextReader(new StringReader(xml))) {
				testResults = new MSTestResults(reader);
			}
		}
		
		void AssertTestResultsAreEqual(TestResult[] expectedResults)
		{
			List<string> expectedResultsAsString = ConvertToStrings(expectedResults);
			List<string> actualResultsAsString = ConvertToStrings(testResults);
			CollectionAssert.AreEqual(expectedResultsAsString, actualResultsAsString);
		}
		
		List<string> ConvertToStrings(IEnumerable<TestResult> results)
		{
			return results.Select(
				result => String.Format(
					"Name: {0}, Result: {1}, Message: '{2}', StackTrace: '{3}', StackPosition: {4}",
					result.Name,
					result.ResultType,
					result.Message,
					result.StackTrace,
					result.StackTraceFilePosition))
				.ToList();
		}
		
		[Test]
		public void Results_OneClassWithTwoPassingTestMethods_ReturnsTwoResults()
		{
			CreateMSTestResults(oneClassWithTwoPassingTestMethods);
			
			var expectedResults = new TestResult[] {
				new TestResult("FooTest.UnitTest1.TestMethod1") {
					ResultType = TestResultType.Success
				},
				new TestResult("FooTest.UnitTest1.TestMethod2") {
					ResultType = TestResultType.Success
				},
			};
			AssertTestResultsAreEqual(expectedResults);
		}
		
		string oneClassWithTwoPassingTestMethods =
			
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<TestRun id=""13473da1-70ea-422c-86a5-3b5d04610561"" name=""FEYNMAN 2012-05-06 11:02:13"" runUser=""Feynman\Matt"" xmlns=""http://microsoft.com/schemas/VisualStudio/TeamTest/2010"">
  <TestSettings name=""Local"" id=""1af0c4fe-35c7-49e5-b22a-40677255db56"">
    <Description>These are default test settings for a local test run.</Description>
    <Deployment runDeploymentRoot=""FEYNMAN 2012-05-06 11_02_13"">
      <DeploymentItem filename=""T4Scaffolding.Test\ExampleScripts\"" />
    </Deployment>
    <Execution>
      <TestTypeSpecific>
        <UnitTestRunConfig testTypeId=""13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"">
          <AssemblyResolution>
            <TestDirectory useLoadContext=""true"" />
          </AssemblyResolution>
        </UnitTestRunConfig>
        <WebTestRunConfiguration testTypeId=""4e7599fa-5ecb-43e9-a887-cd63cf72d207"">
          <Browser name=""Internet Explorer 7.0"">
            <Headers>
              <Header name=""User-Agent"" value=""Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)"" />
              <Header name=""Accept"" value=""*/*"" />
              <Header name=""Accept-Language"" value=""{{$IEAcceptLanguage}}"" />
              <Header name=""Accept-Encoding"" value=""GZIP"" />
            </Headers>
          </Browser>
        </WebTestRunConfiguration>
      </TestTypeSpecific>
      <AgentRule name=""LocalMachineDefaultRole"">
      </AgentRule>
    </Execution>
  </TestSettings>
  <Times creation=""2012-05-06T11:02:13.0655060+01:00"" queuing=""2012-05-06T11:02:16.0845060+01:00"" start=""2012-05-06T11:02:16.4355060+01:00"" finish=""2012-05-06T11:02:17.8915060+01:00"" />
  <ResultSummary outcome=""Completed"">
    <Counters total=""2"" executed=""2"" passed=""2"" error=""0"" failed=""0"" timeout=""0"" aborted=""0"" inconclusive=""0"" passedButRunAborted=""0"" notRunnable=""0"" notExecuted=""0"" disconnected=""0"" warning=""0"" completed=""0"" inProgress=""0"" pending=""0"" />
  </ResultSummary>
  <TestDefinitions>
    <UnitTest name=""TestMethod2"" storage=""d:\temp\test\mvcscaffolding_31fa7ea49b52\footest\bin\debug\footest.dll"" id=""760d70dc-fcd4-bd05-26dd-50b565053466"">
      <Execution id=""4d09aff0-ba01-4c01-9c3c-dd6475e89ef2"" />
      <TestMethod codeBase=""D:/projects/FooTest/bin/Debug/FooTest.DLL"" adapterTypeName=""Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.Adapter, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"" className=""FooTest.UnitTest1, FooTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"" name=""TestMethod2"" />
    </UnitTest>
    <UnitTest name=""TestMethod1"" storage=""d:\temp\test\mvcscaffolding_31fa7ea49b52\footest\bin\debug\footest.dll"" id=""752967dd-f45f-65ac-ca4a-dcd30f56a25a"">
      <Execution id=""ba931d00-d381-43c3-b0f9-f4cf37015438"" />
      <TestMethod codeBase=""D:/projects/FooTest/bin/Debug/FooTest.DLL"" adapterTypeName=""Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.Adapter, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"" className=""FooTest.UnitTest1, FooTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"" name=""TestMethod1"" />
    </UnitTest>
  </TestDefinitions>
  <TestLists>
    <TestList name=""Results Not in a List"" id=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" />
    <TestList name=""All Loaded Results"" id=""19431567-8539-422a-85d7-44ee4e166bda"" />
  </TestLists>
  <TestEntries>
    <TestEntry testId=""752967dd-f45f-65ac-ca4a-dcd30f56a25a"" executionId=""ba931d00-d381-43c3-b0f9-f4cf37015438"" testListId=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" />
    <TestEntry testId=""760d70dc-fcd4-bd05-26dd-50b565053466"" executionId=""4d09aff0-ba01-4c01-9c3c-dd6475e89ef2"" testListId=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" />
  </TestEntries>
  <Results>
    <UnitTestResult executionId=""ba931d00-d381-43c3-b0f9-f4cf37015438"" testId=""752967dd-f45f-65ac-ca4a-dcd30f56a25a"" testName=""TestMethod1"" computerName=""FEYNMAN"" duration=""00:00:00.0501283"" startTime=""2012-05-06T11:02:16.5755060+01:00"" endTime=""2012-05-06T11:02:17.7465060+01:00"" testType=""13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"" outcome=""Passed"" testListId=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" relativeResultsDirectory=""ba931d00-d381-43c3-b0f9-f4cf37015438"">
    </UnitTestResult>
    <UnitTestResult executionId=""4d09aff0-ba01-4c01-9c3c-dd6475e89ef2"" testId=""760d70dc-fcd4-bd05-26dd-50b565053466"" testName=""TestMethod2"" computerName=""FEYNMAN"" duration=""00:00:00.0018331"" startTime=""2012-05-06T11:02:17.7655060+01:00"" endTime=""2012-05-06T11:02:17.7785060+01:00"" testType=""13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"" outcome=""Passed"" testListId=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" relativeResultsDirectory=""4d09aff0-ba01-4c01-9c3c-dd6475e89ef2"">
    </UnitTestResult>
  </Results>
</TestRun>";
		
		[Test]
		public void Results_OneTestMethodThrowsException_ReturnsOneErrorResultWithStackTrace()
		{
			CreateMSTestResults(oneTestMethodThrowsException);
			
			var expectedResults = new TestResult[] {
				new TestResult("FooTest.UnitTest1.TestMethod1") {
					ResultType = TestResultType.Failure,
					Message = "System.ApplicationException: asdfafds",
					StackTrace = "    at FooTest.UnitTest1.TestMethod1() in d:\\projects\\FooTest\\UnitTest1.cs:line 21\r\n",
					StackTraceFilePosition = new DomRegion(@"d:\projects\FooTest\UnitTest1.cs", 21, 1)
				}
			};
			AssertTestResultsAreEqual(expectedResults);
		}
		
		string oneTestMethodThrowsException =
			
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<TestRun id=""7c0b0a20-13c6-4c28-b74c-e29c271f2ec4"" name=""FEYNMAN 2012-05-06 13:07:05"" runUser=""Feynman\Matt"" xmlns=""http://microsoft.com/schemas/VisualStudio/TeamTest/2010"">
  <TestSettings name=""Default Test Settings"" id=""03cf0958-a7e3-4b25-b98b-435f01359ee1"">
    <Execution>
      <TestTypeSpecific />
      <AgentRule name=""Execution Agents"">
      </AgentRule>
    </Execution>
    <Deployment runDeploymentRoot=""FEYNMAN 2012-05-06 13_07_05"" />
  </TestSettings>
  <Times creation=""2012-05-06T13:07:05.9187060+01:00"" queuing=""2012-05-06T13:07:06.6519060+01:00"" start=""2012-05-06T13:07:06.8235060+01:00"" finish=""2012-05-06T13:07:08.0403060+01:00"" />
  <ResultSummary outcome=""Failed"">
    <Counters total=""1"" executed=""1"" passed=""0"" error=""0"" failed=""1"" timeout=""0"" aborted=""0"" inconclusive=""0"" passedButRunAborted=""0"" notRunnable=""0"" notExecuted=""0"" disconnected=""0"" warning=""0"" completed=""0"" inProgress=""0"" pending=""0"" />
  </ResultSummary>
  <TestDefinitions>
    <UnitTest name=""TestMethod1"" storage=""d:\projects\footest\bin\debug\footest.dll"" id=""752967dd-f45f-65ac-ca4a-dcd30f56a25a"">
      <Execution id=""c147e8ae-7ee7-4f28-9db3-a708e350c68d"" />
      <TestMethod codeBase=""D:/projects/FooTest/bin/Debug/FooTest.DLL"" adapterTypeName=""Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.Adapter, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"" className=""FooTest.UnitTest1, FooTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"" name=""TestMethod1"" />
    </UnitTest>
  </TestDefinitions>
  <TestLists>
    <TestList name=""Results Not in a List"" id=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" />
    <TestList name=""All Loaded Results"" id=""19431567-8539-422a-85d7-44ee4e166bda"" />
  </TestLists>
  <TestEntries>
    <TestEntry testId=""752967dd-f45f-65ac-ca4a-dcd30f56a25a"" executionId=""c147e8ae-7ee7-4f28-9db3-a708e350c68d"" testListId=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" />
  </TestEntries>
  <Results>
    <UnitTestResult executionId=""c147e8ae-7ee7-4f28-9db3-a708e350c68d"" testId=""752967dd-f45f-65ac-ca4a-dcd30f56a25a"" testName=""TestMethod1"" computerName=""FEYNMAN"" duration=""00:00:00.0404760"" startTime=""2012-05-06T13:07:06.9015060+01:00"" endTime=""2012-05-06T13:07:07.8375060+01:00"" testType=""13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"" outcome=""Failed"" testListId=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" relativeResultsDirectory=""c147e8ae-7ee7-4f28-9db3-a708e350c68d"">
      <Output>
        <ErrorInfo>
          <Message>Test method FooTest.UnitTest1.TestMethod1 threw exception: 
System.ApplicationException: asdfafds</Message>
          <StackTrace>    at FooTest.UnitTest1.TestMethod1() in d:\projects\FooTest\UnitTest1.cs:line 21
</StackTrace>
        </ErrorInfo>
      </Output>
    </UnitTestResult>
  </Results>
</TestRun>";
	}
}
