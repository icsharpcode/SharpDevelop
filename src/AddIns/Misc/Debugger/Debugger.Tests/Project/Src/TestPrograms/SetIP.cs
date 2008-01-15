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

#if TESTS
namespace Debugger.Tests {
	using NUnit.Framework;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void SetIP()
		{
			StartTest("SetIP");
			WaitForPause();
			
			Assert.IsNotNull(process.SelectedStackFrame.CanSetIP("SetIP.cs", 16, 0));
			Assert.IsNull(process.SelectedStackFrame.CanSetIP("SetIP.cs", 100, 0));
			process.SelectedStackFrame.SetIP("SetIP.cs", 16, 0);
			process.Continue();
			WaitForPause();
			Assert.AreEqual("1\r\n1\r\n", log);
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif