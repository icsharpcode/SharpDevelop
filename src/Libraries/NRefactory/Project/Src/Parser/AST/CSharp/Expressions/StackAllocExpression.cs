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
	public class StackAllocExpression : Expression
	{
		TypeReference typeReference;
		Expression    expression;
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = TypeReference.CheckNull(value);
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
		
		public StackAllocExpression(TypeReference typeReference, Expression expression)
		{
			this.TypeReference = typeReference;
			this.Expression = expression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[StackAllocExpression: TypeReference={0}, Expression={1}]",
			                     typeReference,
			                     expression);
		}
	}
}
