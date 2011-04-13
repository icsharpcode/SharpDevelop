// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.VB.Ast
{
	/// <summary>
	/// Description of IdentifierExpression.
	/// </summary>
	public class IdentifierExpression : Expression
	{
		public string Identifier { get; set; }
		
		public bool IsEscaped { get; set; }
		
		public bool IsKeyword { get; set; }
		
		public TypeCode TypeCharacter { get; set; }
		

		
		protected internal override bool DoMatch(AstNode other, ICSharpCode.NRefactory.PatternMatching.Match match)
		{
			var node = other as IdentifierExpression;
			return node != null
				&& node.Identifier == Identifier
				&& node.IsEscaped == IsEscaped
				&& node.IsKeyword == IsKeyword
				&& node.TypeCharacter == TypeCharacter;
		}
		
		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitIdentifierExpression(this, data);
		}
	}
}
