// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.AvalonEdit.Gui;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Wraps AvalonEdit to provide the ITextEditor interface.
	/// </summary>
	public class AvalonEditTextEditorAdapter : ITextEditor
	{
		readonly TextEditor textEditor;
		readonly AvalonEditDocumentAdapter document;
		
		public AvalonEditTextEditorAdapter(TextEditor textEditor)
		{
			this.textEditor = textEditor;
			this.document = new AvalonEditDocumentAdapter(textEditor.Document);
		}
		
		public IDocument Document {
			get { return document; }
		}
		
		public ITextEditorCaret Caret {
			get {
				throw new NotImplementedException();
			}
		}
		
		sealed class CaretAdapter : ITextEditorCaret
		{
			Caret caret;
			
			public CaretAdapter(Caret caret)
			{
				Debug.Assert(caret != null);
				this.caret = caret;
			}
			
			public int Offset {
				get { return caret.Offset; }
				set { caret.Offset = value; }
			}
			
			public int Line {
				get { return caret.Line; }
				set { caret.Line = value;}
			}
			
			public int Column {
				get { return caret.Column; }
				set { caret.Column = value; }
			}
			
			public ICSharpCode.NRefactory.Location Position {
				get { return AvalonEditDocumentAdapter.ToLocation(caret.Position); }
				set { caret.Position = new TextViewPosition(AvalonEditDocumentAdapter.ToPosition(value)); }
			}
		}
		
		public string FileName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void ShowInsightWindow(ICSharpCode.TextEditor.Gui.InsightWindow.IInsightDataProvider provider)
		{
			throw new NotImplementedException();
		}
		
		public void ShowCompletionWindow(ICSharpCode.TextEditor.Gui.CompletionWindow.ICompletionDataProvider provider, char ch)
		{
			throw new NotImplementedException();
		}
		
		public string GetWordBeforeCaret()
		{
			throw new NotImplementedException();
		}
		
		public object GetService(Type serviceType)
		{
			return textEditor.TextArea.GetService(serviceType);
		}
		
		public int SelectionStart {
			get {
				return textEditor.SelectionStart;
			}
		}
		
		public int SelectionLength {
			get {
				return textEditor.SelectionLength;
			}
		}
		
		public void Select(int selectionStart, int selectionLength)
		{
			textEditor.Select(selectionStart, selectionLength);
		}
	}
}
