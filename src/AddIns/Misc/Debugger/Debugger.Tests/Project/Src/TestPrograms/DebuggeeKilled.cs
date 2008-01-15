// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class DebuggeeKilled
	{
		public static void Main()
		{
			int id = System.Diagnostics.Process.GetCurrentProcess().Id;
			System.Diagnostics.Debug.WriteLine(id.ToString());
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
//		[NUnit.Framework.Test]
//		public void DebuggeeKilled()
//		{
//			StartTest("DebuggeeKilled.cs");
//			WaitForPause();
//			Assert.AreNotEqual(null, lastLogMessage);
//			System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(int.Parse(lastLogMessage));
//			p.Kill();
//			process.WaitForExit();
//		}
	}
}
#endif

#if EXPECTED_OUTPUT
#endif // EXPECTED_OUTPUT