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
			bool flag = true;
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
			
			string input = @"
				b
				i
				pi
				pi - 3
				b + i
				i + b
				b + pi
				pi + b
				hi + pi
				pi + hi
				pi + ' ' + hi
				
				(5 + 6) % (1 + 2)
				15 & 255
				15 && 255
				b + 3 == i
				b + 4 == i
				true == true
				true == false
				
				array
				arrays
				array[1]
				array[i]
				array[i - 1]
				list
				list[1]
				list[i]
				hi[1]
				'abcd'[2]
				
				list.Add(42); list.Add(52);
				list
				
				i = 10
				-i
				++i
				i++
				i
				i += 1
				i
				~i
				flag
				!flag
			";
			
			input = input.Replace("'", "\"");
			
			foreach(string line in input.Split('\n')) {
				Eval(line.Trim());
			}
			
			EndTest();
		}
		
		void Eval(string expr)
		{
			string restultFmted;
			if (string.IsNullOrEmpty(expr)) {
				restultFmted = null;
			} else {
				try {
					Value result = AstEvaluator.Evaluate(expr, SupportedLanguage.CSharp, process.SelectedStackFrame);
					restultFmted = AstEvaluator.FormatValue(result);
				} catch (GetValueException e) {
					restultFmted = "Error: " + e.Message;
				}
			}
			if (restultFmted != null) {
				ObjectDump("Eval", " " + expr + " = " + restultFmted + " ");
			} else {
				ObjectDump("Eval", " " + expr);
			}
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
    <DebuggingPaused>Break AstEval.cs:28,4-28,40</DebuggingPaused>
    <Eval> </Eval>
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
    <Eval> </Eval>
    <Eval> (5 + 6) % (1 + 2) = 2 </Eval>
    <Eval> 15 &amp; 255 = 15 </Eval>
    <Eval> 15 &amp;&amp; 255 = Error: Unsupported operator for integers: LogicalAnd </Eval>
    <Eval> b + 3 == i = True </Eval>
    <Eval> b + 4 == i = False </Eval>
    <Eval> true == true = True </Eval>
    <Eval> true == false = False </Eval>
    <Eval> </Eval>
    <Eval> array = Char[] {H, e, l, l, o} </Eval>
    <Eval> arrays = Char[][] {Char[] {H, e, l, l, o}, Char[] {w, o, r, l, d}} </Eval>
    <Eval> array[1] = e </Eval>
    <Eval> array[i] = o </Eval>
    <Eval> array[i - 1] = l </Eval>
    <Eval> list = List&lt;Char&gt; {H, e, l, l, o} </Eval>
    <Eval> list[1] = e </Eval>
    <Eval> list[i] = o </Eval>
    <Eval> hi[1] = i </Eval>
    <Eval> "abcd"[2] = c </Eval>
    <Eval> </Eval>
    <Eval> list.Add(42); list.Add(52);</Eval>
    <Eval> list = List&lt;Char&gt; {H, e, l, l, o, *, 4} </Eval>
    <Eval> </Eval>
    <Eval> i = 10 = 10 </Eval>
    <Eval> -i = -10 </Eval>
    <Eval> ++i = 11 </Eval>
    <Eval> i++ = 11 </Eval>
    <Eval> i = 12 </Eval>
    <Eval> i += 1 = 13 </Eval>
    <Eval> i = 13 </Eval>
    <Eval> ~i = -14 </Eval>
    <Eval> flag = True </Eval>
    <Eval> !flag = False </Eval>
    <Eval> </Eval>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT