// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.CodeCoverage
{
	public class RunAllTestsWithCodeCoverageCommand : RunTestWithCodeCoverageCommand
	{
		public RunAllTestsWithCodeCoverageCommand()
		{
		}
		
		/// <summary>
		/// Set Owner to null so all tests are run.
		/// </summary>
		public override void Run()
		{
			Owner = null;
			base.Run();
		}
	}
}
