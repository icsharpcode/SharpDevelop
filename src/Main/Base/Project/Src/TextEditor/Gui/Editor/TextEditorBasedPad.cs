// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing.Printing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Description of TextEditorBasedPad.
	/// </summary>
	public abstract class TextEditorBasedPad : AbstractPadContent, IEditable, IUndoHandler, IPositionable, ITextEditorControlProvider, IPrintable, IToolsHost, IClipboardHandler
	{
		public abstract TextEditorControl TextEditorControl {
			get;
		}
		
		public override System.Windows.Forms.Control Control {
			get { return this.TextEditorControl; }
		}
		
		bool IUndoHandler.EnableUndo {
			get {
				return this.TextEditorControl.EnableUndo;
			}
		}
		
		bool IUndoHandler.EnableRedo {
			get {
				return this.TextEditorControl.EnableRedo;
			}
		}
		
		string GetText()
		{
			return this.TextEditorControl.Document.TextContent;
		}
		
		void SetText(string value)
		{
			this.TextEditorControl.Document.Replace(0, this.TextEditorControl.Document.TextLength, value);
		}
		
		string IEditable.Text {
			get {
				if (WorkbenchSingleton.InvokeRequired)
					return WorkbenchSingleton.SafeThreadFunction<string>(GetText);
				else
					return GetText();
			}
			set {
				if (WorkbenchSingleton.InvokeRequired)
					WorkbenchSingleton.SafeThreadCall(SetText, value);
				else
					SetText(value);
			}
		}
		
		public virtual IDocument GetDocumentForFile(OpenedFile file)
		{
			return null;
		}
		
		PrintDocument IPrintable.PrintDocument {
			get {
				return this.TextEditorControl.PrintDocument;
			}
		}
		
		void IUndoHandler.Undo()
		{
			this.TextEditorControl.Undo();
		}
		
		void IUndoHandler.Redo()
		{
			this.TextEditorControl.Redo();
		}
		
		public override void RedrawContent()
		{
			this.TextEditorControl.OptionsChanged();
			this.TextEditorControl.Refresh();
		}
		
		#region IPositionable implementation
		void IPositionable.JumpTo(int line, int column)
		{
			this.TextEditorControl.ActiveTextAreaControl.JumpTo(line, column);
			
			// we need to delay this call here because the text editor does not know its height if it was just created
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					this.TextEditorControl.ActiveTextAreaControl.CenterViewOn(
						line, (int)(0.3 * this.TextEditorControl.ActiveTextAreaControl.TextArea.TextView.VisibleLineCount));
				});
		}
		
		int IPositionable.Line {
			get {
				return this.TextEditorControl.ActiveTextAreaControl.Caret.Line;
			}
		}
		
		int IPositionable.Column {
			get {
				return this.TextEditorControl.ActiveTextAreaControl.Caret.Column;
			}
		}
		#endregion
		
		Control IToolsHost.ToolsControl {
			get { return TextEditorSideBar.Instance; }
		}
		
		#region ICSharpCode.SharpDevelop.Gui.IClipboardHandler interface implementation
		public bool EnableCut {
			get {
				return this.TextEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCut;
			}
		}
		
		public bool EnableCopy {
			get {
				return this.TextEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCopy;
			}
		}
		
		public bool EnablePaste {
			get {
				return this.TextEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste;
			}
		}
		
		public bool EnableDelete {
			get {
				return this.TextEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableDelete;
			}
		}
		
		public bool EnableSelectAll {
			get {
				return this.TextEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableSelectAll;
			}
		}
		
		public void SelectAll()
		{
			this.TextEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(null, null);
		}
		
		public void Delete()
		{
			this.TextEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(null, null);
		}
		
		public void Paste()
		{
			this.TextEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null, null);
		}
		
		public void Copy()
		{
			this.TextEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, null);
		}
		
		public void Cut()
		{
			this.TextEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null);
		}
		#endregion
	}
}
