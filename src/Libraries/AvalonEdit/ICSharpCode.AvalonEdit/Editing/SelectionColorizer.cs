// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			// if SelectionForeground is null, keep the existing foreground color
			if (textArea.SelectionForeground == null)
				return;
			
			int lineStartOffset = context.VisualLine.FirstDocumentLine.Offset;
			int lineEndOffset = context.VisualLine.LastDocumentLine.Offset + context.VisualLine.LastDocumentLine.TotalLength;
			
			foreach (var segment in textArea.Selection.Segments) {
				int segmentStart = segment.StartOffset;
				int segmentEnd = segment.EndOffset;
				if (segmentEnd <= lineStartOffset)
					continue;
				if (segmentStart >= lineEndOffset)
					continue;
				int startColumn = segment.StartVisualColumn;
				int endColumn = segment.EndVisualColumn;
				if (startColumn < 0)
					startColumn = context.VisualLine.GetVisualColumn(Math.Max(0, segmentStart - lineStartOffset));
				if (endColumn < 0)
					endColumn = context.VisualLine.GetVisualColumn(segmentEnd - lineStartOffset);
				
				ChangeVisualElements(
					startColumn, endColumn,
					element => {
						element.TextRunProperties.SetForegroundBrush(textArea.SelectionForeground);
					});
			}
		}
	}
}
