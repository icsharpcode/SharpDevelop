// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class EditPoint : TextPoint
	{
		IRefactoringDocument document;
		
		internal EditPoint(FilePosition filePosition, IDocumentLoader documentLoader)
			: base(filePosition, documentLoader)
		{
		}
		
		public void ReplaceText(object pointOrCount, string text, int flags)
		{
			ReplaceText((TextPoint)pointOrCount, text, (vsEPReplaceTextOptions)flags);
		}
		
		void ReplaceText(TextPoint endPoint, string text, vsEPReplaceTextOptions textFormatOptions)
		{
			OpenDocument();
			int offset = GetStartOffset();
			int endOffset = GetEndOffset(endPoint);
			document.Replace(offset, endOffset - offset, text);
		}
		
		void OpenDocument()
		{
			document = DocumentLoader.LoadRefactoringDocument(FilePosition.FileName);
		}
		
		int GetStartOffset()
		{
			return document.PositionToOffset(Line, LineCharOffset);
		}
		
		int GetEndOffset(TextPoint endPoint)
		{
			return document.PositionToOffset(endPoint.Line, endPoint.LineCharOffset);
		}
	}
}
