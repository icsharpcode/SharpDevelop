// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Description of ChangeMarkerMargin.
	/// </summary>
	public class ChangeMarkerMargin : AbstractMargin, IDisposable
	{
		IChangeWatcher changeWatcher;
		
		public ChangeMarkerMargin(IChangeWatcher changeWatcher)
		{
			this.changeWatcher = changeWatcher;
			this.hoverLogic = new MouseHoverLogic(this);
			this.hoverLogic.MouseHover += delegate(object sender, MouseEventArgs e) { DisplayTooltip(e); };
			changeWatcher.ChangeOccurred += ChangeOccurred;
		}
		
		bool disposed = false;
		
		public void Dispose()
		{
			if (!disposed) {
				changeWatcher.ChangeOccurred -= ChangeOccurred;
				disposed = true;
			}
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			Size renderSize = this.RenderSize;
			TextView textView = this.TextView;
			
			if (textView != null && textView.VisualLinesValid) {
				foreach (VisualLine line in textView.VisualLines) {
					Rect rect = new Rect(0, line.VisualTop - textView.ScrollOffset.Y, 5, line.Height);
					
					LineChangeInfo info = changeWatcher.GetChange(line.FirstDocumentLine.LineNumber);
					
					switch (info.Change) {
						case ChangeType.None:
							break;
						case ChangeType.Added:
							drawingContext.DrawRectangle(Brushes.LightGreen, null, rect);
							break;
						case ChangeType.Deleted:
						case ChangeType.Modified:
							drawingContext.DrawRectangle(Brushes.LightBlue, null, rect);
							break;
						case ChangeType.Unsaved:
							drawingContext.DrawRectangle(Brushes.Yellow, null, rect);
							break;
						default:
							throw new Exception("Invalid value for ChangeType");
					}
				}
			}
		}
		
		protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
		{
			if (oldTextView != null) {
				oldTextView.VisualLinesChanged -= VisualLinesChanged;
				oldTextView.ScrollOffsetChanged -= ScrollOffsetChanged;
				((TextArea)oldTextView.Services.GetService(typeof(TextArea))).KeyDown -= TextViewKeyDown;
			}
			base.OnTextViewChanged(oldTextView, newTextView);
			if (newTextView != null) {
				newTextView.VisualLinesChanged += VisualLinesChanged;
				newTextView.ScrollOffsetChanged += ScrollOffsetChanged;
				((TextArea)newTextView.Services.GetService(typeof(TextArea))).KeyDown += TextViewKeyDown;
			}
		}

		void TextViewKeyDown(object sender, KeyEventArgs e)
		{
			// close tooltip on pressing Esc
			if (e.Key == Key.Escape)
				tooltip.IsOpen = false;
		}
		
		void ChangeOccurred(object sender, EventArgs e)
		{
			InvalidateVisual();
		}
		
		void VisualLinesChanged(object sender, EventArgs e)
		{
			InvalidateVisual();
		}
		
		void ScrollOffsetChanged(object sender, EventArgs e)
		{
			InvalidateVisual();
		}
		
		protected override Size MeasureOverride(Size availableSize)
		{
			return new Size(5, 0);
		}
		
		#region Diffs tooltip
		
		Popup tooltip = new Popup() { StaysOpen = false };
		ITextMarker marker;
		ITextMarkerService markerService;
		MouseHoverLogic hoverLogic;

		void DisplayTooltip(MouseEventArgs e)
		{
			int line = GetLineFromMousePosition(e);
			
			if (line == 0)
				return;
			
			int startLine;
			bool added;
			string oldText = changeWatcher.GetOldVersionFromLine(line, out startLine, out added);
			
			TextEditor editor = this.TextView.Services.GetService(typeof(TextEditor)) as TextEditor;
			markerService = this.TextView.Services.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			
			int offset, length;
			bool hasNewVersion = changeWatcher.GetNewVersionFromLine(line, out offset, out length);
			
			if (hasNewVersion) {
				if (marker != null)
					markerService.Remove(marker);
				if (length <= 0) {
					marker = null;
					length = 0;
				} else {
					marker = markerService.Create(offset, length);
					marker.BackgroundColor = Colors.LightGreen;
				}
			}
			
			if (oldText != null) {
				DiffControl differ = new DiffControl();
				differ.editor.SyntaxHighlighting = editor.SyntaxHighlighting;
				differ.editor.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
				differ.editor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
				differ.editor.Document.Text = oldText;
				differ.Background = Brushes.White;
				
				// TODO : deletions on line 0 cannot be displayed.
				
				LineChangeInfo prevLineInfo = changeWatcher.GetChange(startLine - 1);
				LineChangeInfo lineInfo = changeWatcher.GetChange(startLine);
				
				if (prevLineInfo.Change == ChangeType.Deleted) {
					var docLine = editor.Document.GetLineByNumber(startLine - 1);
					differ.editor.Document.Insert(0, editor.Document.GetText(docLine.Offset, docLine.TotalLength));
				}
				
				if (oldText == string.Empty) {
					differ.editor.Visibility = Visibility.Collapsed;
					differ.copyButton.Visibility = Visibility.Collapsed;
				} else {
					var baseDocument = new TextDocument(changeWatcher.BaseDocument.Text);
					if (differ.editor.SyntaxHighlighting != null) {
						var mainHighlighter = new DocumentHighlighter(baseDocument, differ.editor.SyntaxHighlighting.MainRuleSet);
						var popupHighlighter = differ.editor.TextArea.GetService(typeof(IHighlighter)) as DocumentHighlighter;
						
						if (prevLineInfo.Change == ChangeType.Deleted)
							popupHighlighter.InitialSpanStack = mainHighlighter.GetSpanStack(prevLineInfo.OldStartLineNumber);
						else
							popupHighlighter.InitialSpanStack = mainHighlighter.GetSpanStack(lineInfo.OldStartLineNumber);
					}
				}
				
				differ.revertButton.Click += delegate {
					if (hasNewVersion) {
						int delimiter = 0;
						DocumentLine l = Document.GetLineByOffset(offset + length);
						if (added)
							delimiter = l.DelimiterLength;
						if (length == 0)
							oldText += DocumentUtilitites.GetLineTerminator(new AvalonEditDocumentAdapter(Document, null), l.LineNumber);
						Document.Replace(offset, length + delimiter, oldText);
						tooltip.IsOpen = false;
					}
				};
				
				tooltip.Child = new Border() {
					Child = differ,
					BorderBrush = Brushes.Black,
					BorderThickness = new Thickness(1)
				};
				
				if (tooltip.IsOpen)
					tooltip.IsOpen = false;
				
				tooltip.IsOpen = true;
				
				tooltip.Closed += delegate {
					if (marker != null) markerService.Remove(marker);
				};
				tooltip.HorizontalOffset = -10;
				tooltip.VerticalOffset =
					TextView.GetVisualTopByDocumentLine(startLine) - TextView.ScrollOffset.Y;
				tooltip.Placement = PlacementMode.Top;
				tooltip.PlacementTarget = this.TextView;
			}
		}
		
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			if (marker != null && !tooltip.IsOpen)
				markerService.Remove(marker);
			base.OnMouseLeave(e);
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
		
		#endregion
	}
}