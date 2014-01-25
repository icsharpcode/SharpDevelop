// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestProcessRunnerContext : TestProcessRunnerBaseContext
	{
		public MSTestProcessRunnerContext(TestExecutionOptions options)
			: base(
				options,
				new ProcessRunner(),
				new MSTestMonitor(),
				SD.FileSystem,
				SD.MessageService)
		{
		}
	}
}
