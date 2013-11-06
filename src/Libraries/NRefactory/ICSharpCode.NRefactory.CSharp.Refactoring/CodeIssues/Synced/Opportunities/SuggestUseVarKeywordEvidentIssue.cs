// 
// UseVarKeywordInspector.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin <http://xamarin.com>
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
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;
using System;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Checks for places where the 'var' keyword can be used. Note that the action is actually done with a context
	/// action.
	/// </summary>
	[IssueDescription("Use 'var'",
		Description = "Use 'var' keyword when possible",
		Category = IssueCategories.Opportunities,
		Severity = Severity.Hint,
		AnalysisDisableKeyword = "SuggestUseVarKeywordEvident")]
	public class SuggestUseVarKeywordEvidentIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context, this);
		}

        class GatherVisitor : GatherVisitorBase<SuggestUseVarKeywordEvidentIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx, SuggestUseVarKeywordEvidentIssue qualifierDirectiveEvidentIssueProvider) : base (ctx, qualifierDirectiveEvidentIssueProvider)
			{
            }

			public override void VisitSyntaxTree(SyntaxTree syntaxTree)
			{
				if (!ctx.Supports(UseVarKeywordAction.minimumVersion))
					return;
				base.VisitSyntaxTree(syntaxTree);
			}

			public override void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
			{
				base.VisitVariableDeclarationStatement(variableDeclarationStatement);
				if (variableDeclarationStatement.Type is PrimitiveType) {
					return;
				}
				if (variableDeclarationStatement.Type.IsVar()) {
					return;
				}
				if (variableDeclarationStatement.Variables.Count != 1) {
					return;
				}
				
				//only checks for cases where the type would be obvious - assignment of new, cast, etc.
				//also check the type actually matches else the user might want to assign different subclasses later
				var v = variableDeclarationStatement.Variables.Single();
				
				var arrCreate = v.Initializer as ArrayCreateExpression;
				if (arrCreate != null) {
					var n = variableDeclarationStatement.Type as ComposedType;
					//FIXME: check the specifier compatibility
					if (n != null && n.ArraySpecifiers.Any() && n.BaseType.IsMatch(arrCreate.Type)) {
						AddIssue(variableDeclarationStatement);
					}
				}
				var objCreate = v.Initializer as ObjectCreateExpression;
				if (objCreate != null && objCreate.Type.IsMatch(variableDeclarationStatement.Type)) {
					AddIssue(variableDeclarationStatement);
				}
				var asCast = v.Initializer as AsExpression;
				if (asCast != null && asCast.Type.IsMatch(variableDeclarationStatement.Type)) {
					AddIssue(variableDeclarationStatement);
				}
				var cast = v.Initializer as CastExpression;
				if (cast != null && cast.Type.IsMatch(variableDeclarationStatement.Type)) {
					AddIssue(variableDeclarationStatement);
				}
			}
			
			void AddIssue(VariableDeclarationStatement variableDeclarationStatement)
			{
				AddIssue(new CodeIssue(variableDeclarationStatement.Type, ctx.TranslateString("Use 'var' keyword")) { IssueMarker = IssueMarker.DottedLine, ActionProvider = { typeof(UseVarKeywordAction) } });
			}
		}
	}
}
