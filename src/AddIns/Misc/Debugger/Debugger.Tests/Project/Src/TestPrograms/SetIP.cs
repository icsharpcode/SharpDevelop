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
			WaitForPause();
			
			Assert.IsNotNull(process.SelectedStackFrame.CanSetIP("SetIP.cs", 16, 0));
			Assert.IsNull(process.SelectedStackFrame.CanSetIP("SetIP.cs", 100, 0));
			process.SelectedStackFrame.SetIP("SetIP.cs", 16, 0);
			process.AsyncContinue();
			WaitForPause();
			Assert.AreEqual("1\r\n1\r\n", log);
			
			process.AsyncContinue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="SetIP.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">SetIP.exe</ModuleLoaded>
    <ModuleLoaded symbols="False">System.dll</ModuleLoaded>
    <ModuleLoaded symbols="False">System.Configuration.dll</ModuleLoaded>
    <ModuleLoaded symbols="False">System.Xml.dll</ModuleLoaded>
    <LogMessage>1\r\n</LogMessage>
    <DebuggingPaused>Break</DebuggingPaused>
    <DebuggingPaused>SetIP</DebuggingPaused>
    <LogMessage>1\r\n</LogMessage>
    <DebuggingPaused>Break</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT