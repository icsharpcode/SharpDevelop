/*
 * Created by SharpDevelop.
 * User: User
 * Date: 10/07/2006
 * Time: 01:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;

namespace Debugger
{
	public class InterpreterWrapper
	{
		const string booDll = "Boo.Lang.Interpreter.dll";
		
		NDebugger debugger;
		Variable  interpreter;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public static InterpreterWrapper InjectBooInterpreter(NDebugger debugger, string booPath)
		{
			return new InterpreterWrapper(debugger, booPath);
		}
		
		private InterpreterWrapper(NDebugger debugger, string booInterpreterPath)
		{
			this.debugger = debugger;
			
			Variable assemblyPath = Eval.NewString(debugger, booInterpreterPath).EvaluateNow();
			Variable assembly = Eval.CallFunction(debugger, "mscorlib.dll", "System.Reflection.Assembly", "LoadFrom", false, null, new Variable[] {assemblyPath}).EvaluateNow();
			Variable interpreterType = Eval.NewString(debugger, "Boo.Lang.Interpreter.InteractiveInterpreter").EvaluateNow();
			interpreter = Eval.CallFunction(debugger, "mscorlib.dll", "System.Reflection.Assembly", "CreateInstance", false, assembly, new Variable[] {interpreterType}).EvaluateNow();
			RunCommand("interpreter.RememberLastValue = true");
			
			// Testing:
			RunCommand("1 + 2");
			Variable res = GetLastValue();
			SetValue("a", res);
			RunCommand("a = a + 100");
			Variable a = GetValue("a");
			Variable sug = SuggestCodeCompletion("interpreter.__codecomplete__");
		}
		
		
		public void RunCommand(string code)
		{
			Variable cmd = Eval.NewString(debugger, code).EvaluateNow();
			Eval.CallFunction(debugger, booDll, "Boo.Lang.Interpreter.InteractiveInterpreter", "LoopEval", false, interpreter, new Variable[] {cmd}).EvaluateNow();
		}
		
		public void SetValue(string valueName, Variable newValue)
		{
			Variable name = Eval.NewString(debugger, valueName).EvaluateNow();
			Eval.CallFunction(debugger, booDll, "Boo.Lang.Interpreter.InteractiveInterpreter", "SetValue", false, interpreter, new Variable[] {name, newValue}).EvaluateNow();
		}
		
		public Variable GetValue(string valueName)
		{
			Variable name = Eval.NewString(debugger, valueName).EvaluateNow();
			return Eval.CallFunction(debugger, booDll, "Boo.Lang.Interpreter.InteractiveInterpreter", "GetValue", false, interpreter, new Variable[] {name}).EvaluateNow();
		}
		
		public Variable GetLastValue()
		{
			return Eval.CallFunction(debugger, booDll, "Boo.Lang.Interpreter.InteractiveInterpreter", "get_LastValue", false, interpreter, new Variable[] {}).EvaluateNow();
		}
		
		public Variable SuggestCodeCompletion(string code)
		{
			Variable cmd = Eval.NewString(debugger, code).EvaluateNow();
			return Eval.CallFunction(debugger, booDll, "Boo.Lang.Interpreter.AbstractInterpreter", "SuggestCodeCompletion", false, interpreter, new Variable[] {cmd}).EvaluateNow();
		}
	}
}
