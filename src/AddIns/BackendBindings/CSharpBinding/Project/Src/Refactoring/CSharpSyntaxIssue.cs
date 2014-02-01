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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;

namespace CSharpBinding.Refactoring
{
	[IssueDescription("C# syntax error",
	                  Description = "Displays syntax errors",
	                  Category = IssueCategories.CompilerErrors,
	                  Severity = Severity.Error)]
	public class CSharpSyntaxIssue : CodeIssueProvider
	{
		public override IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context, string subIssue = null)
		{
			var refactoringContext = context as SDRefactoringContext;
			if (refactoringContext == null)
				return Enumerable.Empty<CodeIssue>();
			
			var syntaxTree = context.RootNode as SyntaxTree;
			if (syntaxTree == null)
				return Enumerable.Empty<CodeIssue>();

			return syntaxTree.Errors.Select(error => CreateCodeIssue(error, refactoringContext));
		}
		
		CodeIssue CreateCodeIssue(Error error, SDRefactoringContext context)
		{
			IDocument document = context.Document;
			TextLocation begin = error.Region.Begin;
			
			// Columns seem to be zero-based, SD expects 1-based columns
			int offset = document.GetOffset(begin.Line, begin.Column + 1);
			int endOffset = TextUtilities.GetNextCaretPosition(document, offset, System.Windows.Documents.LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol);
			if (endOffset < 0) endOffset = document.TextLength;
			int length = endOffset - offset;
			
			if (length < 2) {
				// marker should be at least 2 characters long, but take care that we don't make
				// it longer than the document
				length = Math.Min(2, document.TextLength - offset);
			}
			
			TextLocation start = document.GetLocation(offset);
			TextLocation end = document.GetLocation(offset + length);
			return new CodeIssue(start, end, error.Message);
		}
	}
}
