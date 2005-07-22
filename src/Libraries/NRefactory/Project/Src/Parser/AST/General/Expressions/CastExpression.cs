// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public class CastExpression : Expression
	{
		TypeReference castTo;
		Expression    expression;
		bool          isSpecializedCast = false;
		
		public TypeReference CastTo {
			get {
				return castTo;
			}
			set {
				castTo = TypeReference.CheckNull(value);
			}
		}
		
		public Expression Expression {
			get {
				return expression;
			}
			set {
				expression = Expression.CheckNull(value);
			}
		}
		
		public bool IsSpecializedCast {
			get {
				return isSpecializedCast;
			}
			set {
				isSpecializedCast = value;
			}
		}
		
		public CastExpression(TypeReference castTo)
		{
			this.CastTo     = castTo;
			this.expression = Expression.Null;
		}
		
		public CastExpression(TypeReference castTo, Expression expression)
		{
			this.CastTo     = castTo;
			this.Expression = expression;
		}
		
		public CastExpression(TypeReference castTo, Expression expression, bool isSpecializedCast)
		{
			this.CastTo     = castTo;
			this.Expression = expression;
			this.isSpecializedCast = isSpecializedCast;
		}
		
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[CastExpression: CastTo={0}, Expression={1}]",
			                     castTo,
			                     expression);
		}
	}
}
