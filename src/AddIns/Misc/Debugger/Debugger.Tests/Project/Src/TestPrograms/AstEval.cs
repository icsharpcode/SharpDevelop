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
			byte b = 1;
			int i = 4;
			float pi = 3.14f;
			string hi = "hi";
			
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
			
			Eval("b");
			Eval("i");
			Eval("pi");
			Eval("pi - 3");
			Eval("b + i");
			Eval("i + b");
			Eval("b + pi");
			Eval("pi + b");
			Eval("hi + pi");
			Eval("pi + hi");
			Eval("pi + \" \" + hi");
			
			Eval("(5 + 6) % (1 + 2)");
			Eval("b + 3 == i");
			Eval("b + 4 == i");
			Eval("true == true");
			Eval("true == false");
			
			Eval("array");
			Eval("arrays");
			Eval("array[1]");
			Eval("array[i]");
			Eval("array[i - 1]");
			Eval("list");
			Eval("list[1]");
			Eval("list[i]");
			
			Eval("list.Add(42); list.Add(52);");
			Eval("list");
			
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
    <DebuggingPaused>Break AstEval.cs:27,4-27,40</DebuggingPaused>
    <Eval> b = 1 </Eval>
    <Eval> i = 4 </Eval>
    <Eval> pi = 3.14 </Eval>
    <Eval> pi - 3 = 0.140000104904175 </Eval>
    <Eval> b + i = 5 </Eval>
    <Eval> i + b = 5 </Eval>
    <Eval> b + pi = 4.14000010490417 </Eval>
    <Eval> pi + b = 4.14000010490417 </Eval>
    <Eval> hi + pi = hi3.14 </Eval>
    <Eval> pi + hi = 3.14hi </Eval>
    <Eval> pi + " " + hi = 3.14 hi </Eval>
    <Eval> (5 + 6) % (1 + 2) = 2 </Eval>
    <Eval> b + 3 == i = True </Eval>
    <Eval> b + 4 == i = False </Eval>
    <Eval> true == true = True </Eval>
    <Eval> true == false = False </Eval>
    <Eval> array = Char[] {H, e, l, l, o} </Eval>
    <Eval> arrays = Char[][] {Char[] {H, e, l, l, o}, Char[] {w, o, r, l, d}} </Eval>
    <Eval> array[1] = e </Eval>
    <Eval> array[i] = o </Eval>
    <Eval> array[i - 1] = l </Eval>
    <Eval> list = List&lt;Char&gt; {H, e, l, l, o} </Eval>
    <Eval> list[1] = e </Eval>
    <Eval> list[i] = o </Eval>
    <Eval> list.Add(42); list.Add(52); =  </Eval>
    <Eval> list = List&lt;Char&gt; {H, e, l, l, o, *, 4} </Eval>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT