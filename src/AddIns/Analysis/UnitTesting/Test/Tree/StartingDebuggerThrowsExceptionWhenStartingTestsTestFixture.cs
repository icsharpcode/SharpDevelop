// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
