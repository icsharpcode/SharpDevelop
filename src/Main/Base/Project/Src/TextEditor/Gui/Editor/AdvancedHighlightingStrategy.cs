// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Modifies the TextEditor's IHighlightingStrategy to be able to plug in
	/// an <see cref="IAdvancedHighlighter"/>.
	/// </summary>
	internal class AdvancedHighlightingStrategy : DefaultHighlightingStrategy
	{
		readonly IAdvancedHighlighter highlighter;
		
		public AdvancedHighlightingStrategy(DefaultHighlightingStrategy baseStrategy, IAdvancedHighlighter highlighter)
		{
			if (highlighter == null)
				throw new ArgumentNullException("highlighter");
			ImportSettingsFrom(baseStrategy);
			this.highlighter = highlighter;
		}
		
		public override void MarkTokens(IDocument document)
		{
			highlighter.BeginUpdate(document, null);
			base.MarkTokens(document);
			highlighter.EndUpdate();
		}
		
		public override void MarkTokens(IDocument document, List<LineSegment> inputLines)
		{
			highlighter.BeginUpdate(document, inputLines);
			base.MarkTokens(document, inputLines);
			highlighter.EndUpdate();
		}
		
		protected override void OnParsedLine(IDocument document, LineSegment currentLine, List<TextWord> words)
		{
			highlighter.MarkLine(currentLineNumber, currentLine, words);
		}
	}
}
