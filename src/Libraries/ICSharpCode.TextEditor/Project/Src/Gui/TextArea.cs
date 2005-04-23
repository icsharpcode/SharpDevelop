// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
	public delegate bool KeyEventHandler(char ch);
	public delegate bool DialogKeyProcessor(Keys keyData);
	
	/// <summary>
	/// This class paints the textarea.
	/// </summary>
	[ToolboxItem(false)]
	public class TextArea : UserControl
	{
		public static bool HiddenMouseCursor = false;
		
		Point virtualTop        = new Point(0, 0);
		TextAreaControl         motherTextAreaControl;
		TextEditorControl       motherTextEditorControl;
		
		ArrayList                 bracketshemes  = new ArrayList();
		TextAreaClipboardHandler  textAreaClipboardHandler;
		bool autoClearSelection = false;
		
		ArrayList  leftMargins = new ArrayList();
		ArrayList  topMargins  = new ArrayList();
		
		TextView      textView;
		GutterMargin  gutterMargin;
		FoldMargin    foldMargin;
		IconBarMargin iconBarMargin;
		
		SelectionManager selectionManager;
		Caret            caret;
		
		public TextEditorControl MotherTextEditorControl {
			get {
				return motherTextEditorControl;
			}
		}
		
		public TextAreaControl MotherTextAreaControl {
			get {
				return motherTextAreaControl;
			}
		}
		
		public SelectionManager SelectionManager {
			get {
				return selectionManager;
			}
		}
		
		public Caret Caret {
			get {
				return caret;
			}
		}
		
		public TextView TextView {
			get {
				return textView;
			}
		}
		
		public GutterMargin GutterMargin {
			get {
				return gutterMargin;
			}
		}
		
		public FoldMargin FoldMargin {
			get {
				return foldMargin;
			}
		}
		
		public IconBarMargin IconBarMargin {
			get {
				return iconBarMargin;
			}
		}
		
		public Encoding Encoding {
			get {
				return motherTextEditorControl.Encoding;
			}
		}
		public int MaxVScrollValue {
			get {
				return (Document.GetVisibleLine(Document.TotalNumberOfLines - 1) + 1 + TextView.VisibleLineCount * 2 / 3) * Document.TextEditorProperties.Font.Height;
			}
		}
		
		public Point VirtualTop {
			get {
				return virtualTop;
			}
			set {
				Point newVirtualTop = new Point(value.X, Math.Min(MaxVScrollValue, Math.Max(0, value.Y)));
				if (virtualTop != newVirtualTop) {
					virtualTop = newVirtualTop;
					motherTextAreaControl.VScrollBar.Value = virtualTop.Y;
					Invalidate();
				}
			}
		}
		
		public bool AutoClearSelection {
			get {
				return autoClearSelection;
			}
			set {
				autoClearSelection = value;
			}
		}
		
		[Browsable(false)]
		public IDocument Document {
			get {
				return motherTextEditorControl.Document;
			}
		}
		
		public TextAreaClipboardHandler ClipboardHandler {
			get {
				return textAreaClipboardHandler;
			}
		}
		
		
		public ITextEditorProperties TextEditorProperties {
			get {
				return motherTextEditorControl.TextEditorProperties;
			}
		}
		
		public TextArea(TextEditorControl motherTextEditorControl, TextAreaControl motherTextAreaControl)
		{
			this.motherTextAreaControl      = motherTextAreaControl;
			this.motherTextEditorControl    = motherTextEditorControl;
			
			caret            = new Caret(this);
			selectionManager = new SelectionManager(Document);
			
			this.textAreaClipboardHandler = new TextAreaClipboardHandler(this);
			
			ResizeRedraw = true;
			
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
//			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
//			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.Opaque, false);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.Selectable, true);
			
			textView = new TextView(this);
			
			gutterMargin = new GutterMargin(this);
			foldMargin   = new FoldMargin(this);
			iconBarMargin = new IconBarMargin(this);
			leftMargins.AddRange(new AbstractMargin[] { iconBarMargin, gutterMargin, foldMargin });
			OptionsChanged();
			
			
			new TextAreaMouseHandler(this).Attach();
			new TextAreaDragDropHandler().Attach(this);
			
			bracketshemes.Add(new BracketHighlightingSheme('{', '}'));
			bracketshemes.Add(new BracketHighlightingSheme('(', ')'));
			bracketshemes.Add(new BracketHighlightingSheme('[', ']'));
			
			caret.PositionChanged += new EventHandler(SearchMatchingBracket);
			Document.TextContentChanged += new EventHandler(TextContentChanged);
			Document.FoldingManager.FoldingsChanged += new EventHandler(DocumentFoldingsChanged);
		}
		
		public void UpdateMatchingBracket()
		{
			SearchMatchingBracket(null, null);
		}
		
		void TextContentChanged(object sender, EventArgs e)
		{
			Caret.Position = new Point(0, 0);
			SelectionManager.SelectionCollection.Clear();
		}
		void SearchMatchingBracket(object sender, EventArgs e)
		{
			if (!TextEditorProperties.ShowMatchingBracket) {
				textView.Highlight = null;
				return;
			}
			bool changed = false;
			if (caret.Offset == 0) {
				if (textView.Highlight != null) {
					int line  = textView.Highlight.OpenBrace.Y;
					int line2 = textView.Highlight.CloseBrace.Y;
					textView.Highlight = null;
					UpdateLine(line);
					UpdateLine(line2);
				}
				return;
			}
			foreach (BracketHighlightingSheme bracketsheme in bracketshemes) {
//				if (bracketsheme.IsInside(textareapainter.Document, textareapainter.Document.Caret.Offset)) {
				Highlight highlight = bracketsheme.GetHighlight(Document, Caret.Offset - 1);
				if (textView.Highlight != null && textView.Highlight.OpenBrace.Y >=0 && textView.Highlight.OpenBrace.Y < Document.TotalNumberOfLines) {
					UpdateLine(textView.Highlight.OpenBrace.Y);
				}
				if (textView.Highlight != null && textView.Highlight.CloseBrace.Y >=0 && textView.Highlight.CloseBrace.Y < Document.TotalNumberOfLines) {
					UpdateLine(textView.Highlight.CloseBrace.Y);
				}
				textView.Highlight = highlight;
				if (highlight != null) {
					changed = true;
					break;
				}
//				}
			}
			if (changed || textView.Highlight != null) {
				int line = textView.Highlight.OpenBrace.Y;
				int line2 = textView.Highlight.CloseBrace.Y;
				if (!changed) {
					textView.Highlight = null;
				}
				UpdateLine(line);
				UpdateLine(line2);
			}
		}
		
		public void SetDesiredColumn()
		{
			Caret.DesiredColumn = TextView.GetDrawingXPos(Caret.Line, Caret.Column) + (int)(VirtualTop.X * textView.GetWidth(' '));
		}
		
		public void SetCaretToDesiredColumn(int caretLine)
		{
			Caret.Position = textView.GetLogicalColumn(Caret.Line, Caret.DesiredColumn + (int)(VirtualTop.X * textView.GetWidth(' ')));
		}
		
		public void OptionsChanged()
		{
			UpdateMatchingBracket();
			textView.OptionsChanged();
			caret.RecreateCaret();
			caret.UpdateCaretPosition();
			Refresh();
		}
		
		AbstractMargin lastMouseInMargin;
		
		protected override void OnMouseLeave(System.EventArgs e)
		{
			base.OnMouseLeave(e);
			this.Cursor = Cursors.Default;
			if (lastMouseInMargin != null) {
				lastMouseInMargin.HandleMouseLeave(EventArgs.Empty);
				lastMouseInMargin = null;
			}
		}
		
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);
			
			foreach (AbstractMargin margin in leftMargins) {
				if (margin.DrawingPosition.Contains(e.X, e.Y)) {
					margin.HandleMouseDown(new Point(e.X, e.Y), e.Button);
				}
			}
		}
		
		
		// static because the mouse can only be in one text area and we don't want to have
		// tooltips of text areas from inactive tabs floating around.
		static ToolTip toolTip;
		static string oldToolTip;
		bool toolTipSet;
		
		public bool ToolTipVisible {
			get {
				return toolTipSet;
			}
		}
		
		public void SetToolTip(string text)
		{
			if (toolTip == null) toolTip = new ToolTip();
			toolTipSet = (text != null);
			if (oldToolTip == text)
				return;
			if (text == null) {
				toolTip.Hide(this);
			} else {
				Point p = PointToClient(Control.MousePosition);
				p.Offset(3, 3);
				toolTip.Show(text, this, p);
			}
			oldToolTip = text;
		}
		
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			toolTipSet = false;
			base.OnMouseMove(e);
			if (!toolTipSet)
				SetToolTip(null);
			foreach (AbstractMargin margin in leftMargins) {
				if (margin.DrawingPosition.Contains(e.X, e.Y)) {
					this.Cursor = margin.Cursor;
					margin.HandleMouseMove(new Point(e.X, e.Y), e.Button);
					if (lastMouseInMargin != margin) {
						if (lastMouseInMargin != null) {
							lastMouseInMargin.HandleMouseLeave(EventArgs.Empty);
						}
						lastMouseInMargin = margin;
					}
					return;
				}
			}
			if (lastMouseInMargin != null) {
				lastMouseInMargin.HandleMouseLeave(EventArgs.Empty);
				lastMouseInMargin = null;
			}
			if (textView.DrawingPosition.Contains(e.X, e.Y)) {
				this.Cursor = textView.Cursor;
				return;
			}
			this.Cursor = Cursors.Default;
		}
		AbstractMargin updateMargin = null;
		
		public void Refresh(AbstractMargin margin)
		{
			updateMargin = margin;
			Invalidate(updateMargin.DrawingPosition);
			Update();
			updateMargin = null;
		}
		
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
		{
		}
		
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			int currentXPos = 0;
			int currentYPos = 0;
			bool adjustScrollBars = false;
			Graphics  g             = e.Graphics;
			Rectangle clipRectangle = e.ClipRectangle;
			
			
			if (updateMargin != null) {
				try {
					updateMargin.Paint(g, updateMargin.DrawingPosition);
				} catch (Exception ex) {
					Console.WriteLine("Got exception : " + ex);
				}
//				clipRectangle.Intersect(updateMargin.DrawingPosition);
			}
			
			if (clipRectangle.Width <= 0 || clipRectangle.Height <= 0) {
				return;
			}
			
			if (this.TextEditorProperties.UseAntiAliasedFont) {
				g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			} else {
				g.TextRenderingHint = TextRenderingHint.SystemDefault;
			}
			
			foreach (AbstractMargin margin in leftMargins) {
				if (margin.IsVisible) {
					Rectangle marginRectangle = new Rectangle(currentXPos , currentYPos, margin.Size.Width, Height - currentYPos);
					if (marginRectangle != margin.DrawingPosition) {
						adjustScrollBars = true;
						margin.DrawingPosition = marginRectangle;
					}
					currentXPos += margin.DrawingPosition.Width;
					if (clipRectangle.IntersectsWith(marginRectangle)) {
						marginRectangle.Intersect(clipRectangle);
						if (!marginRectangle.IsEmpty) {
							try {
								margin.Paint(g, marginRectangle);
							} catch (Exception ex) {
								Console.WriteLine("Got exception : " + ex);
							}
						}
					}
				}
			}
			
			Rectangle textViewArea = new Rectangle(currentXPos, currentYPos, Width - currentXPos, Height - currentYPos);
			if (textViewArea != textView.DrawingPosition) {
				adjustScrollBars = true;
				textView.DrawingPosition = textViewArea;
			}
			if (clipRectangle.IntersectsWith(textViewArea)) {
				textViewArea.Intersect(clipRectangle);
				if (!textViewArea.IsEmpty) {
					try {
						textView.Paint(g, textViewArea);
					} catch (Exception ex) {
						Console.WriteLine("Got exception : " + ex);
					}
				}
			}
			
			if (adjustScrollBars) {
				try {
					this.motherTextAreaControl.AdjustScrollBars(null, null);
				} catch (Exception) {}
			}
			
			try {
				Caret.UpdateCaretPosition();
			} catch (Exception) {}
			
			base.OnPaint(e);
		}
		void DocumentFoldingsChanged(object sender, EventArgs e)
		{
			this.motherTextAreaControl.AdjustScrollBars(null, null);
		}
		
		#region keyboard handling methods
		
		/// <summary>
		/// This method is called on each Keypress
		/// </rsummary>
		/// <returns>
		/// True, if the key is handled by this method and should NOT be
		/// inserted in the textarea.
		/// </returns>
		protected virtual bool HandleKeyPress(char ch)
		{
			if (KeyEventHandler != null) {
				return KeyEventHandler(ch);
			}
			return false;
		}
		
		public void SimulateKeyPress(char ch)
		{
			if (Document.ReadOnly) {
				return;
			}
			
			if (ch < ' ') {
				return;
			}
			
			if (!HiddenMouseCursor && TextEditorProperties.HideMouseCursor) {
				HiddenMouseCursor = true;
				Cursor.Hide();
			}
			
			motherTextEditorControl.BeginUpdate();
			switch (ch) {
				default: // INSERT char
					if (!HandleKeyPress(ch)) {
						switch (Caret.CaretMode) {
							case CaretMode.InsertMode:
								InsertChar(ch);
								break;
							case CaretMode.OverwriteMode:
								ReplaceChar(ch);
								break;
							default:
								Debug.Assert(false, "Unknown caret mode " + Caret.CaretMode);
								break;
						}
					}
					break;
			}
			
			int currentLineNr = Caret.Line;
			int delta = Document.FormattingStrategy.FormatLine(this, currentLineNr, Document.PositionToOffset(Caret.Position), ch);
			
			motherTextEditorControl.EndUpdate();
			if (delta != 0) {
//				this.motherTextEditorControl.UpdateLines(currentLineNr, currentLineNr);
			}
		}
		
		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			SimulateKeyPress(e.KeyChar);
		}
		
		/// <summary>
		/// This method executes a dialog key
		/// </summary>
		public bool ExecuteDialogKey(Keys keyData)
		{
			// try, if a dialog key processor was set to use this
			if (DoProcessDialogKey != null && DoProcessDialogKey(keyData)) {
				return true;
			}
			
			// if not (or the process was 'silent', use the standard edit actions
			IEditAction action =  motherTextEditorControl.GetEditAction(keyData);
			AutoClearSelection = true;
			if (action != null) {
				motherTextEditorControl.BeginUpdate();
				try {
					lock (Document) {
						action.Execute(this);
						if (SelectionManager.HasSomethingSelected && AutoClearSelection /*&& caretchanged*/) {
							if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal) {
								SelectionManager.ClearSelection();
							}
						}
					}
				} catch (Exception e) {
					Console.WriteLine("Got Exception while executing action " + action + " : " + e.ToString());
				} finally {
					motherTextEditorControl.EndUpdate();
					Caret.UpdateCaretPosition();
				}
				return true;
			}
			return false;
		}
		
		protected override bool ProcessDialogKey(Keys keyData)
		{
			return ExecuteDialogKey(keyData) || base.ProcessDialogKey(keyData);
		}
		#endregion
		
		public void ScrollToCaret()
		{
			motherTextAreaControl.ScrollToCaret();
		}
		
		public void ScrollTo(int line)
		{
			motherTextAreaControl.ScrollTo(line);
		}
		
		public void BeginUpdate()
		{
			motherTextEditorControl.BeginUpdate();
		}
		
		public void EndUpdate()
		{
			motherTextEditorControl.EndUpdate();
		}
		
		string GenerateWhitespaceString(int length)
		{
			return new String(' ', length);
		}
		/// <remarks>
		/// Inserts a single character at the caret position
		/// </remarks>
		public void InsertChar(char ch)
		{
			bool updating = motherTextEditorControl.IsUpdating;
			if (!updating) {
				BeginUpdate();
			}
			
			// filter out forgein whitespace chars and replace them with standard space (ASCII 32)
			if (Char.IsWhiteSpace(ch) && ch != '\t' && ch != '\n') {
				ch = ' ';
			}
			bool removedText = false;
			if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal &&
			    SelectionManager.SelectionCollection.Count > 0) {
				Caret.Position = SelectionManager.SelectionCollection[0].StartPosition;
				SelectionManager.RemoveSelectedText();
				removedText = true;
			}
			LineSegment caretLine = Document.GetLineSegment(Caret.Line);
			int offset = Caret.Offset;
			// use desired column for generated whitespaces
			int dc=Math.Min(Caret.Column,Caret.DesiredColumn);
			if (caretLine.Length < dc && ch != '\n') {
				Document.Insert(offset, GenerateWhitespaceString(dc - caretLine.Length) + ch);
			} else {
				Document.Insert(offset, ch.ToString());
			}
			++Caret.Column;
			
			if (removedText) {
				Document.UndoStack.UndoLast(2);
			}
			
			if (!updating) {
				EndUpdate();
				UpdateLineToEnd(Caret.Line, Caret.Column);
			}
			
			// I prefer to set NOT the standard column, if you type something
