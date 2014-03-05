//
// CheckNamespaceIssue.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
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
	[IssueDescription (
		"Check if a namespace corresponds to a file location",
		Description = "Check if a namespace corresponds to a file location",
		Category = IssueCategories.CodeQualityIssues,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "CheckNamespace")]
	public class CheckNamespaceIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<CheckNamespaceIssue>
		{
			readonly string defaultNamespace;

			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
				defaultNamespace = ctx.DefaultNamespace;
			}

			public override void VisitSyntaxTree(SyntaxTree syntaxTree)
			{
				if (string.IsNullOrEmpty(defaultNamespace))
					return;
				base.VisitSyntaxTree(syntaxTree);
			}

			public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
			{
				base.VisitNamespaceDeclaration(namespaceDeclaration);
				// only check top level namespaces
				if (namespaceDeclaration.Parent is NamespaceDeclaration ||
				    namespaceDeclaration.FullName == defaultNamespace)
					return;
				AddIssue(new CodeIssue(
					namespaceDeclaration.NamespaceName,
					string.Format(ctx.TranslateString("Namespace does not correspond to file location, should be: '{0}'"), ctx.DefaultNamespace)
				));
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				var ns = typeDeclaration.Parent as NamespaceDeclaration;
				if (ns == null) {
					AddIssue(new CodeIssue(
						typeDeclaration.NameToken,
						string.Format(ctx.TranslateString("Type should be declared inside the namespace '{0}'"), ctx.DefaultNamespace)
					));
				}
				// skip children
			}
		}

	}
}

