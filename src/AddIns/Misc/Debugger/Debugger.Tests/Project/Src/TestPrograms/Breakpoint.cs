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
			// Breakpoint
			System.Diagnostics.Debug.WriteLine("Main 3");
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
			Breakpoint breakpoint1 = debugger.AddBreakpoint(@"Breakpoint.cs", 18);
			Breakpoint breakpoint2 = debugger.AddBreakpoint(@"Breakpoint.cs", 19);
			
			StartTest("Breakpoint.cs");
			
			Assert.IsTrue(breakpoint1.IsSet);
			Assert.IsTrue(breakpoint2.IsSet);
			ObjectDump("Breakpoint1", breakpoint1);
			ObjectDump("Breakpoint2", breakpoint2);
			
			process.Continue();
			process.Continue();
			process.Continue();
			process.AsyncContinue();
			process.WaitForExit();
			ObjectDump("Breakpoint1", breakpoint1);
			ObjectDump("Breakpoint2", breakpoint2);
			
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
    <DebuggingPaused>Break Breakpoint.cs:16,4-16,40</DebuggingPaused>
    <Breakpoint1>
      <Breakpoint
        CheckSum="null"
        Column="0"
        Enabled="True"
        FileName="Breakpoint.cs"
        IsSet="True"
        Line="18"
        OriginalLocation="Breakpoint.cs:18,4-18,49" />
    </Breakpoint1>
    <Breakpoint2>
      <Breakpoint
        CheckSum="null"
        Column="0"
        Enabled="True"
        FileName="Breakpoint.cs"
        IsSet="True"
        Line="19"
        OriginalLocation="Breakpoint.cs:20,4-20,49" />
    </Breakpoint2>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>Main 1\r\n</LogMessage>
    <DebuggingPaused>Breakpoint Breakpoint.cs:18,4-18,49</DebuggingPaused>
    <LogMessage>Main 2\r\n</LogMessage>
    <DebuggingPaused>Breakpoint Breakpoint.cs:20,4-20,49</DebuggingPaused>
    <LogMessage>Main 3\r\n</LogMessage>
    <DebuggingPaused>Break Breakpoint.cs:21,4-21,40</DebuggingPaused>
    <ProcessExited />
    <Breakpoint1>
      <Breakpoint
        CheckSum="null"
        Column="0"
        Enabled="True"
        FileName="Breakpoint.cs"
        IsSet="False"
        Line="18"
        OriginalLocation="Breakpoint.cs:18,4-18,49" />
    </Breakpoint1>
    <Breakpoint2>
      <Breakpoint
        CheckSum="null"
        Column="0"
        Enabled="True"
        FileName="Breakpoint.cs"
        IsSet="False"
        Line="19"
        OriginalLocation="Breakpoint.cs:20,4-20,49" />
    </Breakpoint2>
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
