// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing
{
	sealed class SelectionColorizer : ColorizingTransformer
	{
		TextArea textArea;
		
		public SelectionColorizer(TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			this.textArea = textArea;
		}
		
		protected override void Colorize(ITextRunConstructionContext context)
		{
			int lineStartOffset = context.VisualLine.FirstDocumentLine.Offset;
			int lineEndOffset = context.VisualLine.LastDocumentLine.Offset + context.VisualLine.LastDocumentLine.TotalLength;
			
			foreach (ISegment segment in textArea.Selection.Segments) {
				int segmentStart = segment.Offset;
				int segmentEnd = segment.Offset + segment.Length;
				if (segmentEnd <= lineStartOffset)
					return;
				if (segmentStart >= lineEndOffset)
					return;
				int startColumn = context.VisualLine.GetVisualColumn(Math.Max(0, segmentStart - lineStartOffset));
				int endColumn = context.VisualLine.GetVisualColumn(segmentEnd - lineStartOffset);
				ChangeVisualElements(
					startColumn, endColumn,
					element => {
						element.TextRunProperties.SetForegroundBrush(SystemColors.HighlightTextBrush);
						//element.TextRunProperties.SetBackgroundBrush(SystemColors.HighlightBrush);
					});
			}
		}
	}
}
