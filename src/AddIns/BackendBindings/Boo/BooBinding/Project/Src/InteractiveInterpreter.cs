// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop;
using System;
using System.Reflection;
using System.Windows.Forms;
using Boo.Lang.Interpreter;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
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
		
		protected override void AcceptCommand(string command)
		{
			if (command.EndsWith("\n")) {
				ProcessCommand(command);
			} else if (!command.Contains("\n") && !command.EndsWith(":")) {
				ProcessCommand(command);
			} else {
				TextEditor.Caret.Position = TextEditor.Document.OffsetToPosition(TextEditor.Document.TextLength);
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
				interpreter.Print = PrintLine;
				interpreter.SetValue("cls", new Action(Clear));
				
				AddReference(typeof(WorkbenchSingleton).Assembly);
				
				interpreter.LoopEval("import System\n" +
				                     "import System.Collections.Generic\n" +
				                     "import System.IO\n" +
				                     "import System.Text");
			}
			processing = true;
			try {
				interpreter.LoopEval(command);
			} catch (System.Reflection.TargetInvocationException ex) {
				PrintLine(ex.InnerException);
			}
			processing = false;
			
			Append(Environment.NewLine);
		}
		
		void PrintLine(object text)
		{
			if (text == null)
				return;
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadAsyncCall(PrintLine, text);
			} else {
				if (processing)
					Append(text.ToString());
				else
					InsertLineBeforePrompt(text.ToString());
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
