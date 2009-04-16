// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Gui;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Wraps AvalonEdit to provide the ITextEditor interface.
	/// </summary>
	public class AvalonEditTextEditorAdapter : ITextEditor, IWeakEventListener
	{
		readonly TextEditor textEditor;
		AvalonEditDocumentAdapter document;
		
		public AvalonEditTextEditorAdapter(TextEditor textEditor)
		{
			this.textEditor = textEditor;
			this.Caret = new CaretAdapter(textEditor.TextArea.Caret);
			this.Options = new OptionsAdapter(textEditor.Options);
			TextEditorWeakEventManager.DocumentChanged.AddListener(textEditor, this);
			OnDocumentChanged();
		}
		
		protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(TextEditorWeakEventManager.DocumentChanged)) {
				OnDocumentChanged();
				return true;
			}
			return false;
		}
		
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			return ReceiveWeakEvent(managerType, sender, e);
		}
		
		void OnDocumentChanged()
		{
			if (textEditor.Document != null)
				document = new AvalonEditDocumentAdapter(textEditor.Document, this);
			else
				document = null;
		}
		
		public IDocument Document {
			get { return document; }
		}
		
		public ITextEditorCaret Caret { get; private set; }
		public ITextEditorOptions Options { get; private set; }
		
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
		
		sealed class OptionsAdapter : ITextEditorOptions
		{
			TextEditorOptions avalonEditOptions;
			
			public OptionsAdapter(TextEditorOptions avalonEditOptions)
			{
				this.avalonEditOptions = avalonEditOptions;
			}
			
			public string IndentationString {
				get {
					return avalonEditOptions.IndentationString;
				}
			}
		}
		
		public virtual string FileName {
			get { return null; }
		}
		
		public virtual void ShowInsightWindow(ICSharpCode.TextEditor.Gui.InsightWindow.IInsightDataProvider provider)
		{
		}
		
		public virtual void ShowCompletionWindow(ICSharpCode.TextEditor.Gui.CompletionWindow.ICompletionDataProvider provider, char ch)
		{
		}
		
		public void ShowCompletionWindow(ICompletionItemList data)
		{
			if (data == null || !data.Items.Any())
				return;
			CompletionWindow window = CreateCompletionWindow(data);
			if (window != null)
				window.Show();
		}
		
		protected virtual CompletionWindow CreateCompletionWindow(ICompletionItemList data)
		{
			return new SharpDevelopCompletionWindow(this, textEditor.TextArea, data);
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
		
		public string SelectedText {
			get {
				return textEditor.SelectedText;
			}
			set {
				textEditor.SelectedText = value;
			}
		}
		
		public event EventHandler SelectionChanged {
			add    { textEditor.TextArea.SelectionChanged += value; }
			remove { textEditor.TextArea.SelectionChanged -= value; }
		}
		
		public void Select(int selectionStart, int selectionLength)
		{
			textEditor.Select(selectionStart, selectionLength);
		}
		
		public void JumpTo(int line, int column)
		{
			textEditor.TextArea.Selection = Selection.Empty;
			textEditor.TextArea.Caret.Position = new TextViewPosition(line, column, -1);
			textEditor.TextArea.Caret.BringCaretToView();
		}
	}
}
