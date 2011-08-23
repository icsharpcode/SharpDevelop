// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using Boo.Lang.Compiler;
using Boo.Lang.Interpreter;
using Boo.Lang.Interpreter.Builtins;
using ICSharpCode.SharpDevelop.Gui;

namespace Grunwald.BooBinding
{
	/// <summary>
	/// Interactive Boo interpreter.
	/// </summary>
	public class InteractiveInterpreterPad : AbstractConsolePad
	{
		InteractiveInterpreter interpreter;
		
		protected override string Prompt {
			get {
				return ">>> ";
			}
		}
		
		protected override void InitializeConsole()
		{
			base.InitializeConsole();
			
			SetHighlighting("Boo");
		}
		
		protected override bool AcceptCommand(string command)
		{
			if (command.EndsWith("\n")) {
				ProcessCommand(command);
				return true;
			} else if (!command.Contains("\n") && !command.EndsWith(":")) {
				AppendLine("");
				ProcessCommand(command);
				return true;
			} else {
				return false;
			}
		}
		
		bool processing;
		
		void AddReference(Assembly asm)
		{
			if (!interpreter.References.Contains(asm)) {
				interpreter.References.Add(asm);
				foreach (AssemblyName an in asm.GetReferencedAssemblies()) {
					AddReference(Assembly.Load(an.FullName));
				}
			}
		}
		
		void ProcessCommand(string command)
		{
			if (interpreter == null) {
				interpreter = new InteractiveInterpreter();
				interpreter.RememberLastValue = true;
				//interpreter.Print = PrintLine; TODO reimplement print
				interpreter.SetValue("cls", new Action(Clear));
				
				AddReference(typeof(WorkbenchSingleton).Assembly);
				
				interpreter.Eval("import System\n" +
				                 "import System.Collections.Generic\n" +
				                 "import System.IO\n" +
				                 "import System.Text\n" +
				                 "import System.Linq.Enumerable");
			}
			processing = true;
			try {
				CompilerContext results = interpreter.Eval(command);
				if (results.Errors.Count > 0) {
					PrintLine("ERROR: " + results.Errors[0].Message);
				} else if (interpreter.LastValue != null) {
					PrintLine(ReprModule.repr(interpreter.LastValue));
				}
			} catch (System.Reflection.TargetInvocationException ex) {
				PrintLine(ex.InnerException);
			}
			processing = false;
		}
		
		void PrintLine(object text)
		{
			if (text == null)
				return;
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadAsyncCall(PrintLine, text);
			} else {
				if (processing)
					AppendLine(text.ToString());
				else
					InsertBeforePrompt(text.ToString() + Environment.NewLine);
			}
		}
		
		protected override void Clear()
		{
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadAsyncCall(Clear);
			} else {
				base.Clear();
			}
		}
	}
}
