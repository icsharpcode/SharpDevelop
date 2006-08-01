// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Reflection;

using Boo.Lang.Interpreter;
using Boo.InterpreterAddIn;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger
{
	public class DebugeeInterpreterContext: InterpreterContext
	{
		NDebugger debugger;
		Variable interpreter;
		
		public DebugeeInterpreterContext()
		{
			this.Title = "Debuggee";
			this.ToolTipText = "Runs commands in the debugged progam";
			this.Image = ICSharpCode.Core.ResourceService.GetBitmap("Boo.ProjectIcon");
		}
		
		bool CanLoadInterpreter {
			get {
				return interpreter != null || InjectInterpreter();
			}
		}
		
		bool InjectInterpreter()
		{
			if (!DebuggerService.IsDebuggerLoaded) {
				PrintLine("Error: 'Debugger is not loaded'");
				return false;
			}
			WindowsDebugger winDebugger = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (winDebugger == null) {
				PrintLine("Error: 'Incompatible debugger'");
				return false;
			}
			if (!winDebugger.IsDebugging) {
				PrintLine("Error: 'No program is debugged'");
				return false;
			}
			debugger = winDebugger.DebuggerCore;
			debugger.SelectedProcess.Expired += delegate { interpreter = null; };
			debugger.LogMessage -= OnDebuggerLogMessage;
			debugger.LogMessage += OnDebuggerLogMessage;
			
			Variable assembly;
			// Boo.Lang.Interpreter.dll
			string path = Path.Combine(Path.GetDirectoryName(typeof(InterpreterContext).Assembly.Location), "Boo.Lang.Interpreter.dll");
			assembly = LoadAssembly(path);
			// Debugger.BooInterpreter.dll
			assembly = LoadAssembly(typeof(DebugeeInteractiveInterpreter).Assembly.Location);
			Variable interpreterType = Eval.NewString(debugger, typeof(DebugeeInteractiveInterpreter).FullName).EvaluateNow();
			interpreter = Eval.CallFunction(debugger, typeof(Assembly), "CreateInstance", false, assembly, new Variable[] {interpreterType}).EvaluateNow();
			RunCommand(
				"import System\n" + 
				"import System.IO\n" +
				"import System.Text\n" +
				"interpreter.RememberLastValue = true\n" +
				"interpreter.Print = def(msg): System.Diagnostics.Debugger.Log(0xB00, \"DebugeeInterpreterContext.PrintLine\", msg)");
			
			return true;
		}
		
		Variable LoadAssembly(string path)
		{
			Variable assemblyPath = Eval.NewString(debugger, path).EvaluateNow();
			Variable assembly = Eval.CallFunction(debugger, typeof(Assembly), "LoadFrom", false, null, new Variable[] {assemblyPath}).EvaluateNow();
			return assembly;
		}
		
		public override void RunCommand(string code)
		{
			if (CanLoadInterpreter) {
				Variable cmd = Eval.NewString(debugger, code).EvaluateNow();
				Eval.CallFunction(debugger, typeof(InteractiveInterpreter), "LoopEval", false, interpreter, new Variable[] {cmd}).EvaluateNow();
			}
		}
		
		public override string[] GetGlobals()
		{
			return null;
		}
	
		public override string[] SuggestCodeCompletion(string code)
		{
			if (CanLoadInterpreter) {
				Variable cmd = Eval.NewString(debugger, code).EvaluateNow();
				Eval.CallFunction(debugger, typeof(AbstractInterpreter), "SuggestCodeCompletion", false, interpreter, new Variable[] {cmd}).EvaluateNow();
				return null;
			} else {
				return null;
			}
		}
		
		void OnDebuggerLogMessage(object sender, MessageEventArgs e)
		{
			if (e.Level == 0xB00 && interpreter != null) {
				switch (e.Category) {
					case "DebugeeInterpreterContext.PrintLine":
						PrintLine(e.Message);
						break;
					case "DebugeeInterpreterContext.BeforeGetValue":
						BeforeGetValue(e.Message);
						break;
					case "DebugeeInterpreterContext.AfterSetValue":
						AfterSetValue(e.Message);
						break;
				}
			}
		}
		
		void BeforeGetValue(string name)
		{
			// PrintLine("BeforeGetValue: " + name);
			try {
				Variable localVar = debugger.LocalVariables[name];
				PrintLine("Warning: 'Getting of local variables not implemented'");
			} catch (DebuggerException) {
			}
		}
		
		void AfterSetValue(string name)
		{
			//PrintLine("AfterSetValue: " + name);
			try {
				Variable localVar = debugger.LocalVariables[name];
				PrintLine("Warning: 'Setting of local variables not implemented'");
			} catch (DebuggerException) {
			}
		}
		
		void SetValueInternal(string valueName, Variable newValue)
		{
			Variable name = Eval.NewString(debugger, valueName).Result;
			Eval.CallFunction(debugger, typeof(DebugeeInteractiveInterpreter), "SetValueInternal", false, interpreter, new Variable[] {name, newValue}).ScheduleEvaluation();
		}
		
		Variable GetValueInternal(string valueName)
		{
			Variable name = Eval.NewString(debugger, valueName).EvaluateNow();
			return Eval.CallFunction(debugger, typeof(DebugeeInteractiveInterpreter), "GetValueInternal", false, interpreter, new Variable[] {name}).EvaluateNow();
		}
	}
}
