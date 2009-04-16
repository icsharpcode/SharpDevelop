// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom.Refactoring;
using System;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Indentation strategy.
	/// </summary>
	public interface IFormattingStrategy
	{
		/// <summary>
		/// This function formats a specific line after <code>charTyped</code> is pressed.
		/// </summary>
		void FormatLine(ITextEditor editor, char charTyped);
		
		/// <summary>
		/// This function sets the indentation level in a specific line
		/// </summary>
		void IndentLine(ITextEditor editor, IDocumentLine line);
		
		/// <summary>
		/// This function sets the indentation in a range of lines.
		/// </summary>
		void IndentLines(ITextEditor editor, int begin, int end);
	}
	
	public class DefaultFormattingStrategy : IFormattingStrategy
	{
		public virtual void FormatLine(ITextEditor editor, char charTyped)
		{
		}
		
		public virtual void IndentLine(ITextEditor editor, IDocumentLine line)
		{
			IDocument document = editor.Document;
			int lineNumber = line.LineNumber;
			if (lineNumber > 1) {
				IDocumentLine previousLine = editor.Document.GetLine(lineNumber - 1);
				string indentation = DocumentUtilitites.GetIndentation(document, previousLine.Offset);
				// copy indentation to line
				string newIndentation = DocumentUtilitites.GetIndentation(document, line.Offset);
				editor.Document.Replace(line.Offset, newIndentation.Length, indentation);
			}
		}
		
		public virtual void IndentLines(ITextEditor editor, int begin, int end)
		{
			using (editor.Document.OpenUndoGroup()) {
				for (int i = begin; i <= end; i++) {
					IndentLine(editor, editor.Document.GetLine(i));
				}
			}
		}
	}
}
