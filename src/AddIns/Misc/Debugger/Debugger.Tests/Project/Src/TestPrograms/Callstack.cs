// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Callstack
	{
		public static void Main()
		{
			Sub1();
		}
		
		static void Sub1()
		{
			Sub2();
		}
		
		static void Sub2()
		{
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TESTS
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Callstack()
		{
			StartTest("Callstack");
			WaitForPause();
			ObjectDump("Callstack", process.SelectedThread.Callstack);
			
			process.StepOut();
			WaitForPause();
			ObjectDump("Callstack", process.SelectedThread.Callstack);
			
			process.StepOut();
			WaitForPause();
			ObjectDump("Callstack", process.SelectedThread.Callstack);
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif