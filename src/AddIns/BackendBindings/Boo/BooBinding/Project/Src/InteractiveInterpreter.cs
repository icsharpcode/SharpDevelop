// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
	public class InteractiveInterpreterPad : TextEditorBasedPad
	{
		InteractiveInterpreterControl ctl = new InteractiveInterpreterControl();
		
		public override ICSharpCode.TextEditor.TextEditorControl TextEditorControl {
			get {
				return ctl;
			}
		}
	}
	
	sealed class InteractiveInterpreterControl : CommandPromptControl
	{
		InteractiveInterpreter interpreter;
		
		public InteractiveInterpreterControl()
		{
			SetHighlighting("Boo");
			PrintPrompt();
		}
		
		void PrintLine(object text)
		{
			if (text == null)
				return;
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadAsyncCall(PrintLine, text);
			} else {
				if (processing)
					Append(Environment.NewLine + text.ToString());
				else
					InsertLineBeforePrompt(text.ToString());
			}
		}
		
		protected override void PrintPromptInternal()
		{
			Append(">>> ");
		}
		
		protected override void AcceptCommand(string command)
		{
			if (command.EndsWith("\n")) {
				ProcessCommand(command);
			} else if (!command.Contains("\n") && !command.EndsWith(":")) {
				ProcessCommand(command);
			} else {
				Append(Environment.NewLine);
				ActiveTextAreaControl.Caret.Position = this.Document.OffsetToPosition(this.Document.TextLength);
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
			if (this.Document.TextLength != 0) {
				Append(Environment.NewLine);
			}
			PrintPrompt();
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
