// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// Element generator that displays · for spaces and » for tabs and a box for control characeters.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace")]
	public class SingleCharacterElementGenerator : VisualLineElementGenerator, IWeakEventListener
	{
		/// <summary>
		/// Gets/Sets whether to show · for spaces.
		/// </summary>
		public bool ShowSpaces { get; set; }
		
		/// <summary>
		/// Gets/Sets whether to show » for tabs.
		/// </summary>
		public bool ShowTabs { get; set; }
		
		/// <summary>
		/// Gets/Sets whether to show a box with the hex code for control characters.
		/// </summary>
		public bool ShowBoxForControlCharacters { get; set; }
		
		/// <summary>
		/// Creates a new WhitespaceElementGenerator instance.
		/// </summary>
		public SingleCharacterElementGenerator()
		{
			this.ShowSpaces = true;
			this.ShowTabs = true;
			this.ShowBoxForControlCharacters = true;
		}
		
		/// <summary>
		/// Fetch options from the text editor and synchronize with future option changes.
		/// </summary>
		public void SynchronizeOptions(ITextEditorComponent textEditor)
		{
			if (textEditor == null)
				throw new ArgumentNullException("textEditor");
			FetchOptions(textEditor.Options);
			TextEditorWeakEventManager.OptionChanged.AddListener(textEditor, this);
		}
		
		/// <inheritdoc/>
		protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(TextEditorWeakEventManager.OptionChanged)) {
				ITextEditorComponent component = (ITextEditorComponent)sender;
				FetchOptions(component.Options);
				return true;
			}
			return false;
		}
		
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			return ReceiveWeakEvent(managerType, sender, e);
		}
		
		void FetchOptions(TextEditorOptions options)
		{
			this.ShowSpaces = options.ShowSpaces;
			this.ShowTabs = options.ShowTabs;
			this.ShowBoxForControlCharacters = options.ShowBoxForControlCharacters;
		}
		
		/// <inheritdoc/>
		public override int GetFirstInterestedOffset(int startOffset)
		{
			DocumentLine endLine = CurrentContext.VisualLine.LastDocumentLine;
			string relevantText = CurrentContext.Document.GetText(startOffset, endLine.EndOffset - startOffset);
			
			for (int i = 0; i < relevantText.Length; i++) {
				char c = relevantText[i];
				switch (c) {
					case ' ':
						if (ShowSpaces)
							return startOffset + i;
						break;
					case '\t':
						if (ShowTabs)
							return startOffset + i;
						break;
					default:
						if (ShowBoxForControlCharacters && char.IsControl(c)) {
							return startOffset + i;
						}
						break;
				}
			}
			return -1;
		}
		
		/// <inheritdoc/>
		public override VisualLineElement ConstructElement(int offset)
		{
			char c = CurrentContext.Document.GetCharAt(offset);
			if (ShowSpaces && c == ' ') {
				FormattedText text = new FormattedText(
					"\u00B7",
					CurrentContext.GlobalTextRunProperties.CultureInfo,
					FlowDirection.LeftToRight,
					CurrentContext.GlobalTextRunProperties.Typeface,
					CurrentContext.GlobalTextRunProperties.FontRenderingEmSize,
					Brushes.LightGray
				);
				return new SpaceTextElement(text);
			} else if (ShowTabs && c == '\t') {
				FormattedText text = new FormattedText(
					"\u00BB",
					CurrentContext.GlobalTextRunProperties.CultureInfo,
					FlowDirection.LeftToRight,
					CurrentContext.GlobalTextRunProperties.Typeface,
					CurrentContext.GlobalTextRunProperties.FontRenderingEmSize,
					Brushes.LightGray
				);
				return new TabTextElement(text);
			} else if (ShowBoxForControlCharacters && char.IsControl(c)) {
				FormattedText text = new FormattedText(
					TextUtilities.GetControlCharacterName(c),
					CurrentContext.GlobalTextRunProperties.CultureInfo,
					FlowDirection.LeftToRight,
					CurrentContext.GlobalTextRunProperties.Typeface,
					CurrentContext.GlobalTextRunProperties.FontRenderingEmSize * 0.9,
					Brushes.White
				);
				return new SpecialCharacterBoxElement(text);
			} else {
				return null;
			}
		}
		
		sealed class SpaceTextElement : FormattedTextElement
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
		
		sealed class TabTextElement : VisualLineElement
		{
			internal readonly FormattedText text;
			
			public TabTextElement(FormattedText text) : base(2, 1)
			{
				this.text = text;
			}
			
			public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
			{
				// the TabTextElement consists of two TextRuns:
				// first a TabGlyphRun, then TextCharacters '\t' to let WPF handle the tab indentation
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
		
		sealed class TabGlyphRun : TextEmbeddedObject
		{
			readonly TabTextElement element;
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
		
		sealed class SpecialCharacterBoxElement : FormattedTextElement
		{
			public SpecialCharacterBoxElement(FormattedText text) : base(text, 1)
			{
			}
			
			public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
			{
				return new SpecialCharacterTextRun(this, this.TextRunProperties);
			}
		}
		
		sealed class SpecialCharacterTextRun : FormattedTextRun
		{
			public SpecialCharacterTextRun(FormattedTextElement element, TextRunProperties properties)
				: base(element, properties)
			{
			}
			
			public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
			{
				Point newOrigin = new Point(origin.X + 1, origin.Y);
				Rect r = base.ComputeBoundingBox(rightToLeft, sideways);
				r.Offset(newOrigin.X, newOrigin.Y - this.Element.Text.Baseline);
				r.Width += 1;
				drawingContext.DrawRoundedRectangle(Brushes.DarkGray, null, r, 2.5, 2.5);
				base.Draw(drawingContext, newOrigin, rightToLeft, sideways);
			}
			
			public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
			{
				TextEmbeddedObjectMetrics metrics = base.Format(remainingParagraphWidth);
				return new TextEmbeddedObjectMetrics(metrics.Width + 3,
				                                     metrics.Height, metrics.Baseline);
			}
			
			public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
			{
				Rect r = base.ComputeBoundingBox(rightToLeft, sideways);
				r.Width += 3;
				return r;
			}
		}
	}
}
