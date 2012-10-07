// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class EditPoint : TextPoint, global::EnvDTE.EditPoint
	{
		IRefactoringDocument document;
		IRefactoringDocumentView documentView;
		
		internal EditPoint(FilePosition filePosition, IDocumentLoader documentLoader)
			: base(filePosition, documentLoader)
		{
		}
		
		public void ReplaceText(object pointOrCount, string text, int flags)
		{
			ReplaceText((TextPoint)pointOrCount, text, (global::EnvDTE.vsEPReplaceTextOptions)flags);
		}
		
		void ReplaceText(TextPoint endPoint, string text, global::EnvDTE.vsEPReplaceTextOptions textFormatOptions)
		{
			OpenDocument();
			int offset = GetStartOffset();
			int endOffset = GetEndOffset(endPoint);
			document.Replace(offset, endOffset - offset, text);
			IndentReplacedText(text);
		}
		
		void OpenDocument()
		{
			documentView = DocumentLoader.LoadRefactoringDocumentView(FilePosition.FileName);
			document = documentView.RefactoringDocument;
		}
		
		int GetStartOffset()
		{
			return document.PositionToOffset(Line, LineCharOffset);
		}
		
		int GetEndOffset(TextPoint endPoint)
		{
			return document.PositionToOffset(endPoint.Line, endPoint.LineCharOffset);
		}
		
		/// <summary>
		/// Indents all lines apart from the first one since it is assumed
		/// that the first line had the correct indentation.
		/// </summary>
		void IndentReplacedText(string text)
		{
			int lineCount = GetLineCount(text);
			if (lineCount > 1) {
				documentView.IndentLines(Line + 1, Line + lineCount);
			}
		}
		
		int GetLineCount(string text)
		{
			return text.Split('\n').Length;
		}
		
		public void Insert(string text)
		{
			throw new NotImplementedException();
		}
	}
}
