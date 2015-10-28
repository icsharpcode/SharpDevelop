// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			
			Breakpoint breakpoint1 = debugger.AddBreakpoint(filename, 29);
			Breakpoint breakpoint2 = debugger.AddBreakpoint(filename, 30);
			
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
    <Paused>Breakpoint_Tests.cs:27,4-27,40</Paused>
    <Breakpoint1>
      <Breakpoint
        IsEnabled="True"
        IsSet="True"
        Line="29" />
    </Breakpoint1>
    <Breakpoint2>
      <Breakpoint
        IsEnabled="True"
        IsSet="True"
        Line="30" />
    </Breakpoint2>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Core.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>Main 1\r\n</LogMessage>
    <Paused>Breakpoint_Tests.cs:29,4-29,49</Paused>
    <LogMessage>Main 2\r\n</LogMessage>
    <Paused>Breakpoint_Tests.cs:31,4-31,49</Paused>
    <LogMessage>Main 3\r\n</LogMessage>
    <Paused>Breakpoint_Tests.cs:32,4-32,40</Paused>
    <Exited />
    <Breakpoint1>
      <Breakpoint
        IsEnabled="True"
        Line="29" />
    </Breakpoint1>
    <Breakpoint2>
      <Breakpoint
        IsEnabled="True"
        Line="30" />
    </Breakpoint2>
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
