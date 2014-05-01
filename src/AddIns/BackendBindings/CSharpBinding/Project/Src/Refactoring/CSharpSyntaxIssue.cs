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
				yield break;
			
			var syntaxTree = context.RootNode as SyntaxTree;
			if (syntaxTree == null)
				yield break;
			
			int prevLine = 0;
			foreach (var error in syntaxTree.Errors) {
				if (error.Region.BeginLine == prevLine)
					continue; // show at most one error per line
				prevLine = error.Region.BeginLine;
				var issue = CreateCodeIssue(error, refactoringContext);
				if (issue != null)
					yield return issue;
			}
		}
		
		static bool IsSpaceOrTab(char c)
		{
			return c == ' ' || c == '\t';
		}
		
		CodeIssue CreateCodeIssue(Error error, SDRefactoringContext context)
		{
			IDocument document = context.Document;
			TextLocation begin = error.Region.Begin;
			if (begin.Line <= 0 || begin.Line > document.LineCount)
				return null;
			
			int offset = document.GetOffset(begin.Line, begin.Column);
			// the parser sometimes reports errors in the whitespace prior to the invalid token, so search for the next word:
			while (offset < document.TextLength && IsSpaceOrTab(document.GetCharAt(offset)))
				offset++;
			int endOffset = TextUtilities.GetNextCaretPosition(document, offset, System.Windows.Documents.LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol);
			if (endOffset < 0) endOffset = document.TextLength;
			int length = endOffset - offset;

			if (length < 1) {
				// marker should be at least 1 characters long,
				// but take care that we don't make it longer than the document
				length = Math.Min(1, document.TextLength - offset);
			}
			
			TextLocation start = document.GetLocation(offset);
			TextLocation end = document.GetLocation(offset + length);
			return new CodeIssue(start, end, error.Message);
		}
	}
}
