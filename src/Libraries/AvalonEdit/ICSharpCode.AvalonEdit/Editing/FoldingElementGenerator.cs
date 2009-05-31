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

using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// A <see cref="VisualLineElementGenerator"/> that produces line elements for folded <see cref="FoldingSection"/>s.
	/// </summary>
	public class FoldingElementGenerator : VisualLineElementGenerator
	{
		/// <summary>
		/// Gets/Sets the folding manager from which the foldings should be shown.
		/// </summary>
		public FoldingManager FoldingManager { get; set; }
		
		/// <inheritdoc/>
		public override void StartGeneration(ITextRunConstructionContext context)
		{
			base.StartGeneration(context);
			if (FoldingManager != null) {
				if (context.TextView != FoldingManager.textView)
					throw new ArgumentException("Invalid TextView");
				if (context.Document != FoldingManager.document)
					throw new ArgumentException("Invalid document");
			}
		}
		
		/// <inheritdoc/>
		public override int GetFirstInterestedOffset(int startOffset)
		{
			if (FoldingManager != null)
				return FoldingManager.GetNextFoldedFoldingStart(startOffset);
			else
				return -1;
		}
		
		/// <inheritdoc/>
		public override VisualLineElement ConstructElement(int offset)
		{
			if (FoldingManager == null)
				return null;
			int foldedUntil = -1;
			foreach (FoldingSection fs in FoldingManager.GetFoldingsAt(offset)) {
				if (fs.IsFolded) {
					if (fs.EndOffset > foldedUntil)
						foldedUntil = fs.EndOffset;
				}
			}
			if (foldedUntil > offset) {
				FormattedText text = new FormattedText(
					"...",
					CurrentContext.GlobalTextRunProperties.CultureInfo,
					FlowDirection.LeftToRight,
					CurrentContext.GlobalTextRunProperties.Typeface,
					CurrentContext.GlobalTextRunProperties.FontRenderingEmSize,
					Brushes.Gray
				);
				return new FoldingLineElement(text, foldedUntil - offset);
			} else {
				return null;
			}
		}
		
		sealed class FoldingLineElement : FormattedTextElement
		{
			public FoldingLineElement(FormattedText text, int documentLength) : base(text, documentLength)
			{
			}
			
			public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
			{
				return new FoldingLineTextRun(this, this.TextRunProperties);
			}
		}
		
		sealed class FoldingLineTextRun : FormattedTextRun
		{
			public FoldingLineTextRun(FormattedTextElement element, TextRunProperties properties) 
				: base(element, properties)
			{
			}
			
			public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
			{
				Rect r = ComputeBoundingBox(rightToLeft, sideways);
				r.Offset(origin.X, origin.Y - this.Element.Text.Baseline);
				drawingContext.DrawRectangle(null, new Pen(Brushes.Gray, 1), r);
				base.Draw(drawingContext, origin, rightToLeft, sideways);
			}
		}
	}
}
