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
	public class StackFrame_SetIP
	{
		public static void Main()
		{
			System.Diagnostics.Debug.WriteLine("1");
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
		public void StackFrame_SetIP()
		{
			StartTest();
			
			string filename = this.CurrentStackFrame.NextStatement.Filename;
			
			Assert.IsNotNull(this.CurrentStackFrame.SetIP(filename, 12, 0, true));
			Assert.IsFalse(this.CurrentStackFrame.SetIP(filename, 100, 0, true));
			this.CurrentStackFrame.SetIP(filename, 12, 0, false);
			process.Continue();
			Assert.AreEqual("1\r\n1\r\n", log);
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="StackFrame_SetIP.cs">
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>StackFrame_SetIP.exe (Has symbols)</ModuleLoaded>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Core.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>1\r\n</LogMessage>
    <Paused>StackFrame_SetIP.cs:28,4-28,40</Paused>
    <LogMessage>1\r\n</LogMessage>
    <Paused>StackFrame_SetIP.cs:28,4-28,40</Paused>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
