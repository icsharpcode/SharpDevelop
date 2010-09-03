// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	public class RunTestWithProfilerCommand : AbstractRunTestCommand
	{
		protected override ITestRunner CreateTestRunner(IProject project)
		{
			return new ProfilerTestRunner();
		}
	}
}
