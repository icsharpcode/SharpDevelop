// EmptyConstructorIssue.cs
// 
// Author:
//      Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun
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
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SaHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Make constructor in abstract class protected",
                      Description = "Constructor in abstract class should not be public",
	                  Category = IssueCategories.PracticesAndImprovements,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "PublicConstructorInAbstractClass")]
	public class PublicConstructorInAbstractClassIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			var unit = context.RootNode as SyntaxTree;
			if (unit == null)
				return null;
			return new GatherVisitor(context);
		}
		
		class GatherVisitor : GatherVisitorBase<PublicConstructorInAbstractClassIssue>
		{

			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}
			
			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				if (!typeDeclaration.HasModifier(Modifiers.Abstract)) {
					return;
				}
				foreach (var constructor in typeDeclaration.Children.OfType<ConstructorDeclaration>()){
					VisitConstructorDeclaration(constructor);
				}
			}

            public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				if (constructorDeclaration.HasModifier (Modifiers.Public)) {

                    var makeProtected = new CodeAction(ctx.TranslateString("Make constructor protected"), script => script.Replace(constructorDeclaration.ModifierTokens.First(t => t.Modifier == Modifiers.Public), new CSharpModifierToken(TextLocation.Empty, Modifiers.Protected)), constructorDeclaration.NameToken);
                    var makePrivate = new CodeAction(ctx.TranslateString("Make constructor private"), script => script.Remove(constructorDeclaration.ModifierTokens.First(t => t.Modifier == Modifiers.Public)), constructorDeclaration.NameToken);

					AddIssue(new CodeIssue(constructorDeclaration.NameToken, ctx.TranslateString("Constructor in Abstract Class should not be public"), new[] { makeProtected, makePrivate }));
                }
			}
		}
	}
}


