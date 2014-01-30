// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			formattingStrategy.IndentLine(editor, editor.Document.GetLineByNumber(line.LineNumber));
		}
		
		public virtual void IndentLines(TextDocument document, int beginLine, int endLine)
		{
			formattingStrategy.IndentLines(editor, beginLine, endLine);
		}
	}
}
