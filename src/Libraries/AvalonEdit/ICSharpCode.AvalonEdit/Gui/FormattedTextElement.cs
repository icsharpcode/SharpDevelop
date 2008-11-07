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
	/// Formatted text (not normal document text)
	/// </summary>
	class FormattedTextElement : VisualLineElement
	{
		internal readonly FormattedText text;
		
		public FormattedTextElement(FormattedText text, int documentLength) : base(1, documentLength)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			this.text = text;
			this.BreakBefore = LineBreakCondition.BreakPossible;
			this.BreakAfter = LineBreakCondition.BreakPossible;
		}
		
		public LineBreakCondition BreakBefore { get; set; }
		public LineBreakCondition BreakAfter { get; set; }
		
		public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
		{
			return new FormattedTextRun(this, this.TextRunProperties);
		}
	}
	
	class FormattedTextRun : TextEmbeddedObject
	{
		protected readonly FormattedTextElement element;
		TextRunProperties properties;
		
		public FormattedTextRun(FormattedTextElement element, TextRunProperties properties)
		{
			if (properties == null)
				throw new ArgumentNullException("properties");
			this.properties = properties;
			this.element = element;
		}
		
		public override LineBreakCondition BreakBefore {
			get { return element.BreakBefore; }
		}
		
		public override LineBreakCondition BreakAfter {
			get { return element.BreakAfter; }
		}
		
		public override bool HasFixedSize {
			get { return true; }
		}
		
		public override CharacterBufferReference CharacterBufferReference {
			get { return new CharacterBufferReference(); }
		}
		
		public override int Length {
			get { return element.VisualLength; }
		}
		
		public override TextRunProperties Properties {
			get { return properties; }
		}
		
		public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
		{
			return new TextEmbeddedObjectMetrics(element.text.WidthIncludingTrailingWhitespace,
			                                     element.text.Height,
			                                     element.text.Baseline);
		}
		
		public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
		{
			return new Rect(0, 0, element.text.WidthIncludingTrailingWhitespace, element.text.Height);
		}
		
		public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
		{
			origin.Y -= element.text.Baseline;
			drawingContext.DrawText(element.text, origin);
		}
	}
}
