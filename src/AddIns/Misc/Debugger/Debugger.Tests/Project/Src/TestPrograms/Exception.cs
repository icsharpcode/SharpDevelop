// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Exception
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			
			throw new System.Exception("test");
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using NUnit.Framework;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Exception()
		{
			StartTest("Exception.cs");
			
			process.ExceptionThrown += delegate {
				process.Terminate();
			};
			process.Paused += delegate {
				// Should not be raised for dead process
				Assert.Fail();
			};
			
			process.AsyncContinue();
			process.WaitForExit();
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Exception.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Exception.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break Exception.cs:16,4-16,40</DebuggingPaused>
    <ExceptionThrown>System.Exception: test</ExceptionThrown>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT