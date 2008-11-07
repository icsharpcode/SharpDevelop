// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// Element generator that displays · for spaces and » for tabs.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace")]
	public class WhitespaceElementGenerator : VisualLineElementGenerator
	{
		readonly static char[] tabSpace = { ' ', '\t' };
		
		/// <summary>
		/// Gets/Sets whether to show · for spaces.
		/// </summary>
		public bool ShowSpaces { get; set; }
		
		/// <summary>
		/// Gets/Sets whether to show » for tabs.
		/// </summary>
		public bool ShowTabs { get; set; }
		
		/// <summary>
		/// Creates a new WhitespaceElementGenerator instance.
		/// </summary>
		public WhitespaceElementGenerator()
		{
			this.ShowSpaces = true;
			this.ShowTabs = true;
		}
		
		/// <inheritdoc/>
		public override int GetFirstInterestedOffset(int startOffset)
		{
			DocumentLine endLine = CurrentContext.VisualLine.LastDocumentLine;
			int endOffset = endLine.Offset + endLine.Length;
			string relevantText = CurrentContext.Document.GetText(startOffset, endOffset - startOffset);
			
			int pos;
			if (ShowTabs && ShowSpaces)
				pos = relevantText.IndexOfAny(tabSpace);
			else if (ShowTabs)
				pos = relevantText.IndexOf('\t');
			else if (ShowSpaces)
				pos = relevantText.IndexOf(' ');
			else
				pos = -1;
			
			if (pos >= 0)
				return startOffset + pos;
			else
				return -1;
		}
		
		/// <inheritdoc/>
		public override VisualLineElement ConstructElement(int offset)
		{
			char c = CurrentContext.Document.GetCharAt(offset);
			if (c == ' ') {
				FormattedText text = new FormattedText(
					"\u00B7",
					CurrentContext.GlobalTextRunProperties.CultureInfo,
					FlowDirection.LeftToRight,
					CurrentContext.GlobalTextRunProperties.Typeface,
					CurrentContext.GlobalTextRunProperties.FontRenderingEmSize,
					Brushes.LightGray
				);
				return new SpaceTextElement(text);
			} else if (c == '\t') {
				FormattedText text = new FormattedText(
					"\u00BB",
					CurrentContext.GlobalTextRunProperties.CultureInfo,
					FlowDirection.LeftToRight,
					CurrentContext.GlobalTextRunProperties.Typeface,
					CurrentContext.GlobalTextRunProperties.FontRenderingEmSize,
					Brushes.LightGray
				);
				return new TabTextElement(text);
			} else {
				return null;
			}
		}
		
		class SpaceTextElement : FormattedTextElement
		{
			public SpaceTextElement(FormattedText text) : base(text, 1)
			{
				BreakBefore = LineBreakCondition.BreakPossible;
				BreakAfter = LineBreakCondition.BreakDesired;
			}
			
			public override int GetNextCaretPosition(int visualColumn, bool backwards, CaretPositioningMode mode)
			{
				if (mode == CaretPositioningMode.Normal)
					return base.GetNextCaretPosition(visualColumn, backwards, mode);
				else
					return -1;
			}
		}
		
		class TabTextElement : VisualLineElement
		{
			internal readonly FormattedText text;
			
			public TabTextElement(FormattedText text) : base(2, 1)
			{
				this.text = text;
			}
			
			public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
			{
				if (startVisualColumn == this.VisualColumn)
					return new TabGlyphRun(this, this.TextRunProperties);
				else if (startVisualColumn == this.VisualColumn + 1)
					return new TextCharacters("\t", 0, 1, this.TextRunProperties);
				else
					throw new ArgumentOutOfRangeException("startVisualColumn");
			}
			
			public override int GetNextCaretPosition(int visualColumn, bool backwards, CaretPositioningMode mode)
			{
				if (mode == CaretPositioningMode.Normal)
					return base.GetNextCaretPosition(visualColumn, backwards, mode);
				else
					return -1;
			}
		}
		
		class TabGlyphRun : TextEmbeddedObject
		{
			protected readonly TabTextElement element;
			TextRunProperties properties;
			
			public TabGlyphRun(TabTextElement element, TextRunProperties properties)
			{
				if (properties == null)
					throw new ArgumentNullException("properties");
				this.properties = properties;
				this.element = element;
			}
			
			public override LineBreakCondition BreakBefore {
				get { return LineBreakCondition.BreakPossible; }
			}
			
			public override LineBreakCondition BreakAfter {
				get { return LineBreakCondition.BreakRestrained; }
			}
			
			public override bool HasFixedSize {
				get { return true; }
			}
			
			public override CharacterBufferReference CharacterBufferReference {
				get { return new CharacterBufferReference(); }
			}
			
			public override int Length {
				get { return 1; }
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
}
