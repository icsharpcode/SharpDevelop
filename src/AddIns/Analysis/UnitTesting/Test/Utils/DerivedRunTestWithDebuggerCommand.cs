// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class DerivedRunTestWithDebuggerCommand : RunTestWithDebuggerCommand
	{
		public DerivedRunTestWithDebuggerCommand(IRunTestCommandContext context)
			: base(context)
		{
		}
		
		public Action GetCallRunTestCompletedAction()
		{
			Action action = base.TestRunCompleted;
			return action;
		}
	}
}
