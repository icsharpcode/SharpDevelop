// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.UnitTesting
{
	public class RunAllTestsInPadCommand : RunTestInPadCommand
	{
		public RunAllTestsInPadCommand()
		{
		}
		
		public RunAllTestsInPadCommand(IRunTestCommandContext context)
			: base(context)
		{
		}
		
		public override void Run()
		{
			// To make sure all tests are run we set the Owner to null.
			Owner = null;
			base.Run();
		}
	}
}
