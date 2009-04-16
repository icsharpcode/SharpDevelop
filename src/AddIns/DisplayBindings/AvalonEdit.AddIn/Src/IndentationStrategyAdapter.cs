// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Implements AvalonEdit's <see cref="IIndentationStrategy"/> by forwarding calls
	/// to a <see cref="IFormattingStrategy"/>.
	/// </summary>
	public class IndentationStrategyAdapter : IIndentationStrategy
	{
		readonly ITextEditor editor;
		readonly IFormattingStrategy formattingStrategy;
		
		public IndentationStrategyAdapter(ITextEditor editor, IFormattingStrategy formattingStrategy)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			if (formattingStrategy == null)
				throw new ArgumentNullException("formattingStrategy");
			this.editor = editor;
			this.formattingStrategy = formattingStrategy;
		}
		
		public void IndentLine(DocumentLine line)
		{
			formattingStrategy.IndentLine(editor, editor.Document.GetLine(line.LineNumber));
		}
		
		public void IndentLines(int begin, int end)
		{
			formattingStrategy.IndentLines(editor, begin, end);
		}
	}
}
