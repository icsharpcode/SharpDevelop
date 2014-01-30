// 
// ConvertAsToCastAction.cs
//  
// Author:
//       Mansheng Yang <lightyang0@gmail.com>
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Converts an 'as' expression to a cast expression
	/// </summary>
	[ContextAction("Convert 'as' to cast.", Description = "Convert 'as' to cast.")]
	public class ConvertAsToCastAction : SpecializedCodeAction <AsExpression>
	{
		static InsertParenthesesVisitor insertParentheses = new InsertParenthesesVisitor ();
		protected override CodeAction GetAction (RefactoringContext context, AsExpression node)
		{
			if (!node.AsToken.Contains (context.Location))
				return null;
			return new CodeAction (context.TranslateString ("Convert 'as' to cast"),  script => {
				var castExpr = new CastExpression (node.Type.Clone (), node.Expression.Clone ());
				var parenthesizedExpr = node.Parent as ParenthesizedExpression;
				if (parenthesizedExpr != null && parenthesizedExpr.Parent is Expression) {
					// clone parent expression and replace the ParenthesizedExpression with castExpr to remove
					// parentheses, then insert parentheses if necessary
					var parentExpr = (Expression)parenthesizedExpr.Parent.Clone ();
					parentExpr.GetNodeContaining (parenthesizedExpr.StartLocation, parenthesizedExpr.EndLocation)
						.ReplaceWith (castExpr);
					parentExpr.AcceptVisitor (insertParentheses);
					script.Replace (parenthesizedExpr.Parent, parentExpr);
				} else {
					castExpr.AcceptVisitor (insertParentheses);
					script.Replace (node, castExpr);
				}
			}, node.AsToken);
		}
	}
}
