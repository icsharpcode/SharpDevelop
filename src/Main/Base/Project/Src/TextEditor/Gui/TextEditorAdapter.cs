// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop
{
	public class TextEditorAdapter : ITextEditor
	{
		readonly TextEditorControl editor;
		
		public TextEditorAdapter(TextEditorControl editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.editor = editor;
			this.Document = new TextEditorDocument(editor.Document);
			this.Caret = new CaretAdapter(this, editor.ActiveTextAreaControl.Caret);
			this.Options = new OptionsAdapter(editor.TextEditorProperties);
		}
		
		sealed class CaretAdapter : ITextEditorCaret
		{
			readonly TextEditorAdapter parent;
			readonly Caret caret;
			
			public CaretAdapter(TextEditorAdapter parent, Caret caret)
			{
				Debug.Assert(parent != null && caret != null);
				
				this.parent = parent;
				this.caret = caret;
			}
			
			public int Offset {
				get { return caret.Offset; }
				set { caret.Position = parent.editor.Document.OffsetToPosition(value); }
			}
			
			public int Line {
				get { return caret.Line + 1; }
				set { caret.Line = value - 1; }
			}
			
			public int Column {
				get { return caret.Column + 1; }
				set { caret.Column = value - 1; }
			}
			
			public ICSharpCode.NRefactory.Location Position {
				get {
					return ToLocation(caret.Position);
				}
				set {
					caret.Position = ToPosition(value);
				}
			}
			
			public event EventHandler PositionChanged {
				add    { caret.PositionChanged += value; }
				remove { caret.PositionChanged -= value; }
			}
		}
		
		sealed class OptionsAdapter : ITextEditorOptions
		{
			ITextEditorProperties properties;
			
			public OptionsAdapter(ITextEditorProperties properties)
			{
				if (properties == null)
					throw new ArgumentNullException("properties");
				this.properties = properties;
			}
			
			public string IndentationString {
				get {
					return properties.ConvertTabsToSpaces ? new string(' ', properties.IndentationSize) : "\t";
				}
			}
			
			public bool AutoInsertBlockEnd {
				get {
					return true;
				}
			}
			
			public bool ConvertTabsToSpaces {
				get {
					return properties.ConvertTabsToSpaces;
				}
			}
			
			public int IndentationSize {
				get {
					return properties.IndentationSize;
				}
			}
			
			public int VerticalRulerColumn {
				get {
					return properties.VerticalRulerRow;
				}
			}
		}
		
		static ICSharpCode.NRefactory.Location ToLocation(TextLocation position)
		{
			return new ICSharpCode.NRefactory.Location(position.Column + 1, position.Line + 1);
		}
		
		static TextLocation ToPosition(ICSharpCode.NRefactory.Location location)
		{
			return new TextLocation(location.Column - 1, location.Line - 1);
		}
		
		public TextAreaControl ActiveTextAreaControl {
			get {
				return editor.ActiveTextAreaControl;
			}
		}
		
		public ICSharpCode.SharpDevelop.Editor.IDocument Document { get; private set; }
		public ITextEditorCaret Caret { get; private set; }
		public ITextEditorOptions Options { get; private set; }
		
		public FileName FileName {
			get { return FileName.Create(editor.FileName); }
		}
		
		public ICompletionListWindow ActiveCompletionWindow {
			get {
				return null;
			}
		}
		
		public ICompletionListWindow ShowCompletionWindow(ICompletionItemList items)
		{
			return null;
		}
		
		public string GetWordBeforeCaret()
		{
			return "";
		}
		
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(TextArea))
				return editor.ActiveTextAreaControl.TextArea;
			else
				return null;
		}
		
		public int SelectionStart {
			get {
				var sel = editor.ActiveTextAreaControl.SelectionManager;
				if (sel.HasSomethingSelected)
					return sel.SelectionCollection[0].Offset;
				else
					return this.Caret.Offset;
			}
		}
		
		public int SelectionLength {
			get {
				var sel = editor.ActiveTextAreaControl.SelectionManager;
				if (sel.HasSomethingSelected)
					return sel.SelectionCollection[0].Length;
				else
					return 0;
			}
		}
		
		public string SelectedText {
			get {
				var sel = editor.ActiveTextAreaControl.SelectionManager;
				return sel.SelectedText;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public event System.Windows.Input.KeyEventHandler KeyPress {
			add    { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		
		public event EventHandler SelectionChanged {
			add    { editor.ActiveTextAreaControl.SelectionManager.SelectionChanged += value; }
			remove { editor.ActiveTextAreaControl.SelectionManager.SelectionChanged -= value; }
		}
		
		public void Select(int selectionStart, int selectionLength)
		{
			var doc = editor.Document;
			editor.ActiveTextAreaControl.SelectionManager.SetSelection(
				doc.OffsetToPosition(selectionStart), doc.OffsetToPosition(selectionStart + selectionLength));
		}
		
		public void JumpTo(int line, int column)
		{
			editor.ActiveTextAreaControl.JumpTo(line - 1, column - 1);
		}
		
		public IInsightWindow ActiveInsightWindow {
			get {
				return null;
			}
		}
		
		public IInsightWindow ShowInsightWindow(IEnumerable<IInsightItem> items)
		{
			return null;
		}
		
		public ILanguageBinding Language {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ITextEditor PrimaryView {
			get {
				return this;
			}
		}
		
		public IEnumerable<ICompletionItem> GetSnippets()
		{
			return Enumerable.Empty<ICompletionItem>();
		}
	}
	
	sealed class CompletionItemListAdapter : ICompletionDataProvider
	{
		readonly ICompletionItemList list;
		
		public CompletionItemListAdapter(ICompletionItemList list)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			this.list = list;
		}
		
		public System.Windows.Forms.ImageList ImageList {
			get {
				return ClassBrowserIconService.ImageList;
			}
		}
		
		public string PreSelection {
			get {
				return null;
			}
		}
		
		public int DefaultIndex {
			get {
				return 0;
			}
		}
		
		public CompletionDataProviderKeyResult ProcessKey(char key)
		{
			return CompletionDataProviderKeyResult.NormalKey;
		}
		
		public bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
		{
			return false;
		}
		
		public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			return list.Items.Select(item => new CompletionItemAdapter(item)).ToArray();
		}
	}
	
	sealed class CompletionItemAdapter : ICompletionData
	{
		readonly ICompletionItem item;
		
		public CompletionItemAdapter(ICompletionItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			this.item = item;
		}
		
		public int ImageIndex {
			get {
				return -1;
			}
		}
		
		public string Text {
			get { return item.Text; }
			set {
				throw new NotSupportedException();
			}
		}
		
		public string Description {
			get {
				return item.Description;
			}
		}
		
		public double Priority {
			get {
				return 0;
			}
		}
		
		public bool InsertAction(TextArea textArea, char ch)
		{
			return false;
		}
	}
}
