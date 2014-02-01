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
	public class ControlFlow_DebuggeeKilled
	{
		public static void Main()
		{
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
		public void ControlFlow_DebuggeeKilled()
		{
			StartTest();
			
			foreach(System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses()) {
				if (p.ProcessName.StartsWith("ControlFlow_DebuggeeKilled"))
					p.Kill();
			}
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
    name="ControlFlow_DebuggeeKilled.cs">
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_DebuggeeKilled.exe (Has symbols)</ModuleLoaded>
    <Paused>ControlFlow_DebuggeeKilled.cs:27,4-27,40</Paused>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
