// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class handles all mouse stuff for a textArea.
	/// </summary>
	public class TextAreaMouseHandler
	{
		TextArea  textArea;
		bool      doubleclick = false;
		Point     mousepos = new Point(0, 0);
		int       selbegin;
		int       selend;
		bool      clickedOnSelectedText = false;
		
		MouseButtons button;
		
		static readonly Point nilPoint = new Point(-1, -1);
		Point mousedownpos       = nilPoint;
		Point lastmousedownpos   = nilPoint;
		Point selectionStartPos  = nilPoint;
		
		bool gotmousedown = false;
		bool dodragdrop = false;
		
		public TextAreaMouseHandler(TextArea textArea)
		{
			this.textArea = textArea;
		}
		
		public void Attach()
		{
			textArea.Click       += new EventHandler(TextAreaClick);
			textArea.MouseMove   += new MouseEventHandler(TextAreaMouseMove);
			
			textArea.MouseDown   += new MouseEventHandler(OnMouseDown);
			textArea.DoubleClick += new EventHandler(OnDoubleClick);
			textArea.MouseLeave  += new EventHandler(OnMouseLeave);
			textArea.MouseUp     += new MouseEventHandler(OnMouseUp);
			textArea.LostFocus   += new EventHandler(TextAreaLostFocus);
		}
		
		void ShowHiddenCursor()
		{
			if (TextArea.HiddenMouseCursor) {
				Cursor.Show();
				TextArea.HiddenMouseCursor = false;
			}
		}
		
		void TextAreaLostFocus(object sender, EventArgs e)
		{
			ShowHiddenCursor();
		}
		void OnMouseLeave(object sender, EventArgs e)
		{
			ShowHiddenCursor();
			gotmousedown = false;
			mousedownpos = nilPoint;
		}
		
		void OnMouseUp(object sender, MouseEventArgs e)
		{
			gotmousedown = false;
			mousedownpos = nilPoint;
		}
		
		void TextAreaClick(object sender, EventArgs e)
		{
			if (dodragdrop) {
				return;
			}
			
			if (clickedOnSelectedText && textArea.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y)) {
				textArea.SelectionManager.ClearSelection();
				
				Point clickPosition = textArea.TextView.GetLogicalPosition(mousepos.X - textArea.TextView.DrawingPosition.X,
				                                                           mousepos.Y - textArea.TextView.DrawingPosition.Y);
				textArea.Caret.Position = clickPosition;
				textArea.SetDesiredColumn();
			}
		}
		
		
		void TextAreaMouseMove(object sender, MouseEventArgs e)
		{
			ShowHiddenCursor();
			if (dodragdrop) {
				dodragdrop = false;
				return;
			}
			
			doubleclick = false;
			mousepos    = new Point(e.X, e.Y);
			
			if (clickedOnSelectedText) {
				if (Math.Abs(mousedownpos.X - mousepos.X) >= SystemInformation.DragSize.Width / 2 ||
				    Math.Abs(mousedownpos.Y - mousepos.Y) >= SystemInformation.DragSize.Height / 2) {
					clickedOnSelectedText = false;
					ISelection selection = textArea.SelectionManager.GetSelectionAt(textArea.Caret.Offset);
					if (selection != null) {
						string text = selection.SelectedText;
						if (text != null && text.Length > 0) {
							DataObject dataObject = new DataObject ();
							dataObject.SetData(DataFormats.UnicodeText, true, text);
							dataObject.SetData(selection);
							dodragdrop = true;
							textArea.DoDragDrop(dataObject, DragDropEffects.All);
						}
					}
				}
				
				return;
			}
			if (e.Button == MouseButtons.None && !textArea.ToolTipVisible) {
				FoldMarker marker = textArea.TextView.GetFoldMarkerFromPosition(mousepos.X - textArea.TextView.DrawingPosition.X,
				                                                                mousepos.Y - textArea.TextView.DrawingPosition.Y);
				if (marker != null && marker.IsFolded) {
					StringBuilder sb = new StringBuilder(marker.InnerText);
					
					// max 10 lines
					int endLines = 0;
					for (int i = 0; i < sb.Length; ++i) {
						if (sb[i] == '\n') {
							++endLines;
							if (endLines >= 10) {
								sb.Remove(i + 1, sb.Length - i - 1);
								sb.Append(Environment.NewLine);
								sb.Append("...");
								break;
								
							}
						}
					}
					sb.Replace("\t", "    ");
					textArea.SetToolTip(sb.ToString());
					return;
				}
				
				Point clickPosition2 = textArea.TextView.GetLogicalPosition(mousepos.X - textArea.TextView.DrawingPosition.X,
				                                                            mousepos.Y - textArea.TextView.DrawingPosition.Y);
				List<TextMarker> markers = textArea.Document.MarkerStrategy.GetMarkers(clickPosition2);
				foreach (TextMarker tm in markers) {
					if (tm.ToolTip != null) {
						textArea.SetToolTip(tm.ToolTip.Replace("\t", "    "));
						return;
					}
				}
			}
			
			if (e.Button == MouseButtons.Left) {
				if (gotmousedown) {
					ExtendSelectionToMouse();
				}
			}
		}
		
		void ExtendSelectionToMouse()
		{
			Point realmousepos = textArea.TextView.GetLogicalPosition(Math.Max(0, mousepos.X - textArea.TextView.DrawingPosition.X),
			                                                          mousepos.Y - textArea.TextView.DrawingPosition.Y);
			int y = realmousepos.Y;
			realmousepos = textArea.Caret.ValidatePosition(realmousepos);
			Point oldPos = textArea.Caret.Position;
			if (oldPos == realmousepos) {
				return;
			}
			textArea.Caret.Position = realmousepos;
			if (minSelection != nilPoint && textArea.SelectionManager.SelectionCollection.Count > 0) {
				ISelection selection = textArea.SelectionManager.SelectionCollection[0];
				Point min = textArea.SelectionManager.GreaterEqPos(minSelection, maxSelection) ? maxSelection : minSelection;
				Point max = textArea.SelectionManager.GreaterEqPos(minSelection, maxSelection) ? minSelection : maxSelection;
				if (textArea.SelectionManager.GreaterEqPos(max, oldPos) && textArea.SelectionManager.GreaterEqPos(oldPos, min)) {
					textArea.SelectionManager.SetSelection(min, max);
				} else if (textArea.SelectionManager.GreaterEqPos(max, oldPos)) {
					textArea.SelectionManager.SetSelection(oldPos, max);
				} else {
					textArea.SelectionManager.SetSelection(min, oldPos);
				}
			} else {
				textArea.SelectionManager.ExtendSelection(oldPos, textArea.Caret.Position);
			}
			textArea.SetDesiredColumn();
		}
		
		void DoubleClickSelectionExtend()
		{
			textArea.SelectionManager.ClearSelection();
			if (textArea.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y)) {
				FoldMarker marker = textArea.TextView.GetFoldMarkerFromPosition(mousepos.X - textArea.TextView.DrawingPosition.X,
				                                                                mousepos.Y - textArea.TextView.DrawingPosition.Y);
				if (marker != null && marker.IsFolded) {
					marker.IsFolded = false;
					textArea.MotherTextAreaControl.AdjustScrollBars(null, null);
				}
				if (textArea.Caret.Offset < textArea.Document.TextLength) {
					switch (textArea.Document.GetCharAt(textArea.Caret.Offset)) {
						case '"':
							if (textArea.Caret.Offset < textArea.Document.TextLength) {
								int next = FindNext(textArea.Document, textArea.Caret.Offset + 1, '"');
								minSelection = textArea.Caret.Position;
								maxSelection = textArea.Document.OffsetToPosition(next > textArea.Caret.Offset ? next + 1 : next);
							}
							break;
						default:
							minSelection = textArea.Document.OffsetToPosition(FindWordStart(textArea.Document, textArea.Caret.Offset));
							maxSelection = textArea.Document.OffsetToPosition(FindWordEnd(textArea.Document, textArea.Caret.Offset));
							break;
					
					}
					textArea.SelectionManager.ExtendSelection(minSelection, maxSelection);
				}
				// HACK WARNING !!! 
				// must refresh here, because when a error tooltip is showed and the underlined
				// code is double clicked the textArea don't update corrctly, updateline doesn't
				// work ... but the refresh does.
				// Mike
				textArea.Refresh(); 
			}
		}

		
		DateTime lastTime = DateTime.Now;
		void OnMouseDown(object sender, MouseEventArgs e)
		{ 
			if (dodragdrop) {
				return;
			}
			
			if (doubleclick) {
				doubleclick = false;
				return;
			}
			
			
			if (textArea.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y)) {
				gotmousedown = true;
				button = e.Button;
				
				if ((DateTime.Now - lastTime).Milliseconds < SystemInformation.DoubleClickTime) {
					int deltaX   = Math.Abs(lastmousedownpos.X - e.X);
					int deltaY   = Math.Abs(lastmousedownpos.Y - e.Y);
					if (deltaX <= SystemInformation.DoubleClickSize.Width &&
					    deltaY <= SystemInformation.DoubleClickSize.Height) {
						DoubleClickSelectionExtend();
						lastTime = DateTime.Now;
						lastmousedownpos = new Point(e.X, e.Y);
						return;
					}
				}
				minSelection = nilPoint;
				maxSelection = nilPoint;
				
				lastTime = DateTime.Now;
				lastmousedownpos = mousedownpos = new Point(e.X, e.Y);
				
				
				if (button == MouseButtons.Left) {
					FoldMarker marker = textArea.TextView.GetFoldMarkerFromPosition(mousepos.X - textArea.TextView.DrawingPosition.X,
					                                                                mousepos.Y - textArea.TextView.DrawingPosition.Y);
					if (marker != null && marker.IsFolded) {
						if (textArea.SelectionManager.HasSomethingSelected) {	
							clickedOnSelectedText = true;
						}
						
						textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.TextView.Document, new Point(marker.StartColumn, marker.StartLine), new Point(marker.EndColumn, marker.EndLine)));
						textArea.Focus();
						return;
					}

					if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) {
						ExtendSelectionToMouse();
					} else {
						Point realmousepos = textArea.TextView.GetLogicalPosition(mousepos.X - textArea.TextView.DrawingPosition.X, mousepos.Y - textArea.TextView.DrawingPosition.Y);
						clickedOnSelectedText = false;
						
						int offset = textArea.Document.PositionToOffset(realmousepos);
								
						
						if (textArea.SelectionManager.HasSomethingSelected && 
						    textArea.SelectionManager.IsSelected(offset)) {	
							clickedOnSelectedText = true;
						} else {
							selbegin = selend = offset;
							textArea.SelectionManager.ClearSelection();
							if (mousepos.Y > 0 && mousepos.Y < textArea.TextView.DrawingPosition.Height) {
								Point pos = new Point();
								pos.Y = Math.Min(textArea.Document.TotalNumberOfLines - 1,  realmousepos.Y);
								pos.X = realmousepos.X;
								textArea.Caret.Position = pos;//Math.Max(0, Math.Min(textArea.Document.TextLength, line.Offset + Math.Min(line.Length, pos.X)));
								textArea.SetDesiredColumn();
							}
						}
					}
				}
			}
			textArea.Focus();
		}
		
		int FindNext(IDocument document, int offset, char ch)
		{
			LineSegment line = document.GetLineSegmentForOffset(offset);
			int         endPos = line.Offset + line.Length;
			
			while (offset < endPos && document.GetCharAt(offset) != ch) {
				++offset;
			}
			return offset;
		}
		
		bool IsSelectableChar(char ch)
		{
			return Char.IsLetterOrDigit(ch) || ch=='_';
		}
		
		int FindWordStart(IDocument document, int offset)
		{
			LineSegment line = document.GetLineSegmentForOffset(offset);
			
			if (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset - 1)) && Char.IsWhiteSpace(document.GetCharAt(offset))) {
				while (offset > line.Offset && Char.IsWhiteSpace(document.GetCharAt(offset - 1))) {
					--offset;
				}
			} else  if (IsSelectableChar(document.GetCharAt(offset)) || (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset)) && IsSelectableChar(document.GetCharAt(offset - 1))))  {
				while (offset > line.Offset && IsSelectableChar(document.GetCharAt(offset - 1))) {
					--offset;
				}
			} else {
				if (offset > 0 && !Char.IsWhiteSpace(document.GetCharAt(offset - 1)) && !IsSelectableChar(document.GetCharAt(offset - 1)) ) {
					return Math.Max(0, offset - 1);
				}
			}
			return offset;
		}
		
		int FindWordEnd(IDocument document, int offset)
		{
			LineSegment line   = document.GetLineSegmentForOffset(offset);
			int         endPos = line.Offset + line.Length;
			
			if (IsSelectableChar(document.GetCharAt(offset)))  {
				while (offset < endPos && IsSelectableChar(document.GetCharAt(offset))) {
					++offset;
				}
			} else if (Char.IsWhiteSpace(document.GetCharAt(offset))) {
				if (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset - 1))) {
					while (offset < endPos && Char.IsWhiteSpace(document.GetCharAt(offset))) {
						++offset;
					}
				}
			} else {
				return Math.Max(0, offset + 1);
			}
			
			return offset;
		}
		Point minSelection = nilPoint;
		Point maxSelection = nilPoint;
		
		

		void OnDoubleClick(object sender, System.EventArgs e)
		{
			if (dodragdrop) {
				return;
			}
			
			doubleclick = true;
			
		}
	}
}
