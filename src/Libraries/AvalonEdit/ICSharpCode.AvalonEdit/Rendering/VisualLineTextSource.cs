// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// WPF TextSource implementation that creates TextRuns for a VisualLine.
	/// </summary>
	class VisualLineTextSource : TextSource, ITextRunConstructionContext
	{
		public VisualLineTextSource(VisualLine visualLine)
		{
			this.VisualLine = visualLine;
		}
		
		public VisualLine VisualLine { get; private set; }
		public TextView TextView { get; set; }
		public TextDocument Document { get; set; }
		public TextRunProperties GlobalTextRunProperties { get; set; }
		
		public override TextRun GetTextRun(int textSourceCharacterIndex)
		{
			try {
				foreach (VisualLineElement element in VisualLine.Elements) {
					if (textSourceCharacterIndex >= element.VisualColumn
					    && textSourceCharacterIndex < element.VisualColumn + element.VisualLength)
					{
						int relativeOffset = textSourceCharacterIndex - element.VisualColumn;
						TextRun run = element.CreateTextRun(textSourceCharacterIndex, this);
						if (run == null)
							throw new ArgumentNullException(element.GetType().Name + ".CreateTextRun");
						if (run.Length == 0)
							throw new ArgumentException("The returned TextRun must not have length 0.", element.GetType().Name + ".Length");
						if (relativeOffset + run.Length > element.VisualLength)
							throw new ArgumentException("The returned TextRun is too long.", element.GetType().Name + ".CreateTextRun");
						InlineObjectRun inlineRun = run as InlineObjectRun;
						if (inlineRun != null) {
							inlineRun.VisualLine = VisualLine;
							TextView.textLayer.AddInlineObject(inlineRun);
						}
						return run;
					}
				}
				return new TextEndOfParagraph(1);
			} catch (Exception ex) {
				Debug.WriteLine(ex.ToString());
				throw;
			}
		}
		
		public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
		{
			throw new NotImplementedException();
		}
		
		public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
		{
			throw new NotImplementedException();
		}
	}
}
