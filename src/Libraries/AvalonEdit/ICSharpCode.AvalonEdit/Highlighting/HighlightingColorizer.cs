// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Gui;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// A colorizes that interprets a highlighting rule set and colors the document accordingly.
	/// </summary>
	public class HighlightingColorizer : DocumentColorizingTransformer, IWeakEventListener
	{
		readonly TextView textView;
		readonly HighlightingRuleSet ruleSet;
		DocumentHighlighter highlighter;
		bool isInTextView;
		
		/// <summary>
		/// Creates a new HighlightingColorizer instance.
		/// </summary>
		/// <param name="textView">The text view for which the highlighting should be provided.</param>
		/// <param name="ruleSet">The root highlighting rule set.</param>
		public HighlightingColorizer(TextView textView, HighlightingRuleSet ruleSet)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			if (ruleSet == null)
				throw new ArgumentNullException("ruleSet");
			this.textView = textView;
			this.ruleSet = ruleSet;
			TextViewWeakEventManager.DocumentChanged.AddListener(textView, this);
			OnDocumentChanged();
		}
		
		/// <inheritdoc/>
		protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(TextViewWeakEventManager.DocumentChanged)) {
				OnDocumentChanged();
				return true;
			}
			return false;
		}
		
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			return ReceiveWeakEvent(managerType, sender, e);
		}
		
		void OnDocumentChanged()
		{
			if (highlighter != null && isInTextView) {
				textView.Services.RemoveService(typeof(DocumentHighlighter));
			}
			
			TextDocument document = textView.Document;
			if (document != null)
				highlighter = new TextViewDocumentHighlighter(this, document, ruleSet);
			else
				highlighter = null;
			
			if (highlighter != null && isInTextView) {
				textView.Services.AddService(typeof(DocumentHighlighter), highlighter);
			}
		}
		
		/// <inheritdoc/>
		protected override void OnAddToTextView(TextView textView)
		{
			base.OnAddToTextView(textView);
			isInTextView = true;
			if (highlighter != null) {
				textView.Services.AddService(typeof(DocumentHighlighter), highlighter);
			}
		}
		
		/// <inheritdoc/>
		protected override void OnRemoveFromTextView(TextView textView)
		{
			base.OnRemoveFromTextView(textView);
			isInTextView = false;
			if (highlighter != null) {
				textView.Services.RemoveService(typeof(DocumentHighlighter));
			}
		}
		
		int currentLineEndOffset;
		
		/// <inheritdoc/>
		protected override void ColorizeLine(DocumentLine line)
		{
			if (CurrentContext.TextView != textView)
				throw new InvalidOperationException("Wrong TextView");
			if (highlighter != null) {
				currentLineEndOffset = line.Offset + line.TotalLength;
				HighlightedLine hl = highlighter.HighlightLine(line);
				foreach (HighlightedSection section in hl.Sections) {
					ChangeLinePart(section.Offset, section.Offset + section.Length,
					               visualLineElement => ApplyColorToElement(visualLineElement, section.Color));
				}
			}
		}
		
		/// <summary>
		/// Applies a highlighting color to a visual line element.
		/// </summary>
		protected virtual void ApplyColorToElement(VisualLineElement element, HighlightingColor color)
		{
			if (color.Foreground != null) {
				Brush b = color.Foreground.GetBrush(CurrentContext);
				if (b != null)
					element.TextRunProperties.SetForegroundBrush(b);
			}
			if (color.FontWeight != null) {
				Typeface tf = element.TextRunProperties.Typeface;
				element.TextRunProperties.SetTypeface(new Typeface(
					tf.FontFamily,
					color.FontStyle ?? tf.Style,
					color.FontWeight ?? tf.Weight,
					tf.Stretch
				));
			}
		}
		
		sealed class TextViewDocumentHighlighter : DocumentHighlighter
		{
			HighlightingColorizer colorizer;
			
			public TextViewDocumentHighlighter(HighlightingColorizer colorizer, TextDocument document, HighlightingRuleSet baseRuleSet)
				: base(document, baseRuleSet)
			{
				Debug.Assert(colorizer != null);
				this.colorizer = colorizer;
			}
			
			protected override void OnHighlightStateChanged(DocumentLine line, int lineNumber)
			{
				base.OnHighlightStateChanged(line, lineNumber);
				if (colorizer.currentLineEndOffset >= 0) {
					// Do not use colorizer.CurrentContext - the colorizer might not be the only
					// class calling DocumentHighlighter.HighlightLine, the the context might be null.
					int length = this.Document.TextLength - colorizer.currentLineEndOffset;
					if (length != 0) {
						// don't redraw if length == 0: at the end of the document, this would cause
						// the last line which was already constructed to be redrawn ->
						// we would get an exception due to disposing the line that was already constructed
						colorizer.textView.Redraw(colorizer.currentLineEndOffset, length, DispatcherPriority.Normal);
					}
					colorizer.currentLineEndOffset = -1;
				}
			}
		}
	}
}
