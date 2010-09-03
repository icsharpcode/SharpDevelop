// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering
{
	sealed class TextViewCachedElements : IDisposable
	{
		TextFormatter formatter;
		Dictionary<string, TextLine> simpleLightGrayTexts;
		
		public TextLine GetSimpleLightGrayText(string text, ITextRunConstructionContext context)
		{
			if (simpleLightGrayTexts == null)
				simpleLightGrayTexts = new Dictionary<string, TextLine>();
			TextLine textLine;
			if (!simpleLightGrayTexts.TryGetValue(text, out textLine)) {
				var p = new VisualLineElementTextRunProperties(context.GlobalTextRunProperties);
				p.SetForegroundBrush(Brushes.LightGray);
				if (formatter == null)
					formatter = TextFormatterFactory.Create(context.TextView);
				textLine = FormattedTextElement.PrepareText(formatter, text, p);
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
			if (formatter != null)
				formatter.Dispose();
		}
	}
}
