// RedundantArrayCreationExpressionIssue.cs
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

using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Array creation can be replaced with array initializer",
		Description = "When initializing explicitly typed local variable or array type, array creation expression can be replaced with array initializer.",
		Category = IssueCategories.RedundanciesInCode,
		Severity = Severity.Warning)]
	public class ArrayCreationCanBeReplacedWithArrayInitializerIssue : GatherVisitorCodeIssueProvider
	{

		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context, this);
		}


		class GatherVisitor : GatherVisitorBase<ArrayCreationCanBeReplacedWithArrayInitializerIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx, ArrayCreationCanBeReplacedWithArrayInitializerIssue issueProvider) : base (ctx, issueProvider)
			{
			}

			private void AddIssue(AstNode node, AstNode initializer)
			{
				AddIssue(new CodeIssue(
					node.StartLocation, 
					initializer.StartLocation, 
					ctx.TranslateString("Redundant array creation expression"), 
					ctx.TranslateString("Use array initializer"),
					script => {
						var startOffset = script.GetCurrentOffset(node.StartLocation);
						var endOffset = script.GetCurrentOffset(initializer.StartLocation);
						if (startOffset < endOffset)
							script.RemoveText(startOffset, endOffset - startOffset);
					}) { IssueMarker = IssueMarker.GrayOut }
				);
			}

			public override void VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression)
			{
				base.VisitArrayCreateExpression(arrayCreateExpression);

				if (arrayCreateExpression.Initializer.IsNull)
					return;
			
				var variableInilizer = arrayCreateExpression.GetParent<VariableInitializer>();
				if (variableInilizer == null)
					return;

				if (variableInilizer.Parent is VariableDeclarationStatement) {
					var variableDeclaration = variableInilizer.Parent;

					if (variableDeclaration.GetChildByRole(Roles.Type) is ComposedType) {
						var composedType = variableDeclaration.GetChildByRole(Roles.Type) as ComposedType;
						if (composedType.ArraySpecifiers == null)
							return;
						AddIssue(arrayCreateExpression, arrayCreateExpression.Initializer);
					}
				} else if (variableInilizer.Parent is FieldDeclaration) {
					var filedDeclaration = variableInilizer.Parent;

					if (filedDeclaration.GetChildByRole(Roles.Type) is ComposedType) {
						var composedType = filedDeclaration.GetChildByRole(Roles.Type) as ComposedType;
						if (composedType.ArraySpecifiers == null)
							return;
						AddIssue(arrayCreateExpression, arrayCreateExpression.Initializer);
					}
				}
			}
		}
	}
}