// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>
using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// A text area used for an interactive command prompt.
	/// </summary>
	public abstract class CommandPromptControl : SharpDevelopTextAreaControl
	{
		TextMarker readOnlyMarker;
		int promptStartOffset;
		int promptEndOffset;
		
		public CommandPromptControl()
			: base(false, false)
		{
			this.TextEditorProperties.SupportReadOnlySegments = true;
			this.TextEditorProperties.ShowLineNumbers = false;
			base.contextMenuPath = null;
		}
		
		protected void PrintPrompt()
		{
			promptStartOffset = this.Document.TextLength;
			PrintPromptInternal();
			promptEndOffset = this.Document.TextLength;
			MakeReadOnly();
		}
		
		protected virtual void Clear()
		{
			if (readOnlyMarker != null)
				this.Document.MarkerStrategy.RemoveMarker(readOnlyMarker);
			this.Document.Remove(0, this.Document.TextLength);
			this.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			this.Document.CommitUpdate();
			readOnlyMarker = null;
			promptStartOffset = 0;
			promptEndOffset = 0;
		}
		
		protected abstract void PrintPromptInternal();
		
		/// <summary>
		/// Makes the current control content read-only (but still allows appending more content at the end).
		/// </summary>
		protected void MakeReadOnly()
		{
			if (this.Document.TextLength == 0)
				return;
			this.ActiveTextAreaControl.Caret.Position = this.Document.OffsetToPosition(this.Document.TextLength);
			if (readOnlyMarker == null) {
				readOnlyMarker = new TextMarker(0, this.Document.TextLength, TextMarkerType.Invisible) { IsReadOnly = true };
				this.Document.MarkerStrategy.AddMarker(readOnlyMarker);
			}
			readOnlyMarker.Offset = 0;
			readOnlyMarker.Length = this.Document.TextLength;
			this.Document.UndoStack.ClearAll(); // prevent user from undoing the prompt insertion
		}
		
		protected override void InitializeTextAreaControl(TextAreaControl newControl)
		{
			newControl.TextArea.DoProcessDialogKey += HandleDialogKey;
		}
		
		/// <summary>
		/// Gets the current command (text from end of prompt to end of document)
		/// </summary>
		protected string GetCommand()
		{
			return this.Document.GetText(promptEndOffset, this.Document.TextLength - promptEndOffset);
		}
		
		bool HandleDialogKey(Keys keys)
		{
			if (keys == Keys.Enter) {
				AcceptCommand(GetCommand());
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Causes evaluation of a command when the user presses Enter.
		/// </summary>
		/// <param name="command">The command to evaluate.</param>
		protected abstract void AcceptCommand(string command);
		
		/// <summary>
		/// Appends text at the end of the document.
		/// </summary>
		protected void Append(string text)
		{
			this.Document.Insert(this.Document.TextLength, text);
		}
		
		protected void InsertLineBeforePrompt(string text)
		{
			text += Environment.NewLine;
			this.Document.Insert(promptStartOffset, text);
			promptStartOffset += text.Length;
			promptEndOffset += text.Length;
			if (readOnlyMarker != null) {
				readOnlyMarker.Offset = 0;
				readOnlyMarker.Length = promptEndOffset;
			}
		}
	}
}
