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

using developwithpassion.specifications.rhinomocks;
using ICSharpCode.UnitTesting;
using Machine.Specifications;

namespace ICSharpCode.MachineSpecifications.Tests.Src
{
	public class When_getting_test_results_from_external_runner : Observes<MSpecUnitTestMonitor>
	{
		Because of = () => {
			sut.Start();
			sut.TestFinished += (source, e) => FinishedTests.Add(e.Result);
			sut.FileName = FILE_NAME;
			sut.Read();
			sut.Join();
		};

		It should_create_test_result_for_passed_test_with_status_set_to_success = () =>
			AssertHasResultType("Namespace.TestClass.Passed_Test", TestResultType.Success);

		It should_create_result_for_failed_test_with_status_set_to_failure = () =>
			AssertHasResultType("Namespace.TestClass.Failed_Test", TestResultType.Failure);

		It should_have_stack_trace_for_failed_test = () =>
			FinishedTests.Where(t => t.ResultType == TestResultType.Failure)
			.Select(t => t.StackTrace).ShouldContainOnly("Stack trace");

		It should_have_message_for_failed_test = () =>
			FinishedTests.Where(t => t.ResultType == TestResultType.Failure)
			.Select(t => t.Message).ShouldContainOnly("Test failure description.");

		It should_create_result_for_not_implemented_test_with_status_set_to_ignored = () =>
			AssertHasResultType("Namespace.TestClass.Not_Implemented_Test", TestResultType.Ignored);

		It should_create_result_for_ignored_test_with_status_set_to_ignored = () =>
			AssertHasResultType("Namespace.TestClass.Ignored_Test", TestResultType.Ignored);

		static readonly string FILE_NAME = @"Data\\MSpecUnitTestMonitorTestsSampleData.xml";
		static readonly IList<TestResult> FinishedTests = new List<TestResult>();
		
		static void AssertHasResultType(string testName, TestResultType result) {
			FinishedTests.ShouldContain(x => x.Name == testName);
			FinishedTests.Single(x => x.Name == testName).ResultType.ShouldEqual(result);
		}
	}
}
