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

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class RunAllTestsInProjectCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITestService testService = SD.GetRequiredService<ITestService>();
			if (testService.OpenSolution != null) {
				ITestProject project = testService.OpenSolution.GetTestProject(ProjectService.CurrentProject);
				if (project != null)
					testService.RunTestsAsync(new [] { project }, new TestExecutionOptions()).FireAndForget();
			}
		}
	}
	
	public class RunAllTestsInSolutionCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITestService testService = SD.GetRequiredService<ITestService>();
			if (testService.OpenSolution != null)
				testService.RunTestsAsync(new [] { testService.OpenSolution }, new TestExecutionOptions()).FireAndForget();
		}
	}
	
	public class RunSelectedTestCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITestService testService = SD.GetRequiredService<ITestService>();
			IEnumerable<ITest> tests = TestableCondition.GetTests(testService.OpenSolution, Owner);
			testService.RunTestsAsync(tests, new TestExecutionOptions()).FireAndForget();
		}
	}
	
	public class RunSelectedTestWithDebuggerCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITestService testService = SD.GetRequiredService<ITestService>();
			IEnumerable<ITest> tests = TestableCondition.GetTests(testService.OpenSolution, Owner);
			testService.RunTestsAsync(tests, new TestExecutionOptions { UseDebugger = true }).FireAndForget();
		}
	}
	
	public class StopTestsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITestService testService = SD.GetRequiredService<ITestService>();
			testService.CancelRunningTests();
		}
	}
}
