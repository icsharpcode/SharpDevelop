// 
// ConvertCastToAsAction.cs
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

using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Converts a cast expression to an 'as' expression
	/// </summary>
	[ContextAction ("Convert cast to 'as'.", Description = "Convert cast to 'as'.")]
	public class ConvertCastToAsAction : SpecializedCodeAction<CastExpression>
	{
		static InsertParenthesesVisitor insertParentheses = new InsertParenthesesVisitor ();
		protected override CodeAction GetAction (RefactoringContext context, CastExpression node)
		{
			if (node.Expression.Contains (context.Location))
				return null;
			// only works on reference and nullable types
			var type = context.ResolveType (node.Type);
			var typeDef = type.GetDefinition ();
			var isNullable = typeDef != null && typeDef.KnownTypeCode == KnownTypeCode.NullableOfT;
			if (type.IsReferenceType == true || isNullable) {
				return new CodeAction (context.TranslateString ("Convert cast to 'as'"), script => {
					var asExpr = new AsExpression (node.Expression.Clone (), node.Type.Clone ());
					// if parent is an expression, clone parent and replace the case expression with asExpr,
					// so that we can inset parentheses
					var parentExpr = node.Parent.Clone () as Expression;
					if (parentExpr != null) {
						var castExpr = parentExpr.GetNodeContaining (node.StartLocation, node.EndLocation);
						castExpr.ReplaceWith (asExpr);
						parentExpr.AcceptVisitor (insertParentheses);
						script.Replace (node.Parent, parentExpr);
					} else {
						script.Replace (node, asExpr);
					}
				}, node);
			}
			return null;
		}
	}
}
