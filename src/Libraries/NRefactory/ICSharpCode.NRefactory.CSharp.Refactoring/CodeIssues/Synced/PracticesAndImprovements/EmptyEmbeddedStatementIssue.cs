//
// Issue.cs
//
// Author:
//       Ciprian Khlud <ciprian.mustiata@yahoo.com>
//
// Copyright (c) 2013 Ciprian Khlud
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.Collections.Generic;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Empty control statement body",
	                   Description = "';' should be avoided. Use '{}' instead",
	                   Category = IssueCategories.PracticesAndImprovements,
	                   Severity = Severity.Warning,
	                   AnalysisDisableKeyword = "EmptyEmbeddedStatement")]
	public class EmptyEmbeddedStatementIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<EmptyEmbeddedStatementIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			public override void VisitWhileStatement(WhileStatement whileStatement)
			{
				base.VisitWhileStatement(whileStatement);
				var statement = whileStatement.EmbeddedStatement as EmptyStatement;
				if (statement == null)
					return;

				AddIssue(new CodeIssue(whileStatement.EmbeddedStatement,
				                     ctx.TranslateString("';' should be avoided. Use '{}' instead"), ctx.TranslateString("Replace with '{}'"),
					script => script.Replace(whileStatement.EmbeddedStatement, new BlockStatement())));
			}

			public override void VisitForeachStatement(ForeachStatement foreachStatement)
			{
				base.VisitForeachStatement(foreachStatement);
				var statement = foreachStatement.EmbeddedStatement as EmptyStatement;
				if (statement == null)
					return;

				AddIssue(new CodeIssue(foreachStatement.EmbeddedStatement,
				                     ctx.TranslateString("';' should be avoided. Use '{}' instead"), ctx.TranslateString("Replace with '{}'"),
					script => script.Replace(foreachStatement.EmbeddedStatement, new BlockStatement())));
			}

			public override void VisitIfElseStatement(IfElseStatement ifElseStatement)
			{
				base.VisitIfElseStatement(ifElseStatement);
				var statement = ifElseStatement.TrueStatement as EmptyStatement;
				if (statement == null)
					return;
				AddIssue(new CodeIssue(ifElseStatement.TrueStatement, 
				                     ctx.TranslateString("';' should be avoided. Use '{}' instead"), ctx.TranslateString("Replace with '{}'"),
					script => script.Replace(ifElseStatement.TrueStatement, new BlockStatement())));
			}

			public override void VisitForStatement(ForStatement forStatement)
			{
				base.VisitForStatement(forStatement);

				var statement = forStatement.EmbeddedStatement as EmptyStatement;
				if (statement == null)
					return;

				AddIssue(new CodeIssue(forStatement.EmbeddedStatement,
				                     ctx.TranslateString("';' should be avoided. Use '{}' instead"), ctx.TranslateString("Replace with '{}'"),
					script => script.Replace(forStatement.EmbeddedStatement, new BlockStatement())));
			}
		}
	}
}
