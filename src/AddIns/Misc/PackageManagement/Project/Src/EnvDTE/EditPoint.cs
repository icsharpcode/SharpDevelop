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
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class EditPoint : TextPoint, global::EnvDTE.EditPoint
	{
//		IRefactoringDocument document;
//		IRefactoringDocumentView documentView;
		
		internal EditPoint(string fileName, TextLocation location, IDocumentLoader documentLoader)
			: base(fileName, location, documentLoader)
		{
		}
		
		public void ReplaceText(object pointOrCount, string text, int flags)
		{
			ReplaceText((TextPoint)pointOrCount, text, (global::EnvDTE.vsEPReplaceTextOptions)flags);
		}
		
		void ReplaceText(TextPoint endPoint, string text, global::EnvDTE.vsEPReplaceTextOptions textFormatOptions)
		{
			throw new NotImplementedException();
//			OpenDocument();
//			int offset = GetStartOffset();
//			int endOffset = GetEndOffset(endPoint);
//			document.Replace(offset, endOffset - offset, text);
//			IndentReplacedText(text);
		}
		
//		void OpenDocument()
//		{
//			documentView = DocumentLoader.LoadRefactoringDocumentView(FilePosition.FileName);
//			document = documentView.RefactoringDocument;
//		}
//		
//		int GetStartOffset()
//		{
//			return document.PositionToOffset(Line, LineCharOffset);
//		}
//		
//		int GetEndOffset(TextPoint endPoint)
//		{
//			return document.PositionToOffset(endPoint.Line, endPoint.LineCharOffset);
//		}
//		
//		/// <summary>
//		/// Indents all lines apart from the first one since it is assumed
//		/// that the first line had the correct indentation.
//		/// </summary>
//		void IndentReplacedText(string text)
//		{
//			int lineCount = GetLineCount(text);
//			if (lineCount > 1) {
//				documentView.IndentLines(Line + 1, Line + lineCount);
//			}
//		}
//		
//		int GetLineCount(string text)
//		{
//			return text.Split('\n').Length;
//		}
//		
		public void Insert(string text)
		{
			throw new NotImplementedException();
		}
	}
}
