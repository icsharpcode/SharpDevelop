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

#if TESTS
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Breakpoint()
		{
			Breakpoint breakpoint = debugger.AddBreakpoint(@"F:\SharpDevelopTrunk\src\AddIns\Misc\Debugger\Debugger.Tests\Project\Src\TestPrograms\Breakpoint.cs", 18);
			
			StartTest("Breakpoint");
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