// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Icon bar: contains breakpoints and other icons.
	/// </summary>
	public class IconBarMargin : AbstractMargin, IDisposable
	{
		readonly IBookmarkMargin manager;
		readonly MouseHoverLogic hoverLogic;
		
		public IconBarMargin(IBookmarkMargin manager)
		{
			if (manager == null)
				throw new ArgumentNullException("manager");
			this.manager = manager;
			this.hoverLogic = new MouseHoverLogic(this);
			this.hoverLogic.MouseHover += (sender, e) => MouseHover(this, e);
			this.hoverLogic.MouseHoverStopped += (sender, e) => MouseHoverStopped(this, e);
			this.Unloaded += OnUnloaded;
		}
		
		#region OnTextViewChanged
		/// <inheritdoc/>
		protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
		{
			if (oldTextView != null) {
				oldTextView.VisualLinesChanged -= OnRedrawRequested;
				oldTextView.MouseMove -= TextViewMouseMove;
				manager.RedrawRequested -= OnRedrawRequested;
			}
			base.OnTextViewChanged(oldTextView, newTextView);
			if (newTextView != null) {
				newTextView.VisualLinesChanged += OnRedrawRequested;
				newTextView.MouseMove += TextViewMouseMove;
				manager.RedrawRequested += OnRedrawRequested;
			}
			InvalidateVisual();
		}
		
		void OnRedrawRequested(object sender, EventArgs e)
		{
			// Don't invalidate the IconBarMargin if it'll be invalidated again once the
			// visual lines become valid.
			if (this.TextView != null && this.TextView.VisualLinesValid) {
				InvalidateVisual();
			}
		}
		
		public virtual void Dispose()
		{
			this.TextView = null; // detach from TextView (will also detach from manager)
		}
		#endregion
		
		/// <inheritdoc/>
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			// accept clicks even when clicking on the background
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}
		
		/// <inheritdoc/>
		protected override Size MeasureOverride(Size availableSize)
		{
			return new Size(18, 0);
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			Size renderSize = this.RenderSize;
			drawingContext.DrawRectangle(SystemColors.ControlBrush, null,
			                             new Rect(0, 0, renderSize.Width, renderSize.Height));
			drawingContext.DrawLine(new Pen(SystemColors.ControlDarkBrush, 1),
			                        new Point(renderSize.Width - 0.5, 0),
			                        new Point(renderSize.Width - 0.5, renderSize.Height));
			
			TextView textView = this.TextView;
			if (textView != null && textView.VisualLinesValid) {
				// create a dictionary line number => first bookmark
				Dictionary<int, IBookmark> bookmarkDict = new Dictionary<int, IBookmark>();
				foreach (IBookmark bm in manager.Bookmarks) {
					int line = bm.LineNumber;
					IBookmark existingBookmark;
					if (!bookmarkDict.TryGetValue(line, out existingBookmark) || bm.ZOrder > existingBookmark.ZOrder)
						bookmarkDict[line] = bm;
				}
				Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
				foreach (VisualLine line in textView.VisualLines) {
					int lineNumber = line.FirstDocumentLine.LineNumber;
					IBookmark bm;
					if (bookmarkDict.TryGetValue(lineNumber, out bm)) {
						double lineMiddle = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.TextMiddle) - textView.VerticalOffset;
						Rect rect = new Rect(0, PixelSnapHelpers.Round(lineMiddle - 8, pixelSize.Height), 16, 16);
						if (dragDropBookmark == bm && dragStarted)
							drawingContext.PushOpacity(0.5);
						drawingContext.DrawImage((bm.Image ?? BookmarkBase.DefaultBookmarkImage).ImageSource, rect);
						if (dragDropBookmark == bm && dragStarted)
							drawingContext.Pop();
					}
				}
				if (dragDropBookmark != null && dragStarted) {
					Rect rect = new Rect(0, PixelSnapHelpers.Round(dragDropCurrentPoint - 8, pixelSize.Height), 16, 16);
					drawingContext.DrawImage((dragDropBookmark.Image ?? BookmarkBase.DefaultBookmarkImage).ImageSource, rect);
				}
			}
		}
		
		IBookmark dragDropBookmark; // bookmark being dragged (!=null if drag'n'drop is active)
		double dragDropStartPoint;
		double dragDropCurrentPoint;
		bool dragStarted; // whether drag'n'drop operation has started (mouse was moved minimum distance)
		
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			CancelDragDrop();
			base.OnMouseDown(e);
			int line = GetLineFromMousePosition(e);
			if (!e.Handled && line > 0) {
				IBookmark bm = GetBookmarkFromLine(line);
				if (bm != null) {
					bm.MouseDown(e);
					if (!e.Handled) {
						if (e.ChangedButton == MouseButton.Left && bm.CanDragDrop && CaptureMouse()) {
							StartDragDrop(bm, e);
							e.Handled = true;
						}
					}
				}
			}
			// don't allow selecting text through the IconBarMargin
			if (e.ChangedButton == MouseButton.Left)
				e.Handled = true;
		}
		
		IBookmark GetBookmarkFromLine(int line)
		{
			IBookmark result = null;
			foreach (IBookmark bm in manager.Bookmarks) {
				if (bm.LineNumber == line) {
					if (result == null || bm.ZOrder > result.ZOrder)
						result = bm;
				}
			}
			return result;
		}
		
		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			CancelDragDrop();
			base.OnLostMouseCapture(e);
		}
		
		void StartDragDrop(IBookmark bm, MouseEventArgs e)
		{
			dragDropBookmark = bm;
			dragDropStartPoint = dragDropCurrentPoint = e.GetPosition(this).Y;
			if (TextView != null) {
				TextArea area = TextView.GetService(typeof(TextArea)) as TextArea;
				if (area != null)
					area.PreviewKeyDown += TextArea_PreviewKeyDown;
			}
		}
		
		void CancelDragDrop()
		{
			if (dragDropBookmark != null) {
				dragDropBookmark = null;
				dragStarted = false;
				if (TextView != null) {
					TextArea area = TextView.GetService(typeof(TextArea)) as TextArea;
					if (area != null)
						area.PreviewKeyDown -= TextArea_PreviewKeyDown;
				}
				ReleaseMouseCapture();
				InvalidateVisual();
			}
		}
		
		void TextArea_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			// any key press cancels drag'n'drop
			CancelDragDrop();
			if (e.Key == Key.Escape)
				e.Handled = true;
		}
		
		int GetLineFromMousePosition(MouseEventArgs e)
		{
			TextView textView = this.TextView;
			if (textView == null)
				return 0;
			VisualLine vl = textView.GetVisualLineFromVisualTop(e.GetPosition(textView).Y + textView.ScrollOffset.Y);
			if (vl == null)
				return 0;
			return vl.FirstDocumentLine.LineNumber;
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (dragDropBookmark != null) {
				dragDropCurrentPoint = e.GetPosition(this).Y;
				if (Math.Abs(dragDropCurrentPoint - dragDropStartPoint) > SystemParameters.MinimumVerticalDragDistance)
					dragStarted = true;
				InvalidateVisual();
			}
			TextViewMouseMove(TextView, e);
		}
		
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			int line = GetLineFromMousePosition(e);
			if (!e.Handled && dragDropBookmark != null) {
				if (dragStarted) {
					if (line != 0)
						dragDropBookmark.Drop(line);
					e.Handled = true;
				}
				CancelDragDrop();
			}
			if (!e.Handled && line != 0) {
				IBookmark bm = GetBookmarkFromLine(line);
				if (bm != null) {
					bm.MouseUp(e);
					if (e.Handled)
						return;
				}
				if (e.ChangedButton == MouseButton.Left && TextView != null) {
					// no bookmark on the line: create a new breakpoint
					ITextEditor textEditor = TextView.GetService(typeof(ITextEditor)) as ITextEditor;
					if (textEditor != null) {
						SD.Debugger.ToggleBreakpointAt(textEditor, line);
						return;
					}
				}
			}
		}

		#region Tooltip
		ToolTip toolTip;
		Popup popupToolTip;
		
		void MouseHover(object sender, MouseEventArgs e)
		{
			Debug.Assert(sender == this);
			
			if (!TryCloseExistingPopup(false)) {
				return;
			}
			
			int line = GetLineFromMousePosition(e);
			if (line < 1) return;
			IBookmark bm = GetBookmarkFromLine(line);
			if (bm == null) return;
			object content = bm.CreateTooltipContent();
			popupToolTip = content as Popup;
			
			if (popupToolTip != null) {
				var popupPosition = GetPopupPosition(line);
				popupToolTip.Closed += ToolTipClosed;
				popupToolTip.HorizontalOffset = popupPosition.X;
				popupToolTip.VerticalOffset = popupPosition.Y;
				popupToolTip.StaysOpen = true;  // We will close it ourselves
				
				e.Handled = true;
				popupToolTip.IsOpen = true;
				distanceToPopupLimit = double.PositiveInfinity; // reset limit; we'll re-calculate it on the next mouse movement
			} else {
				if (toolTip == null) {
					toolTip = new ToolTip();
					toolTip.Closed += ToolTipClosed;
				}
				toolTip.PlacementTarget = this; // required for property inheritance
				
				if (content is string) {
					toolTip.Content = new TextBlock
					{
						Text = content as string,
						TextWrapping = TextWrapping.Wrap
					};
				} else toolTip.Content = content;
				
				e.Handled = true;
				toolTip.IsOpen = true;
			}
		}
		
		bool TryCloseExistingPopup(bool mouseClick)
		{
			if (popupToolTip != null) {
				if (popupToolTip.IsOpen && !mouseClick && popupToolTip is ITooltip && !((ITooltip)popupToolTip).CloseWhenMouseMovesAway) {
					return false; // Popup does not want to be closed yet
				}
				popupToolTip.IsOpen = false;
				popupToolTip = null;
			}
			return true;
		}
		
		Point GetPopupPosition(int line)
		{
			Point positionInPixels = TextView.PointToScreen(TextView.GetVisualPosition(new TextViewPosition(line, 1), VisualYPosition.LineBottom) - this.TextView.ScrollOffset);
			positionInPixels.X -= 50;
			// use device independent units, because Popup Left/Top are in independent units
			return positionInPixels.TransformFromDevice(this);
		}
		
		void MouseHoverStopped(object sender, MouseEventArgs e)
		{
			// Non-popup tooltips get closed as soon as the mouse starts moving again
			if (toolTip != null) {
				toolTip.IsOpen = false;
				e.Handled = true;
			}
		}
		
		double distanceToPopupLimit;
		const double MaxMovementAwayFromPopup = 5;
		
		double GetDistanceToPopup(MouseEventArgs e)
		{
			Point p = popupToolTip.Child.PointFromScreen(PointToScreen(e.GetPosition(this)));
			Size size = popupToolTip.Child.RenderSize;
			double x = 0;
			if (p.X < 0)
				x = -p.X;
			else if (p.X > size.Width)
				x = p.X - size.Width;
			double y = 0;
			if (p.Y < 0)
				y = -p.Y;
			else if (p.Y > size.Height)
				y = p.Y - size.Height;
			return Math.Sqrt(x * x + y * y);
		}
		
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			if (popupToolTip != null && !popupToolTip.IsMouseOver && GetDistanceToPopup(e) > 10) {
				// do not close popup if mouse moved from editor to popup
				TryCloseExistingPopup(false);
			}
		}

		void TextViewMouseMove(object sender, MouseEventArgs e)
		{
			if (popupToolTip != null) {
				double distanceToPopup = GetDistanceToPopup(e);
				if (distanceToPopup > distanceToPopupLimit) {
					// Close popup if mouse moved away, exceeding the limit
					TryCloseExistingPopup(false);
				} else {
					// reduce distanceToPopupLimit
					distanceToPopupLimit = Math.Min(distanceToPopupLimit, distanceToPopup + MaxMovementAwayFromPopup);
				}
			}
		}
		
		void OnUnloaded(object sender, EventArgs e)
		{
			// Close popup when another document gets selected
			// TextEditorMouseLeave is not sufficient for this because the mouse might be over the popup when the document switch happens (e.g. Ctrl+Tab)
			TryCloseExistingPopup(true);
		}

		void ToolTipClosed(object sender, EventArgs e)
		{
			if (toolTip == sender) {
				toolTip = null;
			}
			if (popupToolTip == sender) {
				// Because popupToolTip instances are created by the tooltip provider,
				// they might be reused; so we should detach the event handler
				popupToolTip.Closed -= ToolTipClosed;
				popupToolTip = null;
			}
		}
		#endregion
	}
}
