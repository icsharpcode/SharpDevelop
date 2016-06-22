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
	public class Exception_StackOverflow
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			new Exception_StackOverflow().Fun(0);
		}
		
		public int Fun(int i)
		{
			return Fun(i + 1);
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
//		The location where the process will break is non-deterministic
//
//		[NUnit.Framework.Test]
//		public void Exception_StackOverflow()
//		{
//			StartTest();
//			
//			process.Continue();
//			//ObjectDump("LastStackFrame", process.SelectedThread.MostRecentStackFrame);
//			
//			EndTest();
//		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Exception_StackOverflow.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Exception_StackOverflow.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break Exception_StackOverflow.cs:12,4-12,40</DebuggingPaused>
    <ExceptionThrown>Could not intercept: System.StackOverflowException</ExceptionThrown>
    <DebuggingPaused>Exception Exception_StackOverflow.cs:17,3-17,4</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
