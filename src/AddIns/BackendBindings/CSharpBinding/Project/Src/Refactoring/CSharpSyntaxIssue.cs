// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
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
			
			int offset = document.GetOffset(begin.Line, begin.Column);
			int endOffset = TextUtilities.GetNextCaretPosition(document, offset, System.Windows.Documents.LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol);
			if (endOffset < 0) endOffset = document.TextLength;
			int length = endOffset - offset;
			
			if (length < 2) {
				// marker should be at least 2 characters long, but take care that we don't make
				// it longer than the document
				length = Math.Min(2, document.TextLength - offset);
			}
			
			TextLocation end = document.GetLocation(offset + length);
			return new CodeIssue(begin, end, error.Message);
		}
	}
}
