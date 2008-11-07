// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Gui
{
	sealed class SelectionColorizer : ColorizingTransformer, IBackgroundRenderer
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
		
		public void Draw(DrawingContext dc)
		{
			BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder();
			geoBuilder.AddSegments(textArea.TextView, textArea.Selection.Segments);
			PathGeometry geometry = geoBuilder.CreateGeometry();
			if (geometry != null) {
				SolidColorBrush lightHighlightBrush = new SolidColorBrush(SystemColors.HighlightColor);
				lightHighlightBrush.Opacity = 0.7;
				lightHighlightBrush.Freeze();
				Pen pen = new Pen(SystemColors.HighlightBrush, 1);
				//pen.LineJoin = PenLineJoin.Round;
				pen.Freeze();
				dc.DrawGeometry(lightHighlightBrush, pen, geometry);
			}
		}
	}
}
