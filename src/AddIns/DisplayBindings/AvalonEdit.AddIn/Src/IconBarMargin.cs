// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Gui;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Icon bar: contains breakpoints and other icons.
	/// </summary>
	public class IconBarMargin : AbstractMargin
	{
		readonly IconBarManager manager;
		
		public IconBarMargin(IconBarManager manager)
		{
			if (manager == null)
				throw new ArgumentNullException("manager");
			this.manager = manager;
		}
		
		#region OnTextViewChanged
		/// <inheritdoc/>
		protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
		{
			if (oldTextView != null) {
				oldTextView.VisualLinesChanged -= TextViewVisualLinesChanged;
			}
			base.OnTextViewChanged(oldTextView, newTextView);
			if (newTextView != null) {
				newTextView.VisualLinesChanged += TextViewVisualLinesChanged;
			}
			InvalidateVisual();
		}
		
		void TextViewVisualLinesChanged(object sender, EventArgs e)
		{
			InvalidateVisual();
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
					if (!bookmarkDict.ContainsKey(line))
						bookmarkDict.Add(line, bm);
				}
				foreach (VisualLine line in textView.VisualLines) {
					int lineNumber = line.FirstDocumentLine.LineNumber;
					IBookmark bm;
					if (bookmarkDict.TryGetValue(lineNumber, out bm)) {
						Rect rect = new Rect(0, line.VisualTop - textView.VerticalOffset, 16, 16);
						drawingContext.DrawImage((bm.Image ?? BookmarkBase.DefaultBookmarkImage).ImageSource, rect);
					}
				}
			}
		}
		
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			TextView textView = this.TextView;
			if (!e.Handled && textView != null) {
				VisualLine visualLine = textView.GetVisualLineFromVisualTop(e.GetPosition(textView).Y + textView.VerticalOffset);
				if (visualLine != null) {
					int line = visualLine.FirstDocumentLine.LineNumber;
					foreach (IBookmark bm in manager.Bookmarks) {
						if (bm.LineNumber == line) {
							bm.MouseDown(e);
							if (e.Handled)
								return;
						}
					}
					// no bookmark on the line: create a new breakpoint
					ITextEditor textEditor = textView.Services.GetService(typeof(ITextEditor)) as ITextEditor;
					if (textEditor != null) {
						ICSharpCode.SharpDevelop.Debugging.DebuggerService.ToggleBreakpointAt(textEditor, line);
					}
				}
			}
			// don't allow selecting text through the IconBarMargin
			e.Handled = true;
		}
	}
}
