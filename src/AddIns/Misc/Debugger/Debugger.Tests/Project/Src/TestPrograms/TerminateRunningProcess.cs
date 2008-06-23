// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Threading;

namespace Debugger.Tests.TestPrograms
{
	public class TerminateRunningProcess
	{
		static ManualResetEvent doSomething = new ManualResetEvent(false);
		
		public static void Main()
		{
			doSomething.WaitOne();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void TerminateRunningProcess()
		{
			StartTest("TerminateRunningProcess.cs", false);
			process.Terminate();
			process.WaitForExit();
			
			StartTest("TerminateRunningProcess.cs", false);
			process.Terminate();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="TerminateRunningProcess.cs">
    <ProcessStarted />
    <ProcessExited />
    <ProcessStarted />
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT