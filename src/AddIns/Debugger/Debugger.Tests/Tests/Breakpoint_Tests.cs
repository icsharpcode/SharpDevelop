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
			StartTest();
			
			string filename = CurrentStackFrame.NextStatement.Filename;
			
			Breakpoint breakpoint1 = debugger.AddBreakpoint(filename, 14);
			Breakpoint breakpoint2 = debugger.AddBreakpoint(filename, 15);
			
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
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Breakpoint_Tests.exe (Has symbols)</ModuleLoaded>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <Paused>Breakpoint_Tests.cs:12,4-12,40</Paused>
    <Breakpoint1>
      <Breakpoint
        IsEnabled="True"
        IsSet="True"
        Line="14" />
    </Breakpoint1>
    <Breakpoint2>
      <Breakpoint
        IsEnabled="True"
        IsSet="True"
        Line="15" />
    </Breakpoint2>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>Main 1\r\n</LogMessage>
    <Paused>Breakpoint_Tests.cs:14,4-14,49</Paused>
    <LogMessage>Main 2\r\n</LogMessage>
    <Paused>Breakpoint_Tests.cs:16,4-16,49</Paused>
    <LogMessage>Main 3\r\n</LogMessage>
    <Paused>Breakpoint_Tests.cs:17,4-17,40</Paused>
    <Exited />
    <Breakpoint1>
      <Breakpoint
        IsEnabled="True"
        Line="14" />
    </Breakpoint1>
    <Breakpoint2>
      <Breakpoint
        IsEnabled="True"
        Line="15" />
    </Breakpoint2>
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
