// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class RunTestWithDebuggerCommand : AbstractRunTestCommand
	{
		public RunTestWithDebuggerCommand()
		{
		}
		
		public RunTestWithDebuggerCommand(IRunTestCommandContext context)
			: base(context)
		{
		}
		
		protected override ITestRunner CreateTestRunner(IProject project)
		{
			return Context.RegisteredTestFrameworks.CreateTestDebugger(project);
		}
	}
}
