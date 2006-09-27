// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
		public static Point selectionStartPos  = nilPoint;
		
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
			textArea.ToolTipRequest += new ToolTipRequestEventHandler(OnToolTipRequest);
		}
		
		void OnToolTipRequest(object sender, ToolTipRequestEventArgs e)
		{
			if (e.ToolTipShown)
				return;
			Point mousepos = e.MousePosition;
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
				e.ShowToolTip(sb.ToString());
				return;
			}
			
			List<TextMarker> markers = textArea.Document.MarkerStrategy.GetMarkers(e.LogicalPosition);
			foreach (TextMarker tm in markers) {
				if (tm.ToolTip != null) {
					e.ShowToolTip(tm.ToolTip.Replace("\t", "    "));
					return;
				}
			}
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
            // honour the starting selection strategy
            switch(SelectFrom.where) {
                case WhereFrom.Gutter:
                    moveGutter(sender, e);
                    break;

                case WhereFrom.TArea:
                    break;

            }
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
				// Extend selection when selection was started with double-click
				ISelection selection = textArea.SelectionManager.SelectionCollection[0];
				Point min = textArea.SelectionManager.GreaterEqPos(minSelection, maxSelection) ? maxSelection : minSelection;
				Point max = textArea.SelectionManager.GreaterEqPos(minSelection, maxSelection) ? minSelection : maxSelection;
				if (textArea.SelectionManager.GreaterEqPos(max, realmousepos) && textArea.SelectionManager.GreaterEqPos(realmousepos, min)) {
					textArea.SelectionManager.SetSelection(min, max);
				} else if (textArea.SelectionManager.GreaterEqPos(max, realmousepos)) {
					//textArea.SelectionManager.SetSelection(realmousepos, max);
					int moff = textArea.Document.PositionToOffset(realmousepos);
					min = textArea.Document.OffsetToPosition(FindWordStart(textArea.Document, moff));
					textArea.SelectionManager.SetSelection(min, max);
				} else {
					//textArea.SelectionManager.SetSelection(min, realmousepos);
					int moff = textArea.Document.PositionToOffset(realmousepos);
					max = textArea.Document.OffsetToPosition(FindWordEnd(textArea.Document, moff));
					textArea.SelectionManager.SetSelection(min, max);
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
					textArea.MotherTextAreaControl.AdjustScrollBars();
				}
				if (textArea.Caret.Offset < textArea.Document.TextLength) {
					switch (textArea.Document.GetCharAt(textArea.Caret.Offset)) {
						case '"':
							if (textArea.Caret.Offset < textArea.Document.TextLength) {
								int next = FindNext(textArea.Document, textArea.Caret.Offset + 1, '"');
								minSelection = textArea.Caret.Position;
								if (next > textArea.Caret.Offset && next < textArea.Document.TextLength)
									next += 1;
								maxSelection = textArea.Document.OffsetToPosition(next);
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
                SelectFrom.where = WhereFrom.TArea;
				button = e.Button;
				
				if (button == MouseButtons.Left && (DateTime.Now - lastTime).Milliseconds < SystemInformation.DoubleClickTime) {
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
				} else if (button == MouseButtons.Right) {
					// Rightclick sets the cursor to the click position unless
					// the previous selection was clicked
					Point realmousepos = textArea.TextView.GetLogicalPosition(mousepos.X - textArea.TextView.DrawingPosition.X, mousepos.Y - textArea.TextView.DrawingPosition.Y);
					int offset = textArea.Document.PositionToOffset(realmousepos);
					if (!textArea.SelectionManager.HasSomethingSelected ||
					    !textArea.SelectionManager.IsSelected(offset))
					{
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
			offset = Math.Min(offset, endPos - 1);
			
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

		bool selectionGutterDirectionDown = true; // direction of gutter selection affects whether a selection starts at the start of a line or at the end of a line
        int gutterlastmoveline = -1; // last gutter movement line
        public int gutterxstart = 0; // if initial selection is made from the gutter then set to zero, otherwise take the original starting position (from a mouse or keyboard select)
        void moveGutter(object sender, MouseEventArgs e) 
        {
            MouseButtons mouseButtons = e.Button;
            if (mouseButtons == MouseButtons.Left)
            {
                bool directionChanged = false; // gone from a downward moving selection to an upward, or vice-versa
                int realline = textArea.TextView.GetLogicalLine(mousepos);
                Point realmousepos = new Point(0, realline);

                // avoid reprocessing the same selected line (prevent flicker)
                if (gutterlastmoveline == realmousepos.Y)
                    return;
                else
                {
                    if (realmousepos.Y < selectionStartPos.Y)
                    {
                        // we need to record when the behaviour changes
                        // used down further
                        if (selectionGutterDirectionDown)
                            directionChanged = true;

                        // determine the selection strategy
                        selectionGutterDirectionDown = false;
                    }
                    else
                    {
                        // we need to record when the behaviour changes
                        // used down further
                        if (!selectionGutterDirectionDown)
                            directionChanged = true;

                        // determine the selection strategy
                        selectionGutterDirectionDown = true;
                    }

                    gutterlastmoveline = realmousepos.Y;
                }

                // between 1st and last lines - last line is handled differently
                if (realmousepos.Y < textArea.Document.TotalNumberOfLines - 1)
                {
                    if (selectionStartPos.Y == realmousepos.Y)
                    {
                        // this setselection defaults for a upward moving selection
                        textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, realmousepos, new Point(selectionStartPos.X, realmousepos.Y + 1)));
                    }
                    else if (selectionStartPos.Y < realmousepos.Y && textArea.SelectionManager.HasSomethingSelected)
                    {
                        // this fixes the selection for moving the selection down
                        if (directionChanged)
                        {
                            textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, new Point(selectionStartPos.X, selectionStartPos.Y)));
                            // this enforces the screen area update
                            textArea.SelectionManager.ExtendSelection(textArea.SelectionManager.SelectionCollection[0].EndPosition, new Point(0, realmousepos.Y + 1));
                        }
                        else
                        {
                            // selection is extended to the end of the current line
                            textArea.SelectionManager.ExtendSelection(textArea.SelectionManager.SelectionCollection[0].EndPosition, new Point(0, realmousepos.Y + 1));
                        }
                    }
                    else
                    {
                        // when something is already selected we must repect the existing selection
                        if (textArea.SelectionManager.HasSomethingSelected)
                        {
                            // this fixes the selection for moving the selection up
                            if (realmousepos.Y < selectionStartPos.Y && directionChanged) // the first time an upward selection moves above the first selection line
                            {
                                textArea.SelectionManager.ClearSelection();
                                selectionStartPos = new Point(selectionStartPos.X, TextAreaMouseHandler.selectionStartPos.Y);

                                if ((Control.ModifierKeys & Keys.Shift) != 0 && selectionStartPos.X != 0)
                                {
                                    // shift-click
                                    textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, new Point(selectionStartPos.X, selectionStartPos.Y)));
                                }
                                else
                                {
                                    textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, new Point(selectionStartPos.X, selectionStartPos.Y + 1)));
                                }
                                textArea.SelectionManager.ExtendSelection(new Point(selectionStartPos.X, selectionStartPos.Y), new Point(realmousepos.X, realmousepos.Y));
                            }
                            else
                            {
                                textArea.SelectionManager.ExtendSelection(textArea.Caret.Position, realmousepos);
                            }
                        }
                    }

                    if (realmousepos.Y >= selectionStartPos.Y) // at or below starting selection, place the cursor on the next line
                        textArea.Caret.Position = new Point(0, realmousepos.Y + 1);
                    else // prior lines to starting selection, place the cursor on the same line as the new selection
                        textArea.Caret.Position = new Point(0, realmousepos.Y);
                }
                else
                {
                    if (realmousepos.Y == textArea.Document.TotalNumberOfLines - 1)
                    {
                        // selection is last line of text
                        textArea.SelectionManager.ClearSelection();
                        textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, new Point(selectionStartPos.X, selectionStartPos.Y), new Point(textArea.Document.GetLineSegment(realline).Length + 1, realline)));
                        textArea.Caret.Position = new Point(textArea.Document.GetLineSegment(realline).Length + 1, realline);
                    }
                }
            }
            else
                gutterlastmoveline = -1;
        }
	}

    public class SelectFrom {
        public static int where = WhereFrom.None;
    }

    public class WhereFrom {
        public const int None = 0;
        public const int Gutter = 1;
        public const int TArea = 2;
    }
}
