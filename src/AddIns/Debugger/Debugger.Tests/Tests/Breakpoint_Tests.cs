// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Debugger.Tests
{
	public class Breakpoint_Tests
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
		public void Breakpoint_Tests()
		{
			Breakpoint breakpoint1 = debugger.Breakpoints.Add(@"Breakpoint_Tests.cs", 14);
			Breakpoint breakpoint2 = debugger.Breakpoints.Add(@"Breakpoint_Tests.cs", 15);
			
			StartTest();
			
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
    name="Breakpoint_Tests.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Breakpoint_Tests.exe (Has symbols)</ModuleLoaded>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <DebuggingPaused>Break Breakpoint_Tests.cs:12,4-12,40</DebuggingPaused>
    <Breakpoint1>
      <Breakpoint
        Enabled="True"
        FileName="Breakpoint_Tests.cs"
        IsSet="True"
        Line="14"
        OriginalLocation="Breakpoint_Tests.cs:14,4-14,49" />
    </Breakpoint1>
    <Breakpoint2>
      <Breakpoint
        Enabled="True"
        FileName="Breakpoint_Tests.cs"
        IsSet="True"
        Line="15"
        OriginalLocation="Breakpoint_Tests.cs:16,4-16,49" />
    </Breakpoint2>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>Main 1\r\n</LogMessage>
    <DebuggingPaused>Breakpoint Breakpoint_Tests.cs:14,4-14,49</DebuggingPaused>
    <LogMessage>Main 2\r\n</LogMessage>
    <DebuggingPaused>Breakpoint Breakpoint_Tests.cs:16,4-16,49</DebuggingPaused>
    <LogMessage>Main 3\r\n</LogMessage>
    <DebuggingPaused>Break Breakpoint_Tests.cs:17,4-17,40</DebuggingPaused>
    <ProcessExited />
    <Breakpoint1>
      <Breakpoint
        Enabled="True"
        FileName="Breakpoint_Tests.cs"
        Line="14"
        OriginalLocation="Breakpoint_Tests.cs:14,4-14,49" />
    </Breakpoint1>
    <Breakpoint2>
      <Breakpoint
        Enabled="True"
        FileName="Breakpoint_Tests.cs"
        Line="15"
        OriginalLocation="Breakpoint_Tests.cs:16,4-16,49" />
    </Breakpoint2>
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