//			++Caret.DesiredColumn;
		}
		
		/// <remarks>
		/// Inserts a whole string at the caret position
		/// </remarks>
		public void InsertString(string str)
		{
			bool updating = motherTextEditorControl.IsUpdating;
			if (!updating) {
				BeginUpdate();
			}
			try {
				bool removedText = false;
				if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal &&
				    SelectionManager.SelectionCollection.Count > 0) {
					Caret.Position = SelectionManager.SelectionCollection[0].StartPosition;
					SelectionManager.RemoveSelectedText();
					removedText = true;
				}
				
				int oldOffset = Document.PositionToOffset(Caret.Position);
				int oldLine   = Caret.Line;
				LineSegment caretLine = Document.GetLineSegment(Caret.Line);
				if (caretLine.Length < Caret.Column) {
					int whiteSpaceLength = Caret.Column - caretLine.Length;
					Document.Insert(oldOffset, GenerateWhitespaceString(whiteSpaceLength) + str);
					Caret.Position = Document.OffsetToPosition(oldOffset + str.Length + whiteSpaceLength);
				} else {
					Document.Insert(oldOffset, str);
					Caret.Position = Document.OffsetToPosition(oldOffset + str.Length);
				}
				if (removedText) {
					Document.UndoStack.UndoLast(2);
				}
				if (oldLine != Caret.Line) {
					UpdateToEnd(oldLine);
				} else {
					UpdateLineToEnd(Caret.Line, Caret.Column);
				}
			} finally {
				if (!updating) {
					EndUpdate();
				}
			}
		}
		
		/// <remarks>
		/// Replaces a char at the caret position
		/// </remarks>
		public void ReplaceChar(char ch)
		{
			bool updating = motherTextEditorControl.IsUpdating;
			if (!updating) {
				BeginUpdate();
			}
			if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal && SelectionManager.SelectionCollection.Count > 0) {
				Caret.Position = SelectionManager.SelectionCollection[0].StartPosition;
				SelectionManager.RemoveSelectedText();
			}
			
			int lineNr   = Caret.Line;
			LineSegment  line = Document.GetLineSegment(lineNr);
			int offset = Document.PositionToOffset(Caret.Position);
			if (offset < line.Offset + line.Length) {
				Document.Replace(offset, 1, ch.ToString());
			} else {
				Document.Insert(offset, ch.ToString());
			}
			if (!updating) {
				EndUpdate();
				UpdateLineToEnd(lineNr, Caret.Column);
			}
			++Caret.Column;
