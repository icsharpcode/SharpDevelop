// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			
			Assert.IsNotNull(process.SelectedStackFrame.CanSetIP("StackFrame_SetIP.cs", 12, 0));
			Assert.IsNull(process.SelectedStackFrame.CanSetIP("StackFrame_SetIP.cs", 100, 0));
			process.SelectedStackFrame.SetIP("StackFrame_SetIP.cs", 12, 0);
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
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>StackFrame_SetIP.exe (Has symbols)</ModuleLoaded>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>1\r\n</LogMessage>
    <DebuggingPaused>Break StackFrame_SetIP.cs:13,4-13,40</DebuggingPaused>
    <DebuggingPaused>SetIP StackFrame_SetIP.cs:12,4-12,44</DebuggingPaused>
    <LogMessage>1\r\n</LogMessage>
    <DebuggingPaused>Break StackFrame_SetIP.cs:13,4-13,40</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
