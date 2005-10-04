// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class views the line numbers and folding markers.
	/// </summary>
	public class GutterMargin : AbstractMargin
	{
		StringFormat numberStringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
		
		public static Cursor RightLeftCursor;
		
		static GutterMargin()
		{
			Stream cursorStream = Assembly.GetCallingAssembly().GetManifestResourceStream("ICSharpCode.TextEditor.Resources.RightArrow.cur");
			RightLeftCursor = new Cursor(cursorStream);
			cursorStream.Close();
		}
		
		
		public override Cursor Cursor {
			get {
				return RightLeftCursor;
			}
		}
		
		public override Size Size {
			get {
				return new Size((int)(textArea.TextView.WideSpaceWidth
				                      * Math.Max(3, (int)Math.Log10(textArea.Document.TotalNumberOfLines) + 1)),
				                -1);
			}
		}
		
		public override bool IsVisible {
			get {
				return textArea.TextEditorProperties.ShowLineNumbers;
			}
		}
		
		public GutterMargin(TextArea textArea) : base(textArea)
		{
			numberStringFormat.LineAlignment = StringAlignment.Far;
			numberStringFormat.FormatFlags   = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.FitBlackBox |
			                                    StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
		}
		
		public override void Paint(Graphics g, Rectangle rect)
		{
			if (rect.Width <= 0 || rect.Height <= 0) {
				return;
			}
			HighlightColor lineNumberPainterColor = textArea.Document.HighlightingStrategy.GetColorFor("LineNumbers");
			int fontHeight = textArea.TextView.FontHeight;
			Brush fillBrush = textArea.Enabled ? BrushRegistry.GetBrush(lineNumberPainterColor.BackgroundColor) : SystemBrushes.InactiveBorder;
			Brush drawBrush = BrushRegistry.GetBrush(lineNumberPainterColor.Color);
			for (int y = 0; y < (DrawingPosition.Height + textArea.TextView.VisibleLineDrawingRemainder) / fontHeight + 1; ++y) {
				int ypos = drawingPosition.Y + fontHeight * y  - textArea.TextView.VisibleLineDrawingRemainder;
				Rectangle backgroundRectangle = new Rectangle(drawingPosition.X, ypos, drawingPosition.Width, fontHeight);
				if (rect.IntersectsWith(backgroundRectangle)) {
					g.FillRectangle(fillBrush, backgroundRectangle);
					int curLine = textArea.Document.GetFirstLogicalLine(textArea.Document.GetVisibleLine(textArea.TextView.FirstVisibleLine) + y);
					
					if (curLine < textArea.Document.TotalNumberOfLines) {
						g.DrawString((curLine + 1).ToString(),
						             lineNumberPainterColor.Font,
						             drawBrush,
						             backgroundRectangle,
						             numberStringFormat);
					}
				}
			}
		}
		
		Point selectionStartPos;
		bool selectionComeFromGutter = false;
		bool selectionGutterDirectionDown = false; // direction of gutter selection affects whether a selection starts at the start of a line or at the end of a line
		public override void HandleMouseDown(Point mousepos, MouseButtons mouseButtons)
		{
			selectionComeFromGutter = true;
			int realline = textArea.TextView.GetLogicalLine(mousepos);
			if (realline >= 0 && realline < textArea.Document.TotalNumberOfLines) {
				if((Control.ModifierKeys & Keys.Shift) != 0 && textArea.SelectionManager.HasSomethingSelected) {
					// let MouseMove handle a shift-click in a gutter
					HandleMouseMove(mousepos, mouseButtons);
				} else {
					selectionGutterDirectionDown = false; // reset the flag for handling in mousemove
					selectionStartPos = new Point(0, realline);
					textArea.SelectionManager.ClearSelection();
					textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, new Point(textArea.Document.GetLineSegment(realline).Length + 1, realline)));
					textArea.Caret.Position = selectionStartPos;
				}
			}
		}
		
		public override void HandleMouseLeave(EventArgs e)
		{
			selectionComeFromGutter = false;
		}
		
		public override void HandleMouseMove(Point mousepos, MouseButtons mouseButtons)
		{
			if (mouseButtons == MouseButtons.Left) {
				if (selectionComeFromGutter) {
					//TODO: Fix handling of mouse moving off to the left of the gutter before moving the selection down.  Behaviour of selection changes after mouse moves left of gutter while selecting lines
					int realline       = textArea.TextView.GetLogicalLine(mousepos);
					Point realmousepos = new Point(0, realline);
					if (realmousepos.Y < textArea.Document.TotalNumberOfLines) {
						if (selectionStartPos.Y == realmousepos.Y) {
							// this setselection defaults for a upward moving selection
							textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, realmousepos, new Point(0, realmousepos.Y + 1)));
							selectionGutterDirectionDown = false;
						} else if (selectionStartPos.Y < realmousepos.Y && textArea.SelectionManager.HasSomethingSelected) {
							// this fixes the selection for moving the selection down
							if (! selectionGutterDirectionDown) { //realmousepos.Y - selectionStartPos.Y == 1) {
								selectionGutterDirectionDown = true;
								textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, new Point(0, selectionStartPos.Y)));
								// this enforces the screen area update
								textArea.SelectionManager.ExtendSelection(textArea.SelectionManager.SelectionCollection[0].EndPosition, new Point(0, realmousepos.Y + 1));
							} else {
								// selection is extended to the end of the current line
								textArea.SelectionManager.ExtendSelection(textArea.SelectionManager.SelectionCollection[0].EndPosition, new Point(0, realmousepos.Y + 1));
							}
						} else {
							if(textArea.SelectionManager.HasSomethingSelected) {
								// this fixes the selection for moving the selection up
								if (selectionGutterDirectionDown) { // selectionStartPos.Y - realmousepos.Y == 1) { // only fix for first line movement
									selectionGutterDirectionDown = false;
									textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, new Point(0, realmousepos.Y + 1)));
									// move the extendselection to here to fix textarea update issues
									textArea.SelectionManager.ExtendSelection(selectionStartPos, realmousepos);
								} else {
									textArea.SelectionManager.ExtendSelection(textArea.Caret.Position, realmousepos);
								}
							}
						}
					
						textArea.Caret.Position = realmousepos;
					}
				} else {
					if (textArea.SelectionManager.HasSomethingSelected) {
						selectionStartPos  = textArea.Document.OffsetToPosition(textArea.SelectionManager.SelectionCollection[0].Offset);
						int realline       = textArea.TextView.GetLogicalLine(mousepos);
						Point realmousepos = new Point(0, realline);
						if (realmousepos.Y < textArea.Document.TotalNumberOfLines) {
							textArea.SelectionManager.ExtendSelection(textArea.Caret.Position, realmousepos);
						}
						textArea.Caret.Position = realmousepos;
					}
				}
			}
		}
	}
}
