// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// A colorizes that interprets a highlighting rule set and colors the document accordingly.
	/// </summary>
	public class HighlightingColorizer : DocumentColorizingTransformer
	{
		readonly IHighlightingDefinition definition;
		TextView textView;
		IHighlighter highlighter;
		
		/// <summary>
		/// Creates a new HighlightingColorizer instance.
		/// </summary>
		/// <param name="ruleSet">The root highlighting rule set.</param>
		public HighlightingColorizer(IHighlightingDefinition definition)
		{
			if (definition == null)
				throw new ArgumentNullException("definition");
			this.definition = definition;
			this.highlighter = highlighter;
		}
		
		/// <summary>
		/// Creates a new HighlightingColorizer instance.
		/// </summary>
		/// <param name="highlighter">The highlighter to be used.</param>
		public HighlightingColorizer(IHighlighter highlighter)
		{
			if (highlighter == null)
				throw new ArgumentNullException("highlighter");
			this.highlighter = highlighter;
		}
		
		void textView_DocumentChanged(object sender, EventArgs e)
		{
			TextView textView = (TextView)sender;
			DeregisterServices(textView);
			RegisterServices(textView);
		}
		
		/// <summary>
		/// This method is called when a text view is removed from this HighlightingColorizer,
		/// and also when the TextDocument on any associated text view changes.
		/// </summary>
		protected virtual void DeregisterServices(TextView textView)
		{
			if (highlighter != null) {
				highlighter.HighlightingStateChanged -= OnHighlightStateChanged;
				// remove highlighter if it is registered
				if (textView.Services.GetService(typeof(IHighlighter)) == highlighter)
					textView.Services.RemoveService(typeof(IHighlighter));
				
			}
		}
		
		/// <summary>
		/// This method is called when a new text view is added to this HighlightingColorizer,
		/// and also when the TextDocument on any associated text view changes.
		/// </summary>
		protected virtual void RegisterServices(TextView textView)
		{
			if (textView.Document != null) {
				highlighter = textView.Document != null ? CreateHighlighter(textView, textView.Document) : null;
				if (highlighter != null) {
					// add service only if it doesn't already exist
					if (textView.Services.GetService(typeof(IHighlighter)) == null) {
						textView.Services.AddService(typeof(IHighlighter), highlighter);
					}
					highlighter.HighlightingStateChanged += OnHighlightStateChanged;
				}
			}
		}
		
		/// <summary>
		/// Creates the IHighlighter instance for the specified text document.
		/// </summary>
		protected virtual IHighlighter CreateHighlighter(TextView textView, TextDocument document)
		{
			return highlighter ?? new DocumentHighlighter(document, definition);
		}
		
		/// <inheritdoc/>
		protected override void OnAddToTextView(TextView textView)
		{
			if (this.textView != null) {
				throw new InvalidOperationException("Cannot use a HighlightingColorizer instance in multiple text views. Please create a separate instance for each text view.");
			}
			base.OnAddToTextView(textView);
			this.textView = textView;
			textView.DocumentChanged += textView_DocumentChanged;
			textView.VisualLineConstructionStarting += textView_VisualLineConstructionStarting;
			RegisterServices(textView);
		}
		
		/// <inheritdoc/>
		protected override void OnRemoveFromTextView(TextView textView)
		{
			DeregisterServices(textView);
			textView.DocumentChanged -= textView_DocumentChanged;
			textView.VisualLineConstructionStarting -= textView_VisualLineConstructionStarting;
			base.OnRemoveFromTextView(textView);
			this.textView = null;
		}
		
		void textView_VisualLineConstructionStarting(object sender, VisualLineConstructionStartEventArgs e)
		{
			if (highlighter != null) {
				// Force update of highlighting state up to the position where we start generating visual lines.
				// This is necessary in case the document gets modified above the FirstLineInView so that the highlighting state changes.
				// We need to detect this case and issue a redraw (through TextViewDocumentHighligher.OnHighlightStateChanged)
				// before the visual line construction reuses existing lines that were built using the invalid highlighting state.
				lineNumberBeingColorized = e.FirstLineInView.LineNumber - 1;
				highlighter.UpdateHighlightingState(lineNumberBeingColorized);
				lineNumberBeingColorized = 0;
			}
		}
		
		DocumentLine lastColorizedLine;
		
		/// <inheritdoc/>
		protected override void Colorize(ITextRunConstructionContext context)
		{
			this.lastColorizedLine = null;
			base.Colorize(context);
			if (this.lastColorizedLine != context.VisualLine.LastDocumentLine) {
				if (highlighter != null) {
					// In some cases, it is possible that we didn't highlight the last document line within the visual line
					// (e.g. when the line ends with a fold marker).
					// But even if we didn't highlight it, we'll have to update the highlighting state for it so that the
					// proof inside TextViewDocumentHighlighter.OnHighlightStateChanged holds.
					lineNumberBeingColorized = context.VisualLine.LastDocumentLine.LineNumber;
					highlighter.UpdateHighlightingState(lineNumberBeingColorized);
					lineNumberBeingColorized = 0;
				}
			}
			this.lastColorizedLine = null;
		}
		
		int lineNumberBeingColorized;
		
		/// <inheritdoc/>
		protected override void ColorizeLine(DocumentLine line)
		{
			if (highlighter != null) {
				lineNumberBeingColorized = line.LineNumber;
				HighlightedLine hl = highlighter.HighlightLine(lineNumberBeingColorized);
				lineNumberBeingColorized = 0;
				foreach (HighlightedSection section in hl.Sections) {
					if (IsEmptyColor(section.Color))
						continue;
					ChangeLinePart(section.Offset, section.Offset + section.Length,
					               visualLineElement => ApplyColorToElement(visualLineElement, section.Color));
				}
			}
			this.lastColorizedLine = line;
		}
		
		/// <summary>
		/// Gets whether the color is empty (has no effect on a VisualLineTextElement).
		/// For example, the C# "Punctuation" is an empty color.
		/// </summary>
		bool IsEmptyColor(HighlightingColor color)
		{
			if (color == null)
				return true;
			return color.Background == null && color.Foreground == null
				&& color.FontStyle == null && color.FontWeight == null;
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
			if (color.Background != null) {
				Brush b = color.Background.GetBrush(CurrentContext);
				if (b != null)
					element.BackgroundBrush = b;
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
		
		/// <summary>
		/// This method is responsible for telling the TextView to redraw lines when the highlighting state has changed.
		/// </summary>
		/// <remarks>
		/// Creation of a VisualLine triggers the syntax highlighter (which works on-demand), so it says:
		/// Hey, the user typed "/*". Don't just recreate that line, but also the next one
		/// because my highlighting state (at end of line) changed!
		/// </remarks>
		void OnHighlightStateChanged(IHighlighter sender, int fromlineNumber, int toLineNumber)
		{
			if (lineNumberBeingColorized < fromlineNumber || lineNumberBeingColorized > toLineNumber) {
				// Ignore notifications for any line except the one we're interested in.
				// This improves the performance as Redraw() can take quite some time when called repeatedly
				// while scanning the document (above the visible area) for highlighting changes.
				return;
			}
			
			// The user may have inserted "/*" into the current line, and so far only that line got redrawn.
			// So when the highlighting state is changed, we issue a redraw for the line immediately below.
			// If the highlighting state change applies to the lines below, too, the construction of each line
			// will invalidate the next line, and the construction pass will regenerate all lines.
			
			Debug.WriteLine("OnHighlightStateChanged forces redraw of lines {0} to {1}", fromlineNumber + 1, toLineNumber + 1);
			
			// If the VisualLine construction is in progress, we have to avoid sending redraw commands for
			// anything above the line currently being constructed.
			// It takes some explanation to see why this cannot happen.
			// VisualLines always get constructed from top to bottom.
			// Each VisualLine construction calls into the highlighter and thus forces an update of the
			// highlighting state for all lines up to the one being constructed.
			
			// To guarantee that we don't redraw lines we just constructed, we need to show that when
			// a VisualLine is being reused, the highlighting state at that location is still up-to-date.
			
			// This isn't exactly trivial and the initial implementation was incorrect in the presence of external document changes
			// (e.g. split view).
			
			// For the first line in the view, the TextView.VisualLineConstructionStarting event is used to check that the
			// highlighting state is up-to-date. If it isn't, this method will be executed, and it'll mark the first line
			// in the view as requiring a redraw. This is safely possible because that event occurs before any lines are reused.
			
			// Once we take care of the first visual line, we won't get in trouble with other lines due to the top-to-bottom
			// construction process.
			
			// We'll prove that: if line N is being reused, then the highlighting state is up-to-date until (end of) line N-1.
			
			// Start of induction: the first line in view is reused only if the highlighting state was up-to-date
			// until line N-1 (no change detected in VisualLineConstructionStarting event).
			
			// Induction step:
			// If another line N+1 is being reused, then either
			//     a) the previous line (the visual line containing document line N) was newly constructed
			// or  b) the previous line was reused
			// In case a, the construction updated the highlighting state. This means the stack at end of line N is up-to-date.
			// In case b, the highlighting state at N-1 was up-to-date, and the text of line N was not changed.
			//   (if the text was changed, the line could not have been reused).
			// From this follows that the highlighting state at N is still up-to-date.
			
			// The above proof holds even in the presence of folding: folding only ever hides text in the middle of a visual line.
			// Our Colorize-override ensures that the highlighting state is always updated for the LastDocumentLine,
			// so it will always invalidate the next visual line when a folded line is constructed
			// and the highlighting stack has changed.
			
			for (int lineNumber = fromlineNumber; lineNumber <= toLineNumber; lineNumber++) {
				if (lineNumber + 1 <= textView.Document.LineCount)
					textView.Redraw(textView.Document.GetLineByNumber(lineNumber + 1), DispatcherPriority.Normal);
			}
			
			/*
			 * Meta-comment: "why does this have to be so complicated?"
			 * 
			 * The problem is that I want to re-highlight only on-demand and incrementally;
			 * and at the same time only repaint changed lines.
			 * So the highlighter and the VisualLine construction both have to run in a single pass.
			 * The highlighter must take care that it never touches already constructed visual lines;
			 * if it detects that something must be redrawn because the highlighting state changed,
			 * it must do so early enough in the construction process.
			 * But doing it too early means it doesn't have the information necessary to re-highlight and redraw only the desired parts.
			 */
		}
	}
}
