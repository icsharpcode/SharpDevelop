// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using ICSharpCode.AvalonEdit.Gui;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

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
				document = new AvalonEditDocumentAdapter(textEditor.Document);
			else
				document = null;
		}
		
		public IDocument Document {
			get { return document; }
		}
		
		public ITextEditorCaret Caret { get; private set; }
		
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
			CompletionWindow window = textEditor.CreateCompletionWindow(data.Items.Select(i => (ICompletionData)new CodeCompletionDataAdapter(i)));
			if (window != null)
				window.Show();
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
	
	sealed class CodeCompletionDataAdapter : ICompletionData
	{
		readonly ICompletionItem item;
		
		public CodeCompletionDataAdapter(ICompletionItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			this.item = item;
		}
		
		public string Text {
			get { return item.Text; }
		}
		
		public object Content {
			get { return item.Text; }
		}
		
		public object Description {
			get { return item.Description; }
		}
	}
}
