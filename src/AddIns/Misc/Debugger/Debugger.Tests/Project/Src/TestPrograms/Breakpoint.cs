// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Breakpoint
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			System.Diagnostics.Debug.WriteLine("Mark 1");
			System.Diagnostics.Debug.WriteLine("Mark 2"); // Breakpoint
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Breakpoint()
		{
			Breakpoint breakpoint = debugger.AddBreakpoint(@"F:\SharpDevelopTrunk\src\AddIns\Misc\Debugger\Debugger.Tests\Project\Src\TestPrograms\Breakpoint.cs", 18);
			
			StartTest("Breakpoint.cs");
			WaitForPause();
			
			ObjectDump(breakpoint);
			
			process.Continue();
			WaitForPause();
			
			process.Continue();
			WaitForPause();
			
			process.Continue();
			process.WaitForExit();
			
			ObjectDump(breakpoint);
			CheckXmlOutput();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="Breakpoint.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">Breakpoint.exe</ModuleLoaded>
    <ModuleLoaded symbols="False">System.dll</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <Object Type="Breakpoint" ToString="Debugger.Breakpoint">
      <SourcecodeSegment>Start=18,0 End=18,0</SourcecodeSegment>
      <HadBeenSet>True</HadBeenSet>
      <Enabled>True</Enabled>
    </Object>
    <ModuleLoaded symbols="False">System.Configuration.dll</ModuleLoaded>
    <ModuleLoaded symbols="False">System.Xml.dll</ModuleLoaded>
    <LogMessage>Mark 1\r\n</LogMessage>
    <DebuggingPaused>Breakpoint</DebuggingPaused>
    <LogMessage>Mark 2\r\n</LogMessage>
    <DebuggingPaused>Break</DebuggingPaused>
    <ProcessExited />
    <Object Type="Breakpoint" ToString="Debugger.Breakpoint">
      <SourcecodeSegment>Start=18,0 End=18,0</SourcecodeSegment>
      <HadBeenSet>False</HadBeenSet>
      <Enabled>True</Enabled>
    </Object>
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT