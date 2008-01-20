// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class StackOverflow
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			new StackOverflow().Fun(0);
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
		[NUnit.Framework.Test]
		public void StackOverflow()
		{
			StartTest("StackOverflow.cs");
			
			process.Continue();
			ObjectDump("LastStackFrame", process.SelectedThread.MostRecentStackFrame);
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="StackOverflow.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">StackOverflow.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <ExceptionThrown>&lt;null&gt;</ExceptionThrown>
    <DebuggingPaused>Exception</DebuggingPaused>
    <LastStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.StackOverflow.Fun">
      <ArgumentCount>1</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo>Fun</MethodInfo>
      <NextStatement>Start=21,3 End=21,4</NextStatement>
    </LastStackFrame>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT