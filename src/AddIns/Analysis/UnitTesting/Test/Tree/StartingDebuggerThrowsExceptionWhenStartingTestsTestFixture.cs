// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class StartingDebuggerThrowsExceptionWhenStartingTestsTestFixture : RunTestWithDebuggerCommandTestFixtureBase
	{
		[SetUp]
		public void Init()
		{
			InitBase();
			
			ApplicationException ex = new ApplicationException();
			debuggerService.MockDebugger.ThrowExceptionOnStart = ex;
			runCommand.Run();
			
			try {
				Assert.Throws<ApplicationException>(
					delegate { buildProject.FireBuildCompleteEvent(); });
			} catch {
				// Do nothing.
			}
		}
		
		[Test]
		public void FiringDebugStoppedEventDoesNotCallTestRunCompletedMethod()
		{
			context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls.Clear();
			debuggerService.MockDebugger.FireDebugStoppedEvent();
			
			Assert.AreEqual(0, context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls.Count);
		}
	}
}
