// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// Formatted text (not normal document text).
	/// This is used as base class for various VisualLineElements that are displayed using a
	/// FormattedText, for example newline markers or collapsed folding sections.
	/// </summary>
	public class FormattedTextElement : VisualLineElement
	{
		readonly FormattedText text;
		
		/// <summary>
		/// Gets the formatted text.
		/// </summary>
		public FormattedText Text {
			get { return text; }
		}
		
		/// <summary>
		/// Creates a new FormattedTextElement that displays the specified text
		/// and occupies the specified length in the document.
		/// </summary>
		public FormattedTextElement(FormattedText text, int documentLength) : base(1, documentLength)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			this.text = text;
			this.BreakBefore = LineBreakCondition.BreakPossible;
			this.BreakAfter = LineBreakCondition.BreakPossible;
		}
		
		/// <summary>
		/// Gets/sets the line break condition before the element.
		/// The default is 'BreakPossible'.
		/// </summary>
		public LineBreakCondition BreakBefore { get; set; }
		
		/// <summary>
		/// Gets/sets the line break condition after the element.
		/// The default is 'BreakPossible'.
		/// </summary>
		public LineBreakCondition BreakAfter { get; set; }
		
		/// <inheritdoc/>
		public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
		{
			return new FormattedTextRun(this, this.TextRunProperties);
		}
	}
	
	/// <summary>
	/// This is the TextRun implementation used by the <see cref="FormattedTextElement"/> class.
	/// </summary>
	public class FormattedTextRun : TextEmbeddedObject
	{
		readonly FormattedTextElement element;
		TextRunProperties properties;
		
		/// <summary>
		/// Creates a new FormattedTextRun.
		/// </summary>
		public FormattedTextRun(FormattedTextElement element, TextRunProperties properties)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			if (properties == null)
				throw new ArgumentNullException("properties");
			this.properties = properties;
			this.element = element;
		}
		
		/// <summary>
		/// Gets the element for which the FormattedTextRun was created.
		/// </summary>
		public FormattedTextElement Element {
			get { return element; }
		}
		
		/// <inheritdoc/>
		public override LineBreakCondition BreakBefore {
			get { return element.BreakBefore; }
		}
		
		/// <inheritdoc/>
		public override LineBreakCondition BreakAfter {
			get { return element.BreakAfter; }
		}
		
		/// <inheritdoc/>
		public override bool HasFixedSize {
			get { return true; }
		}
		
		/// <inheritdoc/>
		public override CharacterBufferReference CharacterBufferReference {
			get { return new CharacterBufferReference(); }
		}
		
		/// <inheritdoc/>
		public override int Length {
			get { return element.VisualLength; }
		}
		
		/// <inheritdoc/>
		public override TextRunProperties Properties {
			get { return properties; }
		}
		
		/// <inheritdoc/>
		public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
		{
			return new TextEmbeddedObjectMetrics(element.Text.WidthIncludingTrailingWhitespace,
			                                     element.Text.Height,
			                                     element.Text.Baseline);
		}
		
		/// <inheritdoc/>
		public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
		{
			return new Rect(0, 0, element.Text.WidthIncludingTrailingWhitespace, element.Text.Height);
		}
		
		/// <inheritdoc/>
		public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
		{
			origin.Y -= element.Text.Baseline;
			drawingContext.DrawText(element.Text, origin);
		}
	}
}
