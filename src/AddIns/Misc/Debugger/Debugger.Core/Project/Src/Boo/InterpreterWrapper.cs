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
		PersistentValue interpreter;
		
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
			
			PersistentValue assemblyPath = Eval.NewString(debugger, booInterpreterPath).EvaluateNow();
			PersistentValue assembly = Eval.CallFunction(debugger, "mscorlib.dll", "System.Reflection.Assembly", "LoadFrom", false, null, new PersistentValue[] {assemblyPath}).EvaluateNow();
			PersistentValue interpreterType = Eval.NewString(debugger, "Boo.Lang.Interpreter.InteractiveInterpreter").EvaluateNow();
			interpreter = Eval.CallFunction(debugger, "mscorlib.dll", "System.Reflection.Assembly", "CreateInstance", false, assembly, new PersistentValue[] {interpreterType}).EvaluateNow();
			RunCommand("interpreter.RememberLastValue = true");
			
			// Testing:
			RunCommand("1 + 2");
			PersistentValue res = GetLastValue();
			SetValue("a", res);
			RunCommand("a = a + 100");
			PersistentValue a = GetValue("a");
			PersistentValue sug = SuggestCodeCompletion("interpreter.__codecomplete__");
		}
		
		
		public void RunCommand(string code)
		{
			PersistentValue cmd = Eval.NewString(debugger, code).EvaluateNow();
			Eval.CallFunction(debugger, booDll, "Boo.Lang.Interpreter.InteractiveInterpreter", "LoopEval", false, interpreter, new PersistentValue[] {cmd}).EvaluateNow();
		}
		
		public void SetValue(string valueName, PersistentValue newValue)
		{
			PersistentValue name = Eval.NewString(debugger, valueName).EvaluateNow();
			Eval.CallFunction(debugger, booDll, "Boo.Lang.Interpreter.InteractiveInterpreter", "SetValue", false, interpreter, new PersistentValue[] {name, newValue}).EvaluateNow();
		}
		
		public PersistentValue GetValue(string valueName)
		{
			PersistentValue name = Eval.NewString(debugger, valueName).EvaluateNow();
			return Eval.CallFunction(debugger, booDll, "Boo.Lang.Interpreter.InteractiveInterpreter", "GetValue", false, interpreter, new PersistentValue[] {name}).EvaluateNow();
		}
		
		public PersistentValue GetLastValue()
		{
			return Eval.CallFunction(debugger, booDll, "Boo.Lang.Interpreter.InteractiveInterpreter", "get_LastValue", false, interpreter, new PersistentValue[] {}).EvaluateNow();
		}
		
		public PersistentValue SuggestCodeCompletion(string code)
		{
			PersistentValue cmd = Eval.NewString(debugger, code).EvaluateNow();
			return Eval.CallFunction(debugger, booDll, "Boo.Lang.Interpreter.AbstractInterpreter", "SuggestCodeCompletion", false, interpreter, new PersistentValue[] {cmd}).EvaluateNow();
		}
	}
}
