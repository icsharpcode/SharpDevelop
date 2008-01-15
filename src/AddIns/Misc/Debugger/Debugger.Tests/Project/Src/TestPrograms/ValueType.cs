// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests
{
	public struct ValueType
	{
		public static void Main()
		{
			new ValueType().Fun();
		}
		
		public void Fun()
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
		public void ValueType()
		{
			StartTest("ValueType");
			WaitForPause();
			
			ObjectDump("this", process.SelectedStackFrame.ThisValue);
			ObjectDump("typeof(this)", process.SelectedStackFrame.ThisValue.Type);
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif