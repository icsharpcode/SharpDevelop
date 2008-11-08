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
using System.Windows;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System.Windows.Threading;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// Handles selection of text using the mouse.
	/// </summary>
	sealed class SelectionMouseHandler
	{
		readonly TextArea textArea;
		
		public SelectionMouseHandler(TextArea textArea)
		{
			this.textArea = textArea;
		}
		
		public void Attach()
		{
			textArea.MouseLeftButtonDown += textArea_MouseLeftButtonDown;
			textArea.MouseMove += textArea_MouseMove;
			textArea.MouseLeftButtonUp += textArea_MouseLeftButtonUp;
			textArea.QueryCursor += textArea_QueryCursor;
			if (AllowTextDragDrop) {
				textArea.AllowDrop = true;
				textArea.GiveFeedback += textArea_GiveFeedback;
				textArea.QueryContinueDrag += textArea_QueryContinueDrag;
				textArea.DragEnter += textArea_DragEnter;
				textArea.DragOver +=  textArea_DragOver;
				textArea.DragLeave += textArea_DragLeave;
				textArea.Drop += textArea_Drop;
			}
		}

		void textArea_DragEnter(object sender, DragEventArgs e)
		{
			try {
				e.Effects = GetEffect(e);
			} catch (Exception ex) {
				OnDragException(ex);
			}
		}

		void textArea_DragOver(object sender, DragEventArgs e)
		{
			try {
				e.Effects = GetEffect(e);
			} catch (Exception ex) {
				OnDragException(ex);
			}
		}
		
		DragDropEffects GetEffect(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.UnicodeText, true)) {
				e.Handled = true;
				int visualColumn;
				int offset = GetOffsetFromMousePosition(e.GetPosition(textArea.TextView), out visualColumn);
				if (offset >= 0) {
					textArea.Caret.Position = new TextViewPosition(textArea.Document.GetLocation(offset), visualColumn);
					textArea.Caret.DesiredXPos = double.NaN;
					if ((e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move
					    && (e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.ControlKey
					    && textArea.ReadOnlySectionProvider.CanInsert(offset))
					{
						return DragDropEffects.Move;
					} else {
						return e.AllowedEffects & DragDropEffects.Copy;
					}
				}
			}
			return DragDropEffects.None;
		}
		
		void textArea_DragLeave(object sender, DragEventArgs e)
		{
			e.Handled = true;
		}
		
		void textArea_Drop(object sender, DragEventArgs e)
		{
			try {
				DragDropEffects effect = GetEffect(e);
				e.Effects = effect;
				if (effect != DragDropEffects.None) {
					string text = e.Data.GetData(DataFormats.UnicodeText, true) as string;
					if (text != null) {
						int start = textArea.Caret.Offset;
						if (mode == SelectionMode.Drag && Contains(textArea.Selection.SurroundingSegment, start)) {
							Debug.WriteLine("Drop: did not drop: drop target is inside selection");
							e.Effects = DragDropEffects.None;
						} else {
							Debug.WriteLine("Drop: insert at " + start);
							textArea.Document.Insert(start, text);
							textArea.Selection = new SimpleSelection(start, start + text.Length);
						}
					}
				}
			} catch (Exception ex) {
				OnDragException(ex);
			}
		}
		
		void OnDragException(Exception ex)
		{
			// WPF swallows exceptions during drag'n'drop or reports them incorrectly, so
			// we re-throw them later to allow the application's unhandled exception handler
			// to catch them
			textArea.Dispatcher.BeginInvoke(
				DispatcherPriority.Normal,
				new Action(delegate {
				           	throw new Exception("Exception during drag'n'drop", ex);
				           }));
		}
		
		public void Detach()
		{
			mode = SelectionMode.None;
			textArea.MouseLeftButtonDown -= textArea_MouseLeftButtonDown;
			textArea.MouseMove -= textArea_MouseMove;
			textArea.MouseLeftButtonUp -= textArea_MouseLeftButtonUp;
			textArea.QueryCursor -= textArea_QueryCursor;
			if (AllowTextDragDrop) {
				textArea.GiveFeedback -= textArea_GiveFeedback;
				textArea.QueryContinueDrag -= textArea_QueryContinueDrag;
			}
		}
		
		// TODO: allow disabling text drag'n'drop
		const bool AllowTextDragDrop = true;
		
		// provide the IBeam Cursor for the text area
		void textArea_QueryCursor(object sender, QueryCursorEventArgs e)
		{
			if (!e.Handled) {
				if (mode != SelectionMode.None || !AllowTextDragDrop) {
					e.Cursor = Cursors.IBeam;
					e.Handled = true;
				} else {
					Point p = e.GetPosition(textArea.TextView);
					if (p.X >= 0 && p.Y >= 0 && p.X <= textArea.TextView.ActualWidth && p.Y <= textArea.TextView.ActualHeight) {
						int visualColumn;
						int offset = GetOffsetFromMousePosition(e, out visualColumn);
						if (SelectionContains(textArea.Selection, offset))
							e.Cursor = Cursors.Arrow;
						else
							e.Cursor = Cursors.IBeam;
						e.Handled = true;
					}
				}
			}
		}
		
		static bool SelectionContains(Selection selection, int offset)
		{
			if (selection.IsEmpty)
				return false;
			if (offset >= 0 && Contains(selection.SurroundingSegment, offset)) {
				foreach (ISegment s in selection.Segments) {
					if (Contains(s, offset)) {
						return true;
					}
				}
			}
			return false;
		}
		
		static bool Contains(ISegment segment, int offset)
		{
			if (segment == null)
				return false;
			int start = segment.Offset;
			int end = start + segment.Length;
			return offset >= start && offset <= end;
		}
		
		enum SelectionMode
		{
			/// <summary>
			/// no selection (no mouse button down)
			/// </summary>
			None,
			/// <summary>
			/// left mouse button down on selection, might be normal click
			/// or might be drag'n'drop
			/// </summary>
			PossibleDragStart,
			/// <summary>
			/// dragging text
			/// </summary>
			Drag,
			/// <summary>
			/// normal selection (click+drag)
			/// </summary>
			Normal,
			/// <summary>
			/// whole-word selection (double click+drag)
			/// </summary>
			WholeWord
		}
		
		SelectionMode mode;
		AnchorSegment startWord;
		Point possibleDragStartMousePos;
		
		void textArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			mode = SelectionMode.None;
			if (!e.Handled && e.ChangedButton == MouseButton.Left) {
				bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
				if (AllowTextDragDrop && e.ClickCount == 1 && !shift) {
					int visualColumn;
					int offset = GetOffsetFromMousePosition(e, out visualColumn);
					if (SelectionContains(textArea.Selection, offset)) {
						if (textArea.CaptureMouse()) {
							mode = SelectionMode.PossibleDragStart;
							possibleDragStartMousePos = e.GetPosition(textArea);
						}
						e.Handled = true;
						return;
					}
				}
				
				int oldOffset = textArea.Caret.Offset;
				SetCaretOffsetToMousePosition(e);
				
				
				if (!shift) {
					textArea.Selection = Selection.Empty;
				}
				if (textArea.CaptureMouse()) {
					if (e.ClickCount == 1) {
						mode = SelectionMode.Normal;
						if (shift) {
							textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(oldOffset, textArea.Caret.Offset);
						}
					} else {
						mode = SelectionMode.WholeWord;
						var startWord = GetWordAtMousePosition(e);
						if (startWord == SimpleSegment.Invalid) {
							mode = SelectionMode.None;
							textArea.ReleaseMouseCapture();
							return;
						}
						if (shift && !textArea.Selection.IsEmpty) {
							if (startWord.Offset < textArea.Selection.SurroundingSegment.Offset) {
								textArea.Selection = textArea.Selection.SetEndpoint(startWord.Offset);
							} else if (startWord.GetEndOffset() > textArea.Selection.SurroundingSegment.GetEndOffset()) {
								textArea.Selection = textArea.Selection.SetEndpoint(startWord.GetEndOffset());
							}
							this.startWord = new AnchorSegment(textArea.Document, textArea.Selection.SurroundingSegment);
						} else {
							textArea.Selection = new SimpleSelection(startWord.Offset, startWord.GetEndOffset());
							this.startWord = new AnchorSegment(textArea.Document, startWord.Offset, startWord.Length);
						}
					}
				}
			}
			e.Handled = true;
		}
		
		SimpleSegment GetWordAtMousePosition(MouseEventArgs e)
		{
			TextView textView = textArea.TextView;
			if (textView == null) return SimpleSegment.Invalid;
			textView.EnsureVisualLines();
			Point pos = e.GetPosition(textView);
			if (pos.Y < 0)
				pos.Y = 0;
			if (pos.Y > textView.ActualHeight)
				pos.Y = textView.ActualHeight;
			pos += textView.ScrollOffset;
			VisualLine line = textView.GetVisualLineFromVisualTop(pos.Y);
			if (line != null) {
				int visualColumn = line.GetVisualColumn(pos);
				int wordStartVC = line.GetNextCaretPosition(visualColumn + 1, true, CaretPositioningMode.WordStart);
				if (wordStartVC == -1)
					wordStartVC = 0;
				int wordEndVC = line.GetNextCaretPosition(wordStartVC, false, CaretPositioningMode.WordBorder);
				if (wordEndVC == -1)
					wordEndVC = line.VisualLength;
				int relOffset = line.FirstDocumentLine.Offset;
				int wordStartOffset = line.GetRelativeOffset(wordStartVC) + relOffset;
				int wordEndOffset = line.GetRelativeOffset(wordEndVC) + relOffset;
				return new SimpleSegment(wordStartOffset, wordEndOffset - wordStartOffset);
			} else {
				return SimpleSegment.Invalid;
			}
		}
		
		void SetCaretOffsetToMousePosition(MouseEventArgs e)
		{
			int visualColumn;
			int offset = GetOffsetFromMousePosition(e, out visualColumn);
			if (offset >= 0) {
				textArea.Caret.Position = new TextViewPosition(textArea.Document.GetLocation(offset), visualColumn);
				textArea.Caret.DesiredXPos = double.NaN;
			}
		}
		
		int GetOffsetFromMousePosition(MouseEventArgs e, out int visualColumn)
		{
			return GetOffsetFromMousePosition(e.GetPosition(textArea.TextView), out visualColumn);
		}
		
		int GetOffsetFromMousePosition(Point positionRelativeToTextView, out int visualColumn)
		{
			visualColumn = 0;
			TextView textView = textArea.TextView;
			textView.EnsureVisualLines();
			Point pos = positionRelativeToTextView;
			if (pos.Y < 0)
				pos.Y = 0;
			if (pos.Y > textView.ActualHeight)
				pos.Y = textView.ActualHeight;
			pos += textView.ScrollOffset;
			if (pos.Y > textView.DocumentHeight)
				pos.Y = textView.DocumentHeight - ExtensionMethods.Epsilon;
			VisualLine line = textView.GetVisualLineFromVisualTop(pos.Y);
			if (line != null) {
				visualColumn = line.GetVisualColumn(pos);
				return line.GetRelativeOffset(visualColumn) + line.FirstDocumentLine.Offset;
			}
			return -1;
		}
		
		void textArea_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Handled)
				return;
			if (mode == SelectionMode.Normal || mode == SelectionMode.WholeWord) {
				e.Handled = true;
				int oldOffset = textArea.Caret.Offset;
				SetCaretOffsetToMousePosition(e);
				if (mode == SelectionMode.Normal) {
					textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(oldOffset, textArea.Caret.Offset);
				} else if (mode == SelectionMode.WholeWord) {
					var newWord = GetWordAtMousePosition(e);
					if (newWord != SimpleSegment.Invalid) {
						textArea.Selection = new SimpleSelection(
							Math.Min(newWord.Offset, startWord.Offset),
							Math.Max(newWord.GetEndOffset(), startWord.GetEndOffset()));
					}
				}
			} else if (mode == SelectionMode.PossibleDragStart) {
				e.Handled = true;
				Vector mouseMovement = e.GetPosition(textArea) - possibleDragStartMousePos;
				if (Math.Abs(mouseMovement.X) > SystemParameters.MinimumHorizontalDragDistance
				    || Math.Abs(mouseMovement.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					StartDrag();
				}
			}
		}
		
		void textArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (mode == SelectionMode.None || e.Handled)
				return;
			e.Handled = true;
			if (mode == SelectionMode.PossibleDragStart) {
				// -> this was not a drag start (mouse didn't move after mousedown)
				SetCaretOffsetToMousePosition(e);
				textArea.Selection = Selection.Empty;
			}
			mode = SelectionMode.None;
			textArea.ReleaseMouseCapture();
		}
		
		void StartDrag()
		{
			// prevent nested StartDrag calls
			mode = SelectionMode.Drag;
			
			// mouse capture and Drag'n'Drop doesn't mix
			textArea.ReleaseMouseCapture();
			
			string text = textArea.Selection.GetText(textArea.Document);
			DataObject dataObject = new DataObject();
			dataObject.SetText(text);
			
			DragDropEffects allowedEffects = DragDropEffects.All;
			List<AnchorSegment> deleteOnMove;
			deleteOnMove = textArea.Selection.Segments.Select(s => new AnchorSegment(textArea.Document, s)).ToList();
			
			Debug.WriteLine("DoDragDrop with allowedEffects=" + allowedEffects);
			DragDropEffects resultEffect = DragDrop.DoDragDrop(textArea, dataObject, allowedEffects);
			Debug.WriteLine("DoDragDrop done, resultEffect=" + resultEffect);
			
			if (deleteOnMove != null && resultEffect == DragDropEffects.Move) {
				textArea.Document.BeginUpdate();
				try {
					foreach (ISegment s in deleteOnMove) {
						textArea.Document.Remove(s.Offset, s.Length);
					}
				} finally {
					textArea.Document.EndUpdate();
				}
			}
		}

		void textArea_GiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			e.UseDefaultCursors = true;
			e.Handled = true;
		}
		
		void textArea_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (e.EscapePressed) {
				e.Action = DragAction.Cancel;
			} else if ((e.KeyStates & DragDropKeyStates.LeftMouseButton) != DragDropKeyStates.LeftMouseButton) {
				e.Action = DragAction.Drop;
			} else {
				e.Action = DragAction.Continue;
			}
			e.Handled = true;
		}
	}
}
