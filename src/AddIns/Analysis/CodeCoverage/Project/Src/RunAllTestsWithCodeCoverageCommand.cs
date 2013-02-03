// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.CodeCoverage
{
	public class RunAllTestsWithCodeCoverageCommand : RunTestWithCodeCoverageCommand
	{
		public override void Run()
		{
			ITestService testService = SD.GetRequiredService<ITestService>();
			if (testService.OpenSolution != null) {
				base.Run();
			}
		}
		
		protected override IEnumerable<ITest> GetTests(ITestService testService)
		{
			return new[] { testService.OpenSolution };
		}
	}
}
