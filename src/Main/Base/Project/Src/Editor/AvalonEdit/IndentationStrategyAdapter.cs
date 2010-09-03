// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;

namespace ICSharpCode.SharpDevelop.Editor.AvalonEdit
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
		
		public virtual void IndentLine(TextDocument document, DocumentLine line)
		{
			if (line == null)
				throw new ArgumentNullException("line");
			formattingStrategy.IndentLine(editor, editor.Document.GetLine(line.LineNumber));
		}
		
		public virtual void IndentLines(TextDocument document, int beginLine, int endLine)
		{
			formattingStrategy.IndentLines(editor, beginLine, endLine);
		}
	}
}
