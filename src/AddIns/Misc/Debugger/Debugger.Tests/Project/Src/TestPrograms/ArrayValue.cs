// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class ArrayValue
	{
		public static void Main()
		{
			int[] array = new int[5];
			for(int i = 0; i < 5; i++) {
				array[i] = i;
			}
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TESTS
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void ArrayValue()
		{
			StartTest("ArrayValue");
			WaitForPause();
			Value array = process.SelectedStackFrame.LocalVariables["array"];
			ObjectDump("array", array);
			ObjectDump("array elements", array.GetArrayElements());
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif