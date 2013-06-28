// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.Profiler.Controller;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	public class RunSelectedTestsWithProfilerCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITestService testService = SD.GetRequiredService<ITestService>();
			IEnumerable<ITest> tests = TestableCondition.GetTests(testService.OpenSolution, Owner);
			string path = tests.FirstOrDefault().ParentProject.Project.GetSessionFileName();
			testService.RunTestsAsync(tests, new TestExecutionOptions { ProcessRunner = new ProfilerProcessRunner(new ProfilingDataSQLiteWriter(path), new ProfilerOptions()) }).FireAndForget();
		}
	}
}
