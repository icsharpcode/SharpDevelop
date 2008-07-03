// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Breakpoint
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			System.Diagnostics.Debug.WriteLine("Main 1");
			System.Diagnostics.Debug.WriteLine("Main 2"); // Breakpoint
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using NUnit.Framework;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Breakpoint()
		{
			Breakpoint breakpoint = debugger.AddBreakpoint(@"Breakpoint.cs", 18);
			
			StartTest("Breakpoint.cs");
			
			Assert.IsTrue(breakpoint.IsSet);
			ObjectDump(breakpoint);
			process.Continue();
			process.Continue();
			process.AsyncContinue();
			process.WaitForExit();
			ObjectDump(breakpoint);
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Breakpoint.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Breakpoint.exe (Has symbols)</ModuleLoaded>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <Breakpoint
      CheckSum="null"
      Column="0"
      Enabled="True"
      FileName="Breakpoint.cs"
      IsSet="True"
      Line="18"
      OriginalLocation="Breakpoint.cs:18,4-18,49" />
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>Main 1\r\n</LogMessage>
    <DebuggingPaused>Breakpoint</DebuggingPaused>
    <LogMessage>Main 2\r\n</LogMessage>
    <DebuggingPaused>Break</DebuggingPaused>
    <ProcessExited />
    <Breakpoint
      CheckSum="null"
      Column="0"
      Enabled="True"
      FileName="Breakpoint.cs"
      IsSet="False"
      Line="18"
      OriginalLocation="Breakpoint.cs:18,4-18,49" />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
