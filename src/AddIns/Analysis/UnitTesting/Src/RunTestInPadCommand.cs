// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public class RunTestInPadCommand : AbstractRunTestCommand
	{
		public RunTestInPadCommand()
		{
		}
		
		public RunTestInPadCommand(IRunTestCommandContext context)
			: base(context)
		{
		}
		
		protected override ITestRunner CreateTestRunner(IProject project)
		{
			return Context.RegisteredTestFrameworks.CreateTestRunner(project);
		}
	}
}
