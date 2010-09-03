// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class DerivedRunTestInPadCommand : RunTestInPadCommand
	{
		public DerivedRunTestInPadCommand(IRunTestCommandContext context)
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
