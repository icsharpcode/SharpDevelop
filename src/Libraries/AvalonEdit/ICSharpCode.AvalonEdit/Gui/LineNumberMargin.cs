// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// Margin showing line numbers.
	/// </summary>
	public class LineNumberMargin : AbstractMargin, IWeakEventListener
	{
		static LineNumberMargin()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LineNumberMargin),
			                                         new FrameworkPropertyMetadata(typeof(LineNumberMargin)));
		}
		
		/// <summary>
		/// TextArea property.
		/// </summary>
		public static readonly DependencyProperty TextAreaProperty =
			DependencyProperty.Register("TextArea", typeof(TextArea), typeof(LineNumberMargin));
		
		/// <summary>
		/// Gets/sets the text area in which text should be selected.
		/// </summary>
		public TextArea TextArea {
			get { return (TextArea)GetValue(TextAreaProperty); }
			set { SetValue(TextAreaProperty, value); }
		}
		
		Typeface typeface;
		double emSize;
		
		/// <inheritdoc/>
		protected override Size MeasureOverride(Size availableSize)
		{
			typeface = this.CreateTypeface();
			emSize = (double)GetValue(TextBlock.FontSizeProperty);
			
			FormattedText text = new FormattedText(
				new string('9', maxLineNumberLength),
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				typeface,
				emSize,
				(Brush)GetValue(Control.ForegroundProperty)
			);
			return new Size(text.Width, 0);
		}
		
		/// <inheritdoc/>
		protected override void OnRender(DrawingContext drawingContext)
		{
			TextView textView = this.TextView;
			Size renderSize = this.RenderSize;
			if (textView != null && textView.VisualLinesValid) {
				var foreground = (Brush)GetValue(Control.ForegroundProperty);
				foreach (VisualLine line in textView.VisualLines) {
					int lineNumber = line.FirstDocumentLine.LineNumber;
					FormattedText text = new FormattedText(
						lineNumber.ToString(CultureInfo.CurrentCulture),
						CultureInfo.CurrentCulture,
						FlowDirection.LeftToRight,
						typeface, emSize, foreground
					);
					drawingContext.DrawText(text, new Point(renderSize.Width - text.Width,
					                                        line.VisualTop - textView.VerticalOffset));
				}
			}
		}
		
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
		
		/// <inheritdoc/>
		protected override void OnDocumentChanged(TextDocument oldDocument, TextDocument newDocument)
		{
			if (oldDocument != null) {
				TextDocumentWeakEventManager.LineCountChanged.RemoveListener(oldDocument, this);
			}
			base.OnDocumentChanged(oldDocument, newDocument);
			if (newDocument != null) {
				TextDocumentWeakEventManager.LineCountChanged.AddListener(newDocument, this);
			}
			OnDocumentLineCountChanged();
		}
		
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(TextDocumentWeakEventManager.LineCountChanged)) {
				OnDocumentLineCountChanged();
				return true;
			}
			return false;
		}
		
		int maxLineNumberLength = 1;
		
		void OnDocumentLineCountChanged()
		{
			int documentLineCount = Document != null ? Document.LineCount : 1;
			int newLength = documentLineCount.ToString(CultureInfo.CurrentCulture).Length;
			if (newLength != maxLineNumberLength) {
				maxLineNumberLength = newLength;
				InvalidateMeasure();
			}
		}
		
		void TextViewVisualLinesChanged(object sender, EventArgs e)
		{
			InvalidateVisual();
		}
		
		AnchorSegment selectionStart;
		bool selecting;
		
		/// <inheritdoc/>
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			if (!e.Handled && TextView != null && TextArea != null) {
				e.Handled = true;
				TextArea.Focus();
				
				SimpleSegment currentSeg = GetTextLineSegment(e);
				if (currentSeg == SimpleSegment.Invalid)
					return;
				TextArea.Caret.Offset = currentSeg.Offset + currentSeg.Length;
				if (CaptureMouse()) {
					selecting = true;
					selectionStart = new AnchorSegment(Document, currentSeg.Offset, currentSeg.Length);
					if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) {
						SimpleSelection simpleSelection = TextArea.Selection as SimpleSelection;
						if (simpleSelection != null)
							selectionStart = new AnchorSegment(Document, simpleSelection);
					}
					TextArea.Selection = new SimpleSelection(selectionStart);
					if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) {
						ExtendSelection(currentSeg);
					}
				}
			}
		}
		
		SimpleSegment GetTextLineSegment(MouseEventArgs e)
		{
			Point pos = e.GetPosition(TextView);
			pos.X = 0;
			pos.Y += TextView.VerticalOffset;
			VisualLine vl = TextView.GetVisualLineFromVisualTop(pos.Y);
			if (vl == null)
				return SimpleSegment.Invalid;
			TextLine tl = vl.GetTextLineByVisualYPosition(pos.Y);
			int visualStartColumn = vl.GetTextLineVisualStartColumn(tl);
			int visualEndColumn = visualStartColumn + tl.Length;
			int relStart = vl.FirstDocumentLine.Offset;
			int startOffset = vl.GetRelativeOffset(visualStartColumn) + relStart;
			int endOffset = vl.GetRelativeOffset(visualEndColumn) + relStart;
			if (endOffset == vl.LastDocumentLine.Offset + vl.LastDocumentLine.Length)
				endOffset += vl.LastDocumentLine.DelimiterLength;
			return new SimpleSegment(startOffset, endOffset - startOffset);
		}
		
		void ExtendSelection(SimpleSegment currentSeg)
		{
			if (currentSeg.Offset < selectionStart.Offset) {
				TextArea.Caret.Offset = currentSeg.Offset;
				TextArea.Selection = new SimpleSelection(currentSeg.Offset, selectionStart.Offset + selectionStart.Length);
			} else {
				TextArea.Caret.Offset = currentSeg.Offset + currentSeg.Length;
				TextArea.Selection = new SimpleSelection(selectionStart.Offset, currentSeg.Offset + currentSeg.Length);
			}
		}
		
		/// <inheritdoc/>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (selecting && TextArea != null && TextView != null) {
				e.Handled = true;
				SimpleSegment currentSeg = GetTextLineSegment(e);
				if (currentSeg == SimpleSegment.Invalid)
					return;
				ExtendSelection(currentSeg);
			}
			base.OnMouseMove(e);
		}
		
		/// <inheritdoc/>
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (selecting) {
				selecting = false;
				selectionStart = null;
				ReleaseMouseCapture();
				e.Handled = true;
			}
			base.OnMouseLeftButtonUp(e);
		}
		
		/// <inheritdoc/>
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			// accept clicks even when clicking on the background
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}
	}
}
