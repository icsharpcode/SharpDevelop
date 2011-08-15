// 
// NamedExpressionList.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2011 Xamarin
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
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp
{
	/// <summary>
	/// name = { expression1, ... , expressionN }
	/// </summary>
	public class NamedExpressionList : Expression
	{
		public NamedExpressionList()
		{
		}
		
		public string Identifier {
			get {
				return GetChildByRole (Roles.Identifier).Name;
			}
			set {
				SetChildByRole(Roles.Identifier, CSharp.Identifier.Create (value, AstLocation.Empty));
			}
		}
		
		public Identifier IdentifierToken {
			get {
				return GetChildByRole (Roles.Identifier);
			}
			set {
				SetChildByRole(Roles.Identifier, value);
			}
		}
		
		public CSharpTokenNode AssignToken {
			get { return GetChildByRole (Roles.Assign); }
		}
		
		public CSharpTokenNode LBraceToken {
			get { return GetChildByRole (Roles.LBrace); }
		}
		
		public AstNodeCollection<Expression> Expressions {
			get { return GetChildrenByRole (Roles.Expression); }
		}
		
		public CSharpTokenNode RBraceToken {
			get { return GetChildByRole (Roles.RBrace); }
		}
		
		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitNamedExpressionList(this, data);
		}
		
		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			var o = other as NamedExpressionList;
			return o != null && MatchString(this.Identifier, o.Identifier) && this.Expressions.DoMatch(o.Expressions, match);
		}
	}
}
