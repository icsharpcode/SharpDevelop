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
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// A colorizes that interprets a highlighting rule set and colors the document accordingly.
	/// </summary>
	public class HighlightingColorizer : DocumentColorizingTransformer
	{
		readonly HighlightingRuleSet ruleSet;
		
		/// <summary>
		/// Creates a new HighlightingColorizer instance.
		/// </summary>
		/// <param name="ruleSet">The root highlighting rule set.</param>
		public HighlightingColorizer(HighlightingRuleSet ruleSet)
		{
			if (ruleSet == null)
				throw new ArgumentNullException("ruleSet");
			this.ruleSet = ruleSet;
		}
		
		/// <summary>
		/// This constructor is obsolete - please use the other overload instead.
		/// </summary>
		/// <param name="textView">UNUSED</param>
		/// <param name="ruleSet">The root highlighting rule set.</param>
		[Obsolete("The TextView parameter is no longer used, please use the constructor taking only HighlightingRuleSet instead")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "textView")]
		public HighlightingColorizer(TextView textView, HighlightingRuleSet ruleSet)
			: this(ruleSet)
		{
		}
		
		void textView_DocumentChanged(object sender, EventArgs e)
		{
			OnDocumentChanged((TextView)sender);
		}
		
		void OnDocumentChanged(TextView textView)
		{
			// remove existing highlighter, if any exists
			textView.Services.RemoveService(typeof(IHighlighter));
			textView.Services.RemoveService(typeof(DocumentHighlighter));
			
			TextDocument document = textView.Document;
			if (document != null) {
				IHighlighter highlighter = CreateHighlighter(textView, document);
				textView.Services.AddService(typeof(IHighlighter), highlighter);
				// for backward compatiblity, we're registering using both the interface and concrete types
				if (highlighter is DocumentHighlighter)
					textView.Services.AddService(typeof(DocumentHighlighter), highlighter);
			}
		}
		
		/// <summary>
		/// Creates the IHighlighter instance for the specified text document.
		/// </summary>
		protected virtual IHighlighter CreateHighlighter(TextView textView, TextDocument document)
		{
			return new TextViewDocumentHighlighter(textView, document, ruleSet);
		}
		
		/// <inheritdoc/>
		protected override void OnAddToTextView(TextView textView)
		{
			base.OnAddToTextView(textView);
			textView.DocumentChanged += textView_DocumentChanged;
			OnDocumentChanged(textView);
		}
		
		/// <inheritdoc/>
		protected override void OnRemoveFromTextView(TextView textView)
		{
			base.OnRemoveFromTextView(textView);
			textView.Services.RemoveService(typeof(IHighlighter));
			textView.Services.RemoveService(typeof(DocumentHighlighter));
			textView.DocumentChanged -= textView_DocumentChanged;
		}
		
		/// <inheritdoc/>
		protected override void ColorizeLine(DocumentLine line)
		{
			IHighlighter highlighter = CurrentContext.TextView.Services.GetService(typeof(IHighlighter)) as IHighlighter;
			if (highlighter != null) {
				HighlightedLine hl = highlighter.HighlightLine(line.LineNumber);
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
			if (color.FontStyle != null || color.FontWeight != null) {
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
			TextView textView;
			
			public TextViewDocumentHighlighter(TextView textView, TextDocument document, HighlightingRuleSet baseRuleSet)
				: base(document, baseRuleSet)
			{
				Debug.Assert(textView != null);
				this.textView = textView;
			}
			
			protected override void OnHighlightStateChanged(DocumentLine line, int lineNumber)
			{
				base.OnHighlightStateChanged(line, lineNumber);
				int lineEndOffset = line.Offset + line.TotalLength;
				if (lineEndOffset >= 0) {
					// Do not use colorizer.CurrentContext - the colorizer might not be the only
					// class calling DocumentHighlighter.HighlightLine, the the context might be null.
					int length = this.Document.TextLength - lineEndOffset;
					if (length != 0) {
						// don't redraw if length == 0: at the end of the document, this would cause
						// the last line which was already constructed to be redrawn ->
						// we would get an exception due to disposing the line that was already constructed
						textView.Redraw(lineEndOffset, length, DispatcherPriority.Normal);
					}
				}
			}
		}
	}
}
