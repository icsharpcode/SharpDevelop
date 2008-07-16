// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class SetIP
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
		public void SetIP()
		{
			StartTest("SetIP.cs");
			
			Assert.IsNotNull(process.SelectedStackFrame.CanSetIP("SetIP.cs", 16, 0));
			Assert.IsNull(process.SelectedStackFrame.CanSetIP("SetIP.cs", 100, 0));
			process.SelectedStackFrame.SetIP("SetIP.cs", 16, 0);
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
    name="SetIP.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>SetIP.exe (Has symbols)</ModuleLoaded>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>1\r\n</LogMessage>
    <DebuggingPaused>Break SetIP.cs:17,4-17,40</DebuggingPaused>
    <DebuggingPaused>SetIP SetIP.cs:16,4-16,44</DebuggingPaused>
    <LogMessage>1\r\n</LogMessage>
    <DebuggingPaused>Break SetIP.cs:17,4-17,40</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT