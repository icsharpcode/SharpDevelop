// 
// CS0659ClassOverrideEqualsWithoutGetHashCode.cs
// 
// Author:
//      Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("CS0659: Class overrides Object.Equals but not Object.GetHashCode.",
		Description = "If two objects are equal then they must both have the same hash code",
		Category = IssueCategories.CompilerWarnings,
		Severity = Severity.Warning,
		PragmaWarning = 1717,
		AnalysisDisableKeyword = "CSharpWarnings::CS0659")]
	public class CS0659ClassOverrideEqualsWithoutGetHashCode : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context, this);
		}
		
		class GatherVisitor : GatherVisitorBase<CS0659ClassOverrideEqualsWithoutGetHashCode>
		{

			public GatherVisitor(BaseRefactoringContext ctx, CS0659ClassOverrideEqualsWithoutGetHashCode issueProvider) : base (ctx, issueProvider)
			{
			}
			
			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				base.VisitMethodDeclaration(methodDeclaration);

				var resolvedResult = ctx.Resolve(methodDeclaration) as MemberResolveResult;
				if (resolvedResult == null)
					return;
				var method = resolvedResult.Member as IMethod;

				if (method == null || !method.Name.Equals("Equals") || ! method.IsOverride)
					return;

				if (methodDeclaration.Parameters.Count != 1)
					return;
	
				if (!method.Parameters.Single().Type.FullName.Equals("System.Object"))
					return;

				var classDeclration = method.DeclaringTypeDefinition;
				if (classDeclration == null)
					return;

				List<IMethod> getHashCode = new List<IMethod>();
				var methods = classDeclration.GetMethods();

				foreach (var m in methods) {
					if (m.Name.Equals("GetHashCode")) {
						getHashCode.Add(m);
					}
				}

				if (!getHashCode.Any()) {
					AddIssue(ctx, methodDeclaration);
					return;
				} else if (getHashCode.Any(f => (f.IsOverride && f.ReturnType.IsKnownType(KnownTypeCode.Int32)))) {
					return;
				}
				AddIssue(ctx, methodDeclaration);
			}

			private void AddIssue(BaseRefactoringContext ctx, AstNode node)
			{
				var getHashCode = new MethodDeclaration();
				getHashCode.Name = "GetHashCode";
				getHashCode.Modifiers = Modifiers.Public;
				getHashCode.Modifiers |= Modifiers.Override;
				getHashCode.ReturnType = new PrimitiveType("int");

				var blockStatement = new BlockStatement();
				var invocationExpression = new InvocationExpression(new MemberReferenceExpression(new BaseReferenceExpression(),"GetHashCode"));
				var returnStatement = new ReturnStatement(invocationExpression);
				blockStatement.Add(returnStatement);
				getHashCode.Body = blockStatement;

				AddIssue(new CodeIssue(
					(node as MethodDeclaration).NameToken, 
					ctx.TranslateString("If two objects are equal then they must both have the same hash code"),
					new CodeAction(
					ctx.TranslateString("Override GetHashCode"),
					script => {
					script.InsertAfter(node, getHashCode); 
				},
				node
					)));
			}
		}
	}
}

	