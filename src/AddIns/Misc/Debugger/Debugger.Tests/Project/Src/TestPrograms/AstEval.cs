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
			char[] array1 = "Hello".ToCharArray();
			char[] array2 = "world".ToCharArray();
			char[][] arrays = new char[][] {array1, array2};
			List<char> list = new List<char>(array1);
			
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
			
			Eval("arrays");
			
			EndTest();
		}
		
		void Eval(string expr)
		{
			Value result = AstEvaluator.Evaluate(expr, SupportedLanguage.CSharp, process.SelectedStackFrame);
			string restultFmted = AstEvaluator.FormatValue(result);
			ObjectDump(expr, restultFmted);
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
    <DebuggingPaused>Break AstEval.cs:22,4-22,40</DebuggingPaused>
    <arrays>Char[][] {Char[] {H, e, l, l, o}, Char[] {w, o, r, l, d}}</arrays>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT