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
using System.Threading;

namespace Debugger.Tests
{
	public class ControlFlow_TerminateRunningProcess
	{
		static ManualResetEvent doSomething = new ManualResetEvent(false);
		
		public static void Main()
		{
			int i = 42;
			System.Diagnostics.Debugger.Break();
			doSomething.WaitOne();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using NUnit.Framework;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void ControlFlow_TerminateRunningProcess()
		{
			for(int i = 0; i < 2; i++) {
				StartTest();
				this.CurrentStackFrame.StepOver();
				process.Paused += delegate {
					Assert.Fail("Should not have received any callbacks after Terminate");
				};
				this.CurrentStackFrame.AsyncStepOver();
				ObjectDump("Log", "Calling terminate");
				process.Terminate();
			}
			
			CheckXmlOutput();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="ControlFlow_TerminateRunningProcess.cs">
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_TerminateRunningProcess.exe (Has symbols)</ModuleLoaded>
    <Paused>ControlFlow_TerminateRunningProcess.cs:31,4-31,40</Paused>
    <Paused>ControlFlow_TerminateRunningProcess.cs:32,4-32,26</Paused>
    <Log>Calling terminate</Log>
    <Exited />
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_TerminateRunningProcess.exe (Has symbols)</ModuleLoaded>
    <Paused>ControlFlow_TerminateRunningProcess.cs:31,4-31,40</Paused>
    <Paused>ControlFlow_TerminateRunningProcess.cs:32,4-32,26</Paused>
    <Log>Calling terminate</Log>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
