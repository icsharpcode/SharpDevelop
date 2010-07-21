// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Rendering
{
	sealed class TextViewCachedElements : IDisposable
	{
		Dictionary<string, TextLine> simpleLightGrayTexts;
		
		public TextLine GetSimpleLightGrayText(string text, ITextRunConstructionContext context)
		{
			if (simpleLightGrayTexts == null)
				simpleLightGrayTexts = new Dictionary<string, TextLine>();
			TextLine textLine;
			if (!simpleLightGrayTexts.TryGetValue(text, out textLine)) {
				var p = new VisualLineElementTextRunProperties(context.GlobalTextRunProperties);
				p.SetForegroundBrush(Brushes.LightGray);
				textLine = FormattedTextElement.PrepareText(context.TextView.TextFormatter, text, p);
				simpleLightGrayTexts[text] = textLine;
			}
			return textLine;
		}
		
		public void Dispose()
		{
			if (simpleLightGrayTexts != null) {
				foreach (TextLine line in simpleLightGrayTexts.Values)
					line.Dispose();
			}
		}
	}
}
