//
// MethodOverloadWithOptionalParameterIssue.cs
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
using System;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription (
		"Method with optional parameter is hidden by overload",
		Description = "Method with optional parameter is hidden by overload",
		Category = IssueCategories.CodeQualityIssues,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "MethodOverloadWithOptionalParameter")]
	public class MethodOverloadWithOptionalParameterIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<MethodOverloadWithOptionalParameterIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}


			void CheckParameters(IParameterizedMember member,  List<IParameterizedMember> overloads, List<ParameterDeclaration> parameterDeclarations)
			{
				for (int i = 0; i < member.Parameters.Count; i++) {
					if (!member.Parameters[i].IsOptional)
						continue;

					foreach (var overload in overloads) {
						if (overload.Parameters.Count != i)
							continue;
						bool equal = true;
						for (int j = 0; j < i; j++)  {
							if (overload.Parameters[j].Type != member.Parameters[j].Type) {
								equal = false;
								break;
							}
						}
						if (equal) {
							AddIssue(new CodeIssue(
								parameterDeclarations[i],
								member.SymbolKind == SymbolKind.Method ?
								ctx.TranslateString("Method with optional parameter is hidden by overload") :
								ctx.TranslateString("Indexer with optional parameter is hidden by overload")));
						}
					}
				}
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				var rr = ctx.Resolve(methodDeclaration) as MemberResolveResult;
				if (rr == null || rr.IsError)
					return;
				var method = rr.Member as IMethod;
				if (method == null)
					return;
				CheckParameters (method, 
					method.DeclaringType.GetMethods(m =>
						m.Name == method.Name && m.TypeParameters.Count == method.TypeParameters.Count).Cast<IParameterizedMember>().ToList(),
					methodDeclaration.Parameters.ToList()
				);

			}

			public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			{
				var rr = ctx.Resolve(indexerDeclaration) as MemberResolveResult;
				if (rr == null || rr.IsError)
					return;
				var method = rr.Member as IProperty;
				if (method == null)
					return;
				CheckParameters (method, 
					method.DeclaringType.GetProperties(m =>
						m.IsIndexer &&
						m != method.UnresolvedMember).Cast<IParameterizedMember>().ToList(),
					indexerDeclaration.Parameters.ToList()
				);
			}


			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				// SKIP
			}
		}
	}
}

