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
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger
{
	public class DebugeeInterpreterContext: InterpreterContext
	{
		Process process;
		Value interpreter;
		Value interpreter_localVariable;
		
		public DebugeeInterpreterContext()
		{
			this.Title = "${res:ICSharpCode.BooInterpreter.Debuggee.Title}";
			this.ToolTipText = "${res:ICSharpCode.BooInterpreter.Debuggee.ToolTip}";
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
				PrintLine(ResourceService.GetString("ICSharpCode.BooInterpreter.Debuggee.ErrorDebuggerNotLoaded"));
				return false;
			}
			WindowsDebugger winDebugger = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (winDebugger == null) {
				PrintLine(ResourceService.GetString("ICSharpCode.BooInterpreter.Debuggee.ErrorIncompatibleDebugger"));
				return false;
			}
			if (winDebugger.DebuggedProcess == null) {
				PrintLine(ResourceService.GetString("ICSharpCode.BooInterpreter.Debuggee.ErrorNoProgramDebugged"));
				return false;
			}
			process = winDebugger.DebuggedProcess;
			process.Expired += delegate { interpreter = null; };
			process.LogMessage -= OnDebuggerLogMessage;
			process.LogMessage += OnDebuggerLogMessage;
			
			Value assembly;
			// Boo.Lang.Interpreter.dll
			string path = Path.Combine(Path.GetDirectoryName(typeof(InterpreterContext).Assembly.Location), "Boo.Lang.Interpreter.dll");
			assembly = LoadAssembly(path);
			// Debugger.BooInterpreter.dll
			assembly = LoadAssembly(typeof(DebugeeInteractiveInterpreter).Assembly.Location);
			Value interpreterType = Eval.NewString(process, typeof(DebugeeInteractiveInterpreter).FullName);
			interpreter = Eval.InvokeMethod(process, typeof(Assembly), "CreateInstance", assembly, new Value[] {interpreterType});
			interpreter_localVariable = interpreter.GetMember("localVariable");
			RunCommand(
				"import System\n" + 
				"import System.IO\n" +
				"import System.Text\n" +
				"interpreter.RememberLastValue = true\n" +
				"interpreter.Print = def(msg): System.Diagnostics.Debugger.Log(0xB00, \"DebugeeInterpreterContext.PrintLine\", msg)");
			
			return true;
		}
		
		Value LoadAssembly(string path)
		{
			Value assemblyPath = Eval.NewString(process, path);
			Value assembly = Eval.InvokeMethod(process, typeof(Assembly), "LoadFrom", null, new Value[] {assemblyPath});
			return assembly;
		}
		
		public override void RunCommand(string code)
		{
			if (CanLoadInterpreter) {
				Value cmd = Eval.NewString(process, code);
				Eval.InvokeMethod(process, typeof(InteractiveInterpreter), "LoopEval", interpreter, new Value[] {cmd});
			}
		}
		
		public override string[] GetGlobals()
		{
			return null;
		}
	
		public override string[] SuggestCodeCompletion(string code)
		{
			if (CanLoadInterpreter) {
				Value cmd = Eval.NewString(process, code);
				Eval.InvokeMethod(process, typeof(AbstractInterpreter), "SuggestCodeCompletion", interpreter, new Value[] {cmd});
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
					case "DebugeeInterpreterContext.BeforeSetValue":
						BeforeSetValue(e.Message);
						break;
				}
			}
		}
		
		void BeforeGetValue(string name)
		{
			NamedValue localVar;
			try {
				localVar = process.LocalVariables[name];
			} catch (DebuggerException) {
				return;
			}
			PrintLine("Getting local variable " + name);
			// First, get out of GC unsafe point
			Stepper stepOut = new Stepper(process.SelectedThread.LastFunction, "Boo interperter");
			stepOut.StepComplete  += delegate {
				process.Debugger.MTA2STA.AsyncCall(delegate {
					if (!interpreter_localVariable.SetValue(localVar)) {
						PrintLine("Getting of local variable " + name + " failed");
					}
					process.Continue();
				});
			};
			stepOut.StepOut();
		}
		
		void BeforeSetValue(string name)
		{
			NamedValue localVar;
			try {
				localVar = process.LocalVariables[name];
			} catch (DebuggerException) {
				return;
			}
			PrintLine("Setting local variable " + name);
			localVar.SetValue(interpreter_localVariable);
		}
	}
}
