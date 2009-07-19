// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.SharpDevelop.Editor.AvalonEdit
{
	using ICSharpCode.AvalonEdit;
	
	/// <summary>
	/// Wraps AvalonEdit to provide the ITextEditor interface.
	/// </summary>
	public class AvalonEditTextEditorAdapter : ITextEditor, IWeakEventListener
	{
		readonly TextEditor textEditor;
		AvalonEditDocumentAdapter document;
		
		public TextEditor TextEditor {
			get { return textEditor; }
		}
		
		public AvalonEditTextEditorAdapter(TextEditor textEditor)
		{
			if (textEditor == null)
				throw new ArgumentNullException("textEditor");
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
		
		public virtual IFormattingStrategy FormattingStrategy {
			get { return DefaultFormattingStrategy.DefaultInstance; }
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
				get { return AvalonEditDocumentAdapter.ToLocation(caret.Location); }
				set { caret.Location = AvalonEditDocumentAdapter.ToPosition(value); }
			}
			
			public event EventHandler PositionChanged {
				add    { caret.PositionChanged += value; }
				remove { caret.PositionChanged -= value; }
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
			
			public bool AutoInsertBlockEnd {
				get {
					return true;
				}
			}
			
			public bool ConvertTabsToSpaces {
				get {
					return avalonEditOptions.ConvertTabsToSpaces;
				}
			}
			
			public int IndendationSize {
				get {
					return avalonEditOptions.IndentationSize;
				}
			}
		}
		
		public virtual string FileName {
			get { return null; }
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
		
		public event KeyEventHandler KeyPress {
			add    { textEditor.TextArea.PreviewKeyDown += value; }
			remove { textEditor.TextArea.PreviewKeyDown -= value; }
		}
		
		public event EventHandler SelectionChanged {
			add    { textEditor.TextArea.SelectionChanged += value; }
			remove { textEditor.TextArea.SelectionChanged -= value; }
		}
		
		public void Select(int selectionStart, int selectionLength)
		{
			textEditor.Select(selectionStart, selectionLength);
			textEditor.TextArea.Caret.BringCaretToView();
		}
		
		public void JumpTo(int line, int column)
		{
			textEditor.TextArea.Selection = Selection.Empty;
			textEditor.TextArea.Caret.Position = new TextViewPosition(line, column);
			textEditor.TextArea.Caret.BringCaretToView();
		}
		
		public virtual IInsightWindow ActiveInsightWindow {
			get {
				return null;
			}
		}
		
		public virtual IInsightWindow ShowInsightWindow(IEnumerable<IInsightItem> items)
		{
			return null;
		}
		
		
		void ITextEditor.ShowCompletionWindow(ICSharpCode.TextEditor.Gui.CompletionWindow.ICompletionDataProvider provider, char ch)
		{
		}
		
		public virtual ICompletionListWindow ActiveCompletionWindow {
			get {
				return null;
			}
		}
		
		public virtual ICompletionListWindow ShowCompletionWindow(ICompletionItemList data)
		{
			return null;
		}
	}
}
