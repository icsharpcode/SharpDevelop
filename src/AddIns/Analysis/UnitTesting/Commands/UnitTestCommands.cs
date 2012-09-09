// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
