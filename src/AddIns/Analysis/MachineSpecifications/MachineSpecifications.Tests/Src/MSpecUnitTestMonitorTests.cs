// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
