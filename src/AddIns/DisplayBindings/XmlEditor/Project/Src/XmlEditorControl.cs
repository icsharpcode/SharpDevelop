// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Xml editor derived from the SharpDevelop TextEditor control.
	/// </summary>
	public class XmlEditorControl : ICSharpCode.TextEditor.TextEditorControl
	{
		static readonly string editActionsPath       = "/AddIns/XmlEditor/EditActions";
		static readonly string contextMenuPath       = "/SharpDevelop/ViewContent/XmlEditor/ContextMenu";
		CodeCompletionWindow codeCompletionWindow;
		XmlSchemaCompletionDataCollection schemaCompletionDataItems = new XmlSchemaCompletionDataCollection();
		XmlSchemaCompletionData defaultSchemaCompletionData = null;
		string defaultNamespacePrefix = String.Empty;
		
		public XmlEditorControl()
		{
			XmlFormattingStrategy strategy = new XmlFormattingStrategy();
			Document.FormattingStrategy = (IFormattingStrategy)strategy;
			
			Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("XML");
			Document.FoldingManager.FoldingStrategy = new XmlFoldingStrategy();
			TextEditorProperties = new SharpDevelopTextEditorProperties();
			
			GenerateEditActions();
		}
		
		/// <summary>
		/// Gets the schemas that the xml editor will use.
		/// </summary>
		/// <remarks>
		/// Probably should have NOT a 'set' property, but allowing one
		/// allows us to share the completion data amongst multiple
		/// xml editor controls.
		/// </remarks>
		public XmlSchemaCompletionDataCollection SchemaCompletionDataItems {
			get {
				return schemaCompletionDataItems;
			}
			set {
				schemaCompletionDataItems = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the default namespace prefix.
		/// </summary>
		public string DefaultNamespacePrefix {
			get {
				return defaultNamespacePrefix;
			}
			set {
				defaultNamespacePrefix = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the default schema completion data associated with this
		/// view.
		/// </summary>
		public XmlSchemaCompletionData DefaultSchemaCompletionData {
			get {
				return defaultSchemaCompletionData;
			}
			
			set {
				defaultSchemaCompletionData = value;
			}
		}
		
		/// <summary>
		/// Called when the user hits Ctrl+Space.
		/// </summary>
		public void ShowCompletionWindow()
		{
			if (!IsCaretAtDocumentStart) {
				// Find character before cursor.
				
				char ch = GetCharacterBeforeCaret();
				
				HandleKeyPress(ch);
			}
		}
		
		protected override void InitializeTextAreaControl(TextAreaControl newControl)
		{
			base.InitializeTextAreaControl(newControl);
			newControl.TextArea.KeyEventHandler += new ICSharpCode.TextEditor.KeyEventHandler(HandleKeyPress);

			newControl.ContextMenuStrip = MenuService.CreateContextMenu(this, contextMenuPath);
			newControl.SelectionManager.SelectionChanged += new EventHandler(SelectionChanged);
			newControl.Document.DocumentChanged += new DocumentEventHandler(DocumentChanged);
			newControl.Caret.PositionChanged += new EventHandler(CaretPositionChanged);
			newControl.TextArea.ClipboardHandler.CopyText += new CopyTextEventHandler(ClipboardHandlerCopyText);
			
			newControl.MouseWheel += new MouseEventHandler(TextAreaMouseWheel);
			newControl.DoHandleMousewheel = false;
		}
		
		/// <summary>
		/// Captures the user's key presses.
		/// </summary>
		/// <remarks>
		/// <para>The code completion window ProcessKeyEvent is not perfect
		/// when typing xml.  If enter a space or ':' the text is
		/// autocompleted when it should not be.</para>
		/// <para>The code completion window has one predefined width,
		/// which cuts off any long namespaces that we show.</para>
		/// <para>The above issues have been resolved by duplicating
		/// the code completion window and fixing the problems in the
		/// duplicated class.</para>
		/// </remarks>
		protected bool HandleKeyPress(char ch)
		{
			if (IsCodeCompletionWindowOpen) {
				if (codeCompletionWindow.ProcessKeyEvent(ch)) {
					return false;
				}
			}
			
			try {
				switch (ch) {
					case '<':
					case ' ':
					case '=':
						ShowCompletionWindow(ch);
						return false;
					default:
						if (XmlParser.IsAttributeValueChar(ch)) {
							if (IsInsideQuotes(ActiveTextAreaControl.TextArea)) {
								// Have to insert the character ourselves since
								// it is not actually inserted yet.  If it is not
								// inserted now the code completion will not work
								// since the completion data provider attempts to
								// include the key typed as the pre-selected text.
								InsertCharacter(ch);
								ShowCompletionWindow(ch);
								return true;
							}
						}
						break;
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
			
			return false;
		}
		
		bool IsCodeCompletionEnabled {
			get {
				return ICSharpCode.SharpDevelop.Dom.CodeCompletionOptions.EnableCodeCompletion;
			}
		}
		
		void CodeCompletionWindowClosed(object sender, EventArgs e)
		{
			codeCompletionWindow.Closed -= new EventHandler(CodeCompletionWindowClosed);
			codeCompletionWindow.Dispose();
			codeCompletionWindow = null;
		}
		
		bool IsCodeCompletionWindowOpen {
			get {
				return ((codeCompletionWindow != null) && (!codeCompletionWindow.IsDisposed));
			}
		}
		
		void ShowCompletionWindow(char ch)
		{
			if (IsCodeCompletionWindowOpen) {
				codeCompletionWindow.Close();
			}
			
			if (IsCodeCompletionEnabled) {
				XmlCompletionDataProvider completionDataProvider = new XmlCompletionDataProvider(schemaCompletionDataItems, defaultSchemaCompletionData, defaultNamespacePrefix);
				codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(ParentForm, this, FileName, completionDataProvider, ch, XmlEditorAddInOptions.ShowSchemaAnnotation);
				
				if (codeCompletionWindow != null) {
					codeCompletionWindow.Closed += new EventHandler(CodeCompletionWindowClosed);
				}
			}
		}
		
		void GenerateEditActions()
		{
			IEditAction[] actions = (IEditAction[])(AddInTree.BuildItems(editActionsPath, this, false).ToArray(typeof(IEditAction)));
			
			foreach (IEditAction action in actions) {
				foreach (Keys key in action.Keys) {
					editactions[key] = action;
				}
			}
		}
		
		void DocumentChanged(object sender, DocumentEventArgs e)
		{
		}
		
		void SelectionChanged(object sender, EventArgs e)
		{
		}
		
		void ClipboardHandlerCopyText(object sender, CopyTextEventArgs e)
		{
			SideBarView.PutInClipboardRing(e.Text);
		}
		
		void CaretPositionChanged(object sender, EventArgs e)
		{
			StatusBarService.SetCaretPosition(ActiveTextAreaControl.TextArea.TextView.GetVisualColumn(ActiveTextAreaControl.Caret.Line, ActiveTextAreaControl.Caret.Column), ActiveTextAreaControl.Caret.Line, ActiveTextAreaControl.Caret.Column);
		}
		
		void TextAreaMouseWheel(object sender, MouseEventArgs e)
		{
			TextAreaControl textAreaControl = (TextAreaControl)sender;

			if (IsCodeCompletionWindowOpen && codeCompletionWindow.Visible) {
				codeCompletionWindow.HandleMouseWheel(e);
			} else {
				textAreaControl.HandleMouseWheel(e);
			}
		}
		
		char GetCharacterBeforeCaret()
		{
			string text = Document.GetText(ActiveTextAreaControl.TextArea.Caret.Offset - 1, 1);
			if (text.Length > 0) {
				return text[0];
			}
			
			return '\0';
		}
		
		bool IsCaretAtDocumentStart {
			get {
				return ActiveTextAreaControl.TextArea.Caret.Offset == 0;
			}
		}
		
		/// <summary>
		/// Checks whether the caret is inside a set of quotes (" or ').
		/// </summary>
		bool IsInsideQuotes(TextArea textArea)
		{
			bool inside = false;
			
			LineSegment line = textArea.Document.GetLineSegment(textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset));
			if (line != null) {
				if ((line.Offset + line.Length > textArea.Caret.Offset) &&
				    (line.Offset < textArea.Caret.Offset)){
					
					char charAfter = textArea.Document.GetCharAt(textArea.Caret.Offset);
					char charBefore = textArea.Document.GetCharAt(textArea.Caret.Offset - 1);
					
					if (((charBefore == '\'') && (charAfter == '\'')) ||
					    ((charBefore == '\"') && (charAfter == '\"'))) {
						inside = true;
					}
				}
			}
			
			return inside;
		}
		
		/// <summary>
		/// Inserts a character into the text editor at the current offset.
		/// </summary>
		/// <remarks>
		/// This code is copied from the TextArea.SimulateKeyPress method.  This
		/// code is needed to handle an issue with code completion.  What if
		/// we want to include the character just typed as the pre-selected text
		/// for autocompletion?  If we do not insert the character before
		/// displaying the autocompletion list we cannot set the pre-selected text
		/// because it is not actually inserted yet.  The autocompletion window
		/// checks the offset of the pre-selected text and closes the window
		/// if the range is wrong.  The offset check is always wrong since the text
		/// does not actually exist yet.  The check occurs in
		/// CodeCompletionWindow.CaretOffsetChanged:
		/// <code>[[!CDATA[	int offset = control.ActiveTextAreaControl.Caret.Offset;
		///
		///	if (offset < startOffset || offset > endOffset) {
		///		Close();
		///	} else {
		///		codeCompletionListView.SelectItemWithStart(control.Document.GetText(startOffset, offset - startOffset));
		///	}]]
		/// </code>
		/// The Close method is called because the offset is out of the range.
		/// </remarks>
		void InsertCharacter(char ch)
		{
			ActiveTextAreaControl.TextArea.MotherTextEditorControl.BeginUpdate();
			
			switch (ActiveTextAreaControl.TextArea.Caret.CaretMode)
			{
				case CaretMode.InsertMode:
					ActiveTextAreaControl.TextArea.InsertChar(ch);
					break;
				case CaretMode.OverwriteMode:
					ActiveTextAreaControl.TextArea.ReplaceChar(ch);
					break;
			}
			int currentLineNr = ActiveTextAreaControl.TextArea.Caret.Line;
			int delta = Document.FormattingStrategy.FormatLine(ActiveTextAreaControl.TextArea, currentLineNr, Document.PositionToOffset(ActiveTextAreaControl.TextArea.Caret.Position), ch);
			
			ActiveTextAreaControl.TextArea.MotherTextEditorControl.EndUpdate();
		}
	}
}
