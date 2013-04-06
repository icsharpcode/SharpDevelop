// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	public class MSpecTestProcessRunnerContext : TestProcessRunnerBaseContext
	{
		public MSpecTestProcessRunnerContext(TestExecutionOptions options)
			: base(
				options,
				new ProcessRunner(),
				new MSpecUnitTestMonitor(),
				SD.FileSystem,
				SD.MessageService)
		{
		}
	}
}
