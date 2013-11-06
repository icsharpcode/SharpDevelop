// 
// ParameterHidesMemberIssue.cs
// 
// Author:
//      Mansheng Yang <lightyang0@gmail.com>
// 
// Copyright (c) 2012 Mansheng Yang <lightyang0@gmail.com>
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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Parameter hides a member",
					   Description = "Parameter has the same name as a member and hides it.",
					   Category = IssueCategories.CodeQualityIssues,
					   Severity = Severity.Warning,
                       AnalysisDisableKeyword = "ParameterHidesMember")]
    public class ParameterHidesMemberIssue : VariableHidesMemberIssue
	{
	    protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ParameterHidesMemberIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitParameterDeclaration (ParameterDeclaration parameterDeclaration)
			{
				base.VisitParameterDeclaration (parameterDeclaration);

				var rr = ctx.Resolve(parameterDeclaration.Parent) as MemberResolveResult;
				if (rr == null || rr.IsError)
					return;
				var parent = rr.Member;
				if (parent.SymbolKind == SymbolKind.Constructor || parent.ImplementedInterfaceMembers.Any ())
					return;
				if (parent.IsOverride || parent.IsAbstract || parent.IsPublic || parent.IsProtected)
					return;
					
				IMember member;
                if (HidesMember(ctx, parameterDeclaration, parameterDeclaration.Name, out member)) {
                    string msg;
                    switch (member.SymbolKind) {
                        case SymbolKind.Field:
                            msg = ctx.TranslateString("Parameter '{0}' hides field '{1}'");
                            break;
                        case SymbolKind.Method:
                            msg = ctx.TranslateString("Parameter '{0}' hides method '{1}'");
                            break;
                        case SymbolKind.Property:
                            msg = ctx.TranslateString("Parameter '{0}' hides property '{1}'");
                            break;
                        case SymbolKind.Event:
                            msg = ctx.TranslateString("Parameter '{0}' hides event '{1}'");
                            break;
                        default:
                            msg = ctx.TranslateString("Parameter '{0}' hides member '{1}'");
                            break;
                    }
					AddIssue(new CodeIssue(parameterDeclaration.NameToken,
						string.Format(msg, parameterDeclaration.Name, member.FullName)));
			    }
			}
		}
	}
}
