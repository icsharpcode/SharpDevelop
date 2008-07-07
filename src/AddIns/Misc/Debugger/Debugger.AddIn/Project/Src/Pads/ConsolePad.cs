// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.AddIn.TreeModel;
using System;
using System.Windows.Forms;
using Debugger;
using Debugger.AddIn;
using ICSharpCode.NRefactory;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class ConsolePad: DebuggerPad
	{
		ConsoleControl editor;
		
		public override Control Control {
			get {
				return editor;
			}
		}
		
		protected override void InitializeComponents()
		{
			editor = new ConsoleControl();
		}
		
		protected override void SelectProcess(Debugger.Process process)
		{
			editor.Process = process;
		}
	}
	
	class ConsoleControl: TextEditorControl
	{
		Process process;
		
		public Process Process {
			get { return process; }
			set { process = value; }
		}
		
		public ConsoleControl()
		{
			SetHighlighting("C#");
			PrintPrompt();
		}
		
		protected override void InitializeTextAreaControl(TextAreaControl newControl)
		{
			newControl.TextArea.DoProcessDialogKey += HandleDialogKey;
			newControl.TextArea.KeyEventHandler += HandleKey;
		}
		
		void PrintPrompt()
		{
			this.Document.Insert(this.Document.TextLength, "> ");
			this.ActiveTextAreaControl.Caret.Position = this.Document.OffsetToPosition(this.Document.TextLength);
		}
		
		string GetLastLineText()
		{
			LineSegment seg = this.Document.LineSegmentCollection[this.Document.LineSegmentCollection.Count - 1];
			return this.Document.GetText(seg.Offset, seg.Length).Substring(2);
		}
		
		bool HandleDialogKey(Keys keys)
		{
			// All lines except the last one are read-only
			int codeStart = this.Document.PositionToOffset(new TextLocation(2, this.Document.LineSegmentCollection.Count - 1));
			this.Document.ReadOnly = (this.ActiveTextAreaControl.Caret.Offset < codeStart);
			
			if (keys == Keys.Back) {
				if (this.ActiveTextAreaControl.Caret.Offset <= codeStart) {
					return true;
				}
			}
			if (keys == Keys.Enter) {
				string code = GetLastLineText();
				if (string.IsNullOrEmpty(code)) return true;
				string result = Evaluate(code);
				if (string.IsNullOrEmpty(result)) {
					this.Document.Insert(this.Document.TextLength, Environment.NewLine);
				} else {
					this.Document.Insert(this.Document.TextLength, Environment.NewLine + result + Environment.NewLine);
				}
				PrintPrompt();
				return true;
			}
			return false;
		}
		
		bool HandleKey(char ch)
		{
			return false;
		}
		
		string Evaluate(string code)
		{
			if (process == null) {
				return "No process is being debugged";
			}
			if (process.IsRunning) {
				return "The process is running";
			}
			try {
				Value val = AstEvaluator.Evaluate(code, SupportedLanguage.CSharp, process.SelectedStackFrame);
				if (val != null) {
					return string.Format("{0} ({1})", val.AsString, val.Type.FullName);
				} else {
					return string.Empty;
				}
			} catch (GetValueException e) {
				return e.Message;
			}
		}
	}
}