//			++Caret.DesiredColumn;
		}
		
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing) {
				Caret.Dispose();
			}
		}
		
		#region UPDATE Commands
		internal void UpdateLine(int line)
		{
			UpdateLines(0, line, line);
		}
		
		internal void UpdateLines(int lineBegin, int lineEnd)
		{
			UpdateLines(0, lineBegin, lineEnd);
		}
		
		internal void UpdateToEnd(int lineBegin)
		{
//			if (lineBegin > FirstPhysicalLine + textView.VisibleLineCount) {
//				return;
//			}
			
			lineBegin     = Math.Min(lineBegin, FirstPhysicalLine);
			int y         = Math.Max(    0, (int)(lineBegin * textView.FontHeight));
			y = Math.Max(0, y - this.virtualTop.Y);
			Rectangle r = new Rectangle(0,
			                            y,
			                            Width,
			                            Height - y);
			Invalidate(r);
		}
		
		internal void UpdateLineToEnd(int lineNr, int xStart)
		{
			UpdateLines(xStart, lineNr, lineNr);
		}
		
		internal void UpdateLine(int line, int begin, int end)
		{
			UpdateLines(line, line);
		}
		int FirstPhysicalLine {
			get {
				return VirtualTop.Y / textView.FontHeight;
			}
		}
		internal void UpdateLines(int xPos, int lineBegin, int lineEnd)
		{
//			if (lineEnd < FirstPhysicalLine || lineBegin > FirstPhysicalLine + textView.VisibleLineCount) {
//				return;
//			}
			
			InvalidateLines((int)(xPos * this.TextView.GetWidth(' ')), lineBegin, lineEnd);
		}
		
		void InvalidateLines(int xPos, int lineBegin, int lineEnd)
		{
			lineBegin     = Math.Max(Document.GetVisibleLine(lineBegin), FirstPhysicalLine);
			lineEnd       = Math.Min(Document.GetVisibleLine(lineEnd),   FirstPhysicalLine + textView.VisibleLineCount);
			int y         = Math.Max(    0, (int)(lineBegin  * textView.FontHeight));
			int height    = Math.Min(textView.DrawingPosition.Height, (int)((1 + lineEnd - lineBegin) * (textView.FontHeight + 1)));
			
			Rectangle r = new Rectangle(0,
			                            y - 1 - this.virtualTop.Y,
			                            Width,
			                            height + 3);
			
			Invalidate(r);
		}
		#endregion
		public event KeyEventHandler    KeyEventHandler;
		public event DialogKeyProcessor DoProcessDialogKey;
	}
}
