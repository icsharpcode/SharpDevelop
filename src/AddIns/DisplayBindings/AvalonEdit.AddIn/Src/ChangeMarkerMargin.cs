// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Widgets.MyersDiff;

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
		
		#region Brushes
		public static readonly DependencyProperty AddedLineBrushProperty =
			DependencyProperty.Register("AddedLineBrush", typeof(Brush), typeof(ChangeMarkerMargin),
			                            new FrameworkPropertyMetadata(Brushes.LightGreen));
		
		public Brush AddedLineBrush {
			get { return (Brush)GetValue(AddedLineBrushProperty); }
			set { SetValue(AddedLineBrushProperty, value); }
		}
		
		public static readonly DependencyProperty ChangedLineBrushProperty =
			DependencyProperty.Register("ChangedLineBrush", typeof(Brush), typeof(ChangeMarkerMargin),
			                            new FrameworkPropertyMetadata(Brushes.LightBlue));
		
		public Brush ChangedLineBrush {
			get { return (Brush)GetValue(ChangedLineBrushProperty); }
			set { SetValue(ChangedLineBrushProperty, value); }
		}
		
		public static readonly DependencyProperty UnsavedLineBrushProperty =
			DependencyProperty.Register("UnsavedLineBrush", typeof(Brush), typeof(ChangeMarkerMargin),
			                            new FrameworkPropertyMetadata(Brushes.Yellow));
		
		public Brush UnsavedLineBrush {
			get { return (Brush)GetValue(UnsavedLineBrushProperty); }
			set { SetValue(UnsavedLineBrushProperty, value); }
		}
		#endregion
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			Size renderSize = this.RenderSize;
			TextView textView = this.TextView;
			
			if (textView != null && textView.VisualLinesValid) {
				var zeroLineInfo = changeWatcher.GetChange(0);
				
				foreach (VisualLine line in textView.VisualLines) {
					Rect rect = new Rect(0, line.VisualTop - textView.ScrollOffset.Y - 1, 5, line.Height + 2);
					
					LineChangeInfo info = changeWatcher.GetChange(line.FirstDocumentLine.LineNumber);
					
					if (zeroLineInfo.Change == ChangeType.Deleted && line.FirstDocumentLine.LineNumber == 1 && info.Change != ChangeType.Unsaved) {
						info.Change = ChangeType.Modified;
					}
					
					switch (info.Change) {
						case ChangeType.None:
							break;
						case ChangeType.Added:
							drawingContext.DrawRectangle(AddedLineBrush, null, rect);
							break;
						case ChangeType.Deleted:
						case ChangeType.Modified:
							drawingContext.DrawRectangle(ChangedLineBrush, null, rect);
							break;
						case ChangeType.Unsaved:
							drawingContext.DrawRectangle(UnsavedLineBrush, null, rect);
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
			
			LineChangeInfo zeroLineInfo = changeWatcher.GetChange(0);
			
			int offset, length;
			bool hasNewVersion = changeWatcher.GetNewVersionFromLine(line, out offset, out length);
			
			if (line == 1 && zeroLineInfo.Change == ChangeType.Deleted) {
				int zeroStartLine; bool zeroAdded;
				startLine = 1;
				string deletedText = changeWatcher.GetOldVersionFromLine(0, out zeroStartLine, out zeroAdded);
				var docLine = editor.Document.GetLineByNumber(line);
				string newLine = DocumentUtilitites.GetLineTerminator(changeWatcher.CurrentDocument, 1);
				deletedText += newLine;
				deletedText += editor.Document.GetText(docLine.Offset, docLine.Length);
				if (oldText != null)
					oldText = deletedText + newLine + oldText;
				else
					oldText = deletedText;
				
				if (!hasNewVersion) {
					offset = 0;
					length = docLine.Length;
					hasNewVersion = true;
				}
			}
			
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
				LineChangeInfo currLineInfo = changeWatcher.GetChange(startLine);
				
				if (currLineInfo.Change == ChangeType.Deleted && !(line == 1 && zeroLineInfo.Change == ChangeType.Deleted)) {
					var docLine = editor.Document.GetLineByNumber(startLine);
					if (docLine.DelimiterLength == 0)
						oldText = DocumentUtilitites.GetLineTerminator(changeWatcher.CurrentDocument, startLine) + oldText;
					oldText = editor.Document.GetText(docLine.Offset, docLine.TotalLength) + oldText;
				}
				
				DiffControl differ = new DiffControl();
				differ.CopyEditorSettings(editor);
				differ.editor.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
				differ.editor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
				differ.editor.Document.Text = oldText;
				
				if (oldText == string.Empty) {
					differ.editor.Visibility = Visibility.Collapsed;
					differ.copyButton.Visibility = Visibility.Collapsed;
				} else {
					var baseDocument = new TextDocument(DocumentUtilitites.GetTextSource(changeWatcher.BaseDocument));
					if (differ.editor.SyntaxHighlighting != null) {
						var mainHighlighter = new DocumentHighlighter(baseDocument, differ.editor.SyntaxHighlighting.MainRuleSet);
						var popupHighlighter = differ.editor.TextArea.GetService(typeof(IHighlighter)) as DocumentHighlighter;
						
						popupHighlighter.InitialSpanStack = mainHighlighter.GetSpanStack(currLineInfo.OldStartLineNumber);
					}
				}
				
				differ.revertButton.Click += delegate {
					if (hasNewVersion) {
						Document.Replace(offset, length, oldText);
						tooltip.IsOpen = false;
					}
				};
				
				const double borderThickness = 1;
				tooltip.Child = new Border() {
					Child = differ,
					BorderBrush = editor.TextArea.Foreground,
					BorderThickness = new Thickness(borderThickness)
				};
				
				if (tooltip.IsOpen)
					tooltip.IsOpen = false;
				
				tooltip.IsOpen = true;
				
				tooltip.Closed += delegate {
					if (marker != null) markerService.Remove(marker);
				};
				tooltip.HorizontalOffset = -borderThickness - TextView.ScrollOffset.X;
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