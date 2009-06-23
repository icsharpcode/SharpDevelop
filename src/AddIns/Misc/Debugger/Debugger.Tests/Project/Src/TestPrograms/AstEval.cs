// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace Debugger.Tests.TestPrograms
{
	public class AstEval
	{
		public static void Main()
		{
			int index = 4;
			
			char[] array = "Hello".ToCharArray();
			char[] array2 = "world".ToCharArray();
			char[][] arrays = new char[][] {array, array2};
			List<char> list = new List<char>(array);
			
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using Debugger.AddIn;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void AstEval()
		{
			StartTest("AstEval.cs");
			
			Eval("array");
			Eval("arrays");
			Eval("array[1]");
			Eval("array[index]");
			Eval("array[index - 1]");
			Eval("list");
			Eval("list[1]");
			Eval("list[index]");
			
			EndTest();
		}
		
		void Eval(string expr)
		{
			string restultFmted;
			try {
				Value result = AstEvaluator.Evaluate(expr, SupportedLanguage.CSharp, process.SelectedStackFrame);
				restultFmted = AstEvaluator.FormatValue(result);
			} catch (GetValueException e) {
				restultFmted = "Error: " + e.Message;
			}
			ObjectDump("Eval", " " + expr + " = " + restultFmted + " ");
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="AstEval.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>AstEval.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break AstEval.cs:24,4-24,40</DebuggingPaused>
    <Eval> array = Char[] {H, e, l, l, o} </Eval>
    <Eval> arrays = Char[][] {Char[] {H, e, l, l, o}, Char[] {w, o, r, l, d}} </Eval>
    <Eval> array[1] = e </Eval>
    <Eval> array[index] = o </Eval>
    <Eval> array[index - 1] = Error: BinaryOperator: Subtract </Eval>
    <Eval> list = System.Collections.Generic.List`1[System.Char] </Eval>
    <Eval> list[1] = e </Eval>
    <Eval> list[index] = o </Eval>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT